using UnityEngine;
using System.Collections;
using System;
using UnityEditor;
using System.Collections.Generic;
using System.Linq;

public class EnumFlagsAttribute : PropertyAttribute
{
    public EnumFlagsAttribute() { }
    public static List<T> GetSelectedIndexes<T>(T val) where T : System.Enum, IConvertible
    {
        List<T> selectedItem = new List<T>();
        var enumAsArray = EnumFlagsAttribute.GetEnumAsArray<T>();
        int length = enumAsArray.Length;
        for (int i = 0; i < length; i++)
        {
            int layer = 1 << i;
            if ((Convert.ToInt32(val) & layer) != 0)
            {
                selectedItem.Add(enumAsArray[i]);
            }
        }
        return selectedItem;
    }

    private static T[] GetEnumAsArray<T>() where T : System.Enum
    {
        return Enum.GetValues(typeof(T))
                              .Cast<T>()
                              .ToArray();
    }

}

[CustomPropertyDrawer(typeof(EnumFlagsAttribute))]
public class EnumFlagsAttributeDrawer : PropertyDrawer
{
    public override void OnGUI(Rect _position, SerializedProperty _property, GUIContent _label)
    {
        _property.intValue = EditorGUI.MaskField(_position, _label, _property.intValue, _property.enumNames);
    }
}
