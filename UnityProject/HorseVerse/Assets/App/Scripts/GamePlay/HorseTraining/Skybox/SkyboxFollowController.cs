using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkyboxFollowController : MonoBehaviour
{
    [SerializeField] Transform target;

    Vector3 position = Vector3.zero;
    Quaternion quat = Quaternion.identity;
    private void Start()
    {
        quat = this.transform.rotation;
    }

    // Update is called once per frame
    void LateUpdate()
    {
        position = target.transform.position;
        this.transform.position = position;
        this.transform.rotation = quat;
    }
}
