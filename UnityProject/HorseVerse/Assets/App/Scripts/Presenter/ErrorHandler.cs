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
                if (EditorUtility.DisplayDialog("Error", $"{condition}\n{stackTrace}", "OK", "Quit"))
                {
                    OnError.Invoke();
                }
                else
                {
                    UnityEditor.EditorApplication.isPlaying = false;
                };
#endif
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