using UnityEngine;

public interface IPredefinePath
{
    float Direction { get; }
    float EndTime { get; }
    float StartTime { get; }
    Vector3 StartPosition { get; }
    Quaternion StartRotation { get; }
    float GetClosestTime(Vector3 worldPoint);
    Quaternion GetRotation(float time);
    Vector3 GetPointAtTime(float time);
}

public abstract class PredefinePathBase : MonoBehaviour, IPredefinePath
{
    public abstract float Direction { get; }
    public abstract float EndTime { get; }
    public abstract float StartTime { get; }
    public abstract Vector3 StartPosition { get; }
    public abstract Quaternion StartRotation { get; }
    public abstract float GetClosestTime(Vector3 worldPoint);
    public abstract Quaternion GetRotation(float time);
    public abstract Vector3 GetPointAtTime(float time);
}