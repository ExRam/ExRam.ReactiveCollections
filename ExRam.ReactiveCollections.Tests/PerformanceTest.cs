using System;
using System.Diagnostics;
using System.Globalization;
using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;
using System.Threading.Tasks;
using Xunit;

namespace ExRam.ReactiveCollections.Tests
{
    public class PerformanceTest
    {
        [Fact(Skip="")]
        public async Task DictionaryPerformanceTest()
        {
            const int count = 10000;
            var dict = new DictionaryReactiveCollectionSource<string, int>();

            var stopWatch = new Stopwatch();
            var lastTask = dict.ReactiveCollection.Changes
                .Skip(count)
                .FirstAsync()
                .ToTask();

            stopWatch.Start();

            for (var i = 0; i < count; i++)
            {
                dict.Add(i.ToString(CultureInfo.InvariantCulture), i);
            }

            for (var i = count - 1; i >= 0; i--)
            {
                dict.Remove(i.ToString(CultureInfo.InvariantCulture));
            }

            await lastTask;

            stopWatch.Stop();
            Console.WriteLine(stopWatch.Elapsed);
        }
    }
}
