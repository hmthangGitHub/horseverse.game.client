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
    public IsVisibleComponent leftBtnVisible;
    public ButtonComponent rightBtn;
    public IsVisibleComponent rightBtnVisible;
    public IsVisibleComponent jump;
    public IsVisibleComponent run;

    protected override void OnSetEntity()
    {
	    runBtn.SetEntity(this.entity.runBtn);
	    leftBtnVisible.SetEntity(false);
	    rightBtnVisible.SetEntity(true);
	    leftBtn.SetEntity(OnLeftBtn);
	    rightBtn.SetEntity(OnRightBtn);
    }

    private void OnRightBtn()
    {
	    jump.SetEntity(true);
	    run.SetEntity(false);
	    leftBtnVisible.SetEntity(true);
	    rightBtnVisible.SetEntity(false);
	    In().Forget();
    }

    private void OnLeftBtn()
    {
	    jump.SetEntity(false);
	    run.SetEntity(true);
	    leftBtnVisible.SetEntity(false);
	    rightBtnVisible.SetEntity(true);
	    In().Forget();
    }
}	