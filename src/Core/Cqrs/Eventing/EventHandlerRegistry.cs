﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Spark.Cqrs.Commanding;
using Spark.Cqrs.Domain;
using Spark.Cqrs.Eventing.Mappings;
using Spark.Cqrs.Eventing.Sagas;
using Spark.Logging;
using Spark.Resources;

/* Copyright (c) 2015 Spark Software Ltd.
 * 
 * Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
 * 
 * The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
 * 
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS
 * FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, 
 * WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,  OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
 */

namespace Spark.Cqrs.Eventing
{
    /// <summary>
    /// An <see cref="EventHandler"/> registry associating event handler handle methods with specific <see cref="Event"/> types.
    /// </summary>
    public sealed class EventHandlerRegistry : IRetrieveEventHandlers
    {
        private static readonly ILog Log = LogManager.GetCurrentClassLogger();
        private readonly IReadOnlyDictionary<Type, EventHandler[]> knownEventHandlers;
        private readonly IReadOnlyDictionary<Type, EventHandler[]> knownSagaTimeoutHandlers;

        /// <summary>
        /// Initializes a new instance of <see cref="EventHandlerRegistry"/> with the specified <paramref name="typeLocator"/> and <paramref name="serviceProvider"/>.
        /// </summary>
        /// <param name="sagaStore">The saga store to pass on to any <see cref="SagaEventHandler"/> instances.</param>
        /// <param name="typeLocator">The type locator used to retrieve all known <see cref="Event"/> types.</param>
        /// <param name="serviceProvider">The service locator used to retrieve singleton event handler dependencies.</param>
        /// <param name="commandPublisher">The command publisher used to publish saga commands.</param>
        public EventHandlerRegistry(ILocateTypes typeLocator, IServiceProvider serviceProvider, IStoreSagas sagaStore, Lazy<IPublishCommands> commandPublisher)
        {
            Verify.NotNull(sagaStore, nameof(sagaStore));
            Verify.NotNull(typeLocator, nameof(typeLocator));
            Verify.NotNull(serviceProvider, nameof(serviceProvider));
            Verify.NotNull(commandPublisher, nameof(commandPublisher));

            knownEventHandlers = DiscoverEventHandlers(typeLocator, serviceProvider, sagaStore, commandPublisher);
            knownSagaTimeoutHandlers = knownEventHandlers.Where(item => typeof(Timeout).IsAssignableFrom(item.Key))
                                                         .SelectMany(item => item.Value)
                                                         .OfType<SagaEventHandler>()
                                                         .Distinct(item => item.HandlerType)
                                                         .ToDictionary(item => item.HandlerType, item => new EventHandler[] { item });
        }

        /// <summary>
        /// Discover all event handlers associated with any locatable class marked with <see cref="EventHandlerAttribute"/>.
        /// </summary>
        /// <param name="sagaStore">The saga store to pass on to any <see cref="SagaEventHandler"/> instances.</param>
        /// <param name="typeLocator">The type locator use to retrieve all known classes marked with <see cref="EventHandlerAttribute"/>.</param>
        /// <param name="serviceProvider">The service locator used to retrieve singleton event handler dependencies.</param>
        /// <param name="commandPublisher">The command publisher used to publish saga commands.</param>
        private static Dictionary<Type, EventHandler[]> DiscoverEventHandlers(ILocateTypes typeLocator, IServiceProvider serviceProvider, IStoreSagas sagaStore, Lazy<IPublishCommands> commandPublisher)
        {
            var knownEvents = typeLocator.GetTypes(type => !type.IsAbstract && type.IsClass && type.DerivesFrom(typeof(Event)));
            var knownHandlers = DiscoverHandleMethods(typeLocator, serviceProvider, sagaStore, commandPublisher);
            var result = new Dictionary<Type, EventHandler[]>();
            var logMessage = new StringBuilder();

            logMessage.AppendLine("Discovered event handlers:");
            foreach (var eventType in knownEvents.OrderBy(type => type.FullName))
            {
                var eventHandlers = eventType.GetTypeHierarchy().Reverse()
                                             .Where(knownHandlers.ContainsKey)
                                             .SelectMany(type => knownHandlers[type])
                                             .OrderBy(handler => handler is SagaEventHandler)
                                             .ThenBy(handler => handler.HandlerType.AssemblyQualifiedName)
                                             .ToArray();

                logMessage.Append("    ");
                logMessage.Append(eventType);
                logMessage.AppendLine();

                foreach (var eventHandler in eventHandlers)
                {
                    logMessage.Append("        ");
                    logMessage.Append(eventHandler.HandlerType);
                    logMessage.AppendLine();
                }

                result.Add(eventType, eventHandlers);
            }

            Log.Debug(logMessage.ToString);

            return result;
        }

        /// <summary>
        /// Discover all event handlers methods associated with any locatable class marked with with <see cref="EventHandlerAttribute"/>.
        /// </summary>
        /// <param name="sagaStore">The saga store to pass on to any <see cref="SagaEventHandler"/> instances.</param>
        /// <param name="typeLocator">The type locator use to retrieve all known classes marked with <see cref="EventHandlerAttribute"/>.</param>
        /// <param name="serviceProvider">The service locator used to retrieve singleton event handler dependencies.</param>
        /// <param name="commandPublisher">The command publisher used to publish saga commands.</param>
        private static Dictionary<Type, List<EventHandler>> DiscoverHandleMethods(ILocateTypes typeLocator, IServiceProvider serviceProvider, IStoreSagas sagaStore, Lazy<IPublishCommands> commandPublisher)
        {
            var handlerTypes = typeLocator.GetTypes(type => !type.IsAbstract && type.IsClass && type.GetCustomAttribute<EventHandlerAttribute>() != null);
            var knownEventHandlers = new Dictionary<Type, List<EventHandler>>();

            foreach (var handlerType in handlerTypes)
            {
                var handleMethods = GetHandleMethods(handlerType, serviceProvider);
                var sagaMetadata = typeof(Saga).IsAssignableFrom(handlerType) ? GetSagaMetadata(handlerType, handleMethods) : null;

                foreach (var handleMethod in handleMethods)
                {
                    List<EventHandler> eventHandlers;
                    Type eventType = handleMethod.Key;
                    if (!knownEventHandlers.TryGetValue(eventType, out eventHandlers))
                        knownEventHandlers.Add(eventType, eventHandlers = new List<EventHandler>());

                    var eventHandler = new EventHandler(handlerType, eventType, handleMethod.Value, GetHandlerFactory(handlerType, serviceProvider));
                    if (sagaMetadata != null)
                        eventHandler = eventType == typeof(Timeout) ? new SagaTimeoutHandler(eventHandler, sagaMetadata, sagaStore, commandPublisher) : new SagaEventHandler(eventHandler, sagaMetadata, sagaStore, commandPublisher);

                    eventHandlers.Add(eventHandler);
                }
            }

            return knownEventHandlers;
        }

        /// <summary>
        /// Gets the saga metadata for the specified <paramref name="sagaType"/> validating against the known <paramref name="handleMethods"/>.
        /// </summary>
        /// <remarks>Called once during saga discovery.</remarks>
        private static SagaMetadata GetSagaMetadata(Type sagaType, HandleMethodCollection handleMethods)
        {
            var saga = (Saga)Activator.CreateInstance(sagaType);
            var metadata = saga.GetMetadata();
            var initiatingEvents = 0;

            foreach (var handleMethod in handleMethods)
            {
                if (metadata.CanStartWith(handleMethod.Key))
                    initiatingEvents++;

                if (metadata.CanHandle(handleMethod.Key))
                    continue;

                throw new MappingException(Exceptions.EventTypeNotConfigured.FormatWith(sagaType, handleMethod.Key));
            }

            if (initiatingEvents == 0)
                throw new MappingException(Exceptions.SagaMustHaveAtLeastOneInitiatingEvent.FormatWith(sagaType));

            return metadata;
        }

        /// <summary>
        /// Gets the factory method associated with the specified <paramref name="handlerType"/>.
        /// </summary>
        /// <param name="handlerType">The event handler type.</param>
        /// <param name="serviceProvider">The service locator used to retrieve singleton event handler dependencies.</param>
        private static Func<Object> GetHandlerFactory(Type handlerType, IServiceProvider serviceProvider)
        {
            if (typeof(Saga).IsAssignableFrom(handlerType))
                return () => { throw new NotSupportedException(); };

            if (!handlerType.GetCustomAttribute<EventHandlerAttribute>().IsReusable)
                return () => serviceProvider.GetService(handlerType);

            var handler = serviceProvider.GetService(handlerType);
            return () => handler;
        }

        /// <summary>
        /// Discover all event handlers methods defined within the specified <paramref name="handlerType"/>.
        /// </summary>
        /// <param name="handlerType">The event handler type.</param>
        /// <param name="serviceProvider">The service locator used to retrieve singleton event handler dependencies.</param>
        private static HandleMethodCollection GetHandleMethods(Type handlerType, IServiceProvider serviceProvider)
        {
            if (typeof(Saga).IsAssignableFrom(handlerType) && handlerType.GetConstructor(Type.EmptyTypes) == null)
                throw new MappingException(Exceptions.SagaDefaultConstructorRequired.FormatWith(handlerType));

            var handleMethodMappings = handlerType.GetCustomAttributes<HandleByStrategyAttribute>().ToArray();
            if (handleMethodMappings.Length > 1)
                throw new MappingException(Exceptions.EventHandlerHandleByStrategyAmbiguous.FormatWith(handlerType));

            return (handleMethodMappings.Length == 0 ? HandleByStrategyAttribute.Default : handleMethodMappings[0]).GetHandleMethods(handlerType, serviceProvider);
        }

        /// <summary>
        /// Gets the set of <see cref="EventHandler"/> instances associated with the specified <paramref name="e"/>.
        /// </summary>
        /// <param name="e">The event for which to retrieve all <see cref="EventHandler"/> instances.</param>
        public IEnumerable<EventHandler> GetHandlersFor(Event e)
        {
            Verify.NotNull(e, nameof(e));

            EventHandler[] eventHandlers;
            Timeout timeout = e as Timeout;
            if (timeout != null && knownSagaTimeoutHandlers.TryGetValue(timeout.SagaType, out eventHandlers))
                return eventHandlers;

            return knownEventHandlers.TryGetValue(e.GetType(), out eventHandlers) ? eventHandlers : Enumerable.Empty<EventHandler>();
        }
    }
}
