using Cysharp.Threading.Tasks;
using System.Linq;
using System.Threading.Tasks;

public interface IBetModeDomainService
{
    UniTask CancelBetAsync();
    UniTask BetAsync((int first, int second)[] keys, int amouth);
    UniTask<RaceMatchData> GetCurrentBetModeRaceMatchData();
}

public class BetModeDomainService : BetModeDomainServiceBase, IBetModeDomainService
{
    public BetModeDomainService(IDIContainer container) : base(container)
    {
    }

    public UniTask BetAsync((int first, int second)[] keys, int amouth)
    {
        throw new System.NotImplementedException();
    }

    public UniTask CancelBetAsync()
    {
        throw new System.NotImplementedException();
    }
}

public class LocalBetModeDomainService : BetModeDomainServiceBase, IBetModeDomainService
{
    public LocalBetModeDomainService(IDIContainer container) : base(container)
    {
    }

    public async UniTask BetAsync((int first, int second)[] keys, int amouth)
    {
        await UniTask.Delay(500);
        var betRates = keys.Select(key => BetRateRepository.Models[key])
            .ToList();
        betRates.ForEach(betRate => betRate.TotalBet += amouth);
        await BetRateRepository.UpdateDataAsync(betRates);
    }

    public async UniTask CancelBetAsync()
    {
        await UniTask.Delay(500);
        var models = BetRateRepository.Models.Values.Select(x => new BetRateModel()
        {
            First = x.First,
            Second = x.Second,
            Rate = x.Rate,
            TotalBet = 0
        });
        await BetRateRepository.UpdateDataAsync(models);
    }
}
