#define DEVELOPMENT
using System;
using UnityEngine;

public class ErrorHandler : IDisposable
{
    public event Action OnError = ActionUtility.EmptyAction.Instance;
    private string error = string.Empty;
    private string stackTrace = string.Empty;

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
    GUIStyle areStyle => gsAlterQuest ??= CreateGs();
    private GUIStyle CreateGs()
    {
        return new GUIStyle()
        {
            normal = new GUIStyleState()
            {
                background = MakeTex(1, 1, new Color(0, 0, 0, 0.8f))
            }
        };
    }

    private void OnGUIFunction()
    {
        if (!string.IsNullOrEmpty(error))
        {
            GUI.skin.label.fontSize = 30;
            GUI.skin.button.fontSize = 30;
            GUI.backgroundColor = new Color(0, 0, 0, 1.0f);
            GUILayout.BeginArea(new Rect(20, 20, Screen.width - 40, Screen.height - 40), areStyle);
            GUI.backgroundColor = Color.white;
            GUILayout.BeginVertical();
            scrollPos = GUILayout.BeginScrollView(scrollPos);
            GUI.backgroundColor = new Color(0, 0, 0, 1.0f);
            GUILayout.Label($"Error : {error}");
            GUILayout.Label(stackTrace);
            GUILayout.EndScrollView();
            GUILayout.BeginHorizontal();
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

            GUILayout.EndHorizontal();
            GUILayout.EndVertical();
            GUILayout.EndArea();
        }
    }

    private Texture2D MakeTex(int width, int height, Color col)
    {
        Color[] pix = new Color[width * height];

        for (int i = 0; i < pix.Length; i++)
            pix[i] = col;

        Texture2D result = new Texture2D(width, height);
        result.SetPixels(pix);
        result.Apply();

        return result;
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
        UnityMessageForwarder.RemoveListener(UnityMessageForwarder.MessageType.OnGUI, OnGUIFunction);
        UnityMessageForwarder.AddListener(UnityMessageForwarder.MessageType.OnGUI, OnGUIFunction);
        this.error = condition;
        this.stackTrace = stackTrace;
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
    }
}