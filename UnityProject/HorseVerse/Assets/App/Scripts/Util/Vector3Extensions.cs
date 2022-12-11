using System;
using System.Collections.Generic;
using UnityEngine;

public static class Vector3Extensions
{
    public static Vector2 XZ(this Vector3 v)
    {
        return new Vector2(v.x, v.z);
    }

    public static Vector3 X0Z(this Vector3 v)
    {
        return new Vector3(v.x,0, v.z);
    }
    
    public static Vector3 RandomPointInBounds(this Bounds bounds) {
        return new Vector3(
            UnityEngine.Random.Range(bounds.min.x, bounds.max.x),
            UnityEngine.Random.Range(bounds.min.y, bounds.max.y),
            UnityEngine.Random.Range(bounds.min.z, bounds.max.z)
        );
    }

    public static float Random(this Vector2 range)
    {
        return UnityEngine.Random.Range(range.x, range.y);
    }
    
    public static float Map(float s, float a1, float a2, float b1, float b2)
    {
        if (Math.Abs(a2 - a1) < 0.0001f)
        {
            return (b1 + b2) * 0.5f;
        }
        else
        {
            return b1 + (s-a1)*(b2-b1)/(a2-a1);    
        }
    }
    
    public static IEnumerable<TSource> DistinctBy<TSource, TKey>
            (this IEnumerable<TSource> source, Func<TSource, TKey> keySelector)
        {
            HashSet<TKey> seenKeys = new HashSet<TKey>();
            foreach (TSource element in source)
            {
                if (seenKeys.Add(keySelector(element)))
                {
                    yield return element;
                }
            }
        }
}
