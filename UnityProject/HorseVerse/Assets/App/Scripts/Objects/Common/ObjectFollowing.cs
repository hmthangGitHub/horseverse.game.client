using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectFollowing : MonoBehaviour
{
    [SerializeField] public Transform target;
    

    // Update is called once per frame
    void Update()
    {
        if(target != default)
        {
            UpdatePosition();
        }
    }

    void UpdatePosition()
    {
        if (target == default) return;
        this.transform.position = target.transform.position;
    }

    private void OnDestroy()
    {
        this.target = default;
    }
}
