using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;
using System.Threading.Tasks;
using FluentAssertions;
using VerifyXunit;
using Xunit;

namespace ExRam.ReactiveCollections.Tests
{
    internal sealed class DeterministicStringKeyComparer : IEqualityComparer<string>
    {
        public static readonly DeterministicStringKeyComparer Instance = new();
        
        private DeterministicStringKeyComparer()
        {
            
        }
        
        public bool Equals(string? x, string? y)
        {
            return StringComparer.Ordinal.Compare(x, y) == 0;
        }

        public int GetHashCode(string str)
        {
            unchecked
            {
                var hash1 = (5381 << 16) + 5381;
                var hash2 = hash1;

                for (var i = 0; i < str.Length; i += 2)
                {
                    hash1 = ((hash1 << 5) + hash1) ^ str[i];
                    if (i == str.Length - 1)
                        break;
                    hash2 = ((hash2 << 5) + hash2) ^ str[i + 1];
                }

                return hash1 + hash2 * 1566083941;
            }
        }
    }
    
    public class DictionaryReactiveCollectionSourceTest : VerifyBase
    {
        public DictionaryReactiveCollectionSourceTest() : base()
        {

        }
        
        [Fact]
        public async Task First_notification_is_reset()
        {
            var list = new DictionaryReactiveCollectionSource<string, int>();

            await Verify(await list.ReactiveCollection.Changes
                .FirstAsync()
                .ToTask());
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

            await Verify(notificationTask);
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

            await Verify(notificationTask);
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
            var list = new DictionaryReactiveCollectionSource<string, int>(DeterministicStringKeyComparer.Instance);

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

            await Verify(notificationTask);
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

            await Verify(notificationTask);
        }

        [Fact]
        public async Task Count_reflects_actual_count()
        {
            var list = new DictionaryReactiveCollectionSource<string, int>(DeterministicStringKeyComparer.Instance)
            {
                { "Key1", 1 },
                { "Key2", 2 },
                { "Key3", 3 }
            };

            await Verify(list);
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

            await Verify(notificationTask);
        }

        [Fact]
        public async Task Contains()
        {
            var dict = new DictionaryReactiveCollectionSource<string, int>
            {
                { "Key", 1 }
            };

            await Verify((
                dict.Contains(new KeyValuePair<string, int>("Key", 1)),
                dict.Contains(new KeyValuePair<string, int>("Key1", 2))));
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

            await Verify(notificationTask);
        }

        [Fact]
        public async Task RemoveRange()
        {
            var list = new DictionaryReactiveCollectionSource<string, int>(DeterministicStringKeyComparer.Instance);

            var notificationsTask = list.ReactiveCollection.Changes
                .Take(3)
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

            await Verify(notificationsTask);
        }

        [Fact]
        public async Task SetItem()
        {
            var list = new DictionaryReactiveCollectionSource<string, int>(DeterministicStringKeyComparer.Instance);

            var notificationTask = list.ReactiveCollection.Changes
                .Skip(2)
                .FirstAsync()
                .ToTask();

            list.AddRange(new Dictionary<string, int>
            {
                { "Key1", 1 },
                { "Key2", 2 },
                { "Key3", 3 }
            });
            list.SetItem("Key2", 3);

            await Verify(notificationTask);
        }

        [Fact]
        public async Task SetItems()
        {
            var list = new DictionaryReactiveCollectionSource<string, int>(DeterministicStringKeyComparer.Instance);

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

            await Verify(notificationTask);
        }

        [Fact]
        public async Task TryGetValue()
        {
            var list = new DictionaryReactiveCollectionSource<string, int>
            {
                { "Key1", 1 }
            };

            await Verify((
                list.TryGetValue("Key1", out var value),
                value,
                list.TryGetValue("Key2", out _)));
        }

        [Fact]
        public async Task Item()
        {
            var list = new DictionaryReactiveCollectionSource<string, int>
            {
                { "Key1", 1 }
            };

            var before = list["Key1"].Should().Be(1);
            list["Key1"] = 2;
            var after = list["Key1"].Should().Be(2);

            await Verify((before, after));
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
