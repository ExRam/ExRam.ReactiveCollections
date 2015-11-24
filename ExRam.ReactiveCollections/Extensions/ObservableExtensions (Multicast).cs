// (c) Copyright 2014 ExRam GmbH & Co. KG http://www.exram.de
//
// Licensed using Microsoft Public License (Ms-PL)
// Full License description can be found in the LICENSE
// file.

using System.Diagnostics.Contracts;
using System.Reactive.Disposables;
using System.Reactive.Subjects;

namespace System.Reactive.Linq
{
    public static partial class ObservableExtensions
    {
        #region MulticastConnectableObservable
        internal sealed class MulticastConnectableObservable<T> : IConnectableObservable<T>
        {
            private readonly IObservable<T> _source;
            private readonly object _syncRoot = new object();
            private readonly Func<ISubject<T>> _subjectFactory;

            private ISubject<T> _currentSubject;
            private IDisposable _currentSubscription;

            public MulticastConnectableObservable(IObservable<T> source, Func<ISubject<T>> subjectFactory)
            {
                Contract.Requires(source != null);
                Contract.Requires(subjectFactory != null);

                this._source = source;
                this._subjectFactory = subjectFactory;
            }

            public IDisposable Connect()
            {
                lock (this._syncRoot)
                {
                    if (this._currentSubscription != null)
                        throw new InvalidOperationException();

                    if (this._currentSubject == null)
                        this._currentSubject = this._subjectFactory();

                    var sourceSubscription = this._currentSubscription = this._source.Subscribe(this._currentSubject);

                    return new CompositeDisposable(
                        sourceSubscription,
                        Disposable.Create(() =>
                        {
                            lock (this._syncRoot)
                            {
                                if (object.ReferenceEquals(this._currentSubscription, sourceSubscription))
                                {
                                    this._currentSubscription = null;

                                    (this._currentSubject as IDisposable)?.Dispose();
                                    this._currentSubject = null;
                                }
                            }
                        }));
                }
            }

            public IDisposable Subscribe(IObserver<T> observer)
            {
                lock (this._syncRoot)
                {
                    if (this._currentSubject == null)
                        this._currentSubject = this._subjectFactory();

                    return this._currentSubject.Subscribe(observer);
                }
            }
        }
        #endregion

        internal static IConnectableObservable<T> Multicast<T>(this IObservable<T> source, Func<ISubject<T>> subjectFactory)
        {
            Contract.Requires(source != null);
            Contract.Ensures(Contract.Result<IObservable<T>>() != null);

            return new MulticastConnectableObservable<T>(source, subjectFactory);
        }

        internal static IConnectableObservable<T> ReplayFresh<T>(this IObservable<T> observable, int bufferSize)
        {
            return observable
                .Multicast(() => new ReplaySubject<T>(bufferSize));
        }
    }
}
