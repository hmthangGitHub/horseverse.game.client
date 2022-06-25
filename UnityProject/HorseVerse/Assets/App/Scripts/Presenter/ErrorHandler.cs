#define EDITOR_DIALOG
using System;
#if UNITY_EDITOR && EDITOR_DIALOG
using UnityEditor;
#endif
using UnityEngine;

public class ErrorHandler : IDisposable
{
    public event Action OnError = ActionUtility.EmptyAction.Instance;

    public ErrorHandler()
    {
        SubscribeEvents();
    }

    private void SubscribeEvents()
    {
        Application.logMessageReceived += ApplicationLogMessageReceived;
    }

    private void ApplicationLogMessageReceived(string condition, string stackTrace, LogType type)
    {
        if (type == LogType.Error || type == LogType.Exception)
        {

            if (condition.Contains("Exception"))
            {
#if UNITY_EDITOR && EDITOR_DIALOG
                EditorUtility.DisplayDialog("Error", $"{condition}\n{stackTrace}", "OK");
#endif
                OnError.Invoke();
            }
        }
    }

    private void UnsubcribeEvents()
    {
        Application.logMessageReceived -= ApplicationLogMessageReceived;
    }

    public void Dispose()
    {
        UnsubcribeEvents();
        OnError = ActionUtility.EmptyAction.Instance;
    }
}