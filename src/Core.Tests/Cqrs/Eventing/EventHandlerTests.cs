﻿using System;
using JetBrains.Annotations;
using Spark;
using Spark.Cqrs.Eventing;
using Spark.Messaging;
using Xunit;
using EventHandler = Spark.Cqrs.Eventing.EventHandler;

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

namespace Test.Spark.Cqrs.Eventing
{
    namespace UsingEventHandler
    {
        public class WhenCreatingNewHandler
        {
            [Fact]
            public void AggregateTypeCannotBeNull()
            {
                var ex = Assert.Throws<ArgumentNullException>(() => new EventHandler(null, typeof(Event), (h, e) => { }, () => new Object()));

                Assert.Equal("handlerType", ex.ParamName);
            }

            [Fact]
            public void AggregateTypeMustBeAnAggregateType()
            {
                var ex = Assert.Throws<ArgumentNullException>(() => new EventHandler(typeof(Object), null, (h, e) => { }, () => new Object()));

                Assert.Equal("eventType", ex.ParamName);
            }

            [Fact]
            public void AggregateStoreCannotBeNull()
            {
                var ex = Assert.Throws<ArgumentNullException>(() => new EventHandler(typeof(Object), typeof(Event), null, () => new Object()));

                Assert.Equal("executor", ex.ParamName);
            }

            [Fact]
            public void ExecutorCannotBeNull()
            {
                var ex = Assert.Throws<ArgumentNullException>(() => new EventHandler(typeof(Object), typeof(Event), (h, e) => { }, null));

                Assert.Equal("eventHandlerFactory", ex.ParamName);
            }
        }


        // ReSharper disable AccessToDisposedClosure
        public class WhenHandlingEventContext
        {
            [Fact]
            public void ContextCannotBeNull()
            {
                var eventHandler = new EventHandler(typeof(Object), typeof(FakeEvent), (h, e) => { }, () => new Object());

                Assert.Throws<ArgumentNullException>(() => eventHandler.Handle(null));
            }

            [Fact]
            public void InvokeUnderlyingExecutorWithEventAndHandlerInstance()
            {
                var handled = false;
                var e = new FakeEvent();
                var handler = new Object();
                var eventHandler = new EventHandler(typeof(Object), typeof(FakeEvent), (a, b) => { handled = a == handler && b == e; }, () => handler);

                using (var context = new EventContext(GuidStrategy.NewGuid(), HeaderCollection.Empty, e))
                    eventHandler.Handle(context);

                Assert.True(handled);
            }
        }
        // ReSharper restore AccessToDisposedClosure

        public class WhenConvertingToString
        {
            [Fact]
            public void ReturnFriendlyDescription()
            {
                var eventHandler = new EventHandler(typeof(Object), typeof(FakeEvent), (h, e) => { }, () => new Object());

                Assert.Equal($"{typeof (FakeEvent)} Event Handler ({typeof (Object)})", eventHandler.ToString());
            }
        }

        [UsedImplicitly]
        internal class FakeEvent : Event
        { }
    }
}
