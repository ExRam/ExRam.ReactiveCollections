ExRam.ReactiveCollections
=========================

To explain the idea behind ReactiveCollections, let's look at this completely ficticious conversation between Alice and Bob:

<b>Alice:</b> Well, what's ReactiveCollections ?

<b>Bob:</b> Glad you ask. Do you know Reactive Extensions ?

<b>Alice:</b> Of course, it's a framework to model and compose asynchronous events. Great stuff by the way.

<b>Bob:</b> It is. But I like to see Rx as a way to model change over time.

<b>Alice:</b> What does change?

<b>Bob:</b> Whatever you like, for example the state of some variable in an object.

<b>Alice:</b> I see. So, ReactiveCollections uses Rx to notify about the state of some collection? Isn't that just like an <code>ObservableCollection</code>? There's a <code>CollectionChanged</code> event on it, couldn't we just use that?

<b>Bob:</b> Yes and no. See, an <code>ObservableCollection</code> still uses an internal mutable state to represent its current content.

<b>Alice:</b> Why is that bad?

<b>Bob:</b> You have to synchronize access to it. You have to lock the collection every time you read or write to it.

<b>Alice:</b> Oh that's ok, I just put <code>lock(items)</code> everywhere and I'll be fine.

<b>Bob:</b> You could just do that. But blocking is so uncool in times of async/await. Moreover, are you really going to block your UI-thread? And why wouldn't you want to allow mutiple readers.

<b>Alice:</b> There's always <code>ReaderWriterLock</code>...

<b>Bob:</b> ...and it's fun to share ReaderWriterLock instances between readers and writers throughout your whole application.

<b>Alice:</b> No it's not. So, I guess that's where Immutable Collections come into play.

<b>Bob:</b> Yes! While we model change of state in a collection with Rx, Immutable Collections model the state itself! And whenever you got hold of some current state of some collection, you can do with it what you want - it's immutable, you cannot break anything.

<b>Alice:</b> So, let's say I need a simple list in my application...

<b>Bob:</b> ...just create one:
    
    var list = new ListReactiveCollectionSource().
    
<b>Alice:</b> <code>ListReactiveCollection<i>Source</i></code>? What's Source?

<b>Bob:</b> It's like <code>TaskCompletionSource</code> or <code>CancellationTokenSource</code>. You basically have a <code>ReactiveCollectionSource</code> that you use to modify the collection...

<b>Alice:</b> ...and like a <code>CancellationTokenSource</code>, <code>ListReactiveCollectionSource</code> has a property that I can give out to consumers of the list?

<b>Bob:</b> Exactly: 

    var list = new ListReactiveCollectionSource();
    var rc = list.ReactiveCollection;

<b>Alice:</b> As I see, rc implements an <code>IObservable</code> of something. What happens if I subscribe to it?

<b>Bob:</b> Your observer will be notified of the current state of the list immediately, which is an <code>ImmutableList</code> in this case.

<b>Alice:</b> Cool. And since it's just an asynchronous sequence of <code>ImmutableList</code>s, I may as well just skip the first state if I'm only interested in the second...

<b>Bob</b> ...yes...

<b>Alice</b> ...or take the first 5 changes in a list...

<b>Bob</b> ...yes...

<b>Alice</b> ...and await it...

<b>Bob</b> ...absolutely.

<b>Alice</b> I see there's more in Immutable Collections: <code>ImmutableDictionary</code>, <code>ImmutableSortedList</code>...

<b>Bob:</b> ...and there's corresponding clases in ReactiveCollections.

<b>Alice:</b> What if I wanted to filter, project or sort such a ReactiveCollection. You know, like in LINQ.

<b>Bob:</b> You can!

<b>Alice</b>: So, that's all good, but at the end of the day, I have to display my filtered, projected and sorted collection in my cool new app. But my UI only can bind to a good old ObservableCollection.

<b>Bob:</b> Just call
    
    var oc = list.ToObservableCollection 

to your ReactiveCollection and you'll be there.

<b>Alice:</b> Now I want to clone this repository and start playing with it.

<b>Bob:</b> Go ahead, thanks for your interest!







