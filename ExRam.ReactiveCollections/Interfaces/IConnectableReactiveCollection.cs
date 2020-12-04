// (c) Copyright 2014 ExRam GmbH & Co. KG http://www.exram.de
//
// Licensed using Microsoft Public License (Ms-PL)
// Full License description can be found in the LICENSE
// file.

using System;

namespace ExRam.ReactiveCollections
{
    public interface IConnectableReactiveCollection<out TNotification> : IReactiveCollection<TNotification>
        where TNotification : ICollectionChangedNotification
    {
        IDisposable Connect();
    }
}
