using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIClientInfo : PopupEntity<UIClientInfo.Entity>
{
	[System.Serializable]
    public class Entity
    {
	    public string clientVersion;
    }
    
    public FormattedTextComponent clientVersion;

    protected override void OnSetEntity()
    {
	    clientVersion.SetEntity(this.entity.clientVersion);
    }
}	