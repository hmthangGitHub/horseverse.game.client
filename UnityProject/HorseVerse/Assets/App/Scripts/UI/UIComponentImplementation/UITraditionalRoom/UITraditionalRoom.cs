using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UITraditionalRoom : PopupEntity<UITraditionalRoom.Entity>
{
	[System.Serializable]
    public class Entity
    {
	    public UITraditionalRoomType.Entity noviceRoom;
	    public UITraditionalRoomType.Entity basicRoom;
	    public UITraditionalRoomType.Entity advanceRoom;
	    public UITraditionalRoomType.Entity expertRoom;
	    public UITraditionalRoomType.Entity masterRoom;
    }
    
    public UITraditionalRoomType noviceRoom;
    public UITraditionalRoomType basicRoom;
    public UITraditionalRoomType advanceRoom;
    public UITraditionalRoomType expertRoom;
    public UITraditionalRoomType masterRoom;

    protected override void OnSetEntity()
    {
	    noviceRoom.SetEntity(this.entity.noviceRoom);
	    basicRoom.SetEntity(this.entity.basicRoom);
	    advanceRoom.SetEntity(this.entity.advanceRoom);
	    expertRoom.SetEntity(this.entity.expertRoom);
	    masterRoom.SetEntity(this.entity.masterRoom);
    }
}	