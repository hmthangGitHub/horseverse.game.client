using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using io.hverse.game.protogen;

public class BetRateRepository : Repository<(int first, int second), WinRate, BetRateModel>, IBetRateRepository
{

    public BetRateRepository() : base(x => (x.First, x.Second), x =>
    {
        var horseIndices = x.IndexOfHorse.ToKey();
        return new BetRateModel()
        {
            First = horseIndices.first,
            Second = horseIndices.second,
            TotalBet = x.TotalAmountOfBet,
            Rate = x.Rate,
        };
    }, GetBetRateData)
    {
    }

    public float TotalBetAmouth => Models.Sum(x => x.Value.TotalBet);

    private static UniTask<IEnumerable<WinRate>> GetBetRateData()
    {
        var horseNumber = 8;
        var betRateModels = new List<WinRate>();
        
        for (int i = 0; i < horseNumber; i++)
        {
            for (int j = 0; j < horseNumber; j++)
            {
                betRateModels.Add(new WinRate()
                {
                    IndexOfHorse = (i + 1, j + 1).ToIndexOfHorse(),
                });
            }
        }
        return UniTask.FromResult(Enumerable.Empty<WinRate>()
                                .Concat(betRateModels));
    }
}

public interface IReadOnlyBetRateRepository : IReadOnlyRepository<(int first, int second), BetRateModel>
{
    float TotalBetAmouth { get; }
}
public interface IBetRateRepository : IRepository<(int first, int second), WinRate, BetRateModel>, IReadOnlyBetRateRepository { }

public static class BetRateRepositoryUtility
{
    private static char HorseBetIndexSeparatorCharacter = '-';

    public static (int first, int second) ToKey(this string indexOfHorse)
    {
        var horseIndices = indexOfHorse.Split(HorseBetIndexSeparatorCharacter);
        return (int.Parse(horseIndices[0]), horseIndices.Length > 1 ? int.Parse(horseIndices[1]) : 0);
    }
    
    public static string ToIndexOfHorse(this (int first, int second) key)
    {
        return key.second == default
            ? key.first.ToString()
            : $"{key.first}{HorseBetIndexSeparatorCharacter}{key.second}"; 
    }
}