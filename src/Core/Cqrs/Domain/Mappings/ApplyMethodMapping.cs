﻿using System;
using System.Collections.Generic;
using Spark.Cqrs.Eventing;

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

namespace Spark.Cqrs.Domain.Mappings
{
    /// <summary>
    /// Represents an explicit <see cref="Aggregate"/> apply method mapping.
    /// </summary>
    public abstract class ApplyMethodMapping
    {
        /// <summary>
        /// Apply method mapping builder.
        /// </summary>
        protected sealed class ApplyMethodMappingBuilder
        {
            private readonly IDictionary<Type, Action<Aggregate, Event>> applyMethods = new Dictionary<Type, Action<Aggregate, Event>>();

            internal IDictionary<Type, Action<Aggregate, Event>> Mappings { get { return applyMethods; } }

            /// <summary>
            /// Register the apply method for the specified <paramref name="eventType"/>.
            /// </summary>
            /// <param name="eventType">The event type associated with the specified <paramref name="applyMethod"/>.</param>
            /// <param name="applyMethod">The apply method to be invoked for events of <paramref name="eventType"/>.</param>
            public void Register(Type eventType, Action<Aggregate, Event> applyMethod)
            {
                Verify.NotNull(eventType, nameof(eventType));
                Verify.NotNull(applyMethod, nameof(applyMethod));
                Verify.TypeDerivesFrom(typeof(Event), eventType, nameof(eventType));

                applyMethods.Add(eventType, applyMethod);
            }
        }

        /// <summary>
        /// Gets the explicitly registered apply methods.
        /// </summary>
        internal IDictionary<Type, Action<Aggregate, Event>> GetMappings()
        {
            var builder = new ApplyMethodMappingBuilder();

            RegisterMappings(builder);

            return builder.Mappings;
        }

        /// <summary>
        /// Register the event type apply methods for a given aggregate type.
        /// </summary>
        /// <param name="builder">The <see cref="ApplyMethodMappingBuilder"/> for the underlying aggregate type.</param>
        protected abstract void RegisterMappings(ApplyMethodMappingBuilder builder);
    }
}
