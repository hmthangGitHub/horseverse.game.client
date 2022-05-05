using System;
using System.Collections.Generic;
using UnityEngine;


[Serializable]
public struct LanguageTextTuple
{
    public Language Language;
}

[CreateAssetMenu(
    fileName = "New Language Dependent Text",
    menuName = "Localization/Language Dependent Text",
    order = 1)]
public class LanguageDependentText : ScriptableObject
{
    public LanguageTextTuple[] texts;

    private Dictionary<string, Dictionary<string, string>> _dictionary;

    private Dictionary<string, Dictionary<string, string>> Dictionary
    {
        get
        {
            if (_dictionary == null)
            {
                Init();
            }
            return _dictionary;
        }
    }

    public void Init()
    {
        _dictionary = new Dictionary<string, Dictionary<string, string>>();
        foreach (var item in texts)
        {
            if (item.Language == null)
                Debug.LogError("Language empty for Language Dependent Text: " + this);
            else
            {
                _dictionary[item.Language.Name] = new Dictionary<string, string>();
                foreach (var couple in item.Language.Text)
                {
                    _dictionary[item.Language.Name][couple.Text] = couple.Localize;
                }
            }
        }
    }

    public string GetString(string language, string key, out bool result)
    {
        result = false;
        if (!Dictionary.ContainsKey(language) || !Dictionary[language].ContainsKey(key)) return "";
        result = true;
        return Dictionary[language][key];
    }
}
