using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using io.hverse.game.protogen;
using UnityEngine;

public class BreedingBreedingDomainService : IBreedingDomainService
{
    private readonly IDIContainer container;
    private ISocketClient socketClient;
    private IUserDataRepository userDataRepository;
    private IBreedSlotRepository breedSlotRepository;
    private IHorseRepository horseRepository;
    private ISocketClient SocketClient => socketClient ??= container.Inject<ISocketClient>();
    private IUserDataRepository UserDataRepository => userDataRepository ??= container.Inject<IUserDataRepository>();
    private IBreedSlotRepository BreedSlotRepository => breedSlotRepository ??= container.Inject<IBreedSlotRepository>();
    private IHorseRepository HorseRepository => horseRepository ??= container.Inject<IHorseRepository>();

    public BreedingBreedingDomainService(IDIContainer container)
    {
        this.container = container;
    }

    public async UniTask Breed(int slotIndex,
                               long maleHorseId,
                               long femaleHorseId)
    {
        var response = await SocketClient.Send<BreedingRequest, BreedingResponse>(new BreedingRequest()
        {
            SlotIndex = slotIndex,
            MaleHorseId = maleHorseId,
            FemaleHorseId = femaleHorseId
        });

        await UserDataRepository.UpdateLightPlayerInfoAsync(response.PlayerInfo);
        await BreedSlotRepository.UpdateDataAsync(new[] { response.SlotInfo });
        await HorseRepository.UpdateModelAsync(response.FemaleHorseRising);
        await HorseRepository.UpdateModelAsync(response.MaleHorseRising);
    }

    public async UniTask FinishBreeding(int slotIndex)

    {
        var childHorse = BreedSlotRepository.Models[slotIndex].ChildHorse;
        var response = await SocketClient.Send<FinishBreedingRequest, FinishBreedingResponse>(
            new FinishBreedingRequest()
            {
                SlotIndex = slotIndex,
            });

        await BreedSlotRepository.UpdateDataAsync(new[] { response.SlotInfo });
        await HorseRepository.AddHorseModelAsync(childHorse);
    }
}

public interface IBreedingDomainService
{
    UniTask Breed(int slotIndex,
                  long maleHorseId,
                  long femaleHorseId);
    
    UniTask FinishBreeding(int slotIndex);
}