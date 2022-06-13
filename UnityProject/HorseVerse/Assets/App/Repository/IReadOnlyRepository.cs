﻿using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;

public interface IReadOnlyRepository<TKey, TModel>
{
    IReadOnlyDictionary<TKey, TModel> Models { get; }
    UniTask LoadRepositoryIfNeedAsync();
    event Action<(TModel before, TModel after)> OnModelUpdate;
    event Action<(IReadOnlyDictionary<TKey, TModel> before, IReadOnlyDictionary<TKey, TModel> after)> OnModelsUpdate;
}

public interface IRepository<TKey, TData, TModel> : IReadOnlyRepository<TKey, TModel>
{
    UniTask UpdateDataAsync(TData[] data);
}