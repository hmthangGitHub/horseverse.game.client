using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using UnityEngine.UI;

namespace CoreData
{
    public static class GameCommon
    {
        public static readonly string AssetBundlesFolder = "AssetBundles/";
        public static readonly string DatabaseFolder = "DataBase/";
        public static readonly string TextureFolder = "Texture/";
        public static string FolderPath(bool inStreamingAssets)
        {
#if UNITY_ANDROID && !UNITY_EDITOR
            return Application.persistentDataPath + "/";
#else
            return (inStreamingAssets ? Application.streamingAssetsPath : Application.persistentDataPath) + "/";
#endif
        }

        public static string FolderPathAssetBundle(bool inStreamingAssets)
        {
            return FolderPath(inStreamingAssets) + AssetBundlesFolder;
        }

        public static string FolderPathDatabase(bool inStreamingAssets)
        {
            return FolderPath(inStreamingAssets) + DatabaseFolder;
        }

        public static string FolderPathTexture(bool inStreamingAssets)
        {
            return FolderPath(inStreamingAssets) + TextureFolder;
        }

        public static string GetWWWFilePathForDatabase(bool inStreamingAssets, string directory)
        {
#if UNITY_ANDROID && !UNITY_EDITOR
            return "jar:file://" + Application.dataPath + "!/assets/" + DatabaseFolder + directory;
            //return new System.Uri(Path.Combine(Application.persistentDataPath, DatabaseFolder + directory)).AbsoluteUri;
#else
            return new System.Uri(Path.Combine(inStreamingAssets ? Application.streamingAssetsPath : Application.persistentDataPath, DatabaseFolder + directory)).AbsoluteUri;
#endif
        }

        public static string GetWWWFilePathForAssetBundle(bool inStreamingAssets, string directory)
        {
#if UNITY_ANDROID && !UNITY_EDITOR
            return new System.Uri(Path.Combine(Application.persistentDataPath, AssetBundlesFolder + directory)).AbsoluteUri;
#else
            return new System.Uri(Path.Combine(inStreamingAssets ? Application.streamingAssetsPath : Application.persistentDataPath, AssetBundlesFolder + directory)).AbsoluteUri;
#endif
        }

        public static string GetWWWFilePathForTexture(bool inStreamingAssets, string directory)
        {
#if UNITY_ANDROID && !UNITY_EDITOR
            return new System.Uri(Path.Combine(Application.persistentDataPath, TextureFolder + directory)).AbsoluteUri;
#else
            return new System.Uri(Path.Combine(inStreamingAssets ? Application.streamingAssetsPath : Application.persistentDataPath, TextureFolder + directory)).AbsoluteUri;
#endif
        }

        public static string GetWWWFilePathFromPath(string path)
        {
            return new System.Uri(path).AbsoluteUri;
        }

        public static bool AssetBundleFileExists(bool inStreamingAssets, string directory)
        {
            string folder;
            string filename;
            FindFolderAndFile(directory, out folder, out filename);
            var directoryFolder = FolderPathAssetBundle(inStreamingAssets) + folder;
            if (!Directory.Exists(directoryFolder))
            {
                return false;
            }
            string[] filePaths = Directory.GetFiles(FolderPathAssetBundle(inStreamingAssets) + folder, "*", SearchOption.AllDirectories);
            if (filePaths.Length == 0)
            {
                return false;
            }
            foreach (var item in filePaths)
            {
                if (item == FolderPathAssetBundle(inStreamingAssets) + directory)
                {
                    return true;
                }
            }
            return false;
        }

        public static void FindFolderAndFile(string directory, out string folder, out string filename)
        {
            var newDirectory = directory;
            folder = newDirectory.Substring(0, newDirectory.LastIndexOf("/", System.StringComparison.Ordinal));
            newDirectory = directory;
            var startIndex = newDirectory.LastIndexOf("/", System.StringComparison.Ordinal) + 1;
            filename = newDirectory.Substring(newDirectory.LastIndexOf("/", System.StringComparison.Ordinal) + 1, newDirectory.Length - startIndex);
        }

        public static void DeleteDirectory(string directory)
        {
            if (Directory.Exists(directory))
            {
                string[] pathList = Directory.GetFiles(directory);
                foreach (var path in pathList)
                {
                    File.Delete(path);
                }
                string[] dirList = Directory.GetDirectories(directory);
                foreach (var dir in dirList)
                {
                    DeleteDirectory(dir);
                }
                Directory.Delete(directory);
            }
        }

        public static void SaveFile(bool inStreamingAssets, string json)
        {
            var path = GameCommon.FolderPath(inStreamingAssets) + "Text/";
            if (!System.IO.Directory.Exists(path))
            {
                System.IO.Directory.CreateDirectory(path);
            }
            System.IO.File.WriteAllText(path + "test" + ".json", json);
        }

        public static void SaveFileLog(bool inStreamingAssets, string log)
        {
            var path = GameCommon.FolderPath(inStreamingAssets) + "LogFile/";
            if (!System.IO.Directory.Exists(path))
            {
                System.IO.Directory.CreateDirectory(path);
            }
            System.IO.File.WriteAllText(path + "test" + ".txt", log);
        }

        public static byte[] ObjectToByteArray(object obj)
        {
            if (obj == null)
                return null;
            BinaryFormatter bf = new BinaryFormatter();
            MemoryStream ms = new MemoryStream();
            bf.Serialize(ms, obj);
            return ms.ToArray();
        }

        public static object ByteArrayToObject(byte[] arrBytes)
        {
            MemoryStream memStream = new MemoryStream();
            BinaryFormatter binForm = new BinaryFormatter();
            memStream.Write(arrBytes, 0, arrBytes.Length);
            memStream.Seek(0, SeekOrigin.Begin);
            object obj = binForm.Deserialize(memStream);
            return obj;
        }

        public static byte[] ReadFully(Stream input)
        {
            byte[] buffer = new byte[16 * 1024];
            using (MemoryStream ms = new MemoryStream())
            {
                int read;
                while ((read = input.Read(buffer, 0, buffer.Length)) > 0)
                {
                    ms.Write(buffer, 0, read);
                }
                return ms.ToArray();
            }
        }

        public static List<float> GetAnglesFromAngleAndNumber(float angle, int n)
        {
            List<float> angles = new List<float>();
            float start = angle;
            if (n > 1)
            {
                start = -n / 2 * angle + (n % 2 == 0 ? 1 : 0) * angle / 2;
            }
            else
            {
                start = 0;
            }
            for (int i = 0; i < n; i++)
            {
                angles.Add(start);
                start += angle;
            }
            return angles;
        }

#region random priority
        class RangePriority
        {
            public float from;
            public float to;
        }

        public static List<T> FindValuesByPriority<T>(this List<T> list, Func<T, float> func, int n, Func<float, float, float> randomFloat = null)
        {
            var outList = new List<T>();
            if (list.Count > 0)
            {
                for (int k = 0; k < n; k++)
                {
                    var index = FindIndexByPriority(list, func, randomFloat);
                    outList.Add(list[index]);
                    list.RemoveAt(index);
                    if (list.Count == 0) break;
                }
            }
            return outList;
        }

        public static T FindValueByPriority<T>(this List<T> list, Func<T, float> func, Func<float, float, float> randomFloat = null)
        {
            var index = FindIndexByPriority(list, func, randomFloat);
            if (index == -1)
            {
                return default(T);
            }
            return list[index];
        }

        public static int FindIndexByPriority<T>(this List<T> list, Func<T, float> func, Func<float, float, float> randomFloat = null)
        {
            if (list.Count == 0) return -1;

            List<RangePriority> ranges = new List<RangePriority>();
            float total = 0;
            for (int i = 0; i < list.Count; i++)
            {
                var obj = list[i];
                var value = func(obj);
                value = value <= 0 ? 1 : value;
                RangePriority range = new RangePriority();
                range.from = total;
                range.to = total + value;
                total += value;
                ranges.Add(range);
            }
            float random = 0;
            if (randomFloat != null)
            {
                random = randomFloat(0, total);
            }
            else
            {
                random = UnityEngine.Random.Range(0, total);
            }
            if (random >= total)
            {
                return ranges.Count - 1;
            }
            int index = -1;
            for (int i = 0; i < ranges.Count; i++)
            {
                var range = ranges[i];
                if (random >= range.from && random < range.to)
                {
                    index = i;
                    break;
                }
            }
            return index;
        }

#endregion

        public static string FormatNumberToString(long number)
        {
            var negative = number < 0;
            if (negative)
            {
                number *= -1;
            }
            string result = "";
            var numberString = number + "";
            int count = 0;
            string temp = "";
            for (int i = numberString.Length - 1; i >= 0; i--)
            {
                count++;
                string point = "";
                if (count == 3 && i > 0)
                {
                    count = 0;
                    point = ",";
                }
                temp += numberString[i] + point;
            }
            if (negative)
            {
                temp += "-";
            }
            for (int i = temp.Length - 1; i >= 0; i--)
            {
                result += temp[i];
            }
            return result;
        }

        public static string GetTextWithCheckLongText(Text textUI, string text)
        {
            TextGenerator textGen = new TextGenerator();
            TextGenerationSettings generationSettings = textUI.GetGenerationSettings(textUI.rectTransform.rect.size);
            textGen.Populate(text, generationSettings);
            bool needDotDotDot = false;
            var length = text.Length;
            if (textGen.characterCount < length)
            {
                needDotDotDot = true;
            }
            var textTemp = text;
            var preText = text;
            int i = 0;
            while (needDotDotDot && i < length - 1)
            {
                preText = textTemp;
                i++;
                textTemp = text.Substring(0, i);
                textGen.Populate(textTemp + "...", generationSettings);
                if (textTemp.Length + 3 > textGen.characterCount)
                {
                    break;
                }
            }
            textTemp = preText;
            text = textTemp + (needDotDotDot ? "..." : "");
            return text;
        }

        public static string FormatTimeToStringMinuteAndSecond(long timeSecond)
        {
            var result = "";
            var minute = timeSecond / 60;
            var second = timeSecond % 60;
            result = (minute < 10 ? "0" : "") + minute;
            result += ":";
            result += (second < 10 ? "0" : "") + second;
            return result;
        }

        public static object CreateObjectByClassName(string name, string assembly)
        {
            string objectToInstantiate = name + ", " + assembly;
            var objectType = Type.GetType(objectToInstantiate);
            return Activator.CreateInstance(objectType);
        }
    }

    public static class ZipCallBackExtension
    {
        public static ZipCallBack Add(this ZipCallBack source, Action<Func<Action>> onComplete)
        {
            var newAction = source.CreateAction();
            source.AddAction(() => {
                ZipCallBack zip = new ZipCallBack(() => {
                    source.StartAction();
                    newAction();
                });
                onComplete(() =>
                {
                    return zip.CreateAction();
                });
                zip.Subscribe();
            });
            return source;
        }
    }

    public class ZipCallBack
    {
        private Action _onComplete;
        private int _count = 0;
        bool _hasSubcribe = false;
        //
        List<Action> _actions = new List<Action>();
        public void AddAction(Action action)
        {
            _actions.Add(action);
        }
        public void StartAction()
        {
            if (_actions.Count > 0)
            {
                var action = _actions[0];
                _actions.RemoveAt(0);
                action();
            }
        }
        //
        protected ZipCallBack()
        {
        }
        public ZipCallBack(Action onComplete)
        {
            _onComplete = onComplete;
            _count = 1;
        }

        private void Descrease()
        {
            _count--;
            if (_count <= 0)
            {
                _onComplete();
            }
        }

        public Action CreateAction()
        {
            Action callback = () => {
                Descrease();
            };
            _count++;
            return callback;
        }

        public void Subscribe()
        {
            if (!_hasSubcribe)
            {
                _hasSubcribe = true;
                StartAction();
                Descrease();
            }
        }
    }

}
