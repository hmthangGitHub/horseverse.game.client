using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

public static class Importer
{
    public static void ImportAsPrefabVariant(this GameObject originalPrefab,
                                              Action<GameObject> variantFactory, 
                                              string destinationPrefab)
    {
        
        var original = PrefabUtility.InstantiatePrefab(originalPrefab) as GameObject;
        try
        {
            variantFactory.Invoke(original);
            PrefabUtility.SaveAsPrefabAsset(original, destinationPrefab);
        }
        finally
        {
            Object.DestroyImmediate(original);
        }
    }
    
    public static void ImportAsPrefabVariant(this string originalPrefabSource,
                                             Action<GameObject> variantFactory, 
                                             string destinationPrefab)
    {
        var objSource = AssetDatabase.LoadAssetAtPath<GameObject>(originalPrefabSource);
        objSource.ImportAsPrefabVariant(variantFactory, destinationPrefab);
    }
}
