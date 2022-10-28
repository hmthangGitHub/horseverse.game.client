using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UIDebugMasterColumn : UIComponent<UIDebugMasterColumn.Entity>
{
	[System.Serializable]
    public class Entity
    {
	    public bool isHeader;
	    public string value;
    }

    public TMP_InputField inputField;
    
    protected override void OnSetEntity()
    {
	    inputField.interactable = !this.entity.isHeader;
	    inputField.text = this.entity.value;
	    
	    inputField.onValueChanged.RemoveAllListeners();
	    inputField.onValueChanged.AddListener(value => this.entity.value = value);
    }
}	