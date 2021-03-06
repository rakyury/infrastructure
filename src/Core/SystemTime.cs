﻿using System;

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

namespace Spark
{
    /// <summary>
    /// Represents an instance in time, typically expressed as a date and time of day.
    /// </summary>
    /// <remarks>Can override usage of <see cref="DateTime.UtcNow"/> by calling <see cref="OverrideWith"/>.</remarks>
    public static class SystemTime
    {
        private static readonly Object TimeLock = new Object();
        private static Func<DateTime> utcNowOverride;
        private static DateTime previous;
        private static Int64 sequence;

        /// <summary>
        /// Get the current system time (UTC).
        /// </summary>
        public static DateTime Now { get { return utcNowOverride == null ? DateTime.UtcNow : utcNowOverride(); } }

        /// <summary>
        /// Get a unique system timestamp (UTC) within the current <see cref="AppDomain"/>.
        /// </summary>
        public static DateTime GetTimestamp()
        {
            var timestamp = utcNowOverride == null ? DateTime.UtcNow : utcNowOverride();

            lock (TimeLock)
            {
                if (timestamp == previous)
                {
                    timestamp = previous.AddTicks(++sequence);
                }
                else
                {
                    previous = timestamp;
                    sequence = 0L;
                }
            }

            return timestamp;
        }
        
        /// <summary>
        /// Override the use of <see cref="DateTime.UtcNow"/> with a custom UTC time function.
        /// </summary>
        /// <param name="timeRetriever">The replacement function to use when retrieving the UTC system time.</param>
        public static void OverrideWith(Func<DateTime> timeRetriever)
        {
            Verify.NotNull(timeRetriever, nameof(timeRetriever));
            Verify.Equal(DateTimeKind.Utc, timeRetriever().Kind, nameof(timeRetriever));

            utcNowOverride = timeRetriever;
        }

        /// <summary>
        /// Clear the overriden system time function.
        /// </summary>
        public static void ClearOverride()
        {
            utcNowOverride = null;
        }
    }
}
