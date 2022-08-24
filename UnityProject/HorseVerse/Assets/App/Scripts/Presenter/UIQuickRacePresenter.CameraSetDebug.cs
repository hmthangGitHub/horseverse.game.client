#define DEVELOPMENT
using System;
using UnityEngine;

#if DEVELOPMENT
public class CameraInMapDebugPanel: IDisposable
{
    private int cameraNumber;
    private GUIStyle gsAlterQuest;
    private Rect screenRect;
    public static string MapNumberKey => $"{Application.productName}_MAP_NUMBER";
    private GUIStyle areaStyle => gsAlterQuest ??= AreaGUIStyleUtil.CreateGs(new Color(0,0,0,0.3f));

    private void OnGUI()
    {
        PreGUI();
        GUILayout.BeginArea(screenRect, areaStyle);
        GUILayout.BeginHorizontal();
        
        GUI.skin.label.alignment = TextAnchor.MiddleCenter;
        GUILayout.Label("Debug Panel");
        GUILayout.EndHorizontal();
        GUILayout.BeginHorizontal();
        GUILayout.Label("Camera Number :");
        var cameraMaxNumber = 4 + 1;
        if (GUILayout.Button("<"))
        {
            cameraNumber--;
            cameraNumber = (( cameraNumber + cameraMaxNumber) % cameraMaxNumber);
        }
        GUILayout.Label(cameraNumber.ToString());
        if (GUILayout.Button(">"))
        {
            cameraNumber++;
            cameraNumber = ((cameraNumber) % cameraMaxNumber);
        }
        GUILayout.EndHorizontal();
        GUILayout.EndArea();
    }

    private void PreGUI()
    {
        var referenceW = 800;
        var ratio = Screen.width / referenceW;
        var w = 250f * ratio;
        var h = (250f / 3) * ratio;
        screenRect = new Rect(Screen.width - w, Screen.height - h, w, h);
        var textFieldFontSize = ratio * 15;
        GUI.skin.label.fontSize = textFieldFontSize;
        GUI.skin.textField.fontSize = textFieldFontSize;
        GUI.backgroundColor = new Color(0, 0, 0, 0.3f);
        GUI.skin.button.fontSize = textFieldFontSize;
    }

    public CameraInMapDebugPanel()
    {
        UnityMessageForwarder.AddListener(UnityMessageForwarder.MessageType.OnGUI, OnGUI);
        cameraNumber = PlayerPrefs.GetInt(MapNumberKey, 0);
    }

    public void Dispose()
    {
        UnityMessageForwarder.RemoveListener(UnityMessageForwarder.MessageType.OnGUI, OnGUI);
        PlayerPrefs.SetInt(MapNumberKey, cameraNumber);
    }
}

public partial class UIQuickRacePresenter
{
    private void DebugSetUp()
    {
    }
    
    private void DebugCleanUp()
    {
    }
}
#endif