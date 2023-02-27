using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class MasterLocalizeContainer
{
    public enum Language
    {
        EN,
        VI
    }
    
    public Language CurrentLanguage { get; set; }

    public string GetString(string key)
    {
        var masterLocalize = MasterLocalizeIndexer[key];
        return CurrentLanguage switch
        {
            Language.EN => masterLocalize.En,
            Language.VI => masterLocalize.Vi,
            _ => throw new ArgumentOutOfRangeException()
        };
    }
}
