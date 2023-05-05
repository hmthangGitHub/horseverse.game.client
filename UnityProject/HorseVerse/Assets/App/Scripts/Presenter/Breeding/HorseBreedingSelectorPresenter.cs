using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using io.hverse.game.protogen;
using UnityEngine;

public class HorseBreedingSelectorPresenter : IDisposable
{
    private const int AgeToStartBreeding = 4;
    private readonly IDIContainer container;
    private UIHorseCardSelectToBreed uiHorseCardSelectToBreed;
    private HorseRepository horseRepository;
    private HorseRepository HorseRepository => horseRepository ??= container.Inject<HorseRepository>();

    public HorseBreedingSelectorPresenter(IDIContainer container)
    {
        this.container = container;
    }

    public async UniTask<long> SelectHorseAsync(HorseSex horseSex, CancellationToken token)
    {
        var ucs = new UniTaskCompletionSource<long>();
        uiHorseCardSelectToBreed ??= await UILoader.Instantiate<UIHorseCardSelectToBreed>(token: token);
        uiHorseCardSelectToBreed.SetEntity(new UIHorseCardSelectToBreed.Entity()
        {
            horseBreedingList = HorseRepository.Models.Values
                                               .Where(x => x.HorseBasic.Age == AgeToStartBreeding && x.HorseBasic.Sex == horseSex)
                                               .Select((x, i) => new UIComponentHorseBreedingCard.Entity()
                                               {
                                                   element = (UIComponentHorseElement.Element)x.HorseBasic.HorseType,
                                                   breedCount = x.HorseRising.BreedingCount,
                                                   maxBreedCount = UserSettingLocalRepository.MasterDataModel.MaxBreedingNumber,
                                                   horseName = x.HorseBasic.Name,
                                                   isLock = IsHorseLock(x),
                                                   isCountDown = x.HorseRising.BreedingCoolDown > DateTimeOffset.UtcNow.ToUnixTimeMilliseconds(),
                                                   countDown = new UIComponentCountDownTimer.Entity()
                                                   {
                                                       utcEndTimeStamp = (int)Math.Ceiling(x.HorseRising.BreedingCoolDown / 1000.0 + 1.0),
                                                       outDatedEvent = x.HorseRising.BreedingCoolDown > DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()
                                                           ? () =>
                                                           {
                                                               var uiComponentHorseBreedingCard = uiHorseCardSelectToBreed.horseBreedingList.instanceList[i];
                                                               uiComponentHorseBreedingCard.isLock.SetEntity(IsHorseLock(x));
                                                               uiComponentHorseBreedingCard.isCountDown.SetEntity(false);
                                                           }
                                                           : ActionUtility.EmptyAction.Instance
                                                   },
                                                   selectBtn = new ButtonComponent.Entity(() =>
                                                   {
                                                       ucs.TrySetResult(x.HorseBasic.Id);
                                                   }),
                                               })
                                               .ToArray()
                                               
                                               
        });
        await uiHorseCardSelectToBreed.In();
        return await ucs.Task.AttachExternalCancellation(cancellationToken: token);
    }

    private static bool IsHorseLock(HorseDataModel x)
    {
        return x.HorseRising.BreedingCount == 0 || x.HorseRising.BreedingCoolDown > DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
    }

    public void Dispose()
    {
        UILoader.SafeRelease(ref uiHorseCardSelectToBreed);
    }
}
