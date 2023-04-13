using System.Collections;
using System.Collections.Generic;
using CW.Common;
using Lean.Touch;
using UnityEngine;

public class HorseRotator : MonoBehaviour
{
    public LeanFingerFilter fingerFilter = new LeanFingerFilter(true);

    public Camera Camera
    {
        set => camera = value;
        get => camera;
    }

    [SerializeField] private Camera camera;

    public float Sensitivity
    {
        set => sensitivity = value;
        get => sensitivity;
    }

    [SerializeField] private float sensitivity = 1.0f;

    public float Damping
    {
        set => damping = value;
        get => damping;
    }

    [SerializeField] private float damping = -1.0f;

    public float Inertia
    {
        set => inertia = value;
        get => inertia;
    }

    [SerializeField] [Range(0.0f, 1.0f)] private float inertia;

    [SerializeField] private Vector3 remainingTranslation;
    private Quaternion transformRotation;
    private float targetRotation;
    private float currentUpRotation;

    protected void Start()
    {
        currentUpRotation = this.transform.rotation.eulerAngles.y;
        targetRotation = currentUpRotation;
    }

    protected void Update()
    {
        var fingers = fingerFilter.UpdateAndGetFingers();
        var screenDelta = LeanGesture.GetScreenDelta(fingers);
        if (screenDelta != Vector2.zero)
        {
            targetRotation = currentUpRotation + -screenDelta.x * Sensitivity;
        }

        currentUpRotation = Mathf.Lerp(currentUpRotation, targetRotation, Time.deltaTime * Damping);
        transform.rotation = Quaternion.Euler(0, currentUpRotation, 0);
    }
}