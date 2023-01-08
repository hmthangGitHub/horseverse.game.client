using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class ParentPositionController : MonoBehaviour
{
    public Transform parentTransform;
    public TMP_InputField x;
    public TMP_InputField y;
    public TMP_InputField z;

    
    private void Start()
    {
        parentTransform = this.transform.parent;
        x.onEndEdit.AddListener(val => SetTransformProperty(parentTransform.SetX, val));
        y.onEndEdit.AddListener(val => SetTransformProperty(parentTransform.SetY, val));
        z.onEndEdit.AddListener(val => SetTransformProperty(parentTransform.SetZ, val));
        
        x.onTextSelection.AddListener((_,__,___) => this.enabled = false);
        y.onTextSelection.AddListener((_,__,___) => this.enabled = false);
        z.onTextSelection.AddListener((_,__,___) => this.enabled = false);
    }

    private void SetTransformProperty(Action<float> setter, string stringValue)
    {
        if (float.TryParse(stringValue, out var val))
        {
            setter(val);
        }

        this.enabled = true;
    }

    private void Update()
    {
        x.text = parentTransform.position.x.ToString();
        y.text = parentTransform.position.y.ToString();
        z.text = parentTransform.position.z.ToString();
    }
}
