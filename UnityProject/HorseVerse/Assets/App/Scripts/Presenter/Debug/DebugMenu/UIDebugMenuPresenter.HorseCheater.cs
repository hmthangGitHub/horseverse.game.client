using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using io.hverse.game.protogen;
using UnityEngine;

public partial class UIDebugMenuPresenter
{
    private IHorseRepository horseRepository;
    private IHorseRepository HorseRepository => horseRepository ??= Container.Inject<IHorseRepository>();
    
    private ISocketClient socketClient;
    private ObjectModifierPresenter objectModifierPresenter;
    private ISocketClient SocketClient => socketClient ??= Container.Inject<ISocketClient>();
    
    private (string debugMenu, Action action) CreateHorseCheatDebugMenu()
    {
        return ("Horse/Load", () =>
        {
            var horsesDebugMenus = HorseRepository.Models
                                             .Values
                                             .Select<HorseDataModel, (string debugMenu, Action action)>(x => ($"Horse/{x.Name}", 
                                                 () => RequestCheatForHorseAsync(x.HorseBasic.Id).Forget()))
                                             .ToArray();
            horsesDebugMenus.ForEach(x => RemoveDebugMenu(x.debugMenu));
            horsesDebugMenus.ForEach(x => AddDebugMenu(x.debugMenu, x.action));
        });
    }

    private async UniTaskVoid RequestCheatForHorseAsync(long horseId)
    {
        var horseCheat = CreateHorseCheatRequest(horseId);

        UpdateDebugMenuState(UIDebugMenuState.State.Collapse);
        var modifiedAction = await objectModifierPresenter.ModifiedObjectAsync(horseCheat, $"Debug/Horse/{horseCheat.Name}");
        switch (modifiedAction)
        {
            case ObjectModifierPresenter.ModifyAction.Cancel:
                UpdateDebugMenuState(UIDebugMenuState.State.Expand);
                break;
            case ObjectModifierPresenter.ModifyAction.Modified:
                await SocketClient.Send<CheatHorseInfoRequest, CheatHorseInfoResponse>(horseCheat);
                await UpdateHorseAsync(horseCheat);
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    private async UniTask UpdateHorseAsync(CheatHorseInfoRequest horseCheat)
    {
        var horseBasic = new HorseBasic()
        {
            Age = horseCheat.Age,
            Sex = horseCheat.Sex,
            HorseType = horseCheat.HorseType,
            ColorType = horseCheat.ColorType,
            Name = horseCheat.Name,
            Rarity = horseCheat.Rarity,
            Id = horseCheat.HorseId,
        };

        var horseAttribute = new HorseAttribute()
        {
            Id = horseCheat.HorseId,
            Ba = horseCheat.Ba,
            Bms = horseCheat.Bms,
            Hms = horseCheat.Hms,
            Mms = horseCheat.Mms,
            Sa = horseCheat.Sa,
            SprintNumber = horseCheat.SprintNumber,
            SprintTime = horseCheat.SprintTime,
            RestoreSprintTime = horseCheat.RestoreSprintTime,
        };

        var horseRising = new HorseRising()
        {
            Id = horseCheat.HorseId,
            Exp = horseCheat.Exp,
            Happiness = horseCheat.Happiness,
            BreedingCount = horseCheat.BreedingCount,
            CoinCollected = horseCheat.CoinCollected,
            GrowUp = horseCheat.GrowUp,
            BreedingCoolDown = horseCheat.BreedingCoolDown
        };
        await HorseRepository.UpdateModelAsync(horseBasic);
        await HorseRepository.UpdateModelAsync(horseAttribute);
        await HorseRepository.UpdateModelAsync(horseRising);
    }

    private CheatHorseInfoRequest CreateHorseCheatRequest(long horseId)
    {
        var horseDataModel = HorseRepository.Models[horseId];
        return new CheatHorseInfoRequest()
        {
            HorseId = horseDataModel.HorseBasic.Id,
            Age = horseDataModel.HorseBasic.Age,
            Sex = horseDataModel.HorseBasic.Sex,
            HorseType = horseDataModel.HorseBasic.HorseType,
            ColorType = horseDataModel.HorseBasic.ColorType,
            Name = horseDataModel.HorseBasic.Name,
            Rarity = horseDataModel.HorseBasic.Rarity,
            Ba = horseDataModel.HorseAttribute.Ba,
            Bms = horseDataModel.HorseAttribute.Bms,
            Hms = horseDataModel.HorseAttribute.Hms,
            Mms = horseDataModel.HorseAttribute.Mms,
            Sa = horseDataModel.HorseAttribute.Sa,
            SprintNumber = horseDataModel.HorseAttribute.SprintNumber,
            SprintTime = horseDataModel.HorseAttribute.SprintTime,
            RestoreSprintTime = horseDataModel.HorseAttribute.RestoreSprintTime,
            Exp = horseDataModel.HorseRising.Exp,
            Happiness = horseDataModel.Happiness,
            BreedingCount = horseDataModel.HorseRising.BreedingCount,
            CoinCollected = horseDataModel.HorseRising.CoinCollected,
            GrowUp = horseDataModel.HorseRising.GrowUp,
            BreedingCoolDown = horseDataModel.HorseRising.BreedingCoolDown
        };
    }
}
