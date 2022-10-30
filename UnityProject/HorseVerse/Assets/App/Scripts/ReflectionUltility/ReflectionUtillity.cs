using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public static class ReflectionUtillity
{
    public static T GetPropertyValue<T>(this object obj, string propertyName) where T : class
    {
        var _propertyNames = propertyName.Split('.');
        var returnObject = obj;
        for (var i = 0; i < _propertyNames.Length; i++)
        {
            if (returnObject != null)
            {
                var _propertyInfo = returnObject.GetType().GetProperty(_propertyNames[i],
                    BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
                if (_propertyInfo != null)
                    returnObject = _propertyInfo.GetValue(returnObject);
                else
                {
                    var fieldInfo = returnObject.GetType().GetField(_propertyNames[i],
                        BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
                    if (fieldInfo != null)
                    {
                        returnObject = fieldInfo.GetValue(returnObject);
                    }
                    else
                    {
                        returnObject = null;
                    }
                }
            }
        }

        return returnObject as T;
    }

    public static object GetPropertyValue(this object obj, string propertyName)
    {
        var _propertyNames = propertyName.Split('.');
        var returnObject = obj;
        for (var i = 0; i < _propertyNames.Length; i++)
        {
            if (returnObject != null)
            {
                var _propertyInfo = returnObject.GetType().GetProperty(_propertyNames[i],
                    BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
                if (_propertyInfo != null)
                    returnObject = _propertyInfo.GetValue(returnObject);
                else
                {
                    var fieldInfo = returnObject.GetType().GetField(_propertyNames[i],
                        BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
                    if (fieldInfo != null)
                    {
                        returnObject = fieldInfo.GetValue(returnObject);
                    }
                    else
                    {
                        returnObject = null;
                    }
                }
            }
        }

        return returnObject;
    }

    public static void SetPropertyValue(this object obj, string propertyName, object value)
    {
        var _propertyNames = propertyName.Split('.');
        PropertyInfo propertyInfo = default;
        FieldInfo fieldInfo = default;
        var returnObject = obj;
        for (var i = 0; i < _propertyNames.Length; i++)
        {
            if (returnObject != null)
            {
                propertyInfo = returnObject.GetType().GetProperty(_propertyNames[i],
                    BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
                if (propertyInfo != null)
                    returnObject = propertyInfo.GetValue(returnObject);
                else
                {
                    fieldInfo = returnObject.GetType().GetField(_propertyNames[i],
                        BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
                    if (fieldInfo != null)
                    {
                        returnObject = fieldInfo.GetValue(returnObject);
                    }
                    else
                    {
                        returnObject = null;
                    }
                }
            }
        }

        propertyInfo?.SetValue(returnObject, value);
        fieldInfo?.SetValue(obj, value);
    }
}