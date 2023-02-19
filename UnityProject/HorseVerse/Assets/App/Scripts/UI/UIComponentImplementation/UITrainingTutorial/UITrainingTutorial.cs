using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Rendering;

public class UITrainingTutorial : PopupEntity<UITrainingTutorial.Entity>
{
	[System.Serializable]
    public class Entity
    {
	    public ButtonComponent.Entity runBtn;
    }

    public ButtonComponent runBtn;
    public ButtonComponent leftBtn;
    public ButtonComponent rightBtn;
    public IsVisibleComponent jump;
    public IsVisibleComponent run;

    protected override void OnSetEntity()
    {
	    runBtn.SetEntity(this.entity.runBtn);
	    leftBtn.SetEntity(OnLeftBtn);
	    rightBtn.SetEntity(OnRightBtn);
	    run.SetEntity(true);
	    jump.SetEntity(false);
    }

    private void OnRightBtn()
    {
	    jump.SetEntity(true);
	    run.SetEntity(false);
	    In().Forget();
    }

    private void OnLeftBtn()
    {
	    jump.SetEntity(false);
	    run.SetEntity(true);
	    In().Forget();
    }
}	