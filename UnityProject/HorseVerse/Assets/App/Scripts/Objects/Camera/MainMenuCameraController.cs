using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuCameraController : MonoBehaviour
{
    public static MainMenuCameraController Instance = default;
    [System.Serializable]
    public class CameraPosition
    {
        public Vector3 position;
        public Vector3 euler;
        public float FOV;
    }

    [SerializeField] public Camera mainCamera;
    [SerializeField] public List<CameraPosition> positions;

    private void Awake()
    {
        if (Instance != default && Instance != this) Destroy(Instance.gameObject);
        Instance = this;
    }

    private void OnDestroy()
    {
        Instance = default;
    }

    public void SetPosition(int index)
    {
        if (index >= positions.Count) return;
        var pos = positions[index];
        mainCamera.transform.position = pos.position;
        mainCamera.transform.eulerAngles = pos.euler;
        mainCamera.fieldOfView = pos.FOV;
    }
}
