using System.Collections.Generic;
using System;
using MiniJSON;
using UnityEngine;
using System.Collections;

namespace CoreData
{
    public static class Parser
    {
        public static string GetString(Dictionary<string, object> dict, string key, string def = "")
        {
            if (dict != null)
            {
                object obj;
                dict.TryGetValue(key, out obj);
                if (obj != null) return obj.ToString();
            }
            return def;
        }

        public static Vector3 GetVector3(Dictionary<string, object> dict, string key)
        {
            Vector3 result = JsonParser.GetVector3(GetString(dict, key));
            return result;
        }

        public static int GetInt(Dictionary<string, object> dict, string key, int def = 0)
        {
            return int.Parse(GetString(dict, key));
            int result;
            if (int.TryParse(GetString(dict, key), out result)) return result;
            return def;
        }

        public static int GetShort(Dictionary<string, object> dict, string key, short def = 0)
        {
            return short.Parse(GetString(dict, key));
            short result;
            if (short.TryParse(GetString(dict, key), out result)) return result;
            return def;
        }

        public static long GetLong(Dictionary<string, object> dict, string key, long def = 0)
        {
            return long.Parse(GetString(dict, key));
            long result;
            if (long.TryParse(GetString(dict, key), out result)) return result;
            return def;
        }

        public static bool GetBool(Dictionary<string, object> dict, string key, bool def = false)
        {
            return bool.Parse(GetString(dict, key));
            bool result;
            if (bool.TryParse(GetString(dict, key), out result)) return result;
            return def;
        }

        public static float GetFloat(Dictionary<string, object> dict, string key, float def = 0)
        {
            return float.Parse(GetString(dict, key));
            float result;
            if (float.TryParse(GetString(dict, key), out result)) return result;
            return def;
        }

        public static double GetDouble(Dictionary<string, object> dict, string key, double def = 0)
        {
            return double.Parse(GetString(dict, key));
            double result;
            if (double.TryParse(GetString(dict, key), out result)) return result;
            return def;
        }

        public static T GetEnum<T>(Dictionary<string, object> dict, string key)
        {
            return ParseEnum<T>(GetString(dict, key));
        }

        public static T ParseEnum<T>(string str)
        {
            //try
            {
                return (T)Enum.Parse(typeof(T), str);
            }
            //catch
            //{
            //    return (T)Activator.CreateInstance(typeof(T));
            //}
        }

        public static List<object> ConvertListValue<T>(List<T> list)
        {
            var outList = new List<object>();
            if (list != null)
            {
                foreach (var item in list)
                {
                    outList.Add(item);
                }
            }
            return outList;
        }

        public static IList GetListEnum(Dictionary<string, object> dict, string key, Type type)
        {
            var listType = typeof(List<>).MakeGenericType(new[] { type });
            var outList = (IList)Activator.CreateInstance(listType);

            var list = GetListObj(dict, key);

            if (list != null)
            {
                foreach (var item in list)
                {
                    if (item != null)
                    {
                        outList.Add(GetEnumByType(item.ToString(), type));
                    }
                }
            }

            return outList;
        }

        public static List<string> GetListString(Dictionary<string, object> dict, string key)
        {
            var outList = new List<string>();
            var list = GetListObj(dict, key);

            if (list != null)
            {
                foreach (var item in list)
                {
                    if (item != null)
                    {
                        outList.Add(item.ToString());
                    }
                }
            }

            return outList;
        }

        public static List<int> GetListInt(Dictionary<string, object> dict, string key)
        {
            var outList = new List<int>();
            var list = GetListObj(dict, key);

            if (list != null)
            {
                foreach (var item in list)
                {
                    if (item != null)
                    {
                        var value = 0;

                        value = (int)item;
                        //try
                        //{
                        //    value = int.Parse(item.ToString());
                        //}
                        //catch
                        //{
                        //    LogError("Parse int: " + key);
                        //}

                        outList.Add(value);
                    }
                }
            }

            return outList;
        }

        public static List<long> GetListLong(Dictionary<string, object> dict, string key)
        {
            var outList = new List<long>();
            var list = GetListObj(dict, key);

            if (list != null)
            {
                foreach (var item in list)
                {
                    if (item != null)
                    {
                        var value = 0L;
                        value = (long)item;
                        //try
                        //{
                        //    value = long.Parse(item.ToString());
                        //}
                        //catch
                        //{
                        //    LogError("Parse long: " + key);
                        //}

                        outList.Add(value);
                    }

                }
            }

            return outList;
        }

        public static List<float> GetListFloat(Dictionary<string, object> dict, string key)
        {
            var outList = new List<float>();
            var list = GetListObj(dict, key);

            if (list != null)
            {
                foreach (var item in list)
                {
                    if (item != null)
                    {
                        var value = 0f;
                        value = (float)item;
                        //try
                        //{
                        //    value = float.Parse(item.ToString());
                        //}
                        //catch
                        //{
                        //    LogError("Parse float: " + key);
                        //}

                        outList.Add(value);
                    }
                }
            }

            return outList;
        }

        public static List<bool> GetListBool(Dictionary<string, object> dict, string key)
        {
            var outList = new List<bool>();
            var list = GetListObj(dict, key);

            if (list != null)
            {
                foreach (var item in list)
                {
                    if (item != null)
                    {
                        var value = false;
                        value = (bool)item;
                        //try
                        //{
                        //    value = bool.Parse(item.ToString());
                        //}
                        //catch
                        //{
                        //    LogError("Parse bool: " + key);
                        //}

                        outList.Add(value);
                    }
                }
            }

            return outList;
        }

        public static List<double> GetListDouble(Dictionary<string, object> dict, string key)
        {
            var outList = new List<double>();
            var list = GetListObj(dict, key);

            if (list != null)
            {
                foreach (var item in list)
                {
                    if (item != null)
                    {
                        double value = 0;
                        value = (double)item;
                        //try
                        //{
                        //    value = double.Parse(item.ToString());
                        //}
                        //catch
                        //{
                        //    LogError("Parse double: " + key);
                        //}

                        outList.Add(value);
                    }

                }
            }

            return outList;
        }

        private static List<object> GetListObj(Dictionary<string, object> dict, string key)
        {
            if (dict != null)
            {
                object obj;
                dict.TryGetValue(key, out obj);
                if (obj != null) return obj as List<object>;
            }
            return null;
        }


        public static Dictionary<string, object> GetDict(Dictionary<string, object> dict, string key)
        {
            if (dict != null)
            {
                object item;
                dict.TryGetValue(key, out item);

                if (item != null)
                {
                    var data = item as Dictionary<string, object>;
                    if (data != null) return data;
                }
            }
            return new Dictionary<string, object>();
        }

        public static List<object> GetList(Dictionary<string, object> dict, string key)
        {
            if (dict != null)
            {
                object item;
                dict.TryGetValue(key, out item);

                if (item != null)
                {
                    var list = item as List<object>;
                    if (list != null) return list;
                }
            }
            return new List<object>();
        }

        public static List<T> GetList<T>(Dictionary<string, object> dict, string key) where T : OriginalModel
        {
            if (dict != null)
            {
                object item;
                dict.TryGetValue(key, out item);

                if (item != null)
                {
                    var list = item as List<object>;
                    var newList = new List<T>();
                    for (int i = 0; i < list.Count; i++)
                    {
                        newList.Add(list[i] as T);
                    }
                }
            }
            return new List<T>();
        }

        public static Dictionary<string, object> GetDict(string json)
        {
            var dict = Json.Deserialize(json) as Dictionary<string, object>;
            if (dict != null) return dict;
            return new Dictionary<string, object>();
        }

        //
        public static List<object> GetListObjectDictFromObject(object obj)
        {
            var list = new List<object>();
            var iList = obj as System.Collections.IList;

            if (iList != null)
            {
                var type = obj.GetType();
                bool isListEnum = TypeHelper.IsListEnum(type);
                bool isListObject = !isListEnum && TypeHelper.IsListOriginModel(type);
                //bool isListString = TypeHelper.IsList<string>(type);
                //bool isListInt = TypeHelper.IsList<int>(type);
                //bool isListFloat = TypeHelper.IsList<float>(type);
                //bool isListLong = TypeHelper.IsList<long>(type);

                foreach (var item in iList)
                {
                    if (isListObject)
                    {
                        list.Add(((OriginalModel)item).GetDict());
                    }
                    else if (isListEnum)
                    {
                        list.Add(item.ToString());
                    }
                    else 
                    {
                        list.Add(item);
                    }
                }
            }

            return list;
        }

        public static Dictionary<string, object> GetDictFromObject(object obj)
        {
            OriginalModel om = obj as OriginalModel;
            if (om != null)
            {
                return om.GetDict();
            }
            return null;
        }

        public static object GetModelByType(Dictionary<string, object> dict, string key, Type type)
        {
            if (dict != null && dict.ContainsKey(key))
            {
                var objDict = dict[key] as Dictionary<string, object>;
                if (objDict != null)
                {
                    object typeModel = null;
                    object obj;
                    if (objDict.TryGetValue("typeNameOfModel", out typeModel))
                    {
                        var typeModelName = (string)typeModel;
                        obj = GameCommon.CreateObjectByClassName(typeModelName, objDict["assembly"].ToString());
                    }
                    else
                    {
                        obj = Activator.CreateInstance(type);
                    }
                    (obj as OriginalModel).ParseFromDict(objDict);
                    return obj;
                }
            }
            return null;
        }

        public static IList GetListModelByType(Dictionary<string, object> dict, string key, Type type)
        {
            var listType = typeof(List<>).MakeGenericType(new[] { type });
            var outList = (IList)Activator.CreateInstance(listType);

            var list = GetListObj(dict, key);
            if (list != null)
            {
                foreach (var item in list)
                {
                    var objDict = item as Dictionary<string, object>;
                    object typeModel = null;
                    object obj;
                    if (objDict.TryGetValue("typeNameOfModel", out typeModel))
                    {
                        var typeModelName = (string)typeModel;
                        obj = GameCommon.CreateObjectByClassName(typeModelName, objDict["assembly"].ToString());
                    }
                    else
                    {
                        obj = Activator.CreateInstance(type);
                    }
                    (obj as OriginalModel).ParseFromDict(objDict);
                    outList.Add(obj);
                }
            }
            return outList;
        }

        public static object GetEnumByType(Dictionary<string, object> dict, string key, Type type)
        {
            //try
            {
                return Enum.Parse(type, Parser.GetString(dict, key));
            }
            //catch
            //{
            //    return Activator.CreateInstance(type);
            //}
        }

        public static object GetEnumByType(string value, Type type)
        {
            return Enum.Parse(type, value);
            //try
            //{
            //    return Enum.Parse(type, value);
            //}
            //catch
            //{
            //    return Activator.CreateInstance(type);
            //}
        }
        //

        private static void LogError(object msg)
        {
        }
    }
}
