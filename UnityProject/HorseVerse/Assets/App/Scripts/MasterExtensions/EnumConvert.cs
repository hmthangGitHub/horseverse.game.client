using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class EnumConvert
{
    public static TTo ConvertTo<TFrom, TTo>(this TFrom from) where TFrom : Enum 
        where TTo : Enum

    {
        var fromEnums = Enum.GetValues(typeof(TFrom))
            .Cast<TFrom>()
            .ToList();
        
        var toEnums = Enum.GetValues(typeof(TTo))
                            .Cast<TTo>()
                            .ToArray();
        if (fromEnums.Count != toEnums.Length)
        {
            throw new ArgumentException($"{typeof(TFrom)} and {typeof(TTo)} Not match length to remap");
        }

        var index = fromEnums.FindIndex(x => x.Equals(from));
        return toEnums[index];
    }
}
