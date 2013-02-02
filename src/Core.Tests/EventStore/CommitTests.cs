﻿using System;
using Spark.Infrastructure.EventStore;
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

namespace Spark.Infrastructure.Tests.EventStore
{
    public static class UsingCommit
    {
        public class WhenCreatingNewCommit
        {
            [Fact]
            public void CommitIdCannotBeEmptyGuid()
            {
                var ex = Assert.Throws<ArgumentException>(() => new Commit(Guid.NewGuid(), 1, Guid.Empty, null, null));

                Assert.Equal("commitId", ex.ParamName);
            }

            [Fact]
            public void StreamIdCannotBeEmptyGuid()
            {
                var ex = Assert.Throws<ArgumentException>(() => new Commit(Guid.Empty, 1, Guid.NewGuid(), null, null));

                Assert.Equal("streamId", ex.ParamName);
            }

            [Fact]
            public void RevisionGreaterThanZero()
            {
                var ex = Assert.Throws<ArgumentOutOfRangeException>(() => new Commit(Guid.NewGuid(), 0, Guid.NewGuid(), null, null));

                Assert.Equal("revision", ex.ParamName);
            }

            [Fact]
            public void HeadersCannotBeNull()
            {
                var commit = new Commit(Guid.NewGuid(), 1, Guid.NewGuid(), null, null);

                Assert.NotNull(commit.Headers);
            }

            [Fact]
            public void EventsCannotBeNull()
            {
                var commit = new Commit(Guid.NewGuid(), 1, Guid.NewGuid(), null, null);

                Assert.NotNull(commit.Events);
            }
        }
    }
}