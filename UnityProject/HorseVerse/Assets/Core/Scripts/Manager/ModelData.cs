using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using SQLite4Unity3d;
using System.Linq;
using UniRx;
using System.IO;
using Core;
using Assets.SQLite4Unity3d.Scripts;

namespace CoreData
{
    public abstract class ModelData
    {
        public bool IsStreamingAssets
        {
            get
            {
                return _inStreamingAssets;
            }
        }
        protected string _folder = "";
        public virtual string FullFolder
        {
            get
            {
                var folder = GameCommon.FolderPathDatabase(_inStreamingAssets);
                folder += _folder;
                return folder;
            }
        }
        protected SQLiteConnection _connection;
        protected string InitFileDatabase()
        {
            var DatabaseName = GetDatabaseName();
            string dbPath = "";
            string folder = "";
            folder = GameCommon.FolderPathDatabase(_inStreamingAssets);
            folder += _folder;
            var filepath = folder + DatabaseName;
            if (!File.Exists(filepath))
            {
                if (!Directory.Exists(folder))
                {
                    Directory.CreateDirectory(folder);
                }
                var loadDb = new WWW(GameCommon.GetWWWFilePathForDatabase(_inStreamingAssets, DatabaseName));
                while (!loadDb.isDone) { }
                File.WriteAllBytes(filepath, loadDb.bytes);
            }
            dbPath = filepath;
            return dbPath;
        }

        protected void InitDatabase()
        {
            if (!_hasCreateDatabase) return;

            var dbPath = InitFileDatabase();
            _connection = new SQLiteConnection(dbPath, SQLiteOpenFlags.ReadWrite | SQLiteOpenFlags.Create);
        }

        protected Dictionary<string, object> _dictData = new Dictionary<string, object>();
        protected bool _hasCreateDatabase = true;
        protected bool _inStreamingAssets = false;
        public void SetCreateDatabase(bool value)
        {
            _hasCreateDatabase = value;
        }

        public virtual string GetDatabaseName()
        {
            return "_";
        }

        public List<string> GetAllModelName()
        {
            List<string> list = new List<string>();
            foreach (var item in _dictData)
            {
                list.Add(item.Key);
            }
            return list;
        }

        public void Drop<T>()
        {
            var name = typeof(T).Name;
            object obj;
            if (_dictData.TryGetValue(name, out obj))
            {
                _dictData[name] = new List<T>();
            }
            if (!_hasCreateDatabase) return;
            _connection.DropTable<T>();
            _connection.CreateTable<T>();
        }

        public void RegistModelData<T>() where T : BaseModel, new()
        {
            try
            {
                if (_hasCreateDatabase)
                {
                    var kq = _connection.CreateTable<T>();
                }
            }
            catch(System.Exception ex)
            {
                Debug.LogError("***** CONNECTION ERROR " + typeof(T) + "  == >" + ex.ToString());
            }
            var name = typeof(T).Name;
            //Debug.Log("==**===== Try to RegistModelData: " + name);
            if (!_dictData.ContainsKey(name))
            {
                _dictData.Add(name, new List<T>());
                if (_hasCreateDatabase)
                {
                    try
                    {
                        TableQuery<T> table = _connection.Table<T>();
                        if (table != null)
                        {
                            var data = table.ToList();
                            data.SortByHashCode();
                            _dictData[name] = data;
                        }
                    }
                    catch (System.Exception ex)
                    {
                        Debug.LogError("***** Error " + name + " =>  " + ex.ToString());
                    }
                }
            }
            //Debug.Log("==**===== RegistModelData: " + name);
        }

        protected List<T> GetData<T>() where T : BaseModel
        {
            var name = typeof(T).Name;
            object obj;
            if (_dictData.TryGetValue(name, out obj))
            {
                var list = obj as List<T>;
                return list;
            }
            return new List<T>();
        }

        public List<T> GetAll<T>() where T : BaseModel, new()
        {
            var list = GetData<T>().ToList();
            return list;
        }

        public List<T> GetList<T>(Func<T, bool> func, bool isClone = false) where T : BaseModel, new()
        {
            var all = GetAll<T>();
            var list = all.Where(x => func(x)).ToList();
            if (isClone)
            {
                list = list.Clone();
            }
            return list;
        }

        public List<T> GetListByListId<T>(List<string> listId, bool isClone = false, bool ignoreNull = false) where T : BaseModel, new()
        {
            var all = GetAll<T>();
            List<T> list = new List<T>();
            if (listId == null)
                return list;
            for (int i = 0; i < listId.Count; i++)
            {
                var id = listId[i];
                if (string.IsNullOrEmpty(id))
                {
                    if (!ignoreNull)
                    {
                        list.Add(null);
                    }
                    continue;
                }
                var t = all.GetByHashCode(id.GetHashCode());
                if (t != null)
                {
                    if (isClone)
                    {
                        t = t.Clone();
                    }
                    list.Add(t);
                }
                else if (!ignoreNull)
                {
                    list.Add(null);
                }
            }
            return list;
        }

        public T GetOne<T>() where T : BaseModel, new()
        {
            var all = GetAll<T>();

            if (all.Count > 0)
            {
                return all[0];
            }
            return null;
        }

        public T Get<T>(string id, bool isClone = false) where T : BaseModel, new()
        {
            if (string.IsNullOrEmpty(id))
            {
                return null;
            }
            var all = GetAll<T>();
            var t = all.GetByHashCode(id.GetHashCode());
            if (isClone)
            {
                t = t.Clone();
            }
            return t;
        }

        public T Get<T>(Func<T, bool> func, bool isClone = false) where T : BaseModel, new()
        {
            var all = GetAll<T>();
            var t = all.FirstOrDefault(x => func(x));
            if (isClone)
            {
                t = t.Clone();
            }
            return t;
        }

        public void Insert<T>(T t, Action<T> onComplete = null) where T : BaseModel, new()
        {
            if (t != null)
            {
                var all = GetData<T>();
                if (string.IsNullOrEmpty(t.Id))
                {
                    t.Id = GenerateID();
                }
                all.AddHasSort(t);
                if (_hasCreateDatabase)
                {
                    _connection.Insert(t);
                }
            }
            if (onComplete != null)
            {
                onComplete(t);
            }
        }

        public void Insert<T>(List<T> list, Action<List<T>> onComplete = null) where T : BaseModel, new()
        {
            var all = GetData<T>();
            for (int i = 0; i < list.Count; i++)
            {
                var t = list[i];
                if (string.IsNullOrEmpty(t.Id))
                {
                    t.Id = GenerateID();
                }
            }
            all.AddRangeHasSort(list);
            if (_hasCreateDatabase)
            {
                _connection.InsertAll(list);
            }
            if (onComplete != null)
            {
                onComplete(list);
            }
        }

        public void Update<T>(T t, Action<T> onComplete = null, bool hasClearCache = false) where T : BaseModel, new()
        {
            var all = GetData<T>();
            if (_hasCreateDatabase)
            {
                _connection.Update(t);
            }
            var data = all.GetByHashCode(t.GetHashCode());
            if (data != null)
            {
                if(data.GetHashCode() != t.GetHashCode())
                {
                    data.ParseFromDict(t.GetDict());
                }
                if (hasClearCache)
                {
                    data.ClearCache();
                }
                data.OnChangedCommand.Execute();
            }
            if (onComplete != null)
            {
                onComplete(t);
            }
        }

        public void Update<T>(List<T> list, Action<List<T>> onComplete = null, bool hasClearCache = false) where T : BaseModel, new()
        {
            var all = GetData<T>();
            if (_hasCreateDatabase)
            {
                _connection.UpdateAll(list);
            }
            for (int i = 0; i < list.Count; i++)
            {
                var t = list[i];
                var data = all.GetByHashCode(t.GetHashCode());
                if (data != null)
                {
                    if (data.GetHashCode() != t.GetHashCode())
                    {
                        data.ParseFromDict(t.GetDict());
                    }
                    if (hasClearCache)
                    {
                        data.ClearCache();
                    }
                    data.OnChangedCommand.Execute();
                }
            }
            if (onComplete != null)
            {
                onComplete(list);
            }
        }

        #region use for get table data
        public void InsertOrUpdate<T>(T t, Action onComplete = null, bool hasClearCache = false) where T : BaseModel, new()
        {
            var all = GetData<T>();
            T data = null;
            if (!string.IsNullOrEmpty(t.Id))
            {
                data = all.GetByHashCode(t.GetHashCode());
            }
            if (data != null)
            {
                Update(t, null, hasClearCache);
            }
            else
            {
                Insert(t, null);
            }
            if (onComplete != null)
            {
                onComplete();
            }
        }

        public void InsertOrUpdate<T>(List<T> list, Action onComplete = null, bool hasClearCache = false) where T : BaseModel, new()
        {
            var all = GetData<T>();
            var listInsert = new List<T>();
            var listUpdate = new List<T>();
            for (int i = 0; i < list.Count; i++)
            {
                var t = list[i];
                T data = null;
                if (!string.IsNullOrEmpty(t.Id))
                {
                    data = all.GetByHashCode(t.GetHashCode());
                }
                if (data == null)
                {
                    listInsert.Add(t);
                }
                else
                {
                    listUpdate.Add(t);
                }
            }
            if (listInsert.Count > 0)
            {
                Insert(listInsert, null);
            }
            if (listUpdate.Count > 0)
            {
                Update(listUpdate, null, hasClearCache);
            }
            if (onComplete != null)
            {
                onComplete();
            }
        }
        #endregion

        public void Delete<T>(T t, Action onComplete = null) where T : BaseModel, new()
        {
            if (t == null)
            {
                if (onComplete != null)
                {
                    onComplete();
                }
                return;
            }
            var all = GetData<T>();
            var data = all.GetByHashCode(t.GetHashCode());
            if (data != null)
            {
                all.Remove(data);
                if (_hasCreateDatabase)
                {
                    _connection.Delete(data);
                }
            }
            if (onComplete != null)
            {
                onComplete();
            }
        }

        public void Delete<T>(List<T> list, Action onComplete = null) where T : BaseModel, new()
        {
            var all = GetData<T>();
            var data = all.Where(x =>
            {
                for (int i = 0; i < list.Count; i++)
                {
                    var record = list[i];
                    if (record != null && record.Id == x.Id)
                    {
                        list.Remove(record);
                        return true;
                    }
                }
                return false;
            }).ToList();
            for (int i = 0; i < data.Count; i++)
            {
                var record = data[i];
                all.Remove(record);
                if (_hasCreateDatabase)
                {
                    _connection.Delete(record);
                }
            }
            if (onComplete != null)
            {
                onComplete();
            }
        }

        public static string GenerateID()
        {
            return Guid.NewGuid().ToString();
        }

        public void ClearData()
        {
            List<Type> types = new List<Type>();
            foreach (var keyPair in _dictData)
            {
                var list = keyPair.Value as IList;
                if(list.Count > 0 && _hasCreateDatabase)
                {
                    types.Add(list[0].GetType());
                }
                list.Clear();
            }
            if (types.Count == 0)
                return;
            for(int i = 0; i < types.Count; i++)
            {
                var type = types[i];
                _connection.DropTable(type);
                _connection.CreateTable(type);
            }
        }

        // save json and excel
        public void SaveFile()
        {
            var path = GameCommon.FolderPath(_inStreamingAssets) + GetDatabaseName() + "Text/";
            if (!System.IO.Directory.Exists(path))
            {
                System.IO.Directory.CreateDirectory(path);
            }
            else
            {
                GameCommon.DeleteDirectory(path);
                System.IO.Directory.CreateDirectory(path);
            }
            foreach (var data in _dictData)
            {
                var list = Parser.GetListObjectDictFromObject(data.Value);
                if (list.Count > 0)
                {
                    var json = MiniJSON.Json.Serialize(list);
                    //Debug.Log(data.Key + " " + list.Count + ": " + json);
                    System.IO.File.WriteAllText(path + data.Key + ".json", json);
                }
            }
#if UNITY_EDITOR
            UnityEditor.EditorUtility.DisplayDialog("Export Json Success", path, "Close");
#endif
        }
        //
    }
}
