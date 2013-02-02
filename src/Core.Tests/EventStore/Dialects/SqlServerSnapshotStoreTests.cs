﻿using System;
using System.Data.SqlClient;
using Spark.Infrastructure.EventStore;
using Spark.Infrastructure.EventStore.Dialects;
using Spark.Infrastructure.Serialization;
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

namespace Spark.Infrastructure.Tests.EventStore.Dialects
{
    public static class UsingSnapshotStoreWithSqlServer
    {
        public abstract class UsingInitializedSnapshotStore : IDisposable
        {
            protected readonly IStoreSnapshots SnapshotStore;

            protected UsingInitializedSnapshotStore()
            {
                SnapshotStore = new DbSnapshotStore(SqlServerConnection.Name, new BinarySerializer(), new SqlServerDialect(5));
                SnapshotStore.Initialize();
            }

            public void Dispose()
            {
                SnapshotStore.Purge();
            }
        }

        public class WhenInitializingSnapshotStore
        {
            [SqlServerFactAttribute]
            public void WillCreateTableIfDoesNotExist()
            {
                var snapshotStore = new DbSnapshotStore(SqlServerConnection.Name, new BinarySerializer());

                DropExistingTable();

                snapshotStore.Initialize();

                Assert.True(TableExists());
            }

            [SqlServerFactAttribute]
            public void WillNotTouchTableIfExists()
            {
                var snapshotStore = new DbSnapshotStore(SqlServerConnection.Name, new BinarySerializer());

                snapshotStore.Initialize();
                snapshotStore.Initialize();

                Assert.True(TableExists());
            }

            private void DropExistingTable()
            {
                using (var connection = SqlServerConnection.Create())
                using (var command = new SqlCommand("IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA = 'dbo' AND TABLE_NAME = 'Snapshot') DROP TABLE [dbo].[Snapshot];", connection))
                {
                    connection.Open();
                    command.ExecuteNonQuery();
                }
            }

            private Boolean TableExists()
            {
                using (var connection = SqlServerConnection.Create())
                using (var command = new SqlCommand("SELECT COUNT(*) FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA = 'dbo' AND TABLE_NAME = 'Snapshot';", connection))
                {
                    connection.Open();

                    return Equals(command.ExecuteScalar(), 1);
                }
            }
        }

        public class WhenSavingSnapshot : UsingInitializedSnapshotStore
        {
            [SqlServerFactAttribute]
            public void ThrowConcurrencyExceptionIfStreamVersionExists()
            {
                var snapshot = new Snapshot(Guid.NewGuid(), 1, new Object());

                SnapshotStore.SaveSnapshot(snapshot);

                Assert.Throws<ConcurrencyException>(() => SnapshotStore.SaveSnapshot(snapshot));
            }

            [SqlServerFactAttribute]
            public void SaveSnapshotfNextVersionInStream()
            {
                var streamId = Guid.NewGuid();
                var snapshot1 = new Snapshot(streamId, 1, new Object());
                var snapshot2 = new Snapshot(streamId, 2, new Object());

                SnapshotStore.SaveSnapshot(snapshot1);
                SnapshotStore.SaveSnapshot(snapshot2);
            }
        }

        public class WhenReplacingSnapshot : UsingInitializedSnapshotStore
        {
            [SqlServerFactAttribute]
            public void RemoveOldSnapshotVersions()
            {
                var streamId = Guid.NewGuid();
                var snapshot1 = new Snapshot(streamId, 1, new Object());
                var snapshot2 = new Snapshot(streamId, 2, new Object());

                SnapshotStore.SaveSnapshot(snapshot1);
                SnapshotStore.ReplaceSnapshot(snapshot2);

                Assert.Equal(1, CountSnapshots(streamId));
            }

            [SqlServerFactAttribute]
            public void NoPreviousVersionRequired()
            {
                var streamId = Guid.NewGuid();
                var snapshot = new Snapshot(streamId, 1, new Object());

                SnapshotStore.SaveSnapshot(snapshot);

                Assert.Equal(1, CountSnapshots(streamId));
            }

            private Int32 CountSnapshots(Guid streamId)
            {
                using (var connection = SqlServerConnection.Create())
                using (var command = new SqlCommand("SELECT COUNT(*) FROM [dbo].[Snapshot] WHERE [StreamId] = @StreamId;", connection))
                {
                    command.Parameters.AddWithValue("@StreamId", streamId);
                    connection.Open();

                    return (Int32) command.ExecuteScalar();
                }
            }
        }

        public class WhenPurgingSnapshots : UsingInitializedSnapshotStore
        {
            [SqlServerFactAttribute]
            public void DeleteAllSnapshots()
            {
                var snapshot1 = new Snapshot(Guid.NewGuid(), 1, new Object());
                var snapshot2 = new Snapshot(Guid.NewGuid(), 1, new Object());

                SnapshotStore.SaveSnapshot(snapshot1);
                SnapshotStore.SaveSnapshot(snapshot2);
                SnapshotStore.Purge();

                Assert.Equal(0, CountSnapshots());
            }

            private Int32 CountSnapshots()
            {
                using (var connection = SqlServerConnection.Create())
                using (var command = new SqlCommand("SELECT COUNT(*) FROM [dbo].[Snapshot];", connection))
                {
                    connection.Open();

                    return (Int32)command.ExecuteScalar();
                }
            }
        }

        public class WhenGettingLastSnapshot : UsingInitializedSnapshotStore
        {
            [SqlServerFactAttribute]
            public void ReturnHighestVersion()
            {
                var streamId = Guid.NewGuid();
                var snapshot1 = new Snapshot(streamId, 1, new Object());
                var snapshot2 = new Snapshot(streamId, 10, new Object());
                var snapshot3 = new Snapshot(streamId, 20, new Object());

                SnapshotStore.SaveSnapshot(snapshot1);
                SnapshotStore.SaveSnapshot(snapshot2);
                SnapshotStore.SaveSnapshot(snapshot3);

                Assert.Equal(20, SnapshotStore.GetLastSnapshot(streamId).Version);
            }
        }

        public class WhenGettingSnapshotVersion : UsingInitializedSnapshotStore
        {
            [SqlServerFactAttribute]
            public void ReturnImmediatelyPreceedingVersion()
            {
                var streamId = Guid.NewGuid();
                var snapshot1 = new Snapshot(streamId, 1, new Object());
                var snapshot2 = new Snapshot(streamId, 10, new Object());

                SnapshotStore.SaveSnapshot(snapshot1);
                SnapshotStore.SaveSnapshot(snapshot2);

                Assert.Equal(10, SnapshotStore.GetSnapshot(streamId, 20).Version);
            }

            [SqlServerFactAttribute]
            public void ReturnExactMatch()
            {
                var streamId = Guid.NewGuid();
                var snapshot1 = new Snapshot(streamId, 1, new Object());
                var snapshot2 = new Snapshot(streamId, 10, new Object());

                SnapshotStore.SaveSnapshot(snapshot1);
                SnapshotStore.SaveSnapshot(snapshot2);

                Assert.Equal(10, SnapshotStore.GetSnapshot(streamId, 10).Version);
            }

            [SqlServerFactAttribute]
            public void ReturnNullIfNoSnapshots()
            {
                var streamId = Guid.NewGuid();

                Assert.Null(SnapshotStore.GetSnapshot(streamId, 10));
            }
        }
    }
}