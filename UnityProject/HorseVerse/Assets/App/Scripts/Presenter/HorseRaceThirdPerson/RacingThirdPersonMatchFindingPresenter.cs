using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine.Rendering;
using Random = System.Random;

public class RacingThirdPersonMatchFindingPresenter : IDisposable
{
    private readonly IDIContainer container;
    private HorseRaceContext horseRaceContext;
    private UIRacingFindMatch uiRacingFindMatch;
    private CancellationTokenSource cts;
    private UITouchDisablePresenter uiTouchDisablePresenter;
    private readonly HorseRaceThirdPersonDataFactory horseRaceThirdPersonDataFactory;

    private HorseRaceContext HorseRaceContext => horseRaceContext ??= container.Inject<HorseRaceContext>();
    private UITouchDisablePresenter UITouchDisablePresenter => uiTouchDisablePresenter ??= container.Inject<UITouchDisablePresenter>();
    
    public RacingThirdPersonMatchFindingPresenter(IDIContainer container)
    {
        this.container = container;
        horseRaceThirdPersonDataFactory = new HorseRaceThirdPersonDataFactory(container);
    }

    public async UniTask FindMatchAsync()
    {
        cts.SafeCancelAndDispose();
        cts = new CancellationTokenSource();
        await UITouchDisablePresenter.ShowTillFinishTaskAsync(InstantiateUI());
        uiRacingFindMatch.SetEntity(new UIRacingFindMatch.Entity()
        {
            gameStartingPopupVisible = true,
        });
        await uiRacingFindMatch.In();
        await UITouchDisablePresenter.Delay(3.0f);
        ApplyHorseRaceContextData();
    }

    private void ApplyHorseRaceContextData()
    {
        HorseRaceContext.HorseRaceThirdPersonMatchData = new HorseRaceThirdPersonMatchData()
        {
            HorseRaceInfos = horseRaceThirdPersonDataFactory.CreateHorseRaceInfo()
        };
        HorseRaceContext.HorseBriefInfos = HorseRaceContext.HorseRaceThirdPersonMatchData.HorseRaceInfos;
        HorseRaceContext.MasterMapId = RacingState.MasterMapId;
    }

    private async UniTask InstantiateUI()
    {
        uiRacingFindMatch = await UILoader.Instantiate<UIRacingFindMatch>(UICanvas.UICanvasType.PopUp, token: cts.Token);
    }

    public void Dispose()
    {
        DisposeUtility.SafeDispose(ref cts);
        UILoader.SafeRelease(ref uiRacingFindMatch);
    }
}
