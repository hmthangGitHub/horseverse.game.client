using Cysharp.Threading.Tasks;
using RobustFSM.Base;
using UnityEngine;

public class LoadingState : BState
{
    public UILoading uiLoading;

    public async void ShowLoading()
    {
        uiLoading ??= await UILoader.Load<UILoading>();
        uiLoading.In().Forget();
    }

    public void HideLoading()
    {
        uiLoading?.Out().Forget();
    }
}