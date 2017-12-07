// (c) Copyright 2014 ExRam GmbH & Co. KG http://www.exram.de
//
// Licensed using Microsoft Public License (Ms-PL)
// Full License description can be found in the LICENSE
// file.

using JetBrains.Annotations;

namespace ExRam.ReactiveCollections
{
    public interface IReactiveCollectionSource<out TNotification>
         where TNotification : ICollectionChangedNotification
    {
        [NotNull]
        IReactiveCollection<TNotification> ReactiveCollection
        {
            get;
        }
    }
}
