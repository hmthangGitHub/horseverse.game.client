using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

namespace CoreData
{
    public static class TypeHelper
    {
        public static Type GetType(string nameSpace, string className)
        {
            string name = string.IsNullOrEmpty(nameSpace) ? className : nameSpace + "." + className;
            return Type.GetType(name);
        }

        public static object CreateInstance(string nameSpace, string className)
        {
            var type = GetType(nameSpace, className);
            var obj = TypeHelper.CreateInstance(type);
            return obj;
        }

        public static object CreateInstance(Type type)
        {
            return Activator.CreateInstance(type);
        }

        public static void SetProperty(object obj, string propertyName, object value)
        {
            obj.GetType().GetProperty(propertyName).SetValue(obj, value, null);
        }

        public static object GetProperty(object obj, string propertyName)
        {
            return obj.GetType().GetProperty(propertyName).GetValue(obj, null);
        }

        public static bool IsEnum(Type type)
        {
#if !NETFX_CORE
            return type.IsEnum;
#else
		return type.GetTypeInfo().IsEnum;
#endif
        }

        public static bool IsOriginalModel(Type type)
        {
            return type.GetInterface(typeof(IOriginalModel).Name) != null;
        }

        public static bool IsList<T>(Type type)
        {
            if (IsListGeneric(type))
            {
                if (IsGenericArgument<T>(type))
                {
                    return true;
                }
            }

            return false;
        }

        public static bool IsListEnum(Type type)
        {
            if (IsListGeneric(type))
            {
                var arguments = type.GetGenericArguments();
                if (arguments.Length == 1 && IsEnum(arguments[0]))
                {
                    return true;
                }
            }

            return false;
        }

        public static bool IsGenericArgument<T>(Type type)
        {
            var arguments = type.GetGenericArguments();
            return arguments.Length == 1 && arguments[0] == typeof(T);
        }

        public static bool IsListGeneric(Type type)
        {
            return type.IsGenericType && (type.GetGenericTypeDefinition() == typeof(IList<>) || type.GetGenericTypeDefinition() == typeof(List<>));
        }

        public static bool IsListOriginModel(Type type)
        {
            if (IsListGeneric(type))
            {
                var arguments = type.GetGenericArguments();
                if (arguments.Length == 1 && arguments[0].GetInterface(typeof(IOriginalModel).Name) != null)
                {
                    return true;
                }
            }

            return false;
        }

        public static object ParseEnum(Type type, object value)
        {
            object obj = null;

            //try
            {
                if (value != null && !string.IsNullOrEmpty((string)value))
                {
                    obj = System.Enum.Parse(type, (string)value);
                }
            }
//            catch (Exception e)
//            {
//#if UNITY_EDITOR
//                Debug.LogError("Error in parse enum: " + type.Name + ";" + e.Message);
//#endif
//                obj = null;
//            }

            return obj;
        }
    }
}
