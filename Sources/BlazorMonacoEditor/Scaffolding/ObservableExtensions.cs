using System;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace BlazorMonacoEditor.Scaffolding;

internal static class ObservableExtensions
{
    public static IObservable<T1> SelectAsync<T, T1>(this IObservable<T> observable, Func<T, CancellationToken, Task<T1>> supplier)
    {
        return observable.Select(x => Observable.FromAsync((token) => supplier(x, token)).Take(1)).Concat();
    }
    
    public static IDisposable SubscribeAsync<T>(this IObservable<T> observable, Func<T, CancellationToken, Task> supplier, Action<Exception> onError)
    {
        return observable.Select(x => Observable.FromAsync(token => supplier(x, token)).Take(1)).Concat().Subscribe(_ => { }, onError);
    }
    
    public static IDisposable SubscribeAsync<T>(this IObservable<T> observable, Func<T, Task> supplier, Action<Exception> onError)
    {
        return observable.Select(x => Observable.FromAsync(_ => supplier(x)).Take(1)).Concat().Subscribe(_ => { }, onError);
    }
    
    public static IDisposable SubscribeAsync<T>(this IObservable<T> observable, Func<T, CancellationToken, Task> supplier)
    {
        return SubscribeAsync(observable, supplier, _ => { });
    }
}