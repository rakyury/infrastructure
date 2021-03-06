﻿using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Spark.Configuration;
using Spark.Cqrs.Domain;
using Spark.Logging;
using Spark.Messaging;
using Spark.Threading;

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

namespace Spark.Cqrs.Commanding
{
    /// <summary>
    /// Executes <see cref="Command"/> instances with the associated <see cref="Aggregate"/> <see cref="CommandHandler"/>.
    /// </summary>
    public sealed class CommandProcessor : IProcessMessages<CommandEnvelope>
    {
        private static readonly TaskCreationOptions TaskCreationOptions = TaskCreationOptions.AttachedToParent | TaskCreationOptions.HideScheduler;
        private static readonly ILog Log = LogManager.GetCurrentClassLogger();
        private readonly IRetrieveCommandHandlers commandHandlerRegistry;
        private readonly IDetectTransientErrors transientErrorRegistry;
        private readonly TaskScheduler taskScheduler;
        private readonly TimeSpan retryTimeout;

        /// <summary>
        /// Creates a new instance of the <see cref="CommandProcessor"/> using the specified <see cref="IRetrieveCommandHandlers"/> and <see cref="IStoreAggregates"/> instances.
        /// </summary>
        /// <param name="commandHandlerRegistry">The <see cref="CommandHandler"/> registry.</param>
        /// <param name="transientErrorRegistries">The set of <see cref="IDetectTransientErrors"/> instances used to detect transient errors.</param>
        public CommandProcessor(IRetrieveCommandHandlers commandHandlerRegistry, IEnumerable<IDetectTransientErrors> transientErrorRegistries)
            : this(commandHandlerRegistry, new TransientErrorRegistry(transientErrorRegistries), Settings.CommandProcessor)
        { }

        /// <summary>
        /// Creates a new instance of the <see cref="CommandProcessor"/> using the specified <see cref="IRetrieveCommandHandlers"/> and <see cref="IStoreAggregates"/> instances.
        /// </summary>
        /// <param name="commandHandlerRegistry">The <see cref="CommandHandler"/> registry.</param>
        /// <param name="transientErrorRegistry">The <see cref="IDetectTransientErrors"/> instance used to detect transient errors.</param>
        /// <param name="settings">The command processor configuration settings.</param>
        internal CommandProcessor(IRetrieveCommandHandlers commandHandlerRegistry, IDetectTransientErrors transientErrorRegistry, IProcessCommandSettings settings)
        {
            Verify.NotNull(commandHandlerRegistry, nameof(commandHandlerRegistry));
            Verify.NotNull(settings, nameof(settings));

            this.retryTimeout = settings.RetryTimeout;
            this.transientErrorRegistry = transientErrorRegistry;
            this.taskScheduler = new PartitionedTaskScheduler(GetAggregateId, settings.MaximumConcurrencyLevel, settings.BoundedCapacity);
            this.commandHandlerRegistry = commandHandlerRegistry;
        }

        /// <summary>
        /// Gets the task partition id based on the underlying command's target aggregate id.
        /// </summary>
        /// <param name="task">The task to partition.</param>
        private static Object GetAggregateId(Task task)
        {
            var message = (Message<CommandEnvelope>)task.AsyncState;

            return message.Payload.AggregateId;
        }

        /// <summary>
        /// Process the received <see cref="Command"/> message instance asynchronously.
        /// </summary>
        /// <param name="message">The <see cref="Command"/> message.</param>
        public Task ProcessAsync(Message<CommandEnvelope> message)
        {
            Verify.NotNull(message, nameof(message));

            return CreateTask(message);
        }

        /// <summary>
        /// Create a new worker task that will be used to process the specified <see cref="Command"/> message.
        /// </summary>
        /// <param name="message">The <see cref="Command"/> message.</param>
        private Task CreateTask(Message<CommandEnvelope> message)
        {
            var task = Task.Factory.StartNew(state => Process((Message<CommandEnvelope>)state), message, CancellationToken.None, TaskCreationOptions, taskScheduler);

            task.ConfigureAwait(continueOnCapturedContext: false);

            return task;
        }

        /// <summary>
        /// Process the received <see cref="Command"/> message instance synchronously.
        /// </summary>
        /// <param name="message">The <see cref="Command"/> message.</param>
        public void Process(Message<CommandEnvelope> message)
        {
            using (Log.PushContext("{0} ({1})", message.Payload.Command.GetType(), message.Id))
            {
                var commandHandler = commandHandlerRegistry.GetHandlerFor(message.Payload.Command);

                ExecuteHandler(commandHandler, message);
            }
        }

        /// <summary>
        ///  Process the received <see cref="Command"/> message instance using the specified <paramref name="commandHandler"/>.
        /// </summary>
        /// <param name="commandHandler">The <see cref="CommandHandler"/> instance that will process the <paramref name="message"/>.</param>
        /// <param name="message">The <see cref="Command"/> message.</param>
        private void ExecuteHandler(CommandHandler commandHandler, Message<CommandEnvelope> message)
        {
            var backoffContext = default(ExponentialBackoff);
            var done = false;

            do
            {
                try
                {
                    using (var context = new CommandContext(message.Id, message.Headers, message.Payload))
                        commandHandler.Handle(context);

                    done = true;
                }
                catch (Exception ex)
                {
                    if (!transientErrorRegistry.IsTransient(ex))
                        throw;

                    backoffContext = backoffContext ?? new ExponentialBackoff(retryTimeout);
                    backoffContext.WaitOrTimeout(ex);
                    Log.Warn(ex.Message);
                }
            } while (!done);
        }
    }
}
