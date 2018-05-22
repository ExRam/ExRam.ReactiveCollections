using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Collections.Specialized;
using System.Globalization;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Reactive.Threading.Tasks;
using System.Threading.Tasks;
using FluentAssertions;
using Xunit;

namespace ExRam.ReactiveCollections.Tests
{
    public class ReactiveCollectionExtensionsTest
    {
        [Fact]
        public async Task Add_to_projected_list()
        {
            var list = new ListReactiveCollectionSource<int>();

            var projectedList = list.ReactiveCollection
                .Select(x => x.ToString(CultureInfo.InvariantCulture));

            var notificationsTask = projectedList.Changes
                .Take(4)
                .ToArray()
                .ToTask();

            list.Add(1);
            list.Add(2);
            list.Add(3);

            var notifications = await notificationsTask;

            notifications[0].Index.Should().NotHaveValue();
            notifications[1].Index.Should().Be(0);
            notifications[2].Index.Should().Be(1);
            notifications[3].Index.Should().Be(2);
            notifications[0].Action.Should().Be(NotifyCollectionChangedAction.Reset);
            notifications[1].Action.Should().Be(NotifyCollectionChangedAction.Add);
            notifications[2].Action.Should().Be(NotifyCollectionChangedAction.Add);
            notifications[3].Action.Should().Be(NotifyCollectionChangedAction.Add);
            notifications[0].NewItems.Should().BeEmpty();
            notifications[1].NewItems.Should().Equal("1");
            notifications[2].NewItems.Should().Equal("2");
            notifications[3].NewItems.Should().Equal("3");
        }

        [Fact]
        public async Task Add_to_projected_set()
        {
            var list = new SortedSetReactiveCollectionSource<int>();

            var projectedList = list.ReactiveCollection
                .Select(x => x.ToString(CultureInfo.InvariantCulture));

            var notificationsTask = projectedList.Changes
                .Take(4)
                .ToArray()
                .ToTask();

            list.Add(1);
            list.Add(2);
            list.Add(3);

            var notifications = await notificationsTask;

            notifications[0].Index.Should().NotHaveValue();
            notifications[1].Index.Should().Be(0);
            notifications[2].Index.Should().Be(1);
            notifications[3].Index.Should().Be(2);
            notifications[0].Action.Should().Be(NotifyCollectionChangedAction.Reset);
            notifications[1].Action.Should().Be(NotifyCollectionChangedAction.Add);
            notifications[2].Action.Should().Be(NotifyCollectionChangedAction.Add);
            notifications[3].Action.Should().Be(NotifyCollectionChangedAction.Add);
            notifications[0].NewItems.Should().BeEmpty();
            notifications[1].NewItems.Should().Equal("1");
            notifications[2].NewItems.Should().Equal("2");
            notifications[3].NewItems.Should().Equal("3");
        }

        [Fact]
        public async Task Add_to_projected_dictionary()
        {
            var list = new DictionaryReactiveCollectionSource<string, int>();

            var projectedList = list.ReactiveCollection
                .Select(x => x.ToString(CultureInfo.InvariantCulture));

            var notificationsTask = projectedList.Changes
                .Take(4)
                .ToArray()
                .ToTask();

            list.Add("Key1", 1);
            list.Add("Key2", 2);
            list.Add("Key3", 3);

            var notifications = await notificationsTask;

            notifications[0].Action.Should().Be(NotifyCollectionChangedAction.Reset);
            notifications[1].Action.Should().Be(NotifyCollectionChangedAction.Add);
            notifications[2].Action.Should().Be(NotifyCollectionChangedAction.Add);
            notifications[3].Action.Should().Be(NotifyCollectionChangedAction.Add);
            notifications[0].NewItems.Should().BeEmpty();
            notifications[1].NewItems.Should().Equal(new KeyValuePair<string, string>("Key1", "1"));
            notifications[2].NewItems.Should().Equal(new KeyValuePair<string, string>("Key2", "2"));
            notifications[3].NewItems.Should().Equal(new KeyValuePair<string, string>("Key3", "3"));
            notifications[3].Current.Should().Equal(new Dictionary<string, string>
            {
                { "Key1", "1" },
                { "Key2", "2" },
                { "Key3", "3" }
            });
        }

        [Fact]
        public async Task Remove_from_projected_list()
        {
            var list = new ListReactiveCollectionSource<int>();

            var projectedList = list.ReactiveCollection
                .Select(x => x.ToString(CultureInfo.InvariantCulture));

            var notificationTask = projectedList.Changes
                .Skip(2)
                .FirstAsync()
                .ToTask();

            list.AddRange(new[] { 1, 2, 3 });
            list.Remove(2);

            var notification = await notificationTask;

            notification.Index.Should().Be(1);
            notification.Action.Should().Be(NotifyCollectionChangedAction.Remove);
            notification.OldItems.Should().Equal("2");
            notification.NewItems.Should().BeEmpty();
            notification.Current.Should().Equal("1", "3");
        }

        [Fact]
        public async Task Remove_from_projected_set()
        {
            var list = new SortedSetReactiveCollectionSource<int>();

            var projectedList = list.ReactiveCollection
                .Select(x => x.ToString(CultureInfo.InvariantCulture));

            var notificationTask = projectedList.Changes
                .Skip(4)
                .FirstAsync()
                .ToTask();

            list.Add(1);
            list.Add(2);
            list.Add(3);
            list.Remove(2);

            var notification = await notificationTask;

            notification.Index.Should().Be(1);
            notification.Action.Should().Be(NotifyCollectionChangedAction.Remove);
            notification.NewItems.Should().BeEmpty();
            notification.OldItems.Should().Equal("2");
            notification.Current.Should().Equal("1", "3");
        }

        [Fact]
        public async Task Remove_from_projected_dictionary()
        {
            var list = new DictionaryReactiveCollectionSource<string, int>();

            var projectedList = list.ReactiveCollection
                .Select(x => x.ToString(CultureInfo.InvariantCulture));

            var notificationTask = projectedList.Changes
                .Skip(4)
                .FirstAsync()
                .ToTask();

            list.Add("Key1", 1);
            list.Add("Key2", 2);
            list.Add("Key3", 3);
            list.Remove("Key2");

            var notification = await notificationTask;

            notification.Action.Should().Be(NotifyCollectionChangedAction.Remove);
            notification.OldItems.Should().Equal(new Dictionary<string, string> { { "Key2", "2" } });
            notification.NewItems.Should().BeEmpty();
            notification.Current.Should().Equal(new Dictionary<string, string> { { "Key1", "1" }, { "Key3", "3" }});
        }

        [Fact]
        public async Task Replace_in_projected_list()
        {
            var list = new ListReactiveCollectionSource<int>();

            var projectedList = list.ReactiveCollection
                .Select(x => x.ToString(CultureInfo.InvariantCulture));

            var notificationTask = projectedList.Changes
                .Skip(2)
                .FirstAsync()
                .ToTask();

            list.AddRange(new[] { 1, 2, 3 });
            list.Replace(2, 4);

            var notification = await notificationTask;

            notification.Index.Should().Be(1);
            notification.Action.Should().Be(NotifyCollectionChangedAction.Replace);
            notification.OldItems.Should().Equal("2");
            notification.NewItems.Should().Equal("4");
            notification.Current.Should().Equal("1", "4", "3");
        }

        [Fact]
        public async Task Replace_in_projected_from_ListChangedNotification_observable()
        {
            var observable = new[]
                {
                    new ListChangedNotification<int>(ImmutableList<int>.Empty, NotifyCollectionChangedAction.Reset, ImmutableList<int>.Empty, ImmutableList<int>.Empty, null),
                    new ListChangedNotification<int>(ImmutableList.Create(1, 2), NotifyCollectionChangedAction.Add, ImmutableList<int>.Empty, ImmutableList.Create(1, 2), 0),
                    new ListChangedNotification<int>(ImmutableList.Create(3, 4), NotifyCollectionChangedAction.Replace, ImmutableList.Create(1, 2), ImmutableList.Create(3, 4), 0)
                }
                .ToObservable()
                .ToReactiveCollection();

            var projectedList = observable.Select(x => x.ToString(CultureInfo.InvariantCulture));

            var notificationsTask = projectedList.Changes
                .Skip(2)
                .Take(2)
                .ToArray()
                .ToTask();

            var notifications = await notificationsTask;

            notifications[0].Index.Should().Be(0);
            notifications[0].NewItems.Should().BeEmpty();
            notifications[0].Action.Should().Be(NotifyCollectionChangedAction.Remove);
            notifications[0].OldItems.Should().Equal("1", "2");
            notifications[1].Index.Should().Be(0);
            notifications[1].OldItems.Should().BeEmpty();
            notifications[1].Action.Should().Be(NotifyCollectionChangedAction.Add);
            notifications[1].NewItems.Should().Equal("3", "4");
            notifications[1].Current.Should().Equal("3", "4");
        }

        [Fact]
        public async Task Replace_in_projected_from_SortedSetChangedNotification_observable()
        {
            var observable = new[]
                {
                    new SortedSetChangedNotification<int>(ImmutableSortedSet<int>.Empty, NotifyCollectionChangedAction.Reset, ImmutableList<int>.Empty, ImmutableList<int>.Empty, null),
                    new SortedSetChangedNotification<int>(ImmutableSortedSet.Create(1, 2), NotifyCollectionChangedAction.Add, ImmutableList<int>.Empty, ImmutableList.Create(1, 2), 0),
                    new SortedSetChangedNotification<int>(ImmutableSortedSet.Create(3, 4), NotifyCollectionChangedAction.Replace, ImmutableList.Create(1, 2), ImmutableList.Create(3, 4), 0)
                }
                .ToObservable()
                .ToReactiveCollection();

            var projectedList = observable.Select(x => x.ToString(CultureInfo.InvariantCulture));

            var notificationsTask = projectedList.Changes
                .Skip(2)
                .Take(2)
                .ToArray()
                .ToTask();

            var notifications = await notificationsTask;

            notifications[0].Index.Should().Be(0);
            notifications[0].OldItems.Should().Equal("1", "2");
            notifications[0].NewItems.Should().BeEmpty();
            notifications[0].Action.Should().Be(NotifyCollectionChangedAction.Remove);
            notifications[1].Index.Should().Be(0);
            notifications[1].OldItems.Should().BeEmpty();
            notifications[1].NewItems.Should().Equal("3", "4");
            notifications[1].Action.Should().Be(NotifyCollectionChangedAction.Add);
            notifications[1].Current.Should().Equal("3", "4");
        }

        [Fact]
        public async Task Replace_in_projected_dictionary()
        {
            var list = new DictionaryReactiveCollectionSource<string, int>();

            var projectedList = list.ReactiveCollection
                .Select(x => x.ToString(CultureInfo.InvariantCulture));

            var notificationsTask = projectedList.Changes
                .Skip(4)
                .Take(2)
                .ToArray()
                .ToTask();

            list.Add("Key1", 1);
            list.Add("Key2", 2);
            list.Add("Key3", 3);

            list["Key2"] = 4;

            var notifications = await notificationsTask;

            notifications[0].Action.Should().Be(NotifyCollectionChangedAction.Remove);
            notifications[0].OldItems.Should().Equal(new KeyValuePair<string, string>("Key2", "2"));
            notifications[1].NewItems.Should().Equal(new KeyValuePair<string, string>("Key2", "4"));
            notifications[1].Current.Should().Equal(new Dictionary<string, string>
            {
                { "Key1", "1" },
                { "Key2", "4" },
                { "Key3", "3" }
            });
        }

        [Fact]
        public async Task Add_to_filtered_list()
        {
            var list = new ListReactiveCollectionSource<int>();

            var projectedList = list
                .ReactiveCollection
                .Where(x => x % 2 == 0);

            var notificationTask = projectedList.Changes
                .Skip(1)
                .FirstAsync()
                .ToTask();

            list.Add(1);
            list.Add(3);           
            list.Add(2);

            var notification = await notificationTask;

            notification.Index.Should().Be(0);
            notification.Action.Should().Be(NotifyCollectionChangedAction.Add);
            notification.NewItems.Should().Equal(2);
        }

        [Fact]
        public async Task Add_to_filtered_set()
        {
            var list = new SortedSetReactiveCollectionSource<int>();

            var projectedList = list
                .ReactiveCollection
                .Where(x => x % 2 == 0);

            var notificationTask = projectedList.Changes
                .Skip(1)
                .FirstAsync()
                .ToTask();

            list.Add(1);
            list.Add(3);
            list.Add(2);

            var notification = await notificationTask;

            notification.Index.Should().Be(0);
            notification.Action.Should().Be(NotifyCollectionChangedAction.Add);
            notification.NewItems.Should().Equal(2);
        }

        [Fact]
        public async Task Add_to_filtered_dictionary()
        {
            var list = new DictionaryReactiveCollectionSource<string, int>();

            var projectedList = list
                .ReactiveCollection
                .Where(x => x % 2 == 0);

            var notificationTask = projectedList.Changes
                .Skip(1)
                .FirstAsync()
                .ToTask();

            list.Add("Key1", 1);
            list.Add("Key3", 3);
            list.Add("Key2", 2);

            var notification = await notificationTask;

            notification.Action.Should().Be(NotifyCollectionChangedAction.Add);
            notification.NewItems.Should().Equal(new KeyValuePair<string, int>("Key2", 2));
        }

        [Fact]
        public async Task Remove_from_filtered_list()
        {
            var list = new ListReactiveCollectionSource<int>();

            var projectedList = list
                .ReactiveCollection
                .Where(x => x % 2 == 0);

            var notificationTask = projectedList.Changes
                .Skip(2)
                .FirstAsync()
                .ToTask();

            list.AddRange(new[] { 1, 2, 3 });
            list.Remove(2);

            var notification = await notificationTask;

            notification.Index.Should().Be(0);
            notification.Action.Should().Be(NotifyCollectionChangedAction.Remove);
            notification.NewItems.Should().BeEmpty();
            notification.OldItems.Should().Equal(2);
            notification.Current.Should().BeEmpty();
        }

        [Fact]
        public async Task Remove_from_filtered_set()
        {
            var list = new SortedSetReactiveCollectionSource<int>();

            var projectedList = list
                .ReactiveCollection
                .Where(x => x % 2 == 0);

            var notificationTask = projectedList.Changes
                .Skip(2)
                .FirstAsync()
                .ToTask();

            list.Add(1);
            list.Add(2);
            list.Add(3);
            list.Remove(2);

            var notification = await notificationTask;

            notification.Index.Should().Be(0);
            notification.Action.Should().Be(NotifyCollectionChangedAction.Remove);
            notification.OldItems.Should().Equal(2);
            notification.NewItems.Should().BeEmpty();
            notification.Current.Should().BeEmpty();
        }

        [Fact]
        public async Task Remove_from_filtered_dictionary()
        {
            var list = new DictionaryReactiveCollectionSource<string, int>();

            var projectedList = list.ReactiveCollection
                .Where(x => x % 2 == 0);

            var notificationTask = projectedList.Changes
                .Skip(2)
                .FirstAsync()
                .ToTask();

            list.Add("Key1", 1);
            list.Add("Key2", 2);
            list.Add("Key3", 3);
            list.Remove("Key2");

            var notification = await notificationTask;

            notification.Action.Should().Be(NotifyCollectionChangedAction.Remove);
            notification.NewItems.Should().BeEmpty();
            notification.OldItems.Should().Equal(new KeyValuePair<string, int>("Key2", 2));
            notification.Current.Should().BeEmpty();
        }

        [Fact]
        public async Task Replace_in_filtered_list_addition()
        {
            var list = new ListReactiveCollectionSource<int>();

            var projectedList = list.ReactiveCollection
                .Where(x => x % 2 == 0);

            var notificationTask = projectedList.Changes
                .Skip(2)
                .FirstAsync()
                .ToTask();

            list.AddRange(new[] { 1, 2, 3 });
            list.Replace(1, 4);

            var notification = await notificationTask;

            notification.Index.Should().Be(1);
            notification.Action.Should().Be(NotifyCollectionChangedAction.Add);
            notification.OldItems.Should().BeEmpty();
            notification.NewItems.Should().Equal(4);
            notification.Current.Should().Equal(2, 4);
        }

        [Fact]
        public async Task Replace_in_filtered_list_removal()
        {
            var list = new ListReactiveCollectionSource<int>();

            var projectedList = list.ReactiveCollection
                .Where(x => x % 2 == 0);

            var notificationTask = projectedList.Changes
                .Skip(2)
                .FirstAsync()
                .ToTask();

            list.AddRange(new[] { 1, 2, 3 });
            list.Replace(2, 3);

            var notification = await notificationTask;

            notification.Index.Should().Be(0);
            notification.Action.Should().Be(NotifyCollectionChangedAction.Remove);
            notification.OldItems.Should().Equal(2);
            notification.NewItems.Should().BeEmpty();
            notification.Current.Should().BeEmpty();
        }

        [Fact]
        public async Task Replace_in_filtered_list_replacement()
        {
            var list = new ListReactiveCollectionSource<int>();

            var projectedList = list.ReactiveCollection
                .Where(x => x % 2 == 0);

            var notificationTask = projectedList.Changes
                .Skip(2)
                .FirstAsync()
                .ToTask();

            list.AddRange(new[] { 1, 2, 3 });
            list.Replace(2, 4);

            var notification = await notificationTask;

            notification.Index.Should().Be(0);
            notification.Action.Should().Be(NotifyCollectionChangedAction.Replace);
            notification.OldItems.Should().Equal(2);
            notification.NewItems.Should().Equal(4);
            notification.Current.Should().Equal(4);
        }

        [Fact]
        public async Task Replace_in_filtered_list_replacement2()
        {
            var list = new ListReactiveCollectionSource<int>(new[] { 1 });

            var projectedList = list.ReactiveCollection
                .Where(x => x % 2 == 0);

            var notificationTask = projectedList.Changes
                .Skip(1)
                .FirstAsync()
                .ToTask();

            list.Replace(1, 3);
            list.Replace(3, 4);

            var notification = await notificationTask;

            notification.Index.Should().Be(0);
            notification.Action.Should().Be(NotifyCollectionChangedAction.Add);
            notification.OldItems.Should().BeEmpty();
            notification.NewItems.Should().Equal(4);
            notification.Current.Should().Equal(4);
        }

        [Fact]
        public async Task Replace_in_filtered_dictionary_addition()
        {
            var list = new DictionaryReactiveCollectionSource<string, int>();

            var projectedList = list.ReactiveCollection
                .Where(x => x % 2 == 0);

            var notificationTask = projectedList.Changes
                .Skip(2)
                .FirstAsync()
                .ToTask();

            list.Add("Key1", 1);
            list.Add("Key2", 2);
            list.Add("Key3", 3);

            list["Key1"] = 4;

            var notification = await notificationTask;

            notification.Action.Should().Be(NotifyCollectionChangedAction.Add);
            notification.OldItems.Should().BeEmpty();
            notification.NewItems.Should().Equal(new KeyValuePair<string, int>("Key1", 4));
            notification.Current.Should().Equal(new Dictionary<string, int>
            {
                { "Key2", 2 },
                { "Key1", 4 }
            });
        }

        [Fact]
        public async Task Replace_in_filtered_dictionary_removal()
        {
            var list = new DictionaryReactiveCollectionSource<string, int>();

            var projectedList = list.ReactiveCollection
                .Where(x => x % 2 == 0);

            var notificationTask = projectedList.Changes
                .Skip(2)
                .FirstAsync()
                .ToTask();

            list.Add("Key1", 1);
            list.Add("Key2", 2);
            list.Add("Key3", 3);

            list["Key2"] = 3;

            var notification = await notificationTask;

            notification.Action.Should().Be(NotifyCollectionChangedAction.Remove);
            notification.OldItems.Should().Equal(new KeyValuePair<string, int>("Key2", 2));
            notification.NewItems.Should().BeEmpty();
        }

        [Fact]
        public async Task Replace_in_filtered_dictionary_replacement()
        {
            var list = new DictionaryReactiveCollectionSource<string, int>();

            var projectedList = list.ReactiveCollection
                .Where(x => x % 2 == 0);

            var notificationsTask = projectedList.Changes
                .Skip(2)
                .Take(2)
                .ToArray()
                .ToTask();

            list.Add("Key1", 1);
            list.Add("Key2", 2);
            list.Add("Key3", 3);

            list["Key2"] = 4;

            var notifications = await notificationsTask;

            notifications[0].Action.Should().Be(NotifyCollectionChangedAction.Remove);
            notifications[1].Action.Should().Be(NotifyCollectionChangedAction.Add);
            notifications[0].OldItems.Should().Equal(new KeyValuePair<string, int>("Key2", 2));
            notifications[1].NewItems.Should().Equal(new KeyValuePair<string, int>("Key2", 4));
            notifications[1].Current.Should().Equal(new Dictionary<string, int>
            {
                { "Key2", 4 }
            });
        }

        [Fact]
        public async Task Replace_in_filtered_from_ListChangedNotification_observable()
        {
            var observable = new[]
                {
                    new ListChangedNotification<int>(ImmutableList<int>.Empty, NotifyCollectionChangedAction.Reset, ImmutableList<int>.Empty, ImmutableList<int>.Empty, null),
                    new ListChangedNotification<int>(ImmutableList.Create(1, 2), NotifyCollectionChangedAction.Add, ImmutableList<int>.Empty, ImmutableList.Create(1, 2), 0),
                    new ListChangedNotification<int>(ImmutableList.Create(3, 4), NotifyCollectionChangedAction.Replace, ImmutableList.Create(1, 2), ImmutableList.Create(3, 4), 0)
                }
            .ToObservable()
            .ToReactiveCollection();

            var projectedList = observable.Where(x => x % 2 == 0);

            var notificationsTask = projectedList.Changes
                .Skip(2)
                .Take(2)
                .ToArray()
                .ToTask();

            var notifications = await notificationsTask;

            notifications[0].Index.Should().Be(0);
            notifications[0].NewItems.Should().BeEmpty();
            notifications[0].Action.Should().Be(NotifyCollectionChangedAction.Remove);
            notifications[0].OldItems.Should().Equal(2);
            notifications[1].Index.Should().Be(0);
            notifications[1].OldItems.Should().BeEmpty();
            notifications[1].Action.Should().Be(NotifyCollectionChangedAction.Add);
            notifications[1].NewItems.Should().Equal(4);
            notifications[1].Current.Should().Equal(4);
        }

        [Fact]
        public async Task Add_to_sorted_list()
        {
            var list = new ListReactiveCollectionSource<int>();

            var projectedList = list
                .ReactiveCollection
                .Sort();

            var notificationTask = projectedList.Changes
                .Skip(3)
                .FirstAsync()
                .ToTask();

            list.Add(3);
            list.Add(1);
            list.Add(2);

            var notification = await notificationTask;

            notification.Action.Should().Be(NotifyCollectionChangedAction.Add);
            notification.NewItems.Should().Equal(2);
            notification.Current.Should().Equal(1, 2, 3);
        }

        [Fact]
        public async Task Add_to_sorted_dictionary()
        {
            var list = new DictionaryReactiveCollectionSource<string, int>();

            var projectedList = list
                .ReactiveCollection
                .Sort(Comparer<KeyValuePair<string, int>>.Create((x, y) => x.Value.CompareTo(y.Value)));

            var notificationTask = projectedList.Changes
                .Skip(3)
                .FirstAsync()
                .ToTask();

            list.Add("Key3", 3);
            list.Add("Key1", 1);
            list.Add("Key2", 2);

            var notification = await notificationTask;

            notification.Action.Should().Be(NotifyCollectionChangedAction.Add);
            notification.NewItems.Should().Equal(new KeyValuePair<string, int>("Key2", 2));
            notification.Current.Should().Equal(new KeyValuePair<string, int>("Key1", 1), new KeyValuePair<string, int>("Key2", 2), new KeyValuePair<string, int>("Key3", 3));
        }

        [Fact]
        public async Task Add_to_setSorted_dictionary()
        {
            var list = new DictionaryReactiveCollectionSource<string, int>();

            var projectedList = list
                .ReactiveCollection
                .SortSet(Comparer<KeyValuePair<string, int>>.Create((x, y) => x.Value.CompareTo(y.Value)));

            var notificationTask = projectedList.Changes
                .Skip(3)
                .FirstAsync()
                .ToTask();

            list.Add("Key3", 3);
            list.Add("Key1", 1);
            list.Add("Key2", 2);

            var notification = await notificationTask;

            notification.Action.Should().Be(NotifyCollectionChangedAction.Add);
            notification.NewItems.Should().Equal(new KeyValuePair<string, int>("Key2", 2));
            notification.Current.Should().Equal(new KeyValuePair<string, int>("Key1", 1), new KeyValuePair<string, int>("Key2", 2), new KeyValuePair<string, int>("Key3", 3));
        }

        [Fact]
        public async Task Add_to_setSorted_dictionary_with_duplicate_value()
        {
            var list = new DictionaryReactiveCollectionSource<string, int>();

            var projectedList = list
                .ReactiveCollection
                .SortSet(Comparer<KeyValuePair<string, int>>.Create((x, y) => x.Value.CompareTo(y.Value)));

            var notificationTask = projectedList.Changes
                .Skip(3)
                .FirstAsync()
                .ToTask();

            list.Add("Key3", 3);
            list.Add("Key1", 1);
            list.Add("Key1", 1);
            list.Add("Key2", 2);

            var notification = await notificationTask;

            notification.Action.Should().Be(NotifyCollectionChangedAction.Add);
            notification.NewItems.Should().Equal(new KeyValuePair<string, int>("Key2", 2));
            notification.Current.Should().Equal(new KeyValuePair<string, int>("Key1", 1), new KeyValuePair<string, int>("Key2", 2), new KeyValuePair<string, int>("Key3", 3));
        }

        [Fact]
        public async Task Add_to_sorted_list_with_Changes()
        {
            var list = new ListReactiveCollectionSource<int>();

            var projectedList = list
                .ReactiveCollection
                .Sort();

            var notificationTask = projectedList.Changes
                .Skip(3)
                .FirstAsync()
                .ToTask();

            list.Add(3);
            list.Add(1);
            list.Add(2);

            var notification = await notificationTask;

            notification.Action.Should().Be(NotifyCollectionChangedAction.Add);
            notification.NewItems.Should().Equal(2);
            notification.Current.Should().Equal(1, 2, 3);
        }

        [Fact]
        public async Task Remove_from_sorted_list()
        {
            var list = new ListReactiveCollectionSource<int>();

            var projectedList = list
                .ReactiveCollection
                .Sort();

            var notificationTask = projectedList.Changes
                .Skip(2)
                .FirstAsync()
                .ToTask();

            list.AddRange(new[] { 3, 2, 1 });
            list.Remove(2);

            var notification = await notificationTask;

            notification.Action.Should().Be(NotifyCollectionChangedAction.Remove);
            notification.NewItems.Should().BeEmpty();
            notification.OldItems.Should().Equal(2);
            notification.Current.Should().Equal(1, 3);
        }

        [Fact]
        public async Task Remove_in_sorted_dictionary()
        {
            var list = new DictionaryReactiveCollectionSource<string, int>();

            var projectedList = list.ReactiveCollection
                .Sort(Comparer<KeyValuePair<string, int>>.Create((x, y) => x.Value.CompareTo(y.Value)));

            var notificationTask = projectedList.Changes
                .Skip(3)
                .FirstAsync()
                .ToTask();

            list.Add("Key1", 1);
            list.Add("Key2", 2);
            list.Remove("Key1");

            var notification = await notificationTask;

            notification.Action.Should().Be(NotifyCollectionChangedAction.Remove);
            notification.OldItems.Should().Equal(new KeyValuePair<string, int>("Key1", 1));
            notification.Index.Should().Be(0);
            notification.Current.Should().Equal(new KeyValuePair<string, int>("Key2", 2));
        }

        [Fact]
        public async Task Remove_in_setSorted_dictionary()
        {
            var list = new DictionaryReactiveCollectionSource<string, int>();

            var projectedList = list.ReactiveCollection
                .SortSet(Comparer<KeyValuePair<string, int>>.Create((x, y) => x.Value.CompareTo(y.Value)));

            var notificationTask = projectedList.Changes
                .Skip(3)
                .FirstAsync()
                .ToTask();

            list.Add("Key1", 1);
            list.Add("Key2", 2);
            list.Remove("Key1");

            var notification = await notificationTask;

            notification.Action.Should().Be(NotifyCollectionChangedAction.Remove);
            notification.OldItems.Should().Equal(new KeyValuePair<string, int>("Key1", 1));
            notification.Current.Should().Equal(new KeyValuePair<string, int>("Key2", 2));
        }

        [Fact]
        public async Task Remove_from_sorted_list_with_Changes()
        {
            var list = new ListReactiveCollectionSource<int>();

            var projectedList = list
                .ReactiveCollection
                .Sort();

            var notificationTask = projectedList.Changes
                .Skip(2)
                .FirstAsync()
                .ToTask();

            list.AddRange(new[] { 3, 2, 1 });
            list.Remove(2);

            var notification = await notificationTask;

            notification.Action.Should().Be(NotifyCollectionChangedAction.Remove);
            notification.NewItems.Should().BeEmpty();
            notification.OldItems.Should().Equal(2);
            notification.Current.Should().Equal(1, 3);
        }

        [Fact]
        public async Task Replace_in_sorted_list()
        {
            var list = new ListReactiveCollectionSource<int>();

            var projectedList = list
                .ReactiveCollection
                .Sort();

            var notificationsTask = projectedList.Changes
                .Skip(2)
                .Take(2)
                .ToArray()
                .ToTask();

            list.AddRange(new[] { 3, 2, 1 });
            list.Replace(2, 4);

            var notifications = await notificationsTask;

            notifications[0].Action.Should().Be(NotifyCollectionChangedAction.Remove);
            notifications[0].OldItems.Should().Equal(2);
            notifications[1].Action.Should().Be(NotifyCollectionChangedAction.Add);
            notifications[1].NewItems.Should().Equal(4);
            notifications[0].Current.Should().Equal(1, 3);
            notifications[1].Current.Should().Equal(1, 3, 4);
        }

        [Fact]
        public async Task Replace_in_setSorted_projected_list()
        {
            var list = new ListReactiveCollectionSource<int>();

            var projectedList = list
                .ReactiveCollection
                .Select(x => x.ToString())
                .SortSet();

            var notificationsTask = projectedList.Changes
                .Skip(4)
                .Take(2)
                .ToArray()
                .ToTask();

            list.AddRange(new[] { 3, 2, 1 });
            list.Replace(2, 4);

            var notifications = await notificationsTask;

            notifications[0].Action.Should().Be(NotifyCollectionChangedAction.Remove);
            notifications[0].OldItems.Should().Equal("2");
            notifications[1].Action.Should().Be(NotifyCollectionChangedAction.Add);
            notifications[1].NewItems.Should().Equal("4");
            notifications[0].Current.Should().Equal("1", "3");
            notifications[1].Current.Should().Equal("1", "3", "4");
        }

        [Fact]
        public async Task Replace_in_setSorted_projected_dictionary()
        {
            var list = new DictionaryReactiveCollectionSource<int, int>();

            var projectedList = list
                .ReactiveCollection
                .Select(x => x.Value.ToString())
                .SortSet();

            var notificationsTask = projectedList.Changes
                .Skip(4)
                .Take(2)
                .ToArray()
                .ToTask();

            list.Add(3, 30);
            list.Add(2, 20);
            list.Add(1, 10);
            list[2] = 40;

            var notifications = await notificationsTask;

            notifications[0].Action.Should().Be(NotifyCollectionChangedAction.Remove);
            notifications[0].OldItems.Should().Equal("20");
            notifications[1].Action.Should().Be(NotifyCollectionChangedAction.Add);
            notifications[1].NewItems.Should().Equal("40");
            notifications[0].Current.Should().Equal("10", "30");
            notifications[1].Current.Should().Equal("10", "30", "40");
        }

        [Fact]
        public async Task Replace_in_sorted_list_with_Changes()
        {
            var list = new ListReactiveCollectionSource<int>();

            var projectedList = list
                .ReactiveCollection
                .Sort();

            var notificationsTask = projectedList.Changes
                .Skip(2)
                .Take(2)
                .ToArray()
                .ToTask();

            list.AddRange(new[] { 3, 2, 1 });
            list.Replace(2, 4);

            var notifications = await notificationsTask;

            notifications[0].Action.Should().Be(NotifyCollectionChangedAction.Remove);
            notifications[0].OldItems.Should().Equal(2);
            notifications[1].Action.Should().Be(NotifyCollectionChangedAction.Add);
            notifications[1].NewItems.Should().Equal(4);
            notifications[0].Current.Should().Equal(1, 3);
            notifications[1].Current.Should().Equal(1, 3, 4);
        }

        [Fact]
        public async Task Replace_in_sorted_from_ListChangedNotification_observable()
        {
            var observable = new[]
                {
                    new ListChangedNotification<int>(ImmutableList<int>.Empty, NotifyCollectionChangedAction.Reset, ImmutableList<int>.Empty, ImmutableList<int>.Empty, null),
                    new ListChangedNotification<int>(ImmutableList.Create(1, 2), NotifyCollectionChangedAction.Add, ImmutableList<int>.Empty, ImmutableList.Create(1, 2), 0),
                    new ListChangedNotification<int>(ImmutableList.Create(3, 4), NotifyCollectionChangedAction.Replace, ImmutableList.Create(1, 2), ImmutableList.Create(3, 4), 0)
                }
                .ToObservable()
                .ToReactiveCollection();

            var projectedList = observable.Sort();

            var notificationsTask = projectedList.Changes
                .Skip(2)
                .Take(2)
                .ToArray()
                .ToTask();

            var notifications = await notificationsTask;

            notifications[0].Action.Should().Be(NotifyCollectionChangedAction.Reset);
            notifications[1].Index.Should().Be(0);
            notifications[1].OldItems.Should().BeEmpty();
            notifications[1].NewItems.Should().Equal(3, 4);
            notifications[1].Action.Should().Be(NotifyCollectionChangedAction.Add);
            notifications[1].Current.Should().Equal(3, 4);
        }

        [Fact]
        public async Task Replace_in_sorted_dictionary()
        {
            var list = new DictionaryReactiveCollectionSource<string, int>();

            var projectedList = list.ReactiveCollection
                .Sort(Comparer<KeyValuePair<string, int>>.Create((x, y) => x.Value.CompareTo(y.Value)));

            var notificationsTask = projectedList.Changes
                .Skip(4)
                .Take(2)
                .ToArray()
                .ToTask();

            list.Add("Key1", 1);
            list.Add("Key2", 2);
            list.Add("Key3", 3);

            list["Key2"] = 4;

            var notifications = await notificationsTask;

            notifications[0].Action.Should().Be(NotifyCollectionChangedAction.Remove);
            notifications[0].OldItems.Should().Equal(new KeyValuePair<string, int>("Key2", 2));
            notifications[0].Index.Should().Be(1);
            notifications[1].Action.Should().Be(NotifyCollectionChangedAction.Add);
            notifications[1].NewItems.Should().Equal(new KeyValuePair<string, int>("Key2", 4));
            notifications[1].Index.Should().Be(2);
            notifications[1].Current.Should().Equal(new KeyValuePair<string, int>("Key1", 1), new KeyValuePair<string, int>("Key3", 3), new KeyValuePair<string, int>("Key2", 4));
        }

        [Fact]
        public async Task Replace_in_setSorted_dictionary()
        {
            var list = new DictionaryReactiveCollectionSource<string, int>();

            var projectedList = list.ReactiveCollection
                .SortSet(Comparer<KeyValuePair<string, int>>.Create((x, y) => x.Value.CompareTo(y.Value)));

            var notificationsTask = projectedList.Changes
                .Skip(4)
                .Take(2)
                .ToArray()
                .ToTask();

            list.Add("Key1", 1);
            list.Add("Key2", 2);
            list.Add("Key3", 3);

            list["Key2"] = 4;

            var notifications = await notificationsTask;

            notifications[0].Action.Should().Be(NotifyCollectionChangedAction.Remove);
            notifications[0].OldItems.Should().Equal(new KeyValuePair<string, int>("Key2", 2));
            notifications[1].Action.Should().Be(NotifyCollectionChangedAction.Add);
            notifications[1].NewItems.Should().Equal(new KeyValuePair<string, int>("Key2", 4));
            notifications[1].Current.Should().Equal(new KeyValuePair<string, int>("Key1", 1), new KeyValuePair<string, int>("Key3", 3), new KeyValuePair<string, int>("Key2", 4));
        }

        [Fact]
        public async Task List_ToObservableCollection_Add()
        {
            var list = new ListReactiveCollectionSource<int>();
            var observableCollection = (INotifyCollectionChanged)list.ReactiveCollection.ToObservableCollection();

            var eventsTask = Observable
                .FromEventPattern<NotifyCollectionChangedEventHandler, NotifyCollectionChangedEventArgs>(eh => observableCollection.CollectionChanged += eh, eh => observableCollection.CollectionChanged -= eh)
                .Take(3)
                .Select(x => x.EventArgs)
                .ToArray()
                .ToTask();

            list.Add(1);
            list.Add(2);
            list.Add(3);

            var events = await eventsTask;

            events.Should().HaveCount(3);
            events[0].Action.Should().Be(NotifyCollectionChangedAction.Add);
            events[1].Action.Should().Be(NotifyCollectionChangedAction.Add);
            events[2].Action.Should().Be(NotifyCollectionChangedAction.Add);
            ((ICollection)observableCollection).Should().Equal(new[] { 1, 2, 3 });
        }

        [Fact]
        public void List_ToObservableCollection_Add_multiple()
        {
            var list = new ListReactiveCollectionSource<int>();
            var observableCollection = list.ReactiveCollection.ToObservableCollection();

            using (Observable
                .FromEventPattern<NotifyCollectionChangedEventHandler, NotifyCollectionChangedEventArgs>(eh => ((INotifyCollectionChanged)observableCollection).CollectionChanged += eh, eh => ((INotifyCollectionChanged)observableCollection).CollectionChanged -= eh)
                .Subscribe())
            {
                list.AddRange(new[] { 1, 2, 3 });
                ((ICollection)observableCollection).Should().Equal(new[] { 1, 2, 3 });

                list.InsertRange(2, new[] { 4, 5, 6 });
                ((ICollection)observableCollection).Should().Equal(new[] { 1, 2, 4, 5, 6, 3 });
            }
        }

        [Fact]
        public async Task List_ToObservableCollection_Remove()
        {
            var list = new ListReactiveCollectionSource<int>();
            var observableCollection = (INotifyCollectionChanged)list.ReactiveCollection.ToObservableCollection();

            var eventsTask = Observable
                .FromEventPattern<NotifyCollectionChangedEventHandler, NotifyCollectionChangedEventArgs>(eh => observableCollection.CollectionChanged += eh, eh => observableCollection.CollectionChanged -= eh)
                .Take(3)
                .Select(x => x.EventArgs)
                .ToArray()
                .ToTask();

            list.Add(1);
            list.Add(2);
            list.Remove(1);

            var events = await eventsTask;

            events[0].Action.Should().Be(NotifyCollectionChangedAction.Add);
            events[1].Action.Should().Be(NotifyCollectionChangedAction.Add);
            events[2].Action.Should().Be(NotifyCollectionChangedAction.Remove);
        }

        [Fact]
        public void List_ToObservableCollection_Remove_multiple()
        {
            var list = new ListReactiveCollectionSource<int>();
            var observableCollection = list.ReactiveCollection.ToObservableCollection();

            using (Observable
                .FromEventPattern<NotifyCollectionChangedEventHandler, NotifyCollectionChangedEventArgs>(eh => ((INotifyCollectionChanged)observableCollection).CollectionChanged += eh, eh => ((INotifyCollectionChanged)observableCollection).CollectionChanged -= eh)
                .Subscribe())
            {
                list.AddRange(new[] { 1, 2, 3, 4, 5, 6, 7 });
                list.RemoveRange(2, 3);

                ((ICollection)observableCollection).Should().Equal(new[] { 1, 2, 6, 7 });
            }
        }

        [Fact]
        public async Task List_ToObservableCollection_Replace()
        {
            var list = new ListReactiveCollectionSource<int>();
            var observableCollection = (INotifyCollectionChanged)list.ReactiveCollection.ToObservableCollection();

            var eventsTask = Observable
                .FromEventPattern<NotifyCollectionChangedEventHandler, NotifyCollectionChangedEventArgs>(eh => observableCollection.CollectionChanged += eh, eh => observableCollection.CollectionChanged -= eh)
                .Take(3)
                .Select(x => x.EventArgs)
                .ToArray()
                .ToTask();

            list.Add(1);
            list.Add(2);
            list.Replace(1, 3);

            var events = await eventsTask;

            events[0].Action.Should().Be(NotifyCollectionChangedAction.Add);
            events[1].Action.Should().Be(NotifyCollectionChangedAction.Add);
            events[2].Action.Should().Be(NotifyCollectionChangedAction.Replace);
            events[2].OldItems.Should().Equal(1);
            events[2].NewItems.Should().Equal(3);
            ((ICollection)observableCollection).Should().Equal(3, 2);
        }

        [Fact]
        public async Task List_ToObservableCollection_Clear()
        {
            var list = new ListReactiveCollectionSource<int>();
            var observableCollection = (INotifyCollectionChanged)list.ReactiveCollection.ToObservableCollection();

            var eventTask = Observable
                .FromEventPattern<NotifyCollectionChangedEventHandler, NotifyCollectionChangedEventArgs>(eh => observableCollection.CollectionChanged += eh, eh => observableCollection.CollectionChanged -= eh)
                .Skip(1)
                .Select(x => x.EventArgs)
                .FirstAsync()
                .ToTask();

            list.AddRange(new[] { 1, 2, 3, 4, 5 });
            list.RemoveAll(x => x > 2);

            var ev = await eventTask;

            ev.Action.Should().Be(NotifyCollectionChangedAction.Reset);
            ((ICollection)observableCollection).Should().Equal(1, 2);
        }

        [Fact]
        public async Task List_ToObservableCollection_does_not_raise_events_on_event_subscription()
        {
            var list = new ListReactiveCollectionSource<int> {1, 2, 3};

            var observableCollection = (INotifyCollectionChanged)list.ReactiveCollection.ToObservableCollection();

            var eventsTask = Observable
                .FromEventPattern<NotifyCollectionChangedEventHandler, NotifyCollectionChangedEventArgs>(eh => observableCollection.CollectionChanged += eh, eh => observableCollection.CollectionChanged -= eh)
                .Select(x => x.EventArgs)
                .FirstAsync()
                .ToTask();

            ((ICollection)observableCollection).Should().BeEquivalentTo(new[] { 1, 2, 3 });
            list.Add(4);

            var ev = await eventsTask;

            ev.Action.Should().Be(NotifyCollectionChangedAction.Add);
            ev.NewItems.Should().Equal(4);
            ((ICollection)observableCollection).Should().Equal(1, 2, 3, 4);
        }

        [Fact]
        public async Task SortedSet_ToObservableCollection_Add()
        {
            var list = new SortedSetReactiveCollectionSource<int>();
            var observableCollection = (INotifyCollectionChanged)list.ReactiveCollection.ToObservableCollection();

            var eventsTask = Observable
                .FromEventPattern<NotifyCollectionChangedEventHandler, NotifyCollectionChangedEventArgs>(eh => observableCollection.CollectionChanged += eh, eh => observableCollection.CollectionChanged -= eh)
                .Take(3)
                .Select(x => x.EventArgs)
                .ToArray()
                .ToTask();

            list.Add(1);
            list.Add(2);
            list.Add(3);

            var events = await eventsTask;

            events[0].Action.Should().Be(NotifyCollectionChangedAction.Add);
            events[1].Action.Should().Be(NotifyCollectionChangedAction.Add);
            events[2].Action.Should().Be(NotifyCollectionChangedAction.Add);
            ((ICollection)observableCollection).Should().Equal(1, 2, 3);
        }

        [Fact]
        public void SortedSet_ToObservableCollection_Add_multiple()
        {
            var list = new SortedSetReactiveCollectionSource<int>();
            var observableCollection = list.ReactiveCollection.ToObservableCollection();

            using (Observable
                .FromEventPattern<NotifyCollectionChangedEventHandler, NotifyCollectionChangedEventArgs>(eh => ((INotifyCollectionChanged)observableCollection).CollectionChanged += eh, eh => ((INotifyCollectionChanged)observableCollection).CollectionChanged -= eh)
                .Subscribe())
            {
                list.AddRange(new[] { 2, 4, 6 });
                ((ICollection)observableCollection).Should().Equal(2, 4, 6);

                list.AddRange(new[] { 1, 3, 5 });
                ((ICollection)observableCollection).Should().Equal(1, 2, 3, 4, 5, 6);
            }
        }

        [Fact]
        public async Task SortedSet_ToObservableCollection_Remove()
        {
            var list = new SortedSetReactiveCollectionSource<int>();
            var observableCollection = (INotifyCollectionChanged)list.ReactiveCollection.ToObservableCollection();

            var eventsTask = Observable
                .FromEventPattern<NotifyCollectionChangedEventHandler, NotifyCollectionChangedEventArgs>(eh => observableCollection.CollectionChanged += eh, eh => observableCollection.CollectionChanged -= eh)
                .Take(3)
                .Select(x => x.EventArgs)
                .ToArray()
                .ToTask();

            list.Add(1);
            list.Add(2);
            list.Remove(1);

            var events = await eventsTask;

            events[0].Action.Should().Be(NotifyCollectionChangedAction.Add);
            events[1].Action.Should().Be(NotifyCollectionChangedAction.Add);
            events[2].Action.Should().Be(NotifyCollectionChangedAction.Remove);
        }

        [Fact]
        public void SortedSet_ToObservableCollection_Remove_multiple()
        {
            var list = new SortedSetReactiveCollectionSource<int>();
            var observableCollection = list.ReactiveCollection.ToObservableCollection();

            using (Observable
                .FromEventPattern<NotifyCollectionChangedEventHandler, NotifyCollectionChangedEventArgs>(eh => ((INotifyCollectionChanged)observableCollection).CollectionChanged += eh, eh => ((INotifyCollectionChanged)observableCollection).CollectionChanged -= eh)
                .Subscribe())
            {
                list.AddRange(new[] { 1, 2, 3, 4, 5, 6, 7 });
                list.Remove(3);
                list.Remove(4);
                list.Remove(5);

                observableCollection.Should().Equal(1, 2, 6, 7);
            }
        }

        [Fact]
        public async Task SortedSet_ToObservableCollection_Clear()
        {
            var list = new ListReactiveCollectionSource<int>();
            var observableCollection = (INotifyCollectionChanged)list.ReactiveCollection.ToObservableCollection();

            var eventTask = Observable
                .FromEventPattern<NotifyCollectionChangedEventHandler, NotifyCollectionChangedEventArgs>(eh => observableCollection.CollectionChanged += eh, eh => observableCollection.CollectionChanged -= eh)
                .Skip(1)
                .Select(x => x.EventArgs)
                .FirstAsync()
                .ToTask();

            list.AddRange(new[] { 1, 2, 3, 4, 5 });
            list.RemoveAll(x => x > 2);

            var ev = await eventTask;

            ev.Action.Should().Be(NotifyCollectionChangedAction.Reset);
            ((ICollection)observableCollection).Should().Equal(new[] { 1, 2 });
        }

        [Fact]
        public async Task SortedSet_ToObservableCollection_does_not_raise_events_on_event_subscription()
        {
            var list = new ListReactiveCollectionSource<int> { 1, 2, 3 };

            var observableCollection = (INotifyCollectionChanged)list.ReactiveCollection.ToObservableCollection();

            var eventsTask = Observable
                .FromEventPattern<NotifyCollectionChangedEventHandler, NotifyCollectionChangedEventArgs>(eh => observableCollection.CollectionChanged += eh, eh => observableCollection.CollectionChanged -= eh)
                .Select(x => x.EventArgs)
                .FirstAsync()
                .ToTask();

            ((ICollection)observableCollection).Should().Equal(1, 2, 3);
            list.Add(4);

            var ev = await eventsTask;

            ev.Action.Should().Be(NotifyCollectionChangedAction.Add);
            ev.NewItems.Should().Equal(4);
            ((ICollection)observableCollection).Should().Equal(1, 2, 3, 4);
        }

        [Fact]
        public async Task GetValueObservable_Test1()
        {
            var dict = new DictionaryReactiveCollectionSource<int, int>();

            var arrayTask = dict.ReactiveCollection.GetValueObservable(1)
                .Take(4)
                .ToArray()
                .ToTask();

            dict.Add(2, 2);
            dict.Add(1, 1);
            dict[2] = 1;
            dict.Remove(1);
            dict[2] = 1;
            dict.Add(1, 2);
            dict[1] = 3;

            (await arrayTask).Should().Equal(1, 1, 2, 3);
        }

        [Fact]
        public async Task Concat_Add()
        {
            var source1 = new ListReactiveCollectionSource<int>();
            var source2 = new ListReactiveCollectionSource<int>();

            var concat = source1.ReactiveCollection.Concat(source2.ReactiveCollection);

            var notificationsTask = concat.Changes
                .Take(3)
                .ToArray()
                .ToTask();

            source1.Add(1);
            source2.Add(2);

            var notifications = await notificationsTask;

            notifications.Should().HaveCount(3);
            notifications[0].Action.Should().Be(NotifyCollectionChangedAction.Reset);
            notifications[0].Current.Should().BeEmpty();
            notifications[1].Action.Should().Be(NotifyCollectionChangedAction.Add);
            notifications[1].Index.Should().Be(0);
            notifications[1].Current.Should().Equal(1);
            notifications[2].Action.Should().Be(NotifyCollectionChangedAction.Add);
            notifications[2].Index.Should().Be(1);
            notifications[2].Current.Should().Equal(1, 2);
        }

        [Fact]
        public async Task Concat_Remove()
        {
            var source1 = new ListReactiveCollectionSource<int>();
            var source2 = new ListReactiveCollectionSource<int>();

            var concat = source1.ReactiveCollection.Concat(source2.ReactiveCollection);

            var notificationsTask = concat.Changes
                .Skip(2)
                .Take(3)
                .ToArray()
                .ToTask();

            source1.Add(1);
            source2.Add(2);
            source1.RemoveAt(0);
            source2.RemoveAt(0);

            var notifications = await notificationsTask;

            notifications.Should().HaveCount(3);
            notifications[0].Current.Should().Equal(1, 2);
            notifications[1].Action.Should().Be(NotifyCollectionChangedAction.Remove);
            notifications[1].Index.Should().Be(0);
            notifications[1].Current.Should().Equal(2);
            notifications[2].Action.Should().Be(NotifyCollectionChangedAction.Remove);
            notifications[2].Index.Should().Be(0);
            notifications[2].Current.Should().BeEmpty();
        }

        [Fact]
        public async Task Concat_Clear()
        {
            var source1 = new ListReactiveCollectionSource<int>();
            var source2 = new ListReactiveCollectionSource<int>();

            var concat = source1.ReactiveCollection.Concat(source2.ReactiveCollection);

            var notificationsTask = concat.Changes
                .Skip(2)
                .Take(3)
                .ToArray()
                .ToTask();

            source1.Add(1);
            source2.Add(2);
            source1.Clear();
            source2.Clear();

            var notifications = await notificationsTask;

            notifications.Should().HaveCount(3);
            notifications[0].Current.Should().Equal(1, 2);
            notifications[1].Action.Should().Be(NotifyCollectionChangedAction.Remove);
            notifications[1].Index.Should().Be(0);
            notifications[1].OldItems.Should().Equal(1);
            notifications[1].Current.Should().Equal(2);
            notifications[2].Action.Should().Be(NotifyCollectionChangedAction.Remove);
            notifications[2].Index.Should().Be(0);
            notifications[2].OldItems.Should().Equal(2);
            notifications[2].Current.Should().BeEmpty();
        }

        [Fact]
        public async Task Concat_Replace_single()
        {
            var source1 = new ListReactiveCollectionSource<int>();
            var source2 = new ListReactiveCollectionSource<int>();

            var concat = source1.ReactiveCollection.Concat(source2.ReactiveCollection);

            var notificationsTask = concat.Changes
                .Skip(2)
                .Take(3)
                .ToArray()
                .ToTask();

            source1.AddRange(new[] { 1, 2, 3 });
            source2.AddRange(new[] { 4, 5, 6 });
            source1.Replace(2, -2);
            source2.Replace(5, -5);

            var notifications = await notificationsTask;

            notifications.Should().HaveCount(3);
            notifications[0].Current.Should().Equal(1, 2, 3, 4, 5, 6);
            notifications[1].Action.Should().Be(NotifyCollectionChangedAction.Replace);
            notifications[1].Index.Should().Be(1);
            notifications[1].OldItems.Should().Equal(2);
            notifications[1].NewItems.Should().Equal(-2);
            notifications[1].Current.Should().Equal(1, -2, 3, 4, 5, 6);
            notifications[2].Action.Should().Be(NotifyCollectionChangedAction.Replace);
            notifications[2].Index.Should().Be(4);
            notifications[2].OldItems.Should().Equal(5);
            notifications[2].NewItems.Should().Equal(-5);
            notifications[2].Current.Should().Equal(1, -2, 3, 4, -5, 6);
        }

        [Fact]
        public async Task Concat_Replace_multiple()
        {
            var changesSubject1 = new Subject<ListChangedNotification<int>>();
            var changesSubject2 = new Subject<ListChangedNotification<int>>();

            var reactiveCollection1 = changesSubject1.ToReactiveCollection();
            var reactiveCollection2 = changesSubject2.ToReactiveCollection();

            var concat = reactiveCollection1.Concat(reactiveCollection2);

            var notificationsTask = concat.Changes
                .Skip(2)
                .Take(3)
                .ToArray()
                .ToTask();

            changesSubject1.OnNext(new ListChangedNotification<int>(ImmutableList<int>.Empty, NotifyCollectionChangedAction.Reset, ImmutableList<int>.Empty, ImmutableList<int>.Empty, null));
            changesSubject2.OnNext(new ListChangedNotification<int>(ImmutableList<int>.Empty, NotifyCollectionChangedAction.Reset, ImmutableList<int>.Empty, ImmutableList<int>.Empty, null));

            changesSubject1.OnNext(new ListChangedNotification<int>(new[] { 1, 2, 3 }.ToImmutableList(), NotifyCollectionChangedAction.Add, ImmutableList<int>.Empty, new[] { 1, 2, 3 }.ToImmutableList(), 0));
            changesSubject2.OnNext(new ListChangedNotification<int>(new[] { 4, 5, 6 }.ToImmutableList(), NotifyCollectionChangedAction.Add, ImmutableList<int>.Empty, new[] { 4, 5, 6 }.ToImmutableList(), 0));

            changesSubject1.OnNext(new ListChangedNotification<int>(new[] { 1, -2, -3 }.ToImmutableList(), NotifyCollectionChangedAction.Replace, new[] { 2, 3 }.ToImmutableList(), new[] { -2, -3 }.ToImmutableList(), 1));
            changesSubject2.OnNext(new ListChangedNotification<int>(new[] { 4, -5, -6 }.ToImmutableList(), NotifyCollectionChangedAction.Replace, new[] { 5, 6 }.ToImmutableList(), new[] { -5, -6 }.ToImmutableList(), 1));

            var notifications = await notificationsTask;

            notifications.Should().HaveCount(3);
            notifications[0].Current.Should().Equal(1, 2, 3, 4, 5, 6);
            notifications[1].Action.Should().Be(NotifyCollectionChangedAction.Replace);
            notifications[1].Index.Should().Be(1);
            notifications[1].OldItems.Should().Equal(2, 3);
            notifications[1].NewItems.Should().Equal(-2, -3);
            notifications[1].Current.Should().Equal(1, -2, -3, 4, 5, 6);

            notifications[2].Action.Should().Be(NotifyCollectionChangedAction.Replace);
            notifications[2].Index.Should().Be(4);
            notifications[2].OldItems.Should().Equal(5, 6);
            notifications[2].NewItems.Should().Equal(-5, -6);
            notifications[2].Current.Should().Equal(1, -2, -3, 4, -5, -6);
        }

        [Fact]
        public async Task Concat_Add_Delayed()
        {
            var source1 = new ListReactiveCollectionSource<int>();
            var source2 = new ListReactiveCollectionSource<int>();

            source1.Add(1);
            source2.Add(2);

            var concat = source1.ReactiveCollection.Concat(source2.ReactiveCollection);

            var notification = await concat.Changes
                .FirstAsync()
                .ToTask();

            notification.Action.Should().Be(NotifyCollectionChangedAction.Reset);
            notification.Current.Should().Equal(1, 2);
        }

        [Fact]
        public async Task Concat_Add_Delayed2()
        {
            var sources = Enumerable
                .Range(0, 7)
                .Select(_ => new ListReactiveCollectionSource<int>())
                .Select((collection, i) =>
                {
                    collection.Add(i);
                    return collection.ReactiveCollection;
                });

            var concat = sources.Concat();

            var notification = await concat.Changes
                .FirstAsync()
                .ToTask();

            notification.Action.Should().Be(NotifyCollectionChangedAction.Reset);
            notification.Current.Should().Equal(0, 1, 2, 3, 4, 5, 6);
        }

        [Fact]
        public void Select_after_Where_squashes_both_operators()
        {
            var list = new ListReactiveCollectionSource<int>();

            var projectedList = list.ReactiveCollection
                .Where(x => x % 2 == 0)
                .Select(x => x.ToString(CultureInfo.InvariantCulture));

            var transformed = projectedList as ListTransformationReactiveCollection<int, string>;

            transformed.Should().NotBeNull();
            transformed.Source.Should().BeSameAs(list.ReactiveCollection);
            transformed.Selector.Should().NotBeNull();
            transformed.Filter.Should().NotBeNull();
        }

        [Fact]
        public void Where_after_Where_squashes_both_operators()
        {
            var list = new ListReactiveCollectionSource<int>();

            var projectedList = list.ReactiveCollection
                .Where(x => x % 2 == 0)
                .Where(x => x % 3 == 0);

            var transformed = projectedList as ListTransformationReactiveCollection<int, int>;

            transformed.Should().NotBeNull();
            transformed.Source.Should().BeSameAs(list.ReactiveCollection);
            transformed.Selector.Should().BeNull();
            transformed.Filter.Should().NotBeNull();
            transformed.Filter(2).Should().BeFalse();
            transformed.Filter(3).Should().BeFalse();
            transformed.Filter(4).Should().BeFalse();
            transformed.Filter(6).Should().BeTrue();
        }

        [Fact]
        public void Select_after_Select_squashes_both_operators()
        {
            var list = new ListReactiveCollectionSource<int>();

            var projectedList = list.ReactiveCollection
                .Select(x => x.ToString())
                .Select(int.Parse);

            var transformed = projectedList as ListTransformationReactiveCollection<int, int>;

            transformed.Should().NotBeNull();
            transformed.Source.Should().BeSameAs(list.ReactiveCollection);
            transformed.Selector.Should().NotBeNull();
            transformed.Filter.Should().BeNull();
        }

        [Fact]
        public async Task Select_after_Where_behaves_correctly()
        {
            var list = new ListReactiveCollectionSource<int>();

            var changesTask = list.ReactiveCollection
                .Where(x => x % 2 == 0)
                .Select(x => x.ToString(CultureInfo.InvariantCulture))
                .Changes
                .Take(6)
                .ToArray()
                .ToTask();

            list.Add(1);
            list.Add(2);
            list.Remove(2);
            list.Remove(1);
            list.Add(4);
            list.Insert(0, 2);
            list[0] = 3;

            var changes = await changesTask;

            changes[0].Action.Should().Be(NotifyCollectionChangedAction.Reset);
            changes[1].Action.Should().Be(NotifyCollectionChangedAction.Add);
            changes[1].Current.Should().Equal("2");
            changes[2].Action.Should().Be(NotifyCollectionChangedAction.Remove);
            changes[3].Action.Should().Be(NotifyCollectionChangedAction.Add);
            changes[3].Current.Should().Equal("4");
            changes[4].Action.Should().Be(NotifyCollectionChangedAction.Add);
            changes[4].Current.Should().Equal("4", "2");
            changes[4].Index.Should().Be(1);
            changes[5].Action.Should().Be(NotifyCollectionChangedAction.Remove);
            changes[5].Index.Should().Be(1);
            changes[5].Index.Should().Be(1);
            changes[5].Current.Should().Equal("4");
        }

        [Fact]
        public async Task Where_after_Where_behaves_correctly()
        {
            var list = new ListReactiveCollectionSource<int>();

            var changesTask = list.ReactiveCollection
                .Where(x => x % 2 == 0)
                .Where(x => x % 3 == 0)
                .Changes
                .Take(3)
                .ToArray()
                .ToTask();

            list.Add(1);
            list.Add(2);
            list.Add(3);
            list.Add(6);
            list.Remove(1);
            list.Remove(2);
            list.Remove(3);
            list.Remove(6);

            var changes = await changesTask;

            changes[0].Action.Should().Be(NotifyCollectionChangedAction.Reset);
            changes[1].Action.Should().Be(NotifyCollectionChangedAction.Add);
            changes[1].Current.Should().Equal(6);
            changes[2].Action.Should().Be(NotifyCollectionChangedAction.Remove);
            changes[2].Current.Should().BeEmpty();
        }
        
        [Fact]
        public void Select_after_Where_on_dictionaries_squashes_both_operators()
        {
            var list = new DictionaryReactiveCollectionSource<int, int>();

            var projectedList = list.ReactiveCollection
                .Where(x => x % 2 == 0)
                .Select(x => x.ToString(CultureInfo.InvariantCulture));

            var transformed = projectedList as DictionaryTransformationReactiveCollection<int, int, string>;

            transformed.Should().NotBeNull();
            transformed.Source.Should().BeSameAs(list.ReactiveCollection);
            transformed.Selector.Should().NotBeNull();
            transformed.Filter.Should().NotBeNull();
        }

        [Fact]
        public void Where_after_Where_on_dictionaries_squashes_both_operators()
        {
            var list = new DictionaryReactiveCollectionSource<int, int>();

            var projectedList = list.ReactiveCollection
                .Where(x => x % 2 == 0)
                .Where(x => x % 3 == 0);

            var transformed = projectedList as DictionaryTransformationReactiveCollection<int, int, int>;

            transformed.Should().NotBeNull();
            transformed.Source.Should().BeSameAs(list.ReactiveCollection);
            transformed.Selector.Should().BeNull();
            transformed.Filter.Should().NotBeNull();
        }

        [Fact]
        public void Select_after_Select_on_dictionaries_squashes_both_operators()
        {
            var list = new DictionaryReactiveCollectionSource<int, int>();

            var projectedList = list.ReactiveCollection
                .Select(x => x.ToString())
                .Select(int.Parse);

            var transformed = projectedList as DictionaryTransformationReactiveCollection<int, int, int>;

            transformed.Should().NotBeNull();
            transformed.Source.Should().BeSameAs(list.ReactiveCollection);
            transformed.Selector.Should().NotBeNull();
            transformed.Filter.Should().BeNull();
        }

        [Fact]
        public async Task Select_after_Where_on_dictionaries_behaves_correctly()
        {
            var list = new DictionaryReactiveCollectionSource<int, int>();

            var changesTask = list.ReactiveCollection
                .Where(x => x % 2 == 0)
                .Select(x => x.ToString(CultureInfo.InvariantCulture))
                .Changes
                .Take(5)
                .ToArray()
                .ToTask();

            list.Add(1, 36);
            list.Add(2, 37);
            list.Remove(2);
            list.Remove(1);
            list.Add(4, 38);
            list[4] = 39;

            var changes = await changesTask;

            changes[0].Action.Should().Be(NotifyCollectionChangedAction.Reset);
            changes[1].Action.Should().Be(NotifyCollectionChangedAction.Add);
            changes[1].Current.Should().HaveCount(1);
            changes[1].Current[1].Should().Be("36");
            changes[2].Action.Should().Be(NotifyCollectionChangedAction.Remove);
            changes[2].Current.Should().BeEmpty();
            changes[3].Action.Should().Be(NotifyCollectionChangedAction.Add);
            changes[3].Current.Should().HaveCount(1);
            changes[3].Current[4].Should().Be("38");
            changes[4].Action.Should().Be(NotifyCollectionChangedAction.Remove);
            changes[4].Current.Should().BeEmpty();
        }

        [Fact]
        public async Task Select_after_Select_on_dictionaries_behaves_correctly()
        {
            var list = new DictionaryReactiveCollectionSource<int, int>();

            var changesTask = list.ReactiveCollection
                .Select(x => x.ToString(CultureInfo.InvariantCulture))
                .Select(x => x + "!")
                .Changes
                .Take(6)
                .ToArray()
                .ToTask();

            list.Add(1, 36);
            list.Add(2, 37);
            list.Remove(2);
            list.Remove(1);
            list.Add(4, 38);

            var changes = await changesTask;

            changes[0].Action.Should().Be(NotifyCollectionChangedAction.Reset);
            changes[1].Action.Should().Be(NotifyCollectionChangedAction.Add);
            changes[1].Current.Should().HaveCount(1);
            changes[1].Current.Should().Contain(1, "36!");
            changes[2].Action.Should().Be(NotifyCollectionChangedAction.Add);
            changes[2].Current.Should().HaveCount(2);
            changes[2].Current.Should().Contain(1, "36!");
            changes[2].Current.Should().Contain(2, "37!");
            changes[3].Action.Should().Be(NotifyCollectionChangedAction.Remove);
            changes[3].Current.Should().HaveCount(1);
            changes[3].Current.Should().Contain(1, "36!");
            changes[4].Action.Should().Be(NotifyCollectionChangedAction.Remove);
            changes[4].Current.Should().BeEmpty();
            changes[5].Action.Should().Be(NotifyCollectionChangedAction.Add);
            changes[5].Current.Should().HaveCount(1);
            changes[5].Current.Should().Contain(4, "38!");
        }

        [Fact]
        public async Task Where_after_Where_on_dictionaries_behaves_correctly()
        {
            var list = new DictionaryReactiveCollectionSource<int, int>();

            var changesTask = list.ReactiveCollection
                .Where(x => x % 2 == 0)
                .Where(x => x % 3 == 0)
                .Changes
                .Take(3)
                .ToArray()
                .ToTask();

            list.Add(1, 1);
            list.Add(2, 2);
            list.Add(3, 3);
            list.Add(4, 6);
            list.Remove(1);
            list.Remove(2);
            list.Remove(3);
            list.Remove(4);

            var changes = await changesTask;

            changes[0].Action.Should().Be(NotifyCollectionChangedAction.Reset);
            changes[1].Action.Should().Be(NotifyCollectionChangedAction.Add);
            changes[1].Current.Should()
                .HaveCount(1).And
                .Contain(4, 6);
            changes[2].Action.Should().Be(NotifyCollectionChangedAction.Remove);
            changes[2].Current.Should().BeEmpty();
        }

        [Fact]
        public void Sort_after_Where_squashes_both_operators()
        {
            var list = new ListReactiveCollectionSource<int>();

            var projectedList = list.ReactiveCollection
                .Where(x => x % 2 == 0)
                .Sort();

            var transformed = projectedList as SortedListTransformationReactiveCollection<int, int>;

            transformed.Should().NotBeNull();
            transformed.Source.Should().BeSameAs(list.ReactiveCollection);
            transformed.Selector.Should().BeNull();
            transformed.Filter.Should().NotBeNull();
        }

        [Fact]
        public void SortSet_after_Where_squashes_both_operators()
        {
            var list = new ListReactiveCollectionSource<int>();

            var projectedList = list.ReactiveCollection
                .Where(x => x % 2 == 0)
                .SortSet();

            var transformed = projectedList as SortedSetTransformationReactiveCollection<int, int>;

            transformed.Should().NotBeNull();
            transformed.Source.Should().BeSameAs(list.ReactiveCollection);
            transformed.Selector.Should().BeNull();
            transformed.Filter.Should().NotBeNull();
        }

        [Fact]
        public async Task Select_after_SortSet_preserves_order()
        {
            var list = new DictionaryReactiveCollectionSource<int, int>();

            var lastTask = list.ReactiveCollection
                .SortSet(new Comparison<KeyValuePair<int, int>>((x, y) => x.Value.CompareTo(y.Value)).ToComparer())
                .Select(x => x.Value.ToString())
                .Changes
                .Skip(4)
                .FirstAsync()
                .ToTask();

            list.Add(1, 36);
            list.Add(2, 35);
            list.Add(4, 34);
            list.Add(0, 37);

            (await lastTask).Current.Should().Equal("34", "35", "36", "37");
        }
    }
}
