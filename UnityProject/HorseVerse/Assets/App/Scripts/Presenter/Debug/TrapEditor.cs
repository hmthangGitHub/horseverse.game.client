using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrapEditor : MonoBehaviour
{
    public enum TYPE
    {
        ROLLING_ROCK,
        WOODEN_PILE,
    }

    public string TrapID;
    public TYPE Type;    public string getExtraData()     {
        var comp = GetComponent<TrapEditorBase>();        if (comp != default) return comp.GetExtraData();        return "";    }}
