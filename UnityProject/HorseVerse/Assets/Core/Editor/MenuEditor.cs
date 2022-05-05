using UnityEngine;
using UnityEditor;
using CoreData;
using System.IO;

namespace Core.Editor
{
    public class MenuEditor
    {
        [MenuItem("Tools/Clear UserData")]
        private static void ClearUserData()
        {
            PlayerPrefs.DeleteAll();
            GameCommon.DeleteDirectory(UserData.Instance.FullFolder);
            Debug.Log("Clear UserData done!!!");
        }

        [MenuItem("Tools/Clear DataBase")]
        private static void ClearDataBase()
        {
            PlayerPrefs.DeleteAll();
            GameCommon.DeleteDirectory(MasterData.Instance.FullFolder);
            GameCommon.DeleteDirectory(UserData.Instance.FullFolder);
            GameCommon.DeleteDirectory(UserDataLocal.Instance.FullFolder);
            Debug.Log("Clear DataBase done!!!");
        }

        [MenuItem("Tools/Clear MasterJsonData")]
        private static void ClearMasterJsonData()
        {
            var target = Application.dataPath + "/Resources/MasterDataJson/master_data.txt";
            if(File.Exists(target))
            {
                File.Delete(target);
            }

            target = Application.dataPath + "/Resources/MasterDataJson/master_version.txt";

            if (File.Exists(target))
            {
                File.Delete(target);
            }
            AssetDatabase.Refresh();
            Debug.Log("Clear MasterJsonData done!!!");
        }
    }
}
