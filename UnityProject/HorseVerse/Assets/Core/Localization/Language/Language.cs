using UnityEngine;
using System;
using System.Collections.Generic;

[System.Serializable]
public struct LanguageText
{
    [TextArea]
    public string Text;
    [TextArea]
    public string Localize;
}

[CreateAssetMenu(
    fileName = "New Language",
    menuName = "Localization/Language",
    order = 2)]
public class Language : ScriptableObject
{
    public string Name;

    public Sprite Icon;

    public SystemLanguage systemLanguageType;

    public List<LanguageText> Text;
}


