
using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BetMatchRepository : Repository<long, BetMatchModel, BetMatchModel>, IReadOnlyBetMatchRepository, IBetMatchRepository
{
    private readonly IDIContainer container;

    public BetMatchRepository(IDIContainer container) : base(x => x.BetMatchId, x => x, () => UniTask.FromResult(Enumerable.Empty<BetMatchModel>()))
    {
        this.container = container;
    }

    public BetMatchModel Current => Models.Values.First();
}

public interface IReadOnlyBetMatchRepository : IReadOnlyRepository<long, BetMatchModel>
{
    BetMatchModel Current { get; }
}

public interface IBetMatchRepository : IRepository<long, BetMatchModel, BetMatchModel>
{
    BetMatchModel Current { get; }
}
