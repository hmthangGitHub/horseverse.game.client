public interface IDIContainer
{
    T Inject<T>();
    void Bind<T>(T dependency);
    void RemoveAndDisposeIfNeed<T>();
}