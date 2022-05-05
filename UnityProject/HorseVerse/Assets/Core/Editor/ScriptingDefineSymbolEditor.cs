using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Linq;

public class ScriptingDefineSymbolEditor : EditorWindow
{
	private static Dictionary<string, string> options = new Dictionary<string, string>();
	private static List<string> currentDefines = new List<string>();
	private static BuildTargetGroup buildTargetGroup = BuildTargetGroup.Android;

	[MenuItem("Tools/Scripting Define Symbol")]
	static void Init()
	{
#if UNITY_STANDALONE
        buildTargetGroup = BuildTargetGroup.Standalone;
#elif UNITY_ANDROID
        buildTargetGroup = BuildTargetGroup.Android;
#elif UNITY_IOS
        buildTargetGroup = BuildTargetGroup.iOS;
#else
		buildTargetGroup = BuildTargetGroup.Unknown;
#endif
		var value = PlayerSettings.GetScriptingDefineSymbolsForGroup(buildTargetGroup);
		currentDefines = value.Split(';').ToList();
		EditorWindow window = GetWindow(typeof(ScriptingDefineSymbolEditor));
		window.Show();
		options = new Dictionary<string, string> {
			{ "ALWAYS_DOWNLOAD", ""},
			{ "USE_LOCALIZATION", ""},
			{ "ENABLE_LOG", "" },
            { "DEV", "" },
		 };
	}

	void OnGUI()
	{
		GUILayout.Space(20);
		foreach (var keypair in options)
		{
			var key = keypair.Key;
			var textDisplay = key + (string.IsNullOrEmpty(keypair.Value) ? "" : "(" + keypair.Value + ")");
			float originalValue = EditorGUIUtility.labelWidth;
			EditorGUIUtility.labelWidth = 300;
			var result = EditorGUILayout.Toggle(textDisplay, currentDefines.Any(x => x == key));
			EditorGUIUtility.labelWidth = originalValue;
			if (result)
			{
				if (!currentDefines.Any(x => x == key))
				{
					currentDefines.Add(key);
				}
			}
			else
			{
				if (currentDefines.Any(x => x == key))
				{
					currentDefines.Remove(key);
				}
			}
		}
		GUILayout.Space(20);
		if (GUILayout.Button("Save"))
		{
			var result = "";
			for (int i = 0; i < currentDefines.Count; i++)
			{
				var value = currentDefines[i];
				if (i > 0)
				{
					result += ";";
				}
				result += currentDefines[i];
			}
			PlayerSettings.SetScriptingDefineSymbolsForGroup(buildTargetGroup, result);
			Debug.Log(result);
		}
	}
}