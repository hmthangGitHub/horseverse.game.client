using System;
using System.Collections;
using System.Collections.Generic;
using RuntimeHandle;
using UnityEngine;

public class HandleEnableController : MonoBehaviour
{
    private void OnDisable()
    {
        this.GetComponentsInChildren<HandleBase>().ForEach(x => x.gameObject.SetActive(false));
        this.GetComponentsInChildren<ParentPositionController>().ForEach(x => x.gameObject.SetActive(false));
    }
    
    private void OnEnable()
    {
        this.GetComponentsInChildren<HandleBase>(true).ForEach(x => x.gameObject.SetActive(true));
        this.GetComponentsInChildren<ParentPositionController>(true).ForEach(x => x.gameObject.SetActive(true));
    }
}
