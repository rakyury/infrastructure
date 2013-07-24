﻿using System;
using Moq;
using Spark.Infrastructure.Configuration;
using Spark.Infrastructure.EventStore;
using Spark.Infrastructure.Eventing;
using Spark.Infrastructure.Messaging;
using Xunit;

/* Copyright (c) 2012 Spark Software Ltd.
 * 
 * This source is subject to the GNU Lesser General Public License.
 * See: http://www.gnu.org/copyleft/lesser.html
 * 
 * The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
 * 
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, 
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER 
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS 
 * IN THE SOFTWARE. 
 */

namespace Spark.Infrastructure.Tests.Eventing
{
    public static class UsingEventDispatcher
    {
        public class WhenCreatingNewDispatcher
        {
            private readonly Mock<IPublishEvents> eventPublisher = new Mock<IPublishEvents>();
            private readonly Mock<IStoreEvents> eventStore = new Mock<IStoreEvents>();

            [Fact]
            public void EventStoreCannotBeNull()
            {
                var ex = Assert.Throws<ArgumentNullException>(() => new EventDispatcher(null, new Mock<IPublishEvents>().Object));

                Assert.Equal("eventStore", ex.ParamName);
            }

            [Fact]
            public void EventPublisherCannotBeNull()
            {
                var ex = Assert.Throws<ArgumentNullException>(() => new EventDispatcher(new Mock<IStoreEvents>().Object, null));

                Assert.Equal("eventPublisher", ex.ParamName);
            }

            [Fact]
            public void DoNotDispatchUndispatchedCommitsIfNotMarkingDispatched()
            {
                var e = new FakeEvent();
                var settings = new Mock<IStoreEventSettings>();
                var commit = new Commit(GuidStrategy.NewGuid(), GuidStrategy.NewGuid(), 1, HeaderCollection.Empty, new EventCollection(new Event[] { e }));

                settings.Setup(mock => mock.MarkDispatched).Returns(false);
                eventStore.Setup(mock => mock.GetUndispatched()).Returns(new[] { commit });

                Assert.DoesNotThrow(() => new EventDispatcher(eventStore.Object, eventPublisher.Object, settings.Object));

                eventPublisher.Verify(mock => mock.Publish(HeaderCollection.Empty, It.IsAny<EventEnvelope>()), Times.Never());
            }

            [Fact]
            public void DispatchUndispatchedCommitsIfMarkingDispatched()
            {
                var e = new FakeEvent();
                var settings = new Mock<IStoreEventSettings>();
                var commit = new Commit(GuidStrategy.NewGuid(), GuidStrategy.NewGuid(), 1, HeaderCollection.Empty, new EventCollection(new Event[] { e }));

                settings.Setup(mock => mock.MarkDispatched).Returns(true);
                eventStore.Setup(mock => mock.GetUndispatched()).Returns(new[] { commit });

                Assert.DoesNotThrow(() => new EventDispatcher(eventStore.Object, eventPublisher.Object, settings.Object));

                eventPublisher.Verify(mock => mock.Publish(HeaderCollection.Empty, It.IsAny<EventEnvelope>()), Times.Once());
            }

            protected sealed class FakeEvent : Event
            { }
        }

        public class OnPostSave
        {
            private readonly Mock<IPublishEvents> eventPublisher = new Mock<IPublishEvents>();
            private readonly Mock<IStoreEvents> eventStore = new Mock<IStoreEvents>();

            [Fact]
            public void AggregateCanBeNull()
            {
                var eventDispatcher = new EventDispatcher(eventStore.Object, eventPublisher.Object);

                Assert.DoesNotThrow(() => eventDispatcher.PostSave(null, new Commit(GuidStrategy.NewGuid(), GuidStrategy.NewGuid(), 1, HeaderCollection.Empty, EventCollection.Empty), null));
            }

            [Fact]
            public void CommitCanBeNull()
            {
                var eventDispatcher = new EventDispatcher(eventStore.Object, eventPublisher.Object);

                eventDispatcher.PostSave(null, null, null);
                eventPublisher.Verify(mock => mock.Publish(HeaderCollection.Empty, It.IsAny<EventEnvelope>()), Times.Never());
            }

            [Fact]
            public void DoNotDispatchCommitWithNoId()
            {
                var commit = new Commit(GuidStrategy.NewGuid(), GuidStrategy.NewGuid(), 1, HeaderCollection.Empty, EventCollection.Empty);
                var eventDispatcher = new EventDispatcher(eventStore.Object, eventPublisher.Object);

                eventDispatcher.PostSave(null, commit, null);

                eventPublisher.Verify(mock => mock.Publish(HeaderCollection.Empty, It.IsAny<EventEnvelope>()), Times.Never());
            }

            [Fact]
            public void DispatchCommitWithId()
            {
                var commit = new Commit(GuidStrategy.NewGuid(), GuidStrategy.NewGuid(), 1, HeaderCollection.Empty, new EventCollection(new Event[] { new FakeEvent() }));
                var eventDispatcher = new EventDispatcher(eventStore.Object, eventPublisher.Object);

                commit.Id = 1;
                eventDispatcher.PostSave(null, commit, null);

                eventPublisher.Verify(mock => mock.Publish(HeaderCollection.Empty, It.IsAny<EventEnvelope>()), Times.Once());
            }

            [Fact]
            public void MarkDispatchedIfEnabled()
            {
                var settings = new Mock<IStoreEventSettings>();
                var commit = new Commit(GuidStrategy.NewGuid(), GuidStrategy.NewGuid(), 1, HeaderCollection.Empty, new EventCollection(new Event[] { new FakeEvent() }));

                settings.Setup(mock => mock.MarkDispatched).Returns(true);

                var eventDispatcher = new EventDispatcher(eventStore.Object, eventPublisher.Object, settings.Object);

                commit.Id = 1;
                eventDispatcher.PostSave(null, commit, null);

                eventStore.Verify(mock => mock.MarkDispatched(commit.Id.GetValueOrDefault()), Times.Once());
            }

            [Fact]
            public void DoNoMarkDispatchedIfDisabled()
            {
                var settings = new Mock<IStoreEventSettings>();
                var commit = new Commit(GuidStrategy.NewGuid(), GuidStrategy.NewGuid(), 1, HeaderCollection.Empty, new EventCollection(new Event[] { new FakeEvent() }));

                settings.Setup(mock => mock.MarkDispatched).Returns(false);

                var eventDispatcher = new EventDispatcher(eventStore.Object, eventPublisher.Object, settings.Object);

                commit.Id = 1;
                eventDispatcher.PostSave(null, commit, null);

                eventStore.Verify(mock => mock.MarkDispatched(commit.Id.GetValueOrDefault()), Times.Never());
            }

            protected sealed class FakeEvent : Event
            { }
        }
    }
}
