using System;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using io.hverse.game.protogen;
using UnityEngine;

internal class BreedingSlotPresenter : IDisposable
{
    private readonly IDIContainer container;
    private UIHorseStableBreedSlot uiHorseStableBreedSlot;
    private IReadOnlyBreedSlotRepository breedSlotRepository;
    private IReadOnlyBreedSlotRepository BreedSlotRepository => breedSlotRepository ??= container.Inject<IReadOnlyBreedSlotRepository>();
    private CancellationTokenSource cts;
    public event Action<int> FinishBreedingSlotEvent = ActionUtility.EmptyAction<int>.Instance;
    public event Action<int> OnEnterBreedingSlotEvent = ActionUtility.EmptyAction<int>.Instance;
    
    public BreedingSlotPresenter(IDIContainer container)
    {
        this.container = container;
        cts = new CancellationTokenSource();
    }
    
    public async UniTaskVoid ShowBreedingSlotAsync()
    {
        await BreedSlotRepository.LoadRepositoryIfNeedAsync();
        uiHorseStableBreedSlot ??= await UILoader.Instantiate<UIHorseStableBreedSlot>(token: cts.Token);
        uiHorseStableBreedSlot.SetEntity(new UIHorseStableBreedSlot.Entity()
        {
            breedSlotList = BreedSlotRepository.Models.Values.Select((x, i) =>
            {
                var currentSlotState = GetCurrentSlotState(x);
                return new UIBreedSlot.Entity()
                {
                    state = currentSlotState,
                    breedingTimeLeft = currentSlotState == UIBreedSlotState.State.Breeding ? new UIComponentCountDownTimer.Entity()
                    {
                        outDatedEvent = x.BreedingTime > DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() 
                            ? () => uiHorseStableBreedSlot.breedSlotList
                                                          .instanceList[i]
                                                          .state
                                                          .SetEntity(UIBreedSlotState.State.CheckingFoal) 
                            : ActionUtility.EmptyAction.Instance,
                        utcEndTimeStamp = (int)Math.Ceiling(x.BreedingTime / 1000.0 + 1.0)
                    } : default,
                    checkingFoalBtn = new ButtonComponent.Entity(() => { FinishBreedingSlotEvent.Invoke((int)x.Index); }),
                    emptySlotBtn = new ButtonComponent.Entity(() => { OnEnterBreedingSlotEvent.Invoke((int)x.Index); })
                };
            }).Concat(Enumerable.Range(0, 6)
                                .Select(x => new UIBreedSlot.Entity()
                                {
                                    state = UIBreedSlotState.State.Locked
                                })).ToArray()
        });
        
        uiHorseStableBreedSlot.In().Forget();
    }

    private static UIBreedSlotState.State GetCurrentSlotState(BreedSlotInfo x)
    {
        return x.Status switch
        {
            BreedSlotStatus.Available => UIBreedSlotState.State.Empty,
            BreedSlotStatus.Breeding when x.BreedingTime <= DateTimeOffset.Now.ToUnixTimeMilliseconds() => UIBreedSlotState.State.CheckingFoal,
            BreedSlotStatus.Breeding when x.BreedingTime > DateTimeOffset.Now.ToUnixTimeMilliseconds() => UIBreedSlotState.State.Breeding,
            BreedSlotStatus.Lock => UIBreedSlotState.State.Locked,
            _ => throw new ArgumentOutOfRangeException()
        };
    }

    public void Dispose()
    {
        UILoader.SafeRelease(ref uiHorseStableBreedSlot);
        DisposeUtility.SafeDispose(ref cts);
        breedSlotRepository = default;
    }
}