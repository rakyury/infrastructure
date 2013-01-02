﻿using Spark.Infrastructure.Threading;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
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

namespace Spark.Infrastructure.Tests.Threading
{
    public static class BlockingThreadPoolTaskSchedulerTests
    {
        public class WhenInitializing
        {
            [Fact]
            public void SetMaximumConcurrencyLevelToNumberOfWorkerThreads()
            {
                Int32 workerThreads, completionPortThreads;
                ThreadPool.GetMaxThreads(out workerThreads, out completionPortThreads);

                Assert.Equal(workerThreads, new BlockingThreadPoolTaskScheduler().MaximumConcurrencyLevel);
            }

            [Fact]
            public void DefaultMaximumQueuedTasksToFiveTimesNumberOfWorkerThreads()
            {
                Int32 workerThreads, completionPortThreads;
                ThreadPool.GetMaxThreads(out workerThreads, out completionPortThreads);

                Assert.Equal(workerThreads * 5, new BlockingThreadPoolTaskScheduler().BoundedCapacity);
            }
        }

        public class WhenQueuingTask
        {
            [Fact]
            public void QueueImmediatelyIfBelowBoundedCapacity()
            {
                var taskScheduler = new BlockingThreadPoolTaskScheduler(1);
                var task = Task.Factory.StartNew(() =>
                    {
                        Task.Factory.StartNew(() => { }, CancellationToken.None, TaskCreationOptions.AttachedToParent, taskScheduler);
                    });

                Assert.True(task.Wait(TimeSpan.FromMilliseconds(100)));
                Assert.Equal(0, taskScheduler.ScheduledTasks.Count());
            }

            [Fact]
            public void BlockQueueUntilBelowBoundedCapacity()
            {
                var monitor = new FakeMonitor();
                var threadPool = new FakeThreadPool();
                var blocked = new ManualResetEvent(false);
                var released = new ManualResetEvent(false);
                var taskScheduler = new BlockingThreadPoolTaskScheduler(1, threadPool, monitor);

                monitor.BeforeWait = () => blocked.Set();
                monitor.AfterPulse = () => released.Set();

                Task.Factory.StartNew(() =>
                    {
                        // Schedule first task (non-blocking).
                        Task.Factory.StartNew(() => { }, CancellationToken.None, TaskCreationOptions.AttachedToParent, taskScheduler);

                        // Schedule second task (blocking).
                        Task.Factory.StartNew(() => { }, CancellationToken.None, TaskCreationOptions.AttachedToParent, taskScheduler);
                    });

                // Wait for second task to be blocked.
                Assert.True(blocked.WaitOne(TimeSpan.FromMilliseconds(100)));
                Assert.Equal(1, threadPool.UserWorkItems.Count);

                threadPool.RunNext();

                // Wait for second task to be released.
                Assert.True(released.WaitOne(TimeSpan.FromMilliseconds(100)));

                threadPool.RunNext();

                Assert.Equal(0, taskScheduler.ScheduledTasks.Count());
            }

            [Fact]
            public void AllowsInlineExecutionAfterBeingQueued()
            {
                var executions = 0;
                var monitor = new FakeMonitor();
                var threadPool = new FakeThreadPool();
                var taskScheduler = new BlockingThreadPoolTaskScheduler(1, threadPool, monitor);

                Task.Factory.StartNew(() => { executions++; }, CancellationToken.None, TaskCreationOptions.AttachedToParent, taskScheduler);

                threadPool.RunNext(2);

                Assert.Equal(1, executions);
                Assert.Equal(0, taskScheduler.ScheduledTasks.Count());
            }
        }

        public class WhenRunningTaskSynchronously
        {
            [Fact]
            public void AllowsInlineExecutionAfterBeingQueued2()
            {
                var executions = 0;
                var taskScheduler = new BlockingThreadPoolTaskScheduler();
                var task = new Task(() => { executions++; }, CancellationToken.None, TaskCreationOptions.AttachedToParent);

                task.RunSynchronously(taskScheduler);

                Assert.Equal(1, executions);
                Assert.Equal(0, taskScheduler.ScheduledTasks.Count());
            }
        }
    }
}
