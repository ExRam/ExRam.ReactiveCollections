// (c) Copyright 2014 ExRam GmbH & Co. KG http://www.exram.de
//
// Licensed using Microsoft Public License (Ms-PL)
// Full License description can be found in the LICENSE
// file.

using System;

namespace ExRam.ReactiveCollections
{
    public interface IReactiveCollection<out TNotification>
        where TNotification : ICollectionChangedNotification
    {
        IObservable<TNotification> Changes
        {
            get;
        }
    }
}
