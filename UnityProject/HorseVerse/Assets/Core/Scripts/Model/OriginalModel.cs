using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using SQLite4Unity3d;
using System.Linq;
using Newtonsoft.Json;

namespace CoreData
{
    public static class OriginalModelExtension
    {
        public static T Clone<T>(this T source) where T : OriginalModel, new()
        {
            if (source == null) return null;
            T t = new T();
            t.ParseFromDict(source.GetDict());
            t.IsOnlyCache = source.IsOnlyCache;
            return t;
        }

        public static List<T> Clone<T>(this List<T> source) where T : OriginalModel, new()
        {
            List<T> list = new List<T>();
            for (int i = 0; i < source.Count; i++)
            {
                var item = source[i];
                if (item == null)
                {
                    list.Add(null);
                    continue;
                }
                T t = new T();
                t.ParseFromDict(item.GetDict());
                t.IsOnlyCache = item.IsOnlyCache;
                list.Add(t);
            }
            return list;
        }
    }

    public interface IOriginalModel
    {
        string GetJson();
        void ParseFromJson(string json);
        Dictionary<string, object> GetDict();
        void ParseFromDict(Dictionary<string, object> dict);
    }

    public abstract class OriginalModel : IOriginalModel
    {
        #region clone
        [Ignore]
        [Newtonsoft.Json.JsonIgnore]
        public bool IsOnlyCache { get; set; } // for multi

        [Ignore]
        [Newtonsoft.Json.JsonIgnore]
        public virtual int HashCodeProperties
        {
            get
            {
                return this.GetHashCode();
            }
        }

        public T MasterGet<T>(string id) where T : BaseModel, new()
        {
            return MasterData.Instance.Get<T>(id);
        }

        public T MasterGet<T>(Func<T, bool> func) where T : BaseModel, new()
        {
            return MasterData.Instance.Get(func);
        }

        public List<T> MasterGetList<T>(Func<T, bool> func) where T : BaseModel, new()
        {
            return MasterData.Instance.GetList(func);
        }

        public List<T> MasterGetListByListId<T>(List<string> listId, bool ignoreNull = false) where T : BaseModel, new()
        {
            return MasterData.Instance.GetListByListId<T>(listId, false, ignoreNull);
        }

        public T UserGet<T>(string id) where T : BaseModel, new()
        {
            if (IsOnlyCache)
            {
                return UserDataCache.Instance.Get<T>(id);
            }
            return UserData.Instance.Get<T>(id);
        }

        public T UserGet<T>(Func<T, bool> func) where T : BaseModel, new()
        {
            if (IsOnlyCache)
            {
                return UserDataCache.Instance.Get(func);
            }
            return UserData.Instance.Get(func);
        }

        public List<T> UserGetList<T>(Func<T, bool> func) where T : BaseModel, new()
        {
            if (IsOnlyCache)
            {
                return UserDataCache.Instance.GetList(func);
            }
            return UserData.Instance.GetList(func);
        }

        public List<T> UserGetListByListId<T>(List<string> listId, bool ignoreNull = false) where T : BaseModel, new()
        {
            if (IsOnlyCache)
            {
                return UserDataCache.Instance.GetListByListId<T>(listId, false, ignoreNull);
            }
            return UserData.Instance.GetListByListId<T>(listId, false, ignoreNull);
        }
        #endregion

        public override string ToString()
        {
            return GetJson();
        }

        public string GetJson()
        {
            var dict = GetDict();
            return MiniJSON.Json.Serialize(dict);
        }

        public void ParseFromJson(string json)
        {
            var data = MiniJSON.Json.Deserialize(json);
            Dictionary<string, object> dict = data as Dictionary<string, object>;
            ParseFromDict(dict);
        }

        public Dictionary<string, object> GetDict()
        {
            Dictionary<string, object> dict = new Dictionary<string, object>();
            var type = this.GetType();
            var properties = type.GetProperties();
            foreach (var propertyInfo in properties)
            {
                var nameField = propertyInfo.Name;
                var firstString = nameField.Substring(0, 1).ToLower();
                var primaryKeySerialized = Attribute.IsDefined(propertyInfo, typeof(PrimaryKeyAttribute));
                if (primaryKeySerialized)
                {
                    nameField = "_" + firstString + nameField.Substring(1);
                }
                else
                {
                    nameField = firstString + nameField.Substring(1);
                }
                var ignoreSerialized = Attribute.IsDefined(propertyInfo, typeof(IgnoreAttribute));
                if (!ignoreSerialized)
                {
                    object data = propertyInfo.GetValue(this, null);
                    if (TypeHelper.IsListOriginModel(propertyInfo.PropertyType)
                        || TypeHelper.IsList<string>(propertyInfo.PropertyType)
                        || TypeHelper.IsList<int>(propertyInfo.PropertyType)
                        || TypeHelper.IsList<long>(propertyInfo.PropertyType)
                        || TypeHelper.IsListEnum(propertyInfo.PropertyType))
                    {
                        dict.Add(nameField, Parser.GetListObjectDictFromObject(data));
                    }
                    else if (TypeHelper.IsOriginalModel(propertyInfo.PropertyType))
                    {
                        dict.Add(nameField, Parser.GetDictFromObject(data));
                    }
                    else
                    {
                        dict.Add(nameField, data);
                    }
                }
            }
            return dict;
        }

        public void ParseFromDict(Dictionary<string, object> dict)
        {
            var type = this.GetType();
            var properties = type.GetProperties();
            foreach (var propertyInfo in properties)
            {
                var nameField = propertyInfo.Name;
                var firstString = nameField.Substring(0, 1).ToLower();
                var primaryKeySerialized = Attribute.IsDefined(propertyInfo, typeof(PrimaryKeyAttribute));
                if (primaryKeySerialized)
                {
                    nameField = "_" + firstString + nameField.Substring(1);
                }
                else
                {
                    nameField = firstString + nameField.Substring(1);
                }
                var ignoreSerialized = Attribute.IsDefined(propertyInfo, typeof(IgnoreAttribute));
                if (!ignoreSerialized)
                {
                    object data = null;
                    if (propertyInfo.PropertyType == typeof(string))
                    {
                        data = Parser.GetString(dict, nameField);
                    }
                    else if (propertyInfo.PropertyType == typeof(int))
                    {
                        data = Parser.GetInt(dict, nameField);
                    }
                    else if (propertyInfo.PropertyType == typeof(float))
                    {
                        data = Parser.GetFloat(dict, nameField);
                    }
                    else if (propertyInfo.PropertyType == typeof(bool))
                    {
                        data = Parser.GetBool(dict, nameField);
                    }
                    else if (propertyInfo.PropertyType == typeof(long))
                    {
                        data = Parser.GetLong(dict, nameField);
                    }
                    else if (propertyInfo.PropertyType == typeof(short))
                    {
                        data = Parser.GetShort(dict, nameField);
                    }
                    else if (propertyInfo.PropertyType == typeof(double))
                    {
                        data = Parser.GetDouble(dict, nameField);
                    }
                    else if (propertyInfo.PropertyType.IsEnum)
                    {
                        data = Enum.Parse(propertyInfo.PropertyType, Parser.GetString(dict, nameField), true);
                        //try
                        //{
                        //    data = Enum.Parse(propertyInfo.PropertyType, Parser.GetString(dict, nameField), true);
                        //}
                        //catch (Exception e)
                        //{
                        //    Debug.LogError("Parse enum error: model " + type.ToString() + ";field: " + nameField + ";value: " + Parser.GetString(dict, nameField));
                        //}

                        //#if UNITY_EDITOR
                        //                        try
                        //                        {
                        //                            data = Enum.Parse(propertyInfo.PropertyType, Parser.GetString(dict, nameField), true);
                        //                        }
                        //                        catch (Exception e)
                        //                        {
                        //                            Debug.LogError("Parse enum error: model " + type.ToString() + ";field: " + nameField + ";value: " + Parser.GetString(dict, nameField));
                        //                        }
                        //#else
                        //                        data = Enum.Parse(propertyInfo.PropertyType, Parser.GetString(dict, nameField), true);
                        //#endif
                    }
                    else if (propertyInfo.PropertyType == typeof(List<int>))
                    {
                        data = Parser.GetListInt(dict, nameField);
                    }
                    else if (propertyInfo.PropertyType == typeof(List<long>))
                    {
                        data = Parser.GetListLong(dict, nameField);
                    }
                    else if (propertyInfo.PropertyType == typeof(List<string>))
                    {
                        var list = Parser.GetListString(dict, nameField);
                        data = list;
                    }
                    else if (TypeHelper.IsListEnum(propertyInfo.PropertyType))
                    {
                        var typeOfElement = propertyInfo.PropertyType.GetGenericArguments()[0];
                        var list = Parser.GetListEnum(dict, nameField, typeOfElement);
                        data = list;
                    }
                    else if (propertyInfo.PropertyType == typeof(Dictionary<string, object>))
                    {
                        data = Parser.GetDict(dict, nameField);
                    }
                    else if (TypeHelper.IsListOriginModel(propertyInfo.PropertyType))
                    {
                        var typeOfElement = propertyInfo.PropertyType.GetGenericArguments()[0];
                        data = Parser.GetListModelByType(dict, nameField, typeOfElement);
                    }
                    else if (TypeHelper.IsOriginalModel(propertyInfo.PropertyType))
                    {
                        data = Parser.GetModelByType(dict, nameField, propertyInfo.PropertyType);
                    }
                    //Debug.Log("type: " + type.ToString() + ";" + nameField);
                    propertyInfo.SetValue(this, data, null);
                }
            }
        }

        protected virtual void ClearCacheDetail()
        {
        }

        public void ClearCache()
        {
            ClearCacheDetail();
        }
    }
}

