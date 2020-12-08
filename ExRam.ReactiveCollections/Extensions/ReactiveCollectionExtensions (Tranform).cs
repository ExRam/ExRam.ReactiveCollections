using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;

namespace ExRam.ReactiveCollections
{
    public static partial class ReactiveCollectionExtensions
    {
        private static IReactiveCollection<TTargetNotification> Transform<TSourceNotification, TTargetNotification>(this IReactiveCollection<TSourceNotification> source, TTargetNotification resetTargetNotification, Func<TSourceNotification, TTargetNotification, IEnumerable<TTargetNotification>> transformation)
            where TSourceNotification : ICollectionChangedNotification
            where TTargetNotification : ICollectionChangedNotification
        {
            return source
                .Changes
                .Scan(
                    new[] { resetTargetNotification },
                    (currentTargetNotification, sourceNotification) =>
                    {
                        var newRet = transformation(sourceNotification, currentTargetNotification[^1])
                            .ToArray();

                        return newRet.Length > 0 ? newRet :
                            currentTargetNotification.Length > 0
                                ? new[] { currentTargetNotification[0] }
                                : currentTargetNotification;
                    })
                .SelectMany(x => x)
                .DistinctUntilChanged()
                .ToReactiveCollection();
        }
    }
}