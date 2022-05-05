using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using MiniJSON;
using System.Text;

namespace CoreData
{
    public static class JsonParser
    {
        public static List<T> GetEnumList<T>(Dictionary<string, object> dict, string key)
        {
            var outList = new List<T>();

            if (dict.ContainsKey(key))
            {
                var list = dict[key] as List<object>;
                if (list != null && list.Count > 0)
                {
                    foreach (var item in list)
                    {
                        outList.Add((T)Enum.Parse(typeof(T), (string)item));
                    }
                }
            }

            return outList;
        }

        public static List<string> GetListString(Dictionary<string, object> dict, string key)
        {
            var outList = new List<string>();

            if (dict.ContainsKey(key))
            {
                var list = dict[key] as List<object>;
                if (list != null && list.Count > 0)
                {
                    foreach (var item in list)
                    {
                        outList.Add((string)item);
                    }
                }
            }

            return outList;
        }

        public static string GetJsonOfList(object obj)
        {
            var list = new List<object>();
            var iList = obj as System.Collections.IList;

            if (iList != null)
            {
                var type = obj.GetType();
                bool isListObject = TypeHelper.IsListOriginModel(type);
                bool isListEnum = TypeHelper.IsListEnum(type);
                bool isListString = TypeHelper.IsList<string>(type);
                bool isListInt = TypeHelper.IsList<int>(type);
                bool isListFloat = TypeHelper.IsList<float>(type);
                bool isListLong = TypeHelper.IsList<long>(type);

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
                    else if (isListString)
                    {
                        list.Add(item);
                    }
                    else if (isListInt)
                    {
                        list.Add(item);
                    }
                    else if (isListLong)
                    {
                        list.Add(item);
                    }
                    else if (isListFloat)
                    {
                        list.Add(item);
                    }
                }
            }

            return Json.Serialize(list);
        }

        public static object GetListObject(string json, Type itemType)
        {
            var list = Activator.CreateInstance(typeof(List<>).MakeGenericType(new Type[] { itemType }));
            var listDict = Json.Deserialize(json) as List<object>;

            if (listDict != null)
            {
                var isListObject = itemType.GetInterface(typeof(IOriginalModel).Name) != null;
                var isListEnum = TypeHelper.IsEnum(itemType);
                var isListString = (itemType == typeof(string));
                var isListInt = (itemType == typeof(int));
                var isListLong = (itemType == typeof(long));
                var isListFloat = (itemType == typeof(float));

                var iList = list as System.Collections.IList;
                foreach (var item in listDict)
                {
                    if (isListObject)
                    {
                        var obj = Activator.CreateInstance(itemType);
                        ((OriginalModel)obj).ParseFromDict((Dictionary<string, object>)item);
                        iList.Add(obj);
                    }
                    else if (isListEnum)
                    {
                        iList.Add(Enum.Parse(itemType, (string)item));
                    }
                    else if (isListString)
                    {
                        iList.Add(item);
                    }
                    else if (isListInt)
                    {
                        iList.Add((int)((long)item));
                    }
                    else if (isListLong)
                    {
                        iList.Add((long)item);
                    }
                    else if (isListFloat)
                    {
                        float x = 0;
                        float.TryParse(item.ToString(), out x);
                        iList.Add(x);
                    }
                }
            }

            return list;
        }

        public static string GetJson(Dictionary<string, object> dict)
        {
            return Json.Serialize(dict);
        }

        public static Vector3 GetVector3(string json)
        {
            Vector3 vector3 = new Vector3();
            string result = "";
            for (int i = 1; i < json.Length - 1; i++)
            {
                var ch = json[i];
                if (ch != ' ')
                {
                    result += ch;
                }
            }
            var list = result.Split(',');
            vector3.x = float.Parse(list[0]);
            vector3.y = float.Parse(list[1]);
            vector3.z = float.Parse(list[2]);
            return vector3;
        }

        public static Dictionary<string, object> GetDict(string json)
        {
            return Json.Deserialize(json) as Dictionary<string, object>;
        }

        public static List<T> GetListObject<T>(Dictionary<string, object> dict, string key) where T : OriginalModel, new()
        {
            var outList = new List<T>();

            if (dict.ContainsKey(key))
            {
                var list = dict[key] as List<object>;
                if (list != null && list.Count > 0)
                {
                    int count = list.Count;
                    for (int i = 0; i < count; i++)
                    {
                        if (list[i] != null)
                        {
                            var t = new T();
                            t.ParseFromDict(list[i] as Dictionary<string, object>);
                            outList.Add(t);
                        }
                    }
                }
            }

            return outList;
        }

        public static T GetObject<T>(Dictionary<string, object> dict, string key) where T : OriginalModel, new()
        {
            if (dict.ContainsKey(key))
            {
                var objectDict = dict[key] as Dictionary<string, object>;
                if (objectDict != null)
                {
                    var t = new T();
                    t.ParseFromDict(objectDict);
                    return t;
                }
            }

            return default(T);
        }

        static public bool GetBool(IDictionary<string, object> dict, string key, bool def = false)
        {
            if (dict.ContainsKey(key))
            {
                return (bool)dict[key];
            }
            else
            {
                return def;
            }
        }

        static public string GetStringDefault(IDictionary<string, object> dict, string key, string defaultValue = "")
        {
            object value;
            if (dict.TryGetValue(key, out value) && value != null)
            {
                return value.ToString();
            }
            else
            {
                return defaultValue;
            }
        }

        static public string GetString(IDictionary<string, object> dict, string key)
        {
            object value;
            if (dict.TryGetValue(key, out value) && value != null)
            {
                return value.ToString();
            }

            return string.Empty;
        }

        static public int GetInt(IDictionary<string, object> dict, string key, int def = 0)
        {
            if (dict.ContainsKey(key))
            {
                return int.Parse(dict[key].ToString());
            }
            else
            {
                return def;
            }
        }

        static public long GetLong(IDictionary<string, object> dict, string key, long def = 0)
        {
            if (dict.ContainsKey(key))
            {
                return long.Parse(dict[key].ToString());
            }
            else
            {
                return def;
            }
        }

        static public float GetFloat(IDictionary<string, object> dict, string key, float def = 0.0f)
        {
            if (dict.ContainsKey(key))
            {
                return float.Parse(dict[key].ToString());
            }
            else
            {
                return def;
            }
        }

        static public double GetDouble(IDictionary<string, object> dict, string key, double def = 0.0f)
        {
            if (dict.ContainsKey(key))
            {
                return double.Parse(dict[key].ToString());
            }
            else
            {
                return def;
            }
        }

        public static T GetEnum<T>(IDictionary<string, object> dict, string key, T def)
        {
            string value = GetStringDefault(dict, key, def.ToString());
            return ParseEnum<T>(value);
        }

        public static T ParseEnum<T>(string value)
        {
            try
            {
                if (string.IsNullOrEmpty(value))
                {
                    return default(T);
                }
                return (T)System.Enum.Parse(typeof(T), value, true);
            }
            catch (Exception e)
            {
#if UNITY_EDITOR
                Debug.LogError(string.Format("Error in parse enum: {0}, value: {1}", e, value));
#endif
                return default(T);
            }
        }

        public static T ParseEnumDefault<T>(string value) where T : new()
        {
            try
            {
                if (string.IsNullOrEmpty(value))
                {
                    return new T();
                }
                return (T)System.Enum.Parse(typeof(T), value, true);
            }
            catch
            {
                return new T();
            }
        }

        public static List<T> GetListDict<T>(Dictionary<string, object> dict, string key, List<T> def = null) where T : BaseModel, new()
        {
            if (dict.ContainsKey(key))
            {
                var list = (List<T>)dict[key];
                var listDict = new List<T>();
                foreach (T obj in list)
                {
                    listDict.Add(obj);
                }
                return listDict;
            }
            else
            {
                if (def == null)
                    return new List<T>();
                else
                    return def;
            }
        }

        static public Dictionary<string, object> GetDict(IDictionary<string, object> dict, string key, Dictionary<string, object> def = null)
        {
            if (dict.ContainsKey(key) && dict[key] is IDictionary)
            {
                return (Dictionary<string, object>)dict[key];
            }
            else
            {
                if (def == null)
                    return new Dictionary<string, object>();
                else
                    return def;
            }
        }

        static public List<object> GetListDict(IDictionary<string, object> dict, string key, List<object> def = null)
        {
            if (dict.ContainsKey(key))
            {
                var list = (List<object>)dict[key];
                var listDict = new List<object>();
                foreach (object obj in list)
                {
                    listDict.Add(obj);
                }
                return listDict;
            }
            else
            {
                if (def == null)
                    return new List<object>();
                else
                    return def;
            }
        }

        static public List<int> GetListInt(IDictionary<string, object> dict, string key, List<int> def = null)
        {
            if (dict.ContainsKey(key))
            {
                var list = (List<object>)dict[key];
                var listInt = new List<int>();
                foreach (object obj in list)
                {
                    listInt.Add(int.Parse(obj.ToString()));
                }
                return listInt;
            }
            else
            {
                if (def == null)
                    return new List<int>();
                else
                    return def;
            }
        }

        public static List<object> GetObjectList<T>(List<T> listObject) where T : OriginalModel, new()
        {
            List<object> list = new List<object>();
            if (listObject != null)
            {
                foreach (var item in listObject)
                {
                    list.Add(item.GetDict());
                }
            }

            return list;
        }

        static public List<long> GetListLong(IDictionary<string, object> dict, string key, List<long> def = null)
        {
            if (dict.ContainsKey(key))
            {
                var list = (List<object>)dict[key];
                var listLong = new List<long>();
                foreach (object obj in list)
                {
                    listLong.Add(long.Parse(obj.ToString()));
                }
                return listLong;
            }
            else
            {
                if (def == null)
                    return new List<long>();
                else
                    return def;
            }
        }

        static public List<float> GetListFloat(IDictionary<string, object> dict, string key, List<float> def = null)
        {
            if (dict.ContainsKey(key))
            {
                var list = (List<object>)dict[key];
                var listFloat = new List<float>();
                foreach (object obj in list)
                {
                    listFloat.Add(float.Parse(obj.ToString()));
                }
                return listFloat;
            }
            else
            {
                if (def == null)
                    return new List<float>();
                else
                    return def;
            }
        }

        public static T GetEnum<T>(Dictionary<string, object> dict, string key) where T : new()
        {
            return ParseEnumDefault<T>(GetString(dict, key));
        }

        public static List<object> ToListObject(this List<string> list)
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

        public static List<object> ToListObject(this List<long> list)
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

        public static List<object> ToListObject(this List<int> list)
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
    }
}