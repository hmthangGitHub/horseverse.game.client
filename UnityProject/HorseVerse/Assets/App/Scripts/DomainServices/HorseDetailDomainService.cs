using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IHorseDetailDomainService
{
    UniTask LevelUp(long nftId);
}

public class HorseDetailDomainServiceBase
{
    protected IHorseRepository horseRepository;
    protected IDIContainer container;
    protected IHorseRepository HorseRepository => horseRepository ??= container.Inject<IHorseRepository>();
    public HorseDetailDomainServiceBase(IDIContainer container)
    {
        this.container = container;
    }
}

public class HorseDetailDomainService : HorseDetailDomainServiceBase, IHorseDetailDomainService
{
    public HorseDetailDomainService(IDIContainer container) : base(container) { }
    public UniTask LevelUp(long nftId)
    {
        throw new System.NotImplementedException();
    }
}

public class LocalHorseDetailDomainService : HorseDetailDomainServiceBase, IHorseDetailDomainService
{
    public LocalHorseDetailDomainService(IDIContainer container) : base(container) { }
    public async UniTask LevelUp(long nftId)
    {
        await UniTask.Delay(500);
        var oldModel = HorseRepository.Models[nftId];
        await HorseRepository.UpdateDataAsync(new HorseDataModel[]
        {
            new HorseDataModel()
            {
                Earning = oldModel.Earning,
                // HorseNtfId = nftId,
                PowerBonus = oldModel.PowerBonus + 0.015f,
                PowerRatio = oldModel.PowerRatio + 0.015f,
                SpeedBonus = oldModel.SpeedBonus + 0.015f,
                SpeedRatio = oldModel.SpeedRatio + 0.015f,
                TechnicallyBonus = oldModel.TechnicallyBonus + 0.015f,
                TechnicallyRatio = oldModel.TechnicallyRatio + 0.015f,
            }
        });
    }
}
