using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;


public class FeaturePresenter : IDisposable
{
    private IDIContainer container = default;
    private List<FEATURE_TYPE> _featureList = new List<FEATURE_TYPE>();
    public FeaturePresenter(IDIContainer container)
    {
        this.container = container;
    }

    public void Dispose()
    {
        _featureList.Clear();
    }

    public void SetFeatureList(FEATURE_TYPE[] li)
    {
        _featureList.Clear();
        _featureList.AddRange(li);
    }

    public bool CheckFeature(FEATURE_TYPE type)
    {
        return _featureList.Contains(type);
    }
}
