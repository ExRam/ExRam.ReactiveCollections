using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;
using System.Threading.Tasks;
using FluentAssertions;
using Xunit;

namespace ExRam.ReactiveCollections.Tests
{
    public class DictionaryReactiveCollectionSourceTest
    {
        [Fact]
        public async Task First_notification_is_reset()
        {
            var list = new DictionaryReactiveCollectionSource<string, int>();

            var notification = await list.ReactiveCollection.Changes
                .FirstAsync()
                .ToTask();

            notification.Action.Should().Be(NotifyCollectionChangedAction.Reset);
            notification.OldItems.Should().BeEmpty();
            notification.NewItems.Should().BeEmpty();
            notification.Current.Should().BeEmpty();
        }

        [Fact]
        public async Task Add()
        {
            var list = new DictionaryReactiveCollectionSource<string, int>();

            var notificationTask = list.ReactiveCollection.Changes
                .Skip(1)
                .FirstAsync()
                .ToTask();

            list.Add("Key", 1);

            var notification = await notificationTask;

            notification.Action.Should().Be(NotifyCollectionChangedAction.Add);
            notification.OldItems.Should().BeEmpty();
            notification.NewItems.Should().Equal(new KeyValuePair<string, int>("Key", 1));
            notification.Current.Should().Contain("Key", 1);
        }

        [Fact]
        public async Task Add_null()
        {
            var list = new DictionaryReactiveCollectionSource<string, string?>();

            var notificationTask = list.ReactiveCollection.Changes
                .Skip(1)
                .FirstAsync()
                .ToTask();

            list.Add("Key", null);

            var notification = await notificationTask;

            notification.Action.Should().Be(NotifyCollectionChangedAction.Add);
            notification.OldItems.Should().BeEmpty();
            notification.NewItems.Should().Equal(new KeyValuePair<string, string?>("Key", null));
            notification.Current.Should().Contain("Key", null);
        }

        [Fact]
        public void Adding_existing_key_throws()
        {
            Assert.Throws<ArgumentException>(
                () =>
                {
                    // ReSharper disable ObjectCreationAsStatement
                    new DictionaryReactiveCollectionSource<string, int>
                    {
                        { "Key", 1 },
                        { "Key", 2 }
                    };
                    // ReSharper restore ObjectCreationAsStatement
                });
        }

        [Fact]
        public async Task AddRange1()
        {
            var list = new DictionaryReactiveCollectionSource<string, int>();

            var notificationTask = list.ReactiveCollection.Changes
                .Skip(1)
                .FirstAsync()
                .ToTask();

            var range = new Dictionary<string, int>
            {
                { "Key1", 1 },
                { "Key2", 2 },
                { "Key3", 3 }
            };

            list.AddRange(range);

            var notification = await notificationTask;

            notification.Action.Should().Be(NotifyCollectionChangedAction.Add);
            notification.NewItems.Should().BeEquivalentTo(range);
            notification.OldItems.Should().BeEmpty();
            notification.Current.Should().Equal(range);
        }

        [Fact]
        public async Task AddRange2()
        {
            var list = new DictionaryReactiveCollectionSource<int, string>();

            var notificationTask = list.ReactiveCollection.Changes
                .Skip(1)
                .FirstAsync()
                .ToTask();

            var range = new[]
            {
                "A",
                "BB",
                "CCC"
            };

            list.AddRange(range, x => x.Length);

            var notification = await notificationTask;

            notification.Action.Should().Be(NotifyCollectionChangedAction.Add);
            notification.NewItems.Should().HaveCount(3);
            notification.OldItems.Should().BeEmpty();
            notification.Current.Should()
                .Contain(1, "A").And
                .Contain(2, "BB").And
                .Contain(3, "CCC");
        }

        [Fact]
        public void Count_reflects_actual_count()
        {
            var list = new DictionaryReactiveCollectionSource<string, int>
            {
                { "Key1", 1 },
                { "Key2", 2 },
                { "Key3", 3 }
            };

            list.Should().HaveCount(3);
        }

        [Fact]
        public async Task Clear()
        {
            var list = new DictionaryReactiveCollectionSource<string, int>();

            var notificationTask = list.ReactiveCollection.Changes
                .Skip(2)
                .FirstAsync()
                .ToTask();

            list.Add("Key", 1);
            list.Clear();

            var notification = await notificationTask;

            notification.Action.Should().Be(NotifyCollectionChangedAction.Reset);
            notification.NewItems.Should().BeEmpty();
            notification.OldItems.Should().BeEmpty();
            notification.Current.Should().BeEmpty();
        }

        [Fact]
        public void Contains()
        {
            var dict = new DictionaryReactiveCollectionSource<string, int>
            {
                { "Key", 1 }
            };

            dict.Contains(new KeyValuePair<string, int>("Key", 1)).Should().BeTrue();
            dict.Contains(new KeyValuePair<string, int>("Key1", 2)).Should().BeFalse();
        }

        [Fact]
        public async Task Remove()
        {
            var list = new DictionaryReactiveCollectionSource<string, int>();

            var notificationTask = list.ReactiveCollection.Changes
                .Skip(2)
                .FirstAsync()
                .ToTask();

            list.Add("Key1", 1);
            list.Remove("Key1");

            var notification = await notificationTask;

            notification.Action.Should().Be(NotifyCollectionChangedAction.Remove);
            notification.NewItems.Should().BeEmpty();
            notification.OldItems.Should().Equal(new KeyValuePair<string, int>("Key1", 1));
            notification.Current.Should().BeEmpty();
        }

        [Fact]
        public async Task RemoveRange()
        {
            var list = new DictionaryReactiveCollectionSource<string, int>();

            var notificationsTask = list.ReactiveCollection.Changes
                .Skip(2)
                .Take(2)
                .ToArray()
                .ToTask();

            list.AddRange(new[]
            {
                new KeyValuePair<string, int>("Key1", 1),
                new KeyValuePair<string, int>("Key2", 2),
                new KeyValuePair<string, int>("Key3", 3)
            });

            list.RemoveRange(new[]
            {
                "Key1",
                "Key2"
            });

            var notifications = await notificationsTask;

            notifications[0].Action.Should().Be(NotifyCollectionChangedAction.Remove);
            notifications[0].OldItems.Should().Equal(new KeyValuePair<string, int>("Key1", 1));
            notifications[1].Action.Should().Be(NotifyCollectionChangedAction.Remove);
            notifications[1].OldItems.Should().Equal(new KeyValuePair<string, int>("Key2", 2));
            notifications[1].Current.Should().Equal(new Dictionary<string, int>
            {
                { "Key3", 3 }
            });
        }

        [Fact]
        public async Task SetItem()
        {
            var list = new DictionaryReactiveCollectionSource<string, int>();

            var notificationTask = list.ReactiveCollection.Changes
                .Skip(2)
                .FirstAsync()
                .ToTask();

            var range = new Dictionary<string, int>
            {
                { "Key1", 1 },
                { "Key2", 2 },
                { "Key3", 3 }
            };

            list.AddRange(range);

            list.SetItem("Key2", 3);

            var notification = await notificationTask;

            notification.Action.Should().Be(NotifyCollectionChangedAction.Replace);
            notification.OldItems.Should().HaveCount(1);
            notification.OldItems.Should().Contain(new KeyValuePair<string, int>("Key2", 2));
            notification.NewItems.Should().Contain(new KeyValuePair<string, int>("Key2", 3));
            notification.Current.Should().Equal(new Dictionary<string, int>
            {
                { "Key1", 1 },
                { "Key2", 3 },
                { "Key3", 3 }
            });
        }

        [Fact]
        public async Task SetItems()
        {
            var list = new DictionaryReactiveCollectionSource<string, int>();

            var notificationTask = list.ReactiveCollection.Changes
                .Skip(2)
                .FirstAsync()
                .ToTask();

            list.AddRange(new[]
            {
                new KeyValuePair<string, int>("Key1", 1),
                new KeyValuePair<string, int>("Key2", 2),
                new KeyValuePair<string, int>("Key3", 3)
            });

            list.SetItems(new[]
            {
                new KeyValuePair<string, int>("Key1", 4),
                new KeyValuePair<string, int>("Key2", 5),
                new KeyValuePair<string, int>("Key3", 6)
            });

            var notification = await notificationTask;

            notification.Action.Should().Be(NotifyCollectionChangedAction.Reset);
            notification.Current.Should().Equal(new Dictionary<string, int>
            {
                { "Key1", 4 },
                { "Key2", 5 },
                { "Key3", 6 }
            });
        }

        [Fact]
        public void TryGetValue()
        {
            var list = new DictionaryReactiveCollectionSource<string, int>
            {
                { "Key1", 1 }
            };

            int value;

            list.TryGetValue("Key1", out value).Should().BeTrue();
            value.Should().Be(1);
            list.TryGetValue("Key2", out value).Should().BeFalse();
        }

        [Fact]
        public void Item()
        {
            var list = new DictionaryReactiveCollectionSource<string, int>
            {
                { "Key1", 1 }
            };

            list["Key1"].Should().Be(1);
            list["Key1"] = 2;
            list["Key1"].Should().Be(2);
        }

        [Fact]
        public void Item2()
        {
            var list = new DictionaryReactiveCollectionSource<string, int>
            {
                { "Key1", 1 }
            };

            Assert.Throws<KeyNotFoundException>(() =>
            {
                var v = list["Key"];
            });
        }

        [Fact]
        public void GetEnumerator()
        {
            var list = new DictionaryReactiveCollectionSource<string, int>
            {
                { "Key1", 1 }
            };

            using (var enumerator = list.GetEnumerator())
            {
                enumerator.MoveNext().Should().BeTrue();
                enumerator.Current.Should().Be(new KeyValuePair<string, int>("Key1", 1));
                enumerator.MoveNext().Should().BeFalse();
            }
        }
    }
}
