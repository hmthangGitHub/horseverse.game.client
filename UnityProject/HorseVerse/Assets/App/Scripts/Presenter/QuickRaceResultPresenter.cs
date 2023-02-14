using Cysharp.Threading.Tasks;
using System;
using System.Threading;
using System.Linq;
using System.Threading.Tasks;

public class QuickRaceResultPresenter : IDisposable
{
    private IDIContainer Container { get; }
    private RaceSummaryResultPresenter raceSummaryResultPresenter;

    public QuickRaceResultPresenter(IDIContainer container)
    {
        Container = container;
        raceSummaryResultPresenter = new RaceSummaryResultPresenter(Container);
    }

    public async UniTask ShowResultAsync()
    {
        await raceSummaryResultPresenter.ShowSummaryResultAsync();
    }
    
    public void Dispose()
    {
        DisposeUtility.SafeDispose(ref raceSummaryResultPresenter);
    }
}