using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneEntityComponent : MonoBehaviour
{
    static SceneEntityComponent _instance = default;
    public static SceneEntityComponent Instance => _instance;

    [SerializeField] ObjectFollowing _cameraFollower;
    [SerializeField] Material skybox;

    public Material Skybox => skybox;

    private void Start()
    {
        _instance = this;
    }
    private void OnEnable()
    {
        _instance = this;
    }

    public void SetCameraTarget(Transform _target)
    {
        if(_cameraFollower != default)
        {
            _cameraFollower.target = _target;
            if (_cameraFollower.target == default)
            {
                var obj = GameObject.FindGameObjectWithTag("TrainingHorse");
                if (obj != default)
                {
                    _cameraFollower.target = obj.transform;
                }
            }
            Debug.Log("Set Target " + _cameraFollower.target.name);
        }
    }

    private void OnDestroy()
    {
        if (_instance == this) _instance = default;
    }
}
