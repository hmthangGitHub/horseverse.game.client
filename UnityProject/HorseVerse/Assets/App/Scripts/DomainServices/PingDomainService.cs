using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using io.hverse.game.protogen;

public interface IPingDomainService
{
    UniTaskVoid StartPingService();
    void StopPingService();
}

internal class PingDomainService : IDisposable, IPingDomainService
{
    private readonly IDIContainer container;
    private CancellationTokenSource cts;
    private ISocketClient socketClient;
    private ISocketClient SocketClient => socketClient ??= container.Inject<ISocketClient>();

    public static PingDomainService Instantiate(IDIContainer container)
    {
        return new PingDomainService(container);
    }

    public async UniTaskVoid StartPingService()
    {
        cts.SafeCancelAndDispose();
        cts = new CancellationTokenSource();
        
        while (!cts.IsCancellationRequested)
        {
            await UniTask.Delay(TimeSpan.FromSeconds(5), ignoreTimeScale : true, cancellationToken : cts.Token);
            await SocketClient.Send<GameMessage, GameMessage>(new GameMessage()
            {
                MsgType = GameMessageType.PingMessage
            }, token : cts.Token);
        }
    }

    public void StopPingService()
    {
        DisposeUtility.SafeDispose(ref cts);
    }

    private PingDomainService(IDIContainer container)
    {
        this.container = container;
    }

    public void Dispose()
    {
        DisposeUtility.SafeDispose(ref cts);
        socketClient = default;
    }
}
