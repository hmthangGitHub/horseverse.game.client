using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using io.hverse.game.protogen;

public interface IPingDomainService
{
    UniTaskVoid StartPingService();
}

internal class PingDomainService : IDisposable, IPingDomainService
{
    private readonly IDIContainer container;
    private CancellationTokenSource cts;
    private CancellationToken token;
    private ISocketClient socketClient;
    private ISocketClient SocketClient => socketClient ??= container.Inject<ISocketClient>();

    public static PingDomainService Instantiate(IDIContainer container)
    {
        return new PingDomainService(container);
    }

    public async UniTaskVoid StartPingService()
    {
        DisposeUtility.SafeDispose(ref cts);
        cts = new CancellationTokenSource();
        token = cts.Token;
        
        while (!cts.IsCancellationRequested)
        {
            await UniTask.Delay(TimeSpan.FromSeconds(5), cancellationToken : token);
            await SocketClient.Send<GameMessage, GameMessage>(new GameMessage()
            {
                MsgType = GameMessageType.PingMessage
            }, token : token);
        }
    }
    
    private PingDomainService(IDIContainer container)
    {
        this.container = container;
        
    }

    public void Dispose()
    {
        DisposeUtility.SafeDispose(ref cts);
        token = default;
        socketClient = default;
    }
}
