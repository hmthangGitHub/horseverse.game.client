using Cysharp.Threading.Tasks;

public class BetModeDomainServiceBase
{
    private IBetRateRepository betRateRepository;
    protected readonly IDIContainer container = default;
    protected IBetRateRepository BetRateRepository => betRateRepository ??= container.Inject<IBetRateRepository>();

    public BetModeDomainServiceBase(IDIContainer container)
    {
        this.container = container;
    }
}