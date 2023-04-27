using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using io.hverse.game.protogen;
using UnityEngine;

public class BreedSlotRepository : Repository<long, BreedSlotInfo, BreedSlotInfo>, IReadOnlyBreedSlotRepository, IBreedSlotRepository
{
    public BreedSlotRepository(IDIContainer container) : base(x => x.Index, x => x, () => GetBreedSlotInfo(container))
    {
    }

    private static async UniTask<IEnumerable<BreedSlotInfo>> GetBreedSlotInfo(IDIContainer container)
    {
        var socketClient = container.Inject<ISocketClient>();
        var response = await socketClient.Send<BreedingInfoRequest, BreedingInfoResponse>(new BreedingInfoRequest());
        return response.SlotInfos.ToArray();
    }
}

public interface IReadOnlyBreedSlotRepository : IReadOnlyRepository<long, BreedSlotInfo>
{
}

public interface IBreedSlotRepository : IRepository<long, BreedSlotInfo, BreedSlotInfo>
{
}



