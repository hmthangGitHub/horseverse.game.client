using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using System.Linq;

public class Repository<TKey, TData, TModel> : IRepository<TKey, TData, TModel>
{
    public IReadOnlyDictionary<TKey, TModel> Models => models;

    public event Action<(TModel before, TModel after)> OnModelUpdate = _ => { };
    public event Action<(IReadOnlyDictionary<TKey, TModel> before, IReadOnlyDictionary<TKey, TModel> after)> OnModelsUpdate = _ => { };
    private Dictionary<TKey, TModel> models = new Dictionary<TKey, TModel>();
    private Func<TModel, TKey> keyPredicator;
    private Func<TData, TModel> modelPredicator;
    private Func<UniTask<IEnumerable<TData>>> datasPredicator;
    private bool isInitilized = false;
    private CancellationTokenSource cts;

    public Repository(Func<TModel, TKey> keyPredicator,
                      Func<TData, TModel> modelPredicator,
                      Func<UniTask<IEnumerable<TData>>> datasPredicator)
    {
        this.keyPredicator = keyPredicator;
        this.modelPredicator = modelPredicator;
        this.datasPredicator = datasPredicator;
    }

    public void Reset()
    {
        isInitilized = false;
        models.Clear();
    }

    public async UniTask LoadRepositoryIfNeedAsync()
    {
        if (!isInitilized)
        {
            cts.SafeCancelAndDispose();
            await Load().AttachExternalCancellation(cts.Token);
            isInitilized = true;
        }
    }

    private async UniTask Load()
    {
        var datas = await datasPredicator();
        models = datas.Select(modelPredicator)
                      .ToDictionary(keyPredicator);
    }

    public async UniTask UpdateDataAsync(TData[] data)
    {
        var beforeModels = models.ToDictionary(x => x.Key, x => x.Value);
        data.Select(modelPredicator)
            .ToList()
            .ForEach(UpdateModel);
        OnModelsUpdate((beforeModels, models));
        await UniTask.CompletedTask;
    }

    private void UpdateModel(TModel model)
    {
        var key = keyPredicator(model);
        models.TryGetValue(key, out TModel oldModel);
        models[key] = model;
        OnModelUpdate((oldModel, model));
    }
}