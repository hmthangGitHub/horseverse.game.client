using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Linq;

public class AssetBundleGenerateName
{ 
    public static void GenerateAssetBundlePath(string Collection, List<Object> assets, bool useFolderName)
    {
        foreach (var a in assets)
        {
            string assetPath = AssetDatabase.GetAssetPath(a);
            var path = useFolderName ? System.IO.Path.GetFileName(System.IO.Path.GetDirectoryName(assetPath)) : a.name;
            string assetName = $"{Collection}/{path}";
            Debug.Log("asset " + assetName);
            AssetImporter.GetAtPath(assetPath).SetAssetBundleNameAndVariant(assetName, "");
        }
    }
}


public class AssetBundleGenerateNameEditor : EditorWindow
{
    bool toggleFolder = false;
    int _selected = 0;
    string[] _options = {   "localization",
                            "data",
                        };

    [MenuItem("Tools/AssetBundle/Change Bundle Name")]
    static void Init()
    {
        EditorWindow window = GetWindow(typeof(AssetBundleGenerateNameEditor));
        window.Show();
    }


    void OnGUI()
	{
        EditorGUI.BeginChangeCheck();

        this._selected = EditorGUILayout.Popup("Resource Type", _selected, _options);

        toggleFolder = GUILayout.Toggle(toggleFolder, "Use Folder Name");

        if (EditorGUI.EndChangeCheck())
        {
            Debug.Log(_options[_selected]);
        }

        GUILayout.Space(20);
		if (GUILayout.Button("Generate"))
		{
            changeBundleName(_options[_selected]);
        }
	}

	void changeBundleName(string Collection)
    {
        var assets = Selection.objects.Where(o => !string.IsNullOrEmpty(AssetDatabase.GetAssetPath(o))).ToList();
        AssetBundleGenerateName.GenerateAssetBundlePath(Collection, assets, toggleFolder);
    }
}
