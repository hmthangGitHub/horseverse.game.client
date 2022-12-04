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
}
