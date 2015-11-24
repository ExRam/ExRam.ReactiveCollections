using System;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Reactive.Threading.Tasks;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ExRam.ReactiveCollections.Tests
{
    [TestClass]
    public class ObservableExtensionsTest
    {
        [TestMethod]
        public async Task ReplayFresh_provides_a_fresh_observable()
        {
            var sourceSubject = new Subject<int>();

            var replayed = sourceSubject
                .ReplayFresh(1)
                .RefCount();

            using (replayed.Subscribe())
            {
                var task1 = replayed
                    .FirstAsync()
                    .ToTask();

                sourceSubject.OnNext(36);

                Assert.AreEqual(36, await task1);
                Assert.AreEqual(36, await replayed
                    .FirstAsync()
                    .ToTask());
            }

            var task2 = replayed
                .FirstAsync()
                .ToTask();

            sourceSubject.OnNext(37);

            Assert.AreEqual(37, await task2);
        }
    }
}
