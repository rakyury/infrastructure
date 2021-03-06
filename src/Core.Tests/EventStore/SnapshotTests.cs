﻿using System;
using Spark;
using Spark.EventStore;
using Spark.Resources;
using Xunit;

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

namespace Test.Spark.EventStore
{
    // ReSharper disable NotResolvedInText
    namespace UsingSnapshot
    {
        public  class WhenCreatingSnapshot
        {
            [Fact]
            public void StreamIdCannotBeEmptyGuid()
            {
                var expectedEx = new ArgumentException(Exceptions.ArgumentEqualToValue.FormatWith(Guid.Empty), "streamId");
                var actualEx = Assert.Throws<ArgumentException>(() => new Snapshot(Guid.Empty, 1, new Object()));

                Assert.Equal(expectedEx.Message, actualEx.Message);
            }

            [Fact]
            public void VersionMustBeGreaterThanZero()
            {
                var expectedEx = new ArgumentOutOfRangeException("version", 0, Exceptions.ArgumentNotGreaterThanValue.FormatWith(0));
                var actualEx = Assert.Throws<ArgumentOutOfRangeException>(() => new Snapshot(Guid.NewGuid(), 0, new Object()));

                Assert.Equal(expectedEx.Message, actualEx.Message);
            }

            [Fact]
            public void StateCannotBeNull()
            {
                var expectedEx = new ArgumentNullException("state");
                var actualEx = Assert.Throws<ArgumentNullException>(() => new Snapshot(Guid.NewGuid(), 1, null));

                Assert.Equal(expectedEx.Message, actualEx.Message);
            }
            
            [Fact]
            public void CanConstructValidSnapshot()
            {
                var streamId = Guid.NewGuid();
                var state = new Object();
                var version = 1;

                var snapshot = new Snapshot(streamId, version, state);

                Assert.Equal(streamId, snapshot.StreamId);
                Assert.Equal(version, snapshot.Version);
                Assert.Equal(state, snapshot.State);
            }
        }
    }
}
