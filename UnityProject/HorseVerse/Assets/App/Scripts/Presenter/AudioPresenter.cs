using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class AudioPresenter : IDisposable
{
    private IDIContainer container = default;

    private ObjectAudio objAudio = default;
    private CancellationTokenSource cts = default;


    public AudioPresenter(IDIContainer container)
    {
        this.container = container;
    }

    public void Dispose()
    {
        ObjectLoader.SafeRelease("Object", ref objAudio);
    }

    public async UniTaskVoid ShowAudioAsync()
    {
        cts.SafeCancelAndDispose();
        cts = new CancellationTokenSource();

        objAudio ??= await ObjectLoader.Instantiate<ObjectAudio>("Object", ObjectHolder.Holder, token: cts.Token);
    }
}
