using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using SynapseGames.AssetBundle;

public class AssetBundleBuilderEditor : MonoBehaviour
{
    [MenuItem("Assets/AssetBundle/Build Android Assets")]
    static void BuildAndroidAssetBundle()
    {
        AssetBundleBuilder.BuildAssetBundles(BuildTarget.Android, BuildAssetBundleOptions.AppendHashToAssetBundleName);
    }

    [MenuItem("Assets/AssetBundle/Build Ios Assets")]
    static void BuildIosAssetBundle()
    {
        AssetBundleBuilder.BuildAssetBundles(BuildTarget.iOS, BuildAssetBundleOptions.AppendHashToAssetBundleName);
    }

    [MenuItem("Assets/AssetBundle/Build Window Assets")]
    static void BuildWin64AssetBundle()
    {
        AssetBundleBuilder.BuildAssetBundles(BuildTarget.StandaloneWindows64, BuildAssetBundleOptions.AppendHashToAssetBundleName);
    }

    [MenuItem("Assets/AssetBundle/Build Mac Assets")]
    static void BuildOSXAssetBundle()
    {
        AssetBundleBuilder.BuildAssetBundles(BuildTarget.StandaloneOSX, BuildAssetBundleOptions.AppendHashToAssetBundleName);
    }
}
