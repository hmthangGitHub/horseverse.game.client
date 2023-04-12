using System;
using System.Collections;
using System.Collections.Generic;
using Lean.Touch;
using UnityEditor.UIElements;
using UnityEngine;

public class UISwipeRegister : PopupEntity<UISwipeRegister.Entity>
{
	public enum Direction
	{
		LEFT,
		RIGHT,
		UP,
		DOWN
	}
	
	[System.Serializable]
    public class Entity
    {
	    public Action<Direction> OnSwipeDirection = ActionUtility.EmptyAction<Direction>.Instance;
	    public Action<Direction> OnHorizontalDirection = ActionUtility.EmptyAction<Direction>.Instance;
	    public Action<Vector2> OnDelta = ActionUtility.EmptyAction<Vector2>.Instance;
    }

    public LeanFingerSwipe swipe;
    
    protected override void OnSetEntity()
    {
	    swipe.OnDelta.RemoveAllListeners();
	    swipe.OnDelta.AddListener(delta =>
	    {
		    this.entity.OnDelta(delta);
		    OnDirection(delta);
		    OnHorizontalDirection(delta);
	    });
	    swipe.OnDelta.AddListener(x => this.entity.OnDelta(x));
    }

    private void OnHorizontalDirection(Vector2 delta)
    {
	    if (delta.x <= 0)
	    {
		    this.entity.OnHorizontalDirection(Direction.LEFT);
	    }
	    else
	    {
		    this.entity.OnHorizontalDirection(Direction.RIGHT);
	    }
    }

    private void OnDirection(Vector2 delta)
    {
	    var angle = Mathf.Atan2(delta.y, delta.x);
	    var quarant = Mathf.PI / 4;
	    if (angle >= -quarant && angle < quarant)
	    {
		    this.entity.OnSwipeDirection(Direction.RIGHT);
	    }

	    if (angle >= quarant && angle < (3 * quarant))
	    {
		    this.entity.OnSwipeDirection(Direction.UP);
	    }

	    if (angle <= (-3 * quarant) || angle >= (3 * quarant))
	    {
		    this.entity.OnSwipeDirection(Direction.LEFT);
	    }

	    if (angle <= (-quarant) && angle >= (-3 * quarant))
	    {
		    this.entity.OnSwipeDirection(Direction.DOWN);
	    }
    }
}	