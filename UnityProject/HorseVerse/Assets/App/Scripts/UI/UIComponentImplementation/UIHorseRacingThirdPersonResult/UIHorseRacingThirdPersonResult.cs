using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;

public class UIHorseRacingThirdPersonResult : PopupEntity<UIHorseRacingThirdPersonResult.Entity>
{
	[System.Serializable]
    public class Entity
    {
	    public int position;
	    public int raceTime;
	    public ButtonComponent.Entity outerBtn;
	    public UIComponentRealmIntro.Entity realm;
        public float timeOut;
    }

    public UIComponentOrdinalNumber position;
    public UIComponentTimeSpan raceTime;
    public ButtonComponent outerBtn;
    public UIComponentRealmIntro realm;
    
    protected override void OnSetEntity()
    {
	    position.SetEntity(this.entity.position);
	    raceTime.SetEntity(this.entity.raceTime);
	    outerBtn.SetEntity(this.entity.outerBtn);
	    realm.SetEntity(this.entity.realm);
        if (this.entity.timeOut > 0) StartCoroutine(TimeOut());
    }

    private void OnEnable()
    {
        if (this.entity != default && this.entity.timeOut > 0) StartCoroutine(TimeOut());
    }

    private IEnumerator TimeOut()
    {
        yield return new WaitForSeconds(this.entity.timeOut);
        this.entity.outerBtn?.onClickEvent.Invoke();
    }
}	