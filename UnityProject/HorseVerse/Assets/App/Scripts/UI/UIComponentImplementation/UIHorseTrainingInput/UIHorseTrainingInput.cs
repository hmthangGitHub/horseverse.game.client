using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIHorseTrainingInput : PopupEntity<UIHorseTrainingInput.Entity>
{
	[System.Serializable]
    public class Entity
    {
        public System.Action<float, float> turning;
        public ButtonComponent.Entity jumpRight;
    }
    
    public ButtonComponent jumpRight;
    public SteeringView joystick;
    public VerticalInputComponent input;

    private void Start()
    {
        input.OnUpdateDirection += UpdateSteering;
    }

    protected override void OnSetEntity()
    {
	    jumpRight.SetEntity(this.entity.jumpRight); 
    }

    private void OnDestroy()
    {
        input.OnUpdateDirection -= UpdateSteering;
    }

    private void UpdateSteering(Vector3 point)
    {
        this.entity?.turning?.Invoke(point.x, point.y);
    }
}	