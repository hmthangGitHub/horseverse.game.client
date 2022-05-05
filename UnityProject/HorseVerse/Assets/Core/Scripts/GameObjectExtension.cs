using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class GameObjectExtension
{
    public static GameObject FindGameObjectByName(this GameObject source, string name, bool includeInactive = false)
    {
        var transforms = source.GetComponentsInChildren<Transform>(includeInactive);
        for (int i = 0; i < transforms.Length; i++)
        {
            var transform = transforms[i];
            if (transform.name == name)
            {
                return transform.gameObject;
            }
        }
        return null;
    }
}
