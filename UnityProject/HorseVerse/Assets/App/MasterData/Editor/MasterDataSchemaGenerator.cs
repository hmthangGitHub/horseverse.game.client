using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class MasterDataSchemaGenerator : EditorWindow
{
    [MenuItem("Window/MasterData/Generator")]
    public static void ShowWindows()
    {
        EditorWindow.GetWindow(typeof(MasterDataSchemaGenerator));
    }

    public string csvFolderPath = "App/MasterData/Editor/MasterDataCSV";
    public Object csvJsonFolder;
    private const string CSV_FILES_PROPERTY = "csvFiles";
    public TextAsset[] csvFiles;

    void OnGUI()
    {
        ScriptableObject scriptableObj = this;
        SerializedObject serialObj = new SerializedObject(scriptableObj);
        SerializedProperty serialProp = serialObj.FindProperty(CSV_FILES_PROPERTY);

        EditorGUILayout.PropertyField(serialProp, true);
        serialObj.ApplyModifiedProperties();

        csvJsonFolder = EditorGUILayout.ObjectField("Output Folder", csvJsonFolder, typeof(Object), allowSceneObjects: true);

        if (GUILayout.Button("Generate"))
        {
            csvFiles.Where(x => x != null)
                    .ToList()
                    .ForEach(x => GenerateMaster(x));
        }
    }

    private void GenerateMaster(TextAsset master)
    {
        if (ValidateMaster(master))
        {
            GenerateSchema(master);
            GenerateData(master);
        }
    }

    private void GenerateData(TextAsset master)
    {
        var filePath = $"{Application.dataPath}/{AssetDatabase.GetAssetPath(csvJsonFolder).Replace("Assets", "")}/{ToTitleCaseFromMasterName(master.name)}.json";
        var jsonString = CSVFileToJson.ConvertCsvFileToJsonObject("");
        File.WriteAllText(filePath, jsonString);
        AssetDatabase.Refresh();
    }

    private string ToTitleCaseFromMasterName(string masterName)
    {
        return ToTitleCase(masterName.Replace("_", " ")).Replace(" ","");
    }

    public string ToTitleCase(string str)
    {
        return CultureInfo.CurrentCulture.TextInfo.ToTitleCase(str.ToLower());
    }


    private void GenerateSchema(TextAsset master)
    {
        var csvDatas = CSVReader.Read(master);
    }

    private bool ValidateMaster(TextAsset master)
    {
        string masterPath = AssetDatabase.GetAssetPath(master);
        if (!masterPath.Contains(".csv"))
        {
            EditorUtility.DisplayDialog("Error", $"Not csv file Format {masterPath}", "OK");
            return false;
        }
        return true;
    }
}
