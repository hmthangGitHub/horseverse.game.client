using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

public partial class MasterDataGenerator
{
    public const string masterEnumTemplateFile = "Assets/App/MasterData/Editor/Templates/MasterEnumTemplate.txt";
    public const string masterEnumValueTemplateFile = "Assets/App/MasterData/Editor/Templates/MasterEnumValueTemplate.txt";

    private void GenerateEnum(TextAsset enumMaster)
    {
        var lines = CSVReader.ReadLines(enumMaster);
        var enumTypeCol = FindIndexOfColumn(lines, "enum_type");
        var valueCol = FindIndexOfColumn(lines, "value");
        lines.Skip(3)
            .Select(x =>
            {
                var columns = x.Split(',');
                return (enumType: columns[enumTypeCol], value: columns[valueCol]);
            })
            .GroupBy(x => x.enumType)
            .ToList()
            .ForEach(GenerateMasterEnum);
    }
    
    private void GenerateMasterEnum(IGrouping<string, (string enumType, string value)> enumMaster)
    {
        var masterEnumValueTemplateText = AssetDatabase.LoadAssetAtPath<TextAsset>(MasterDataGenerator.masterEnumValueTemplateFile) as TextAsset;
        var masterDataContainerTemplateText = AssetDatabase.LoadAssetAtPath<TextAsset>(MasterDataGenerator.masterEnumTemplateFile) as TextAsset;
        var masterUpperCaseName = ToTitleCaseWith_(enumMaster.Key);
        var path = $"{GetAbsolutePathOfAsset(outPutSchemaFolder)}/{masterUpperCaseName}.cs";

        var values = enumMaster
            .Select(x => x.value)
            .ToArray();

        var valueSources = values.Aggregate(string.Empty, (current, value) => current + $"\t {value}, \n");

        var masterDataSource = string.Format(masterDataContainerTemplateText.text, masterUpperCaseName, valueSources);
        File.WriteAllText(path, masterDataSource);
        AssetDatabase.Refresh();
    }
}