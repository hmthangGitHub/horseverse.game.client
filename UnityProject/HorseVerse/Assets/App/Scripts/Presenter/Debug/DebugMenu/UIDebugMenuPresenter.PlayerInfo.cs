#if ENABLE_DEBUG_MODULE
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using io.hverse.game.protogen;
using UnityEngine;

public partial class UIDebugMenuPresenter
{
    private IUserDataRepository userDataRepository;
    private IUserDataRepository UserDataRepository => userDataRepository ??= Container.Inject<IUserDataRepository>();
    private (string debugMenu, Action action) CreatePlayerInfoCheatDebugMenu()
    {
        return ("PlayerInfo", () =>
        {
            RequestCheatForPlayerAsync().Forget();
        });
    }
    
    private async UniTaskVoid RequestCheatForPlayerAsync()
    {
        var playerInfoCheatRequest = CreatePlayerInfoCheatRequest();

        UpdateDebugMenuState(UIDebugMenuState.State.Collapse);
        var modifiedAction = await objectModifierPresenter.ModifiedObjectAsync(playerInfoCheatRequest, $"Debug/PlayerInfo/");
        switch (modifiedAction)
        {
            case ObjectModifierPresenter.ModifyAction.Cancel:
                UpdateDebugMenuState(UIDebugMenuState.State.Expand);
                break;
            case ObjectModifierPresenter.ModifyAction.Modified:
                await SocketClient.Send<CheatPlayerInfoRequest, CheatPlayerInfoResponse>(playerInfoCheatRequest);
                await UpdatePlayerInfoAsync(playerInfoCheatRequest);
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    private async UniTask UpdatePlayerInfoAsync(CheatPlayerInfoRequest playerInfoCheatRequest)
    {
        await UserDataRepository.UpdateLightPlayerInfoAsync(new LitePlayerInfo()
        {
            Chip = playerInfoCheatRequest.Chip,
            FreeRacingNumber = playerInfoCheatRequest.FreeRacingNumber,
            TrainingHighestScore = playerInfoCheatRequest.TrainingHighestScore,
        });
    }

    private CheatPlayerInfoRequest CreatePlayerInfoCheatRequest()
    {
        return new CheatPlayerInfoRequest()
        {
            Chip = UserDataRepository.Current.Coin,
            Name = UserDataRepository.Current.UserName,
            FreeRacingNumber = UserDataRepository.Current.DailyRacingNumberLeft,
            TrainingHighestScore = UserDataRepository.Current.TrainingHighScore
        };
    }
}
#endif