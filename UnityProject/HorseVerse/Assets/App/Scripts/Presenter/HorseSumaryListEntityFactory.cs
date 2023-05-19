using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using UnityEngine;

public class HorseSumaryListEntityFactory
{
    private IDIContainer container;
    private IReadOnlyHorseRepository horseRepository;
    private IReadOnlyHorseRepository HorseRepository => horseRepository ??= container.Inject<IReadOnlyHorseRepository>();
    private IReadOnlyUserDataRepository userReponsitory;
    private IReadOnlyUserDataRepository UserReponsitory => userReponsitory??= container.Inject<IReadOnlyUserDataRepository>();
    private MasterHorseContainer masterHorseContainer;
    private MasterHorseContainer MasterHorseContainer => masterHorseContainer ??= container.Inject<MasterHorseContainer>();
    private IQuickRaceDomainService quickRaceDomainService;
    private IQuickRaceDomainService QuickRaceDomainService => quickRaceDomainService ??= container.Inject<IQuickRaceDomainService>();
    public HorseSumaryListEntityFactory(IDIContainer container)
    {
        this.container = container;
    }

    public async UniTask<UIComponentTraningHorseSelectSumaryList.Entity> InstantiateHorseSelectSumaryListEntity(CancellationToken token = default)
    {
        var current = UserReponsitory.Current.CurrentHorseNftId;
        List<UIComponentTraningHorseSelectSumary.Entity> ss = new List<UIComponentTraningHorseSelectSumary.Entity>();
        foreach(var x in HorseRepository.Models)
        {
            var sp = await HorseSpriteAssetLoader.InstantiateHorseAvatar(HorseDataModelHelper.GetSpritePath(x.Value.HorseShortId), token);
            var item = new UIComponentTraningHorseSelectSumary.Entity()
            {
                horseNFTId = x.Value.HorseNtfId,
                horseName = x.Value.Name,
                horseRace = new UIComponentHorseRace.Entity() { type = (int)x.Value.HorseType },
                horseRank = new UIComponentHorseRace.Entity() { type = (int)x.Value.Rarity },
                horseRankBorder = new UIComponentHorseRankBorder.Entity() { Rank = ((int)x.Value.Rarity - 1) },
                selectBtn = new ButtonSelectedComponent.Entity(() => OnSelectHorse(x.Key).Forget(), x.Value.HorseNtfId == current),
                horseAvatar = new UIImage.Entity() { sprite = sp },
            };
            ss.Add(item);
        }
        return new UIComponentTraningHorseSelectSumaryList.Entity()
        {
            entities = ss.ToArray()
        };
    }

    private async UniTaskVoid OnSelectHorse(long horseNtfId)
    {
        await QuickRaceDomainService.ChangeHorse(horseNtfId);
    }
}
