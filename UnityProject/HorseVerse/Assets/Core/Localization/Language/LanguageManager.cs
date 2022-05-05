using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class LanguageManager
{
    private static Dictionary<SystemLanguage, Language> _languageDictionary = new Dictionary<SystemLanguage, Language>();
    private static Dictionary<SystemLanguage, Language> _localLanguageDictionary = new Dictionary<SystemLanguage, Language>();

    public static Language SelectedLanguage { get; private set; }
    private static SystemLanguage SelectedSystemLanguage { get; set; }


    public static Language SelectedLocalLanguage { get; private set; }
    private static SystemLanguage SelectedLocalSystemLanguage { get; set; }

    public static event Action<Language> OnLanguageChange;

    private static Language _defaultLanguage;
    private static LanguageDependentText _textDict;

    private static Language _defaultLocalLanguage;
    private static LanguageDependentText _localTextDict;

    public static void Initialize(SystemLanguage language, List<Language> languages, LanguageDependentText textDict)
    {
        foreach (Language lang in languages)
        {
            if(!_languageDictionary.ContainsKey(lang.systemLanguageType))
                _languageDictionary.Add(lang.systemLanguageType, lang);
        }
        _textDict = textDict;
        SetDefaultLanguage(language);
    }

    public static void InitializeLocal(SystemLanguage language, LanguageDependentText textDict)
    {
        foreach (var lang in textDict.texts)
        {
            if (!_localLanguageDictionary.ContainsKey(lang.Language.systemLanguageType))
                _localLanguageDictionary.Add(lang.Language.systemLanguageType, lang.Language);
        }
        _localTextDict = textDict;
        SetDefaultLocalLanguage(language);
    }

    public static string GetText(string key)
    {
        bool kq = false;
        string text = "";
        if(_textDict != null)
            text = _textDict.GetString(SelectedLanguage.Name, key, out kq);
        if (!kq)
        {
            var sText = _localTextDict.GetString(SelectedLocalLanguage.Name, key, out kq);
            if (kq) text = sText; 
        }
        return text;
    }

    public static string GetText(string key, params object[] args)
    {
        bool kq = false;
        string text = "";
        if(_textDict != null)
            text = _textDict.GetString(SelectedLanguage.Name, key, out kq);
        if (!kq)
        {
            var sText = _localTextDict.GetString(SelectedLocalLanguage.Name, key, out kq);
            if (kq) text = sText;
        }
        return string.Format(text, args);
    }

    public static string GetText(LanguageDependentText text, string key)
    {
        bool kq = false;
        return text.GetString(SelectedLanguage.Name, key, out kq);
    }

    public static string GetText(LanguageDependentText text, string key, params object[] args)
    {
        bool kq = false;
        return string.Format(text.GetString(SelectedLanguage.Name, key, out kq), args);
    }

    private static void SetDefaultLanguage(SystemLanguage lang)
    {
        if (_languageDictionary.ContainsKey(lang))
        {
            SelectedLanguage = _languageDictionary[lang];
            _defaultLanguage = _languageDictionary[lang];
            SelectedSystemLanguage = lang;
        }
        else
        {
            SelectedLanguage = _languageDictionary[SystemLanguage.English];
            _defaultLanguage = _languageDictionary[SystemLanguage.English];
            SelectedSystemLanguage = SystemLanguage.English;

        }
    }

    private static void SetDefaultLocalLanguage(SystemLanguage lang)
    {
        if (_localLanguageDictionary.ContainsKey(lang))
        {
            SelectedLocalLanguage = _localLanguageDictionary[lang];
            _defaultLocalLanguage = _localLanguageDictionary[lang];
            SelectedLocalSystemLanguage = lang;
        }
        else
        {
            SelectedLocalLanguage = _localLanguageDictionary[SystemLanguage.English];
            _defaultLocalLanguage = _localLanguageDictionary[SystemLanguage.English];
            SelectedLocalSystemLanguage = SystemLanguage.English;

        }
    }

}
