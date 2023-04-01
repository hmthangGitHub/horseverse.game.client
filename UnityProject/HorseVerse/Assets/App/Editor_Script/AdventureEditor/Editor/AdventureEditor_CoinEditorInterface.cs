using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(AdventureEditor_CoinEditor))]
[CanEditMultipleObjects]
public class AdventureEditor_CoinEditorInterface : Editor
{
    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        DrawDefaultInspector();
        if(GUILayout.Button("Update Position"))
        {
            for (int i = 0; i < targets.Length; i++)
                ((AdventureEditor_CoinEditor)targets[i]).UpdatePosition();
        }
    }
}
