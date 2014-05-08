// (c) Copyright 2014 ExRam GmbH & Co. KG http://www.exram.de
//
// Licensed using Microsoft Public License (Ms-PL)
// Full License description can be found in the LICENSE
// file.

namespace ExRam.ReactiveCollections
{
    public interface IReactiveCollectionSource<out TNotification, T>
         where TNotification : ICollectionChangedNotification<T>
    {
        IReactiveCollection<TNotification, T> ReactiveCollection
        {
            get;
        }
    }
}
