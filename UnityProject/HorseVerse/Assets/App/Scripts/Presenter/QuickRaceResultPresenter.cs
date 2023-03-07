using Cysharp.Threading.Tasks;
using System;
using System.Threading;
using System.Linq;
using System.Threading.Tasks;
using io.hverse.game.protogen;

public class QuickRaceResultPresenter : IDisposable
{
    private IDIContainer Container { get; }
    private RacingModeSummaryResultPresenter racingSummaryResultPresenter;

    private ISocketClient socketClient;
    private ISocketClient SocketClient => socketClient ??= Container.Inject<ISocketClient>();

    private UserDataRepository userDataRepository;
    private UserDataRepository UserDataRepository => userDataRepository ??= Container.Inject<UserDataRepository>();


    public QuickRaceResultPresenter(IDIContainer container)
    {
        Container = container;
        racingSummaryResultPresenter = new RacingModeSummaryResultPresenter(Container);
    }

    public async UniTask ShowResultAsync()
    {
        await racingSummaryResultPresenter.ShowSummaryResultAsync();
    }
    
    public void Dispose()
    {
        DisposeUtility.SafeDispose(ref racingSummaryResultPresenter);
    }


    public async UniTask UpdatePlayerInfo()
    {
        var res = await SocketClient.Send<PlayerInfoRequest, PlayerInfoResponse>(new PlayerInfoRequest(), isHighPriority: true);
        if (res.PlayerInfo != default) {
            await UserDataRepository.UpdateCoin(res.PlayerInfo.Chip);
        }

    }
}