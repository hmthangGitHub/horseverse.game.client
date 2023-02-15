using Cysharp.Threading.Tasks;
using System;
using System.Threading;
using System.Linq;
using System.Threading.Tasks;

public class QuickRaceResultPresenter : IDisposable
{
    private IDIContainer Container { get; }
    private RacingModeSummaryResultPresenter racingSummaryResultPresenter;

    public QuickRaceResultPresenter(IDIContainer container)
    {
        Container = container;
        racingSummaryResultPresenter = new RacingModeSummaryResultPresenter(Container);
    }

    public async UniTask ShowResultAsync()
    {
        await racingSummaryResultPresenter.ShowSummaryResultAsync();
    }
    
    public void Dispose()
    {
        DisposeUtility.SafeDispose(ref racingSummaryResultPresenter);
    }
}