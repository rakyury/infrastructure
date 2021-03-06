﻿using System;
using Spark.Cqrs.Domain;

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
    /// The event message envelope that pairs an <see cref="Event"/> with the source <see cref="Aggregate"/> identifier and <see cref="Version"/>.
    /// </summary>
    [Serializable]
    public sealed class EventEnvelope
    {
        private class NullEvent : Event { }
        private readonly EventVersion version;
        private readonly Guid correlationId;
        private readonly Guid aggregateId;
        private readonly Event e;

        /// <summary>
        /// Represents an empty <see cref="EventEnvelope"/>. This field is read-only.
        /// </summary>
        public static readonly EventEnvelope Empty = new EventEnvelope(Guid.Empty, Guid.Empty, EventVersion.Empty, new NullEvent());

        /// <summary>
        /// The message correlation identifier that is assocaited with this <see cref="EventEnvelope"/>.
        /// </summary>
        public Guid CorrelationId { get { return correlationId; } }

        /// <summary>
        /// The unique <see cref="Aggregate"/> identifier that is the source of the associated <see cref="Event"/>.
        /// </summary>
        public Guid AggregateId { get { return aggregateId; } }

        /// <summary>
        /// The aggregate version and associated event index that is the source of the associated <see cref="Event"/>.
        /// </summary>
        public EventVersion Version { get { return version; } }

        /// <summary>
        /// The event payload that originated from the specified <see cref="Aggregate"/>.
        /// </summary>
        public Event Event { get { return e; } }

        /// <summary>
        /// Creates a new instance of <see cref="EventEnvelope"/> for the specified <paramref name="aggregateId"/>, <paramref name="version"/> and <paramref name="e"/>.
        /// </summary>
        /// <param name="correlationId"> The message correlation identifier that is assocaited with this <see cref="EventEnvelope"/>.</param>
        /// <param name="aggregateId">The unique <see cref="Aggregate"/> identifier that is the source of the associated <see cref="Event"/>.</param>
        /// <param name="version"> The aggregate version and associated event index that is the source of the associated <see cref="Event"/>.</param>
        /// <param name="e">The event payload that originated from the specified <see cref="Aggregate"/>.</param>
        public EventEnvelope(Guid correlationId, Guid aggregateId, EventVersion version, Event e)
        {
            Verify.NotNull(e, nameof(e));

            this.correlationId = correlationId;
            this.aggregateId = aggregateId;
            this.version = version;
            this.e = e;
        }

        /// <summary>
        /// Returns the <see cref="EventEnvelope"/> description for this instance.
        /// </summary>
        public override String ToString()
        {
            return $"{Event.GetType()} - {AggregateId}";
        }
    }
}
