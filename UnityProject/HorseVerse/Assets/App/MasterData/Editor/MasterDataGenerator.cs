using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using GoogleSheetsToUnity;
using Newtonsoft.Json;
using UnityEditor;
using UnityEngine;
using UnityEngine.Networking;
using Object = UnityEngine.Object;

public partial class MasterDataGenerator : EditorWindow
{
    [MenuItem("Window/MasterData/Generator")]
    public static void ShowWindows()
    {
        EditorWindow.GetWindow(typeof(MasterDataGenerator));
    }

    public Object outputDataFolder;
    public Object outPutSchemaFolder;
    public string encryptKey = "DefaultKey";

    private const string CSV_FILES_PROPERTY = "csvFiles";
    public TextAsset[] csvFiles;

    public const string MasterDataTemplateFile = "Assets/App/MasterData/Editor/Templates/MasterDataTemplate.txt";
    public const string masterDataContainerTemplateFile = "Assets/App/MasterData/Editor/Templates/MasterDataContainerTemplate.txt";
    public const string masterDataFieldTemplate = "Assets/App/MasterData/Editor/Templates/MasterDataFieldTemplate.txt";

    public enum Prefix
    {
        type_name,
        type,
        out_put_client,
        out_put_server,
        id
    }

    void OnGUI()
    {
        ChooseMasterData();

        outPutSchemaFolder = EditorGUILayout.ObjectField("Output Schema Folder", outPutSchemaFolder, typeof(Object), allowSceneObjects: true);
        outputDataFolder = EditorGUILayout.ObjectField("Output Data Folder", outputDataFolder, typeof(Object), allowSceneObjects: true);
        encryptKey = EditorGUILayout.TextField("EncryptKey", encryptKey);

        if (GUILayout.Button("Generate"))
        {
            csvFiles.Where(x => x != null && x.name != "enum")
                    .ToList()
                    .ForEach(GenerateMaster);

            var enumMaster = csvFiles.FirstOrDefault(x => x != null && x.name == "enum");
            if (enumMaster != default)
            {
                GenerateEnum(enumMaster);
            }
        }

        if (GUILayout.Button("Fetch new master"))
        {
            BatchReadRawAsync("1_tPCfwDF2iiWversmLs8kbPrHGWjqg1bJfG4qgend_I", csvFiles.Select(x => (name : x.name, path : AssetDatabase.GetAssetPath(x)))
                .ToArray()
                    , false)
                .Forget();
        }
    }

    private static int FindIndexOfColumn(string[] lines, string columnName)
    {
        return lines.First()
            .Split(',')
            .ToList()
            .FindIndex(x => x == columnName);
    }

    private void ChooseMasterData()
    {
        ScriptableObject scriptableObj = this;
        SerializedObject serialObj = new SerializedObject(scriptableObj);
        SerializedProperty serialProp = serialObj.FindProperty(CSV_FILES_PROPERTY);

        EditorGUILayout.PropertyField(serialProp, true);
        serialObj.ApplyModifiedProperties();
    }

    private void GenerateMaster(TextAsset master)
    {
        if (ValidateMaster(master))
        {
            var executedCSVData = PreExecuteCSVData(master);
            GenerateSchema(executedCSVData.outputFieldNames, executedCSVData.outPutTypeNames, executedCSVData.idColumn, master.name);
            GenerateData(executedCSVData.preExecuteCSVData, master.name);
        }
    }

    private void GenerateSchema(string[] outputFieldNames, string[] outPutTypeNames, string idColumn, string masterName)
    {
        var masterDataContainerTemplateFile = AssetDatabase.LoadAssetAtPath<TextAsset>(MasterDataGenerator.masterDataContainerTemplateFile) as TextAsset;
        var masterUpperCaseName = ToTitleCaseWith_(masterName);
        GenerateMasterDataSource(outputFieldNames, outPutTypeNames, masterUpperCaseName);
        GenerateMasterDataContainerSource(outputFieldNames, outPutTypeNames, idColumn, masterDataContainerTemplateFile, masterUpperCaseName);
        AssetDatabase.Refresh();
    }

    private void GenerateMasterDataContainerSource(string[] outputFieldNames, string[] outPutTypeNames, string idColumn, TextAsset masterDataContaienrTemplateFile, string masterUpperCaseName)
    {
        var idColumnIndex = outputFieldNames.ToList().FindIndex(x => x == idColumn);
        var masterDataContainerSource = string.Format(masterDataContaienrTemplateFile.text,
                                                      masterUpperCaseName,
                                                      outPutTypeNames[idColumnIndex],
                                                      ToTitleCaseWith_(idColumn));
        var containerPath = $"{GetAbsolutePathOfAsset(outPutSchemaFolder)}/{masterUpperCaseName}Container.cs";
        File.WriteAllText(containerPath, masterDataContainerSource);
    }

    private void GenerateMasterDataSource(string[] outputFieldNames, string[] outPutTypeNames, string masterUpperCaseName)
    {
        var masterFieldTemplateFile = AssetDatabase.LoadAssetAtPath<TextAsset>(MasterDataGenerator.masterDataFieldTemplate) as TextAsset;
        var path = $"{GetAbsolutePathOfAsset(outPutSchemaFolder)}/{masterUpperCaseName}.cs";
        var masterDataTemplateFile = AssetDatabase.LoadAssetAtPath<TextAsset>(MasterDataGenerator.MasterDataTemplateFile) as TextAsset;
        var fieldSources = string.Empty;
        for (int i = 0; i < outputFieldNames.Length; i++)
        {
            var typeName = outPutTypeNames[i];
            var fieldName = outputFieldNames[i];
            var propertyName = ToTitleCaseWith_(fieldName);
            fieldSources += string.Format(masterFieldTemplateFile.text, typeName, fieldName, propertyName);
            fieldSources += i < outputFieldNames.Length - 1 ? "\n" : string.Empty;
        }

        var masterDataSource = string.Format(masterDataTemplateFile.text, masterUpperCaseName, fieldSources);
        File.WriteAllText(path, masterDataSource);
    }

    private string GetAbsolutePathOfAsset(Object asset)
    {
        return $"{Application.dataPath}/{AssetDatabase.GetAssetPath(asset).Replace("Assets", "")}";
    }

    private (string[] preExecuteCSVData, string[] outputFieldNames, string[] outPutTypeNames, string idColumn)  PreExecuteCSVData(TextAsset master)
    {
        string[] lines = CSVReader.ReadLines(master)
                                  .Select(x => x.Replace("\"\"" ,"\""))
                                  .ToArray();
        var linesAsColumn = lines.Select(x => x.Split(','));
        
        var outputClientColumns = linesAsColumn.FirstOrDefault(line => line[0] == Prefix.out_put_client.ToString())
                                               .Select((x, i) => (x, i))
                                               .Where(x => x.x.ToLower() == "true")
                                               .Select(x => x.i)
                                               .ToArray();
        var outputFieldNames = linesAsColumn.FirstOrDefault(line => line[0] == Prefix.type_name.ToString())
                                            .Where((x, i) => outputClientColumns.Contains(i) && !string.IsNullOrEmpty(x))
                                            .ToArray();
        var outPutTypes = linesAsColumn.FirstOrDefault(line => line[0] == Prefix.type.ToString())
                                       .Where((x, i) => outputClientColumns.Contains(i))
                                       .ToArray();

        var idColumn = linesAsColumn.FirstOrDefault(line => line[0] == Prefix.id.ToString())
                                    .Select((x, i) => (x, i))
                                    .Where(x => outputClientColumns.Contains(x.i) && x.x == Prefix.id.ToString())
                                    .Select(x => linesAsColumn.FirstOrDefault(line => line[0] == Prefix.type_name.ToString())[x.i])
                                    .FirstOrDefault();

        var preExecuteCSVData = new List<string>();

        var removedPrefixLines = linesAsColumn.Where(line => line[0] == Prefix.type_name.ToString())
                                              .ToList();
        removedPrefixLines.AddRange(linesAsColumn.Where(x => string.IsNullOrEmpty(x[0])));

        foreach (var line in removedPrefixLines)
        {
            var cols = line.Where((x, i) => outputClientColumns.Contains(i))
                           .ToArray();
            var newLine = string.Join(",", cols);
            preExecuteCSVData.Add(newLine);
        }

        outPutTypes = outPutTypes.Select(x => x.Contains("_") ? ToTitleCaseWith_(x) : x).ToArray();
            
        return (preExecuteCSVData.ToArray(), outputFieldNames, outPutTypes, idColumn);
    }

    private void GenerateData(string[] preExecuteCSVData, string masterName)
    {
        var filePath = $"{Application.dataPath}/{AssetDatabase.GetAssetPath(outputDataFolder).Replace("Assets", "")}/{ToTitleCaseWith_(masterName)}.json";
        var jsonString = CSVFileToJson.ConvertCsvFileToJsonObject(preExecuteCSVData);
        File.WriteAllText(filePath, jsonString);
        AssetDatabase.Refresh();
    }

    private string ToTitleCaseWith_(string masterName)
    {
        return ToTitleCase(masterName.Replace("_", " ")).Replace(" ","");
    }

    public string ToTitleCase(string str)
    {
        return CultureInfo.CurrentCulture.TextInfo.ToTitleCase(str.ToLower());
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
