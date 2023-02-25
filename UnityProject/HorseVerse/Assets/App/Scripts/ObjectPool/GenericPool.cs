using System;

public class GenericPool<T> : ObjectPools<T> where T : IPoolableObject, IDisposable
{
    private readonly Func<T> onCreateInstance;
    private readonly Action<T> onBeforeRentEvent;
    private readonly Action<T> onBeforeReturnEvent;

    public GenericPool(Func<T> onCreateInstance,
                       Action<T> onBeforeRentEvent,
                       Action<T> onBeforeReturnEvent)
    {
        this.onCreateInstance = onCreateInstance;
        this.onBeforeRentEvent = onBeforeRentEvent;
        this.onBeforeReturnEvent = onBeforeReturnEvent;
    }

    protected override T CreateInstance() => onCreateInstance();
    protected override void OnBeforeRent(T instance) => onBeforeRentEvent(instance);
    protected override void OnBeforeReturn(T instance) => onBeforeReturnEvent(instance);
    protected override void OnClear(T instance) => instance.Dispose();
}
