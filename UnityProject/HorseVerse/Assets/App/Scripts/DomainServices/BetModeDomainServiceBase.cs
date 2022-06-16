﻿using Cysharp.Threading.Tasks;

public class BetModeDomainServiceBase
{
    protected IBetRateRepository betRateRepository;
    protected IDIContainer container = default;
    protected IBetRateRepository BetRateRepository => betRateRepository ??= container.Inject<IBetRateRepository>();

    public BetModeDomainServiceBase(IDIContainer container)
    {
        this.container = container;
    }

}