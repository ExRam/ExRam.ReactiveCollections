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
using Microsoft.VisualStudio.TestPlatform.CommunicationUtilities.Serialization;
using VerifyXunit;
using Xunit;

namespace ExRam.ReactiveCollections.Tests
{
    public class ReactiveCollectionExtensionsTest : VerifyBase
    {
        public ReactiveCollectionExtensionsTest() : base()
        {

        }
        
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

            await Verify(notificationsTask);
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

            await Verify(notificationsTask);
        }

        [Fact]
        public async Task Add_to_projected_dictionary()
        {
            var list = new DictionaryReactiveCollectionSource<string, int>(DeterministicStringKeyComparer.Instance);

            var projectedList = list.ReactiveCollection
                .Select(x => x.ToString(CultureInfo.InvariantCulture));

            var notificationsTask = projectedList.Changes
                .Take(4)
                .ToArray()
                .ToTask();

            list.Add("Key1", 1);
            list.Add("Key2", 2);
            list.Add("Key3", 3);

            await Verify(notificationsTask);
        }

        [Fact]
        public async Task Remove_from_projected_list()
        {
            var list = new ListReactiveCollectionSource<int>();

            var projectedList = list.ReactiveCollection
                .Select(x => x.ToString(CultureInfo.InvariantCulture));

            var notificationsTask = projectedList.Changes
                .Take(3)
                .ToArray()
                .ToTask();

            list.AddRange(new[] { 1, 2, 3 });
            list.Remove(2);

            await Verify(notificationsTask);
        }

        [Fact]
        public async Task Remove_from_projected_set()
        {
            var list = new SortedSetReactiveCollectionSource<int>();

            var projectedList = list.ReactiveCollection
                .Select(x => x.ToString(CultureInfo.InvariantCulture));

            var notificationsTask = projectedList.Changes
                .Take(5)
                .ToArray()
                .ToTask();

            list.Add(1);
            list.Add(2);
            list.Add(3);
            list.Remove(2);

            await Verify(notificationsTask);
        }

        [Fact]
        public async Task Remove_from_projected_dictionary()
        {
            var list = new DictionaryReactiveCollectionSource<string, int>(DeterministicStringKeyComparer.Instance);

            var projectedList = list.ReactiveCollection
                .Select(x => x.ToString(CultureInfo.InvariantCulture));

            var notificationsTask = projectedList.Changes
                .Take(5)
                .ToArray()
                .ToTask();

            list.Add("Key1", 1);
            list.Add("Key2", 2);
            list.Add("Key3", 3);
            list.Remove("Key2");

            await Verify(notificationsTask);
        }

        [Fact]
        public async Task Replace_in_projected_list()
        {
            var list = new ListReactiveCollectionSource<int>();

            var projectedList = list.ReactiveCollection
                .Select(x => x.ToString(CultureInfo.InvariantCulture));

            var notificationsTask = projectedList.Changes
                .Take(3)
                .ToArray()
                .ToTask();

            list.AddRange(new[] { 1, 2, 3 });
            list.Replace(2, 4);

            await Verify(notificationsTask);
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

            var projectedList = observable
                .Select(x => x.ToString(CultureInfo.InvariantCulture));

            var notificationsTask = projectedList.Changes
                .Take(5)
                .ToArray()
                .ToTask();

            await Verify(notificationsTask);
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
                .Take(4)
                .ToArray()
                .ToTask();

            await Verify(notificationsTask);
        }

        [Fact]
        public async Task Replace_in_projected_dictionary()
        {
            var list = new DictionaryReactiveCollectionSource<string, int>(DeterministicStringKeyComparer.Instance);

            var projectedList = list.ReactiveCollection
                .Select(x => x.ToString(CultureInfo.InvariantCulture));

            var notificationsTask = projectedList.Changes
                .Take(6)
                .ToArray()
                .ToTask();

            list.Add("Key1", 1);
            list.Add("Key2", 2);
            list.Add("Key3", 3);

            list["Key2"] = 4;

            await Verify(notificationsTask);
        }

        [Fact]
        public async Task Add_to_filtered_list()
        {
            var list = new ListReactiveCollectionSource<int>();

            var projectedList = list
                .ReactiveCollection
                .Where(x => x % 2 == 0);

            var notificationsTask = projectedList.Changes
                .Take(2)
                .ToArray()
                .ToTask();

            list.Add(1);
            list.Add(3);           
            list.Add(2);

            await Verify(notificationsTask);
        }

        [Fact]
        public async Task Add_to_filtered_set()
        {
            var list = new SortedSetReactiveCollectionSource<int>();

            var projectedList = list
                .ReactiveCollection
                .Where(x => x % 2 == 0);

            var notificationsTask = projectedList.Changes
                .Take(2)
                .ToArray()
                .ToTask();

            list.Add(1);
            list.Add(3);
            list.Add(2);

            await Verify(notificationsTask);
        }

        [Fact]
        public async Task Add_to_filtered_dictionary()
        {
            var list = new DictionaryReactiveCollectionSource<string, int>();

            var projectedList = list
                .ReactiveCollection
                .Where(x => x % 2 == 0);

            var notificationsTask = projectedList.Changes
                .Take(2)
                .ToArray()
                .ToTask();

            list.Add("Key1", 1);
            list.Add("Key3", 3);
            list.Add("Key2", 2);

            await Verify(notificationsTask);
        }

        [Fact]
        public async Task Remove_from_filtered_list()
        {
            var list = new ListReactiveCollectionSource<int>();

            var projectedList = list
                .ReactiveCollection
                .Where(x => x % 2 == 0);

            var notificationsTask = projectedList.Changes
                .Take(3)
                .ToArray()
                .ToTask();

            list.AddRange(new[] { 1, 2, 3 });
            list.Remove(2);

            await Verify(notificationsTask);
        }

        [Fact]
        public async Task Remove_from_filtered_set()
        {
            var list = new SortedSetReactiveCollectionSource<int>();

            var projectedList = list
                .ReactiveCollection
                .Where(x => x % 2 == 0);

            var notificationsTask = projectedList.Changes
                .Take(3)
                .ToArray()
                .ToTask();

            list.Add(1);
            list.Add(2);
            list.Add(3);
            list.Remove(2);

            await Verify(notificationsTask);
        }

        [Fact]
        public async Task Remove_from_filtered_dictionary()
        {
            var list = new DictionaryReactiveCollectionSource<string, int>();

            var projectedList = list.ReactiveCollection
                .Where(x => x % 2 == 0);

            var notificationsTask = projectedList.Changes
                .Take(3)
                .ToArray()
                .ToTask();

            list.Add("Key1", 1);
            list.Add("Key2", 2);
            list.Add("Key3", 3);
            list.Remove("Key2");

            await Verify(notificationsTask);
        }

        [Fact]
        public async Task Replace_in_filtered_list_addition()
        {
            var list = new ListReactiveCollectionSource<int>();

            var projectedList = list.ReactiveCollection
                .Where(x => x % 2 == 0);

            var notificationsTask = projectedList.Changes
                .Take(3)
                .ToArray()
                .ToTask();

            list.AddRange(new[] { 1, 2, 3 });
            list.Replace(1, 4);

            await Verify(notificationsTask);
        }

        [Fact]
        public async Task Replace_in_filtered_list_removal()
        {
            var list = new ListReactiveCollectionSource<int>();

            var projectedList = list.ReactiveCollection
                .Where(x => x % 2 == 0);

            var notificationsTask = projectedList.Changes
                .Take(3)
                .ToArray()
                .ToTask();

            list.AddRange(new[] { 1, 2, 3 });
            list.Replace(2, 3);

            await Verify(await notificationsTask);
        }

        [Fact]
        public async Task Replace_in_filtered_list_replacement()
        {
            var list = new ListReactiveCollectionSource<int>();

            var projectedList = list.ReactiveCollection
                .Where(x => x % 2 == 0);

            var notificationsTask = projectedList.Changes
                .Take(3)
                .ToArray()
                .ToTask();

            list.AddRange(new[] { 1, 2, 3 });
            list.Replace(2, 4);

            await Verify(await notificationsTask);
        }

        [Fact]
        public async Task Replace_in_filtered_list_replacement2()
        {
            var list = new ListReactiveCollectionSource<int>(new[] { 1 });

            var projectedList = list.ReactiveCollection
                .Where(x => x % 2 == 0);

            var notificationsTask = projectedList.Changes
                .Take(2)
                .ToArray()
                .ToTask();

            list.Replace(1, 3);
            list.Replace(3, 4);

            await Verify(await notificationsTask);
        }

        [Fact]
        public async Task Replace_in_filtered_dictionary_addition()
        {
            var list = new DictionaryReactiveCollectionSource<string, int>(DeterministicStringKeyComparer.Instance);

            var projectedList = list.ReactiveCollection
                .Where(x => x % 2 == 0);

            var notificationsTask = projectedList.Changes
                .Take(3)
                .ToArray()
                .ToTask();

            list.Add("Key1", 1);
            list.Add("Key2", 2);
            list.Add("Key3", 3);

            list["Key1"] = 4;

            await Verify(await notificationsTask);
        }

        [Fact]
        public async Task Replace_in_filtered_dictionary_removal()
        {
            var list = new DictionaryReactiveCollectionSource<string, int>();

            var projectedList = list.ReactiveCollection
                .Where(x => x % 2 == 0);

            var notificationsTask = projectedList.Changes
                .Take(3)
                .ToArray()
                .ToTask();

            list.Add("Key1", 1);
            list.Add("Key2", 2);
            list.Add("Key3", 3);

            list["Key2"] = 3;

            await Verify(await notificationsTask);
        }

        [Fact]
        public async Task Replace_in_filtered_dictionary_replacement()
        {
            var list = new DictionaryReactiveCollectionSource<string, int>();

            var projectedList = list.ReactiveCollection
                .Where(x => x % 2 == 0);

            var notificationsTask = projectedList.Changes
                .Take(4)
                .ToArray()
                .ToTask();

            list.Add("Key1", 1);
            list.Add("Key2", 2);
            list.Add("Key3", 3);

            list["Key2"] = 4;

            await Verify(await notificationsTask);
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
                .Take(4)
                .ToArray()
                .ToTask();

            await Verify(await notificationsTask);
        }

        [Fact]
        public async Task Add_to_sorted_list()
        {
            var list = new ListReactiveCollectionSource<int>();

            var projectedList = list
                .ReactiveCollection
                .Sort();

            var notificationsTask = projectedList.Changes
                .Take(4)
                .ToArray()
                .ToTask();

            list.Add(3);
            list.Add(1);
            list.Add(2);

            await Verify(await notificationsTask);
        }

        [Fact]
        public async Task Add_to_sorted_dictionary()
        {
            var list = new DictionaryReactiveCollectionSource<string, int>();

            var projectedList = list
                .ReactiveCollection
                .Sort(Comparer<KeyValuePair<string, int>>.Create((x, y) => x.Value.CompareTo(y.Value)));

            var notificationsTask = projectedList.Changes
                .Take(4)
                .ToArray()
                .ToTask();

            list.Add("Key3", 3);
            list.Add("Key1", 1);
            list.Add("Key2", 2);

            await Verify(await notificationsTask);
        }

        [Fact]
        public async Task Add_to_setSorted_dictionary()
        {
            var list = new DictionaryReactiveCollectionSource<string, int>();

            var projectedList = list
                .ReactiveCollection
                .SortSet(Comparer<KeyValuePair<string, int>>.Create((x, y) => x.Value.CompareTo(y.Value)));

            var notificationsTask = projectedList.Changes
                .Take(4)
                .ToArray()
                .ToTask();

            list.Add("Key3", 3);
            list.Add("Key1", 1);
            list.Add("Key2", 2);

            await Verify(await notificationsTask);
        }

        [Fact]
        public async Task Add_to_setSorted_dictionary_with_duplicate_value()
        {
            var list = new DictionaryReactiveCollectionSource<string, int>();

            var projectedList = list
                .ReactiveCollection
                .SortSet(Comparer<KeyValuePair<string, int>>.Create((x, y) => x.Value.CompareTo(y.Value)));

            var notificationsTask = projectedList.Changes
                .Take(4)
                .ToArray()
                .ToTask();

            list.Add("Key3", 3);
            list.Add("Key1", 1);
            list.Add("Key1", 1);
            list.Add("Key2", 2);

            await Verify(await notificationsTask);
        }

        [Fact]
        public async Task Add_to_sorted_list_with_Changes()
        {
            var list = new ListReactiveCollectionSource<int>();

            var projectedList = list
                .ReactiveCollection
                .Sort();

            var notificationsTask = projectedList.Changes
                .Take(4)
                .ToArray()
                .ToTask();

            list.Add(3);
            list.Add(1);
            list.Add(2);

            await Verify(await notificationsTask);
        }

        [Fact]
        public async Task Remove_from_sorted_list()
        {
            var list = new ListReactiveCollectionSource<int>();

            var projectedList = list
                .ReactiveCollection
                .Sort();

            var notificationsTask = projectedList.Changes
                .Take(5)
                .ToArray()
                .ToTask();

            list.AddRange(new[] { 3, 2, 1 });
            list.Remove(2);

            await Verify(notificationsTask);
        }

        [Fact]
        public async Task Remove_in_sorted_dictionary()
        {
            var list = new DictionaryReactiveCollectionSource<string, int>();

            var projectedList = list.ReactiveCollection
                .Sort(Comparer<KeyValuePair<string, int>>.Create((x, y) => x.Value.CompareTo(y.Value)));

            var notificationsTask = projectedList.Changes
                .Take(4)
                .ToArray()
                .ToTask();

            list.Add("Key1", 1);
            list.Add("Key2", 2);
            list.Remove("Key1");

            await Verify(await notificationsTask);
        }

        [Fact]
        public async Task Remove_in_setSorted_dictionary()
        {
            var list = new DictionaryReactiveCollectionSource<string, int>();

            var projectedList = list.ReactiveCollection
                .SortSet(Comparer<KeyValuePair<string, int>>.Create((x, y) => x.Value.CompareTo(y.Value)));

            var notificationsTask = projectedList.Changes
                .Take(4)
                .ToArray()
                .ToTask();

            list.Add("Key1", 1);
            list.Add("Key2", 2);
            list.Remove("Key1");

            await Verify(await notificationsTask);
        }

        [Fact]
        public async Task Remove_from_sorted_list_with_Changes()
        {
            var list = new ListReactiveCollectionSource<int>();

            var projectedList = list
                .ReactiveCollection
                .Sort();

            var notificationsTask = projectedList.Changes
                .Take(5)
                .ToArray()
                .ToTask();

            list.AddRange(new[] { 3, 2, 1 });
            list.Remove(2);

            await Verify(notificationsTask);
        }

        [Fact]
        public async Task Replace_in_sorted_list()
        {
            var list = new ListReactiveCollectionSource<int>();

            var projectedList = list
                .ReactiveCollection
                .Sort();

            var notificationsTask = projectedList.Changes
                .Take(6)
                .ToArray()
                .ToTask();

            list.AddRange(new[] { 3, 2, 1 });
            list.Replace(2, 4);

            await Verify(notificationsTask);
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
                .Take(6)
                .ToArray()
                .ToTask();

            list.AddRange(new[] { 3, 2, 1 });
            list.Replace(2, 4);

            await Verify(await notificationsTask);
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
                .Take(6)
                .ToArray()
                .ToTask();

            list.Add(3, 30);
            list.Add(2, 20);
            list.Add(1, 10);
            list[2] = 40;

            await Verify(await notificationsTask);
        }

        [Fact]
        public async Task Replace_in_sorted_list_with_Changes()
        {
            var list = new ListReactiveCollectionSource<int>();

            var projectedList = list
                .ReactiveCollection
                .Sort();

            var notificationsTask = projectedList.Changes
                .Take(6)
                .ToArray()
                .ToTask();

            list.AddRange(new[] { 3, 2, 1 });
            list.Replace(2, 4);

            await Verify(notificationsTask);
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
                .Take(7)
                .ToArray()
                .ToTask();

            await Verify(notificationsTask);
        }

        [Fact]
        public async Task Replace_in_sorted_dictionary()
        {
            var list = new DictionaryReactiveCollectionSource<string, int>();

            var projectedList = list.ReactiveCollection
                .Sort(Comparer<KeyValuePair<string, int>>.Create((x, y) => x.Value.CompareTo(y.Value)));

            var notificationsTask = projectedList.Changes
                .Take(6)
                .ToArray()
                .ToTask();

            list.Add("Key1", 3);
            list.Add("Key2", 1);
            list.Add("Key3", 4);

            list["Key2"] = 2;

            await Verify(notificationsTask);
        }

        [Fact]
        public async Task Replace_in_setSorted_dictionary()
        {
            var list = new DictionaryReactiveCollectionSource<string, int>();

            var projectedList = list.ReactiveCollection
                .SortSet(Comparer<KeyValuePair<string, int>>.Create((x, y) => x.Value.CompareTo(y.Value)));

            var notificationsTask = projectedList.Changes
                .Take(6)
                .ToArray()
                .ToTask();

            list.Add("Key1", 3);
            list.Add("Key2", 1);
            list.Add("Key3", 4);

            list["Key2"] = 2;

            await Verify(notificationsTask);
        }

        [Fact]
        public async Task List_ToObservableCollection_Add()
        {
            var list = new ListReactiveCollectionSource<int>();
            var observableCollection = (INotifyCollectionChanged)list.ReactiveCollection.ToObservableCollection();

            var notificationsTask = Observable
                .FromEventPattern<NotifyCollectionChangedEventHandler, NotifyCollectionChangedEventArgs>(eh => observableCollection.CollectionChanged += eh, eh => observableCollection.CollectionChanged -= eh)
                .Take(3)
                .Select(x => x.EventArgs)
                .ToArray()
                .ToTask();

            list.Add(1);
            list.Add(2);
            list.Add(3);

            await Verify(await notificationsTask);
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

            var notificationsTask = Observable
                .FromEventPattern<NotifyCollectionChangedEventHandler, NotifyCollectionChangedEventArgs>(eh => observableCollection.CollectionChanged += eh, eh => observableCollection.CollectionChanged -= eh)
                .Take(3)
                .Select(x => x.EventArgs)
                .ToArray()
                .ToTask();

            list.Add(1);
            list.Add(2);
            list.Remove(1);

            await Verify(await notificationsTask);
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

            var notificationsTask = Observable
                .FromEventPattern<NotifyCollectionChangedEventHandler, NotifyCollectionChangedEventArgs>(eh => observableCollection.CollectionChanged += eh, eh => observableCollection.CollectionChanged -= eh)
                .Take(3)
                .Select(x => x.EventArgs)
                .ToArray()
                .ToTask();

            list.Add(1);
            list.Add(2);
            list.Replace(1, 3);

            await Verify(await notificationsTask);
        }

        [Fact]
        public async Task List_ToObservableCollection_Clear()
        {
            var list = new ListReactiveCollectionSource<int>();
            var observableCollection = (INotifyCollectionChanged)list.ReactiveCollection.ToObservableCollection();

            var notificationsTask = Observable
                .FromEventPattern<NotifyCollectionChangedEventHandler, NotifyCollectionChangedEventArgs>(eh => observableCollection.CollectionChanged += eh, eh => observableCollection.CollectionChanged -= eh)
                .Select(x => x.EventArgs)
                .Take(3)
                .ToArray()
                .ToTask();

            list.AddRange(new[] { 1, 2, 3, 4, 5 });
            list.RemoveAll(x => x > 2);

            await Verify(await notificationsTask);
        }

        [Fact]
        public async Task List_ToObservableCollection_does_not_raise_events_on_event_subscription()
        {
            var list = new ListReactiveCollectionSource<int> {1, 2, 3};

            var observableCollection = (INotifyCollectionChanged)list.ReactiveCollection.ToObservableCollection();

            var notificationsTask = Observable
                .FromEventPattern<NotifyCollectionChangedEventHandler, NotifyCollectionChangedEventArgs>(eh => observableCollection.CollectionChanged += eh, eh => observableCollection.CollectionChanged -= eh)
                .Select(x => x.EventArgs)
                .Take(1)
                .ToArray()
                .ToTask();

            ((ICollection)observableCollection).Should().BeEquivalentTo(new[] { 1, 2, 3 });
            list.Add(4);

            await Verify(await notificationsTask);
        }

        [Fact]
        public async Task SortedSet_ToObservableCollection_Add()
        {
            var list = new SortedSetReactiveCollectionSource<int>();
            var observableCollection = (INotifyCollectionChanged)list.ReactiveCollection.ToObservableCollection();

            var notificationsTask = Observable
                .FromEventPattern<NotifyCollectionChangedEventHandler, NotifyCollectionChangedEventArgs>(eh => observableCollection.CollectionChanged += eh, eh => observableCollection.CollectionChanged -= eh)
                .Take(3)
                .Select(x => x.EventArgs)
                .ToArray()
                .ToTask();

            list.Add(1);
            list.Add(2);
            list.Add(3);

            await Verify(await notificationsTask);
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

            var notificationsTask = Observable
                .FromEventPattern<NotifyCollectionChangedEventHandler, NotifyCollectionChangedEventArgs>(eh => observableCollection.CollectionChanged += eh, eh => observableCollection.CollectionChanged -= eh)
                .Take(3)
                .Select(x => x.EventArgs)
                .ToArray()
                .ToTask();

            list.Add(1);
            list.Add(2);
            list.Remove(1);

            await Verify(await notificationsTask);
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

            var notificationsTask = Observable
                .FromEventPattern<NotifyCollectionChangedEventHandler, NotifyCollectionChangedEventArgs>(eh => observableCollection.CollectionChanged += eh, eh => observableCollection.CollectionChanged -= eh)
                .Take(3)
                .Select(x => x.EventArgs)
                .ToArray()
                .ToTask();

            list.AddRange(new[] { 1, 2, 3, 4, 5 });
            list.RemoveAll(x => x > 2);

            await Verify(await notificationsTask);
        }

        [Fact]
        public async Task SortedSet_ToObservableCollection_does_not_raise_events_on_event_subscription()
        {
            var list = new ListReactiveCollectionSource<int> { 1, 2, 3 };

            var observableCollection = (INotifyCollectionChanged)list.ReactiveCollection.ToObservableCollection();

            var notificationsTask = Observable
                .FromEventPattern<NotifyCollectionChangedEventHandler, NotifyCollectionChangedEventArgs>(eh => observableCollection.CollectionChanged += eh, eh => observableCollection.CollectionChanged -= eh)
                .Select(x => x.EventArgs)
                .Take(1)
                .ToArray()
                .ToTask();

            ((ICollection)observableCollection).Should().Equal(1, 2, 3);
            list.Add(4);

            await Verify(await notificationsTask);
        }

        [Fact]
        public async Task GetValueObservable_Test1()
        {
            var dict = new DictionaryReactiveCollectionSource<int, int>();

            var arrayTask = dict.ReactiveCollection.GetValues(1)
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

            await Verify(await notificationsTask);
        }

        [Fact]
        public async Task Concat_Remove()
        {
            var source1 = new ListReactiveCollectionSource<int>();
            var source2 = new ListReactiveCollectionSource<int>();

            var concat = source1.ReactiveCollection.Concat(source2.ReactiveCollection);

            var notificationsTask = concat.Changes
                .Take(5)
                .ToArray()
                .ToTask();

            source1.Add(1);
            source2.Add(2);
            source1.RemoveAt(0);
            source2.RemoveAt(0);
            
            await Verify(await notificationsTask);
        }

        [Fact]
        public async Task Concat_Clear()
        {
            var source1 = new ListReactiveCollectionSource<int>();
            var source2 = new ListReactiveCollectionSource<int>();

            var concat = source1.ReactiveCollection.Concat(source2.ReactiveCollection);

            var notificationsTask = concat.Changes
                .Take(5)
                .ToArray()
                .ToTask();

            source1.Add(1);
            source2.Add(2);
            source1.Clear();
            source2.Clear();

            await Verify(await notificationsTask);
        }

        [Fact]
        public async Task Concat_Replace_single()
        {
            var source1 = new ListReactiveCollectionSource<int>();
            var source2 = new ListReactiveCollectionSource<int>();

            var concat = source1.ReactiveCollection.Concat(source2.ReactiveCollection);

            var notificationsTask = concat.Changes
                .Take(5)
                .ToArray()
                .ToTask();

            source1.AddRange(new[] { 1, 2, 3 });
            source2.AddRange(new[] { 4, 5, 6 });
            source1.Replace(2, -2);
            source2.Replace(5, -5);

            await Verify(await notificationsTask);
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
                .Take(5)
                .ToArray()
                .ToTask();

            changesSubject1.OnNext(new ListChangedNotification<int>(ImmutableList<int>.Empty, NotifyCollectionChangedAction.Reset, ImmutableList<int>.Empty, ImmutableList<int>.Empty, null));
            changesSubject2.OnNext(new ListChangedNotification<int>(ImmutableList<int>.Empty, NotifyCollectionChangedAction.Reset, ImmutableList<int>.Empty, ImmutableList<int>.Empty, null));

            changesSubject1.OnNext(new ListChangedNotification<int>(new[] { 1, 2, 3 }.ToImmutableList(), NotifyCollectionChangedAction.Add, ImmutableList<int>.Empty, new[] { 1, 2, 3 }.ToImmutableList(), 0));
            changesSubject2.OnNext(new ListChangedNotification<int>(new[] { 4, 5, 6 }.ToImmutableList(), NotifyCollectionChangedAction.Add, ImmutableList<int>.Empty, new[] { 4, 5, 6 }.ToImmutableList(), 0));

            changesSubject1.OnNext(new ListChangedNotification<int>(new[] { 1, -2, -3 }.ToImmutableList(), NotifyCollectionChangedAction.Replace, new[] { 2, 3 }.ToImmutableList(), new[] { -2, -3 }.ToImmutableList(), 1));
            changesSubject2.OnNext(new ListChangedNotification<int>(new[] { 4, -5, -6 }.ToImmutableList(), NotifyCollectionChangedAction.Replace, new[] { 5, 6 }.ToImmutableList(), new[] { -5, -6 }.ToImmutableList(), 1));

            await Verify(await notificationsTask);
        }

        [Fact]
        public async Task Concat_Add_Delayed()
        {
            var source1 = new ListReactiveCollectionSource<int>();
            var source2 = new ListReactiveCollectionSource<int>();

            source1.Add(1);
            source2.Add(2);

            var concat = source1.ReactiveCollection.Concat(source2.ReactiveCollection);

            var notificationsTask = await concat.Changes
                .Take(1)
                .ToArray()
                .ToTask();

            await Verify(notificationsTask);
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

            var notificationsTask = await concat.Changes
                .Take(1)
                .ToArray()
                .ToTask();

            await Verify(notificationsTask);
        }

        [Fact(Skip = "x")]
        public void Select_after_Where_squashes_both_operators()
        {
            //var list = new ListReactiveCollectionSource<int>();

            //var projectedList = list.ReactiveCollection
            //    .Where(x => x % 2 == 0)
            //    .Select(x => x.ToString(CultureInfo.InvariantCulture));

            //var transformed = projectedList as ListTransformationReactiveCollection<int, string>;

            //transformed.Should().NotBeNull();
            //transformed.Source.Should().BeSameAs(list.ReactiveCollection);
            //transformed.Selector.Should().NotBeNull();
            //transformed.Filter.Should().NotBeNull();
        }

        [Fact(Skip = "x")]
        public void Where_after_Where_squashes_both_operators()
        {
            //var list = new ListReactiveCollectionSource<int>();

            //var projectedList = list.ReactiveCollection
            //    .Where(x => x % 2 == 0)
            //    .Where(x => x % 3 == 0);

            //var transformed = projectedList as ListTransformationReactiveCollection<int, int>;

            //transformed.Should().NotBeNull();
            //transformed.Source.Should().BeSameAs(list.ReactiveCollection);
            //transformed.Selector.Should().BeNull();
            //transformed.Filter.Should().NotBeNull();
            //transformed.Filter(2).Should().BeFalse();
            //transformed.Filter(3).Should().BeFalse();
            //transformed.Filter(4).Should().BeFalse();
            //transformed.Filter(6).Should().BeTrue();
        }

        [Fact(Skip = "x")]
        public void Select_after_Select_squashes_both_operators()
        {
            //var list = new ListReactiveCollectionSource<int>();

            //var projectedList = list.ReactiveCollection
            //    .Select(x => x.ToString())
            //    .Select(int.Parse);

            //var transformed = projectedList as ListTransformationReactiveCollection<int, int>;

            //transformed.Should().NotBeNull();
            //transformed.Source.Should().BeSameAs(list.ReactiveCollection);
            //transformed.Selector.Should().NotBeNull();
            //transformed.Filter.Should().BeNull();
        }

        [Fact]
        public async Task Select_after_Where_behaves_correctly()
        {
            var list = new ListReactiveCollectionSource<int>();

            var notificationsTask = list.ReactiveCollection
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

            await Verify(await notificationsTask);
        }

        [Fact]
        public async Task Where_after_Where_behaves_correctly()
        {
            var list = new ListReactiveCollectionSource<int>();

            var notificationsTask = list.ReactiveCollection
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

            await Verify(await notificationsTask);
        }
        
        [Fact(Skip= "x")]
        public void Select_after_Where_on_dictionaries_squashes_both_operators()
        {
            //var list = new DictionaryReactiveCollectionSource<int, int>();

            //var projectedList = list.ReactiveCollection
            //    .Where(x => x % 2 == 0)
            //    .Select(x => x.ToString(CultureInfo.InvariantCulture));

            //var transformed = projectedList as DictionaryTransformationReactiveCollection<int, int, string>;

            //transformed.Should().NotBeNull();
            //transformed.Source.Should().BeSameAs(list.ReactiveCollection);
            //transformed.Selector.Should().NotBeNull();
            //transformed.Filter.Should().NotBeNull();
        }

        [Fact(Skip = "x")]
        public void Where_after_Where_on_dictionaries_squashes_both_operators()
        {
            //var list = new DictionaryReactiveCollectionSource<int, int>();

            //var projectedList = list.ReactiveCollection
            //    .Where(x => x % 2 == 0)
            //    .Where(x => x % 3 == 0);

            //var transformed = projectedList as DictionaryTransformationReactiveCollection<int, int, int>;

            //transformed.Should().NotBeNull();
            //transformed.Source.Should().BeSameAs(list.ReactiveCollection);
            //transformed.Selector.Should().BeNull();
            //transformed.Filter.Should().NotBeNull();
        }

        [Fact(Skip = "x")]
        public void Select_after_Select_on_dictionaries_squashes_both_operators()
        {
            //var list = new DictionaryReactiveCollectionSource<int, int>();

            //var projectedList = list.ReactiveCollection
            //    .Select(x => x.ToString())
            //    .Select(int.Parse);

            //var transformed = projectedList as DictionaryTransformationReactiveCollection<int, int, int>;

            //transformed.Should().NotBeNull();
            //transformed.Source.Should().BeSameAs(list.ReactiveCollection);
            //transformed.Selector.Should().NotBeNull();
            //transformed.Filter.Should().BeNull();
        }

        [Fact]
        public async Task Select_after_Where_on_dictionaries_behaves_correctly()
        {
            var list = new DictionaryReactiveCollectionSource<int, int>();

            var notificationsTask = list.ReactiveCollection
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

            await Verify(await notificationsTask);
        }

        [Fact]
        public async Task Select_after_Select_on_dictionaries_behaves_correctly()
        {
            var list = new DictionaryReactiveCollectionSource<int, int>();

            var notificationsTask = list.ReactiveCollection
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

            await Verify(await notificationsTask);
        }

        [Fact]
        public async Task Where_after_Where_on_dictionaries_behaves_correctly()
        {
            var list = new DictionaryReactiveCollectionSource<int, int>();

            var notificationsTask = list.ReactiveCollection
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

            await Verify(await notificationsTask);
        }

        [Fact(Skip = "x")]
        public void Sort_after_Where_squashes_both_operators()
        {
            //var list = new ListReactiveCollectionSource<int>();

            //var projectedList = list.ReactiveCollection
            //    .Where(x => x % 2 == 0)
            //    .Sort();

            //var transformed = projectedList as SortedListTransformationReactiveCollection<int, int>;

            //transformed.Should().NotBeNull();
            //transformed.Source.Should().BeSameAs(list.ReactiveCollection);
            //transformed.Selector.Should().BeNull();
            //transformed.Filter.Should().NotBeNull();
        }

        [Fact(Skip = "x")]
        public void SortSet_after_Where_squashes_both_operators()
        {
            //var list = new ListReactiveCollectionSource<int>();

            //var projectedList = list.ReactiveCollection
            //    .Where(x => x % 2 == 0)
            //    .SortSet();

            //var transformed = projectedList as SortedSetTransformationReactiveCollection<int, int>;

            //transformed.Should().NotBeNull();
            //transformed.Source.Should().BeSameAs(list.ReactiveCollection);
            //transformed.Selector.Should().BeNull();
            //transformed.Filter.Should().NotBeNull();
        }

        [Fact]
        public async Task Select_after_SortSet_preserves_order()
        {
            var list = new DictionaryReactiveCollectionSource<int, int>();

            var notificationsTask = list.ReactiveCollection
                .SortSet(new Comparison<KeyValuePair<int, int>>((x, y) => x.Value.CompareTo(y.Value)).ToComparer())
                .Select(x => x.Value.ToString())
                .Changes
                .Take(5)
                .ToArray()
                .ToTask();

            list.Add(1, 36);
            list.Add(2, 35);
            list.Add(4, 34);
            list.Add(0, 37);

            await Verify(await notificationsTask);
        }
    }
}
