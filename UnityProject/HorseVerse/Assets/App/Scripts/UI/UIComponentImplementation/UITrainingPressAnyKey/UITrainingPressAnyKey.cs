using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Google.Protobuf.WellKnownTypes;
using UnityEngine;

public class UITrainingPressAnyKey : PopupEntity<UITrainingPressAnyKey.Entity>
{
	[System.Serializable]
    public class Entity
    {
	    public Action onInput;
    }

    protected override void OnSetEntity()
    {
    }

    private void Update()
    {
	    if (Input.GetMouseButtonUp(0))
	    {
		    this.entity.onInput?.Invoke();
	    }
    }
}	