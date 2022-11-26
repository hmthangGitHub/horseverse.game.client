#define DEVELOPMENT
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ErrorHandler : IDisposable
{
    public event Action OnError = ActionUtility.EmptyAction.Instance;
    private string error => errorList[currentErrorIndex].error;
    private string stackTrace => errorList[currentErrorIndex].stackTrace;

    private List<(string error, string stackTrace)> errorList = new List<(string error, string stackTrace)>();
    private int currentErrorIndex = 0;
    public ErrorHandler()
    {
        SubscribeEvents();
    }

    private void SubscribeEvents()
    {
        Application.logMessageReceived += ApplicationLogMessageReceived;
    }

#if UNITY_EDITOR || DEVELOPMENT
    private Vector2 scrollPos;
    GUIStyle gsAlterQuest;
    GUIStyle areaStyle => gsAlterQuest ??= AreaGUIStyleUtil.CreateGs(new Color(0,0,0, 0.8f));

    private void OnGUIFunction()
    {
        if (!string.IsNullOrEmpty(error))
        {
            GUI.skin.label.fontSize = 30;
            GUI.skin.button.fontSize = 30;
            GUI.backgroundColor = new Color(0, 0, 0, 1.0f);
            GUILayout.BeginArea(new Rect(20, 20, Screen.width - 40, Screen.height - 40), areaStyle);
            GUI.backgroundColor = Color.white;
            GUILayout.BeginVertical();
            scrollPos = GUILayout.BeginScrollView(scrollPos);
            GUI.backgroundColor = new Color(0, 0, 0, 1.0f);
            GUILayout.Label($"Error : {error}");
            GUILayout.Label(stackTrace);
            GUILayout.EndScrollView();
            GUILayout.BeginHorizontal();
            if (GUILayout.Button($"<{currentErrorIndex}"))
            {
                currentErrorIndex--;
                currentErrorIndex += errorList.Count;
                currentErrorIndex %= errorList.Count;
            }
            if (GUILayout.Button("OK"))
            {
                OnError.Invoke();
            }

            if (GUILayout.Button("Copy error"))
            {
                GUIUtility.systemCopyBuffer = stackTrace;
            }

            if (GUILayout.Button("Quit"))
            {
#if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
#else
                Application.Quit();
#endif
            }

            if (GUILayout.Button($"{errorList.Count - currentErrorIndex}>"))
            {
                currentErrorIndex++;
                currentErrorIndex %= errorList.Count;
            }

            GUILayout.EndHorizontal();
            GUILayout.EndVertical();
            GUILayout.EndArea();
        }
    }

    
#endif

    private void ApplicationLogMessageReceived(string condition, string stackTrace, LogType type)
    {
        if (type == LogType.Error || type == LogType.Exception)
        {
            if (condition.Contains("Exception"))
            {
                ShowError(condition, stackTrace);
            }
        }
    }

    private void ShowError(string condition, string stackTrace)
    {
        if(!errorList.Any())
        {
            UnityMessageForwarder.AddListener(UnityMessageForwarder.MessageType.OnGUI, OnGUIFunction);
        }
        if(errorList.Count < 999)
        {
            errorList.Add((condition, stackTrace));
        }
    }

    private void UnsubcribeEvents()
    {
        UnityMessageForwarder.RemoveListener(UnityMessageForwarder.MessageType.OnGUI, OnGUIFunction);
        Application.logMessageReceived -= ApplicationLogMessageReceived;
    }

    public void Dispose()
    {
        UnsubcribeEvents();
        OnError = ActionUtility.EmptyAction.Instance;
        errorList.Clear();
    }
}