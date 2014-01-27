﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ExRam.ReactiveCollections.Tests
{
    [TestClass]
    public class PerformanceTest
    {
        [Ignore]
        [TestMethod]
        public async Task DictionaryPerformanceTest()
        {
            var count = 10000;
            var dict = new DictionaryReactiveCollectionSource<string, int>();

            var stopWatch = new Stopwatch();
            var lastTask = dict.ReactiveCollection.Changes
                .Skip(count)
                .FirstAsync()
                .ToTask();

            stopWatch.Start();

            for (var i = 0; i < count; i++)
            {
                dict.Add(i.ToString(), i);
            }

            for (var i = count - 1; i >= 0; i--)
            {
                dict.Remove(i.ToString());
            }

            var last = await lastTask;

            stopWatch.Stop();
            Console.WriteLine(stopWatch.Elapsed);
        }
    }
}
