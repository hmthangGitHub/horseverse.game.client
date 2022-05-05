using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CoreData
{
    public static class ExtensionModel
    {
        public static string ToString<T>(this List<T> source) where T : OriginalModel
        {
            return GetJson(source);
        }

        public static string GetJson<T>(this List<T> source) where T : OriginalModel
        {
            string result = "";
            List<object> listObj = new List<object>();
            for (int i = 0; i < source.Count; i++)
            {
                var t = source[i];
                var dict = t.GetDict();
                listObj.Add(dict);
            }
            result = MiniJSON.Json.Serialize(listObj);
            return result;
        }

        public static string GetJson(this List<object> source)
        {
            string result = "";
            result = MiniJSON.Json.Serialize(source);
            return result;
        }

        public static List<T> ParseFromJson<T>(this string source) where T : OriginalModel, new()
        {
            List<T> list = new List<T>();
            var data = MiniJSON.Json.Deserialize(source);
            List<object> listObj = data as List<object>;
            if (listObj == null)
                return list;
            for (int i = 0; i < listObj.Count; i++)
            {
                var dict = listObj[i] as Dictionary<string, object>;
                T t = new T();
                t.ParseFromDict(dict);
                list.Add(t);
            }
            return list;
        }
    }

}