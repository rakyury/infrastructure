﻿using System;
using System.Configuration;
using Spark.Cqrs.Eventing;
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

namespace Spark.Configuration
{
    /// <summary>
    /// <see cref="EventProcessor"/> configuration settings.
    /// </summary>
    internal interface IProcessEventSettings
    {
        /// <summary>
        /// The bounded capacity associated with the <see cref="EventProcessor"/>'s <see cref="PartitionedTaskScheduler"/> (default = <value>1000</value>).
        /// </summary>
        Int32 BoundedCapacity { get; }
        
        /// <summary>
        /// The maximum concurrency level associated with the <see cref="EventProcessor"/>'s <see cref="PartitionedTaskScheduler"/> (default = <value>47</value>).
        /// </summary>
        Int32 MaximumConcurrencyLevel { get; }

        /// <summary>
        /// The maximum amount of time to spend trying to execute an event handler (default = <value>00:01:00</value>).
        /// </summary>
        TimeSpan RetryTimeout { get; }
    }

    internal sealed class EventProcessorElement : ConfigurationElement, IProcessEventSettings
    {
        [ConfigurationProperty("boundedCapacity", IsRequired = false, DefaultValue = 1000)]
        public Int32 BoundedCapacity { get { return (Int32)base["boundedCapacity"]; } }

        [ConfigurationProperty("maximumConcurrencyLevel", IsRequired = false, DefaultValue = 47)]
        public Int32 MaximumConcurrencyLevel { get { return (Int32)base["maximumConcurrencyLevel"]; } }

        [ConfigurationProperty("retryTimeout", IsRequired = false, DefaultValue = "00:01:00")]
        public TimeSpan RetryTimeout { get { return (TimeSpan)base["retryTimeout"]; } }
    }
}
