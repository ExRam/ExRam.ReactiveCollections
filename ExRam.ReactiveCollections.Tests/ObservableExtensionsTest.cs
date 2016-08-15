using System;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Reactive.Threading.Tasks;
using System.Threading.Tasks;
using FluentAssertions;
using Xunit;

namespace ExRam.ReactiveCollections.Tests
{
    public class ObservableExtensionsTest
    {
        [Fact]
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

                (await task1).Should().Be(36);
                (await replayed.FirstAsync().ToTask()).Should().Be(36);
            }

            var task2 = replayed
                .FirstAsync()
                .ToTask();

            sourceSubject.OnNext(37);

            (await task2).Should().Be(37);
        }
    }
}
