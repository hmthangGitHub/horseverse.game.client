using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIComponentHorseStableAvatar : UIComponent<UIComponentHorseStableAvatar.Entity>
{
	[System.Serializable]
    public class Entity
    {
        public ButtonComponent.Entity selectBtn;
    }

    public ButtonComponent selectBtn;
    public GameObject backGroundContainer;
    protected override void OnSetEntity()
    {
        this.selectBtn.SetEntity(this.entity.selectBtn);
        
        var allBg = backGroundContainer.GetComponentsInChildren<Image>();
        var randomBg = allBg.RandomElement();
        allBg.ForEach(x => x.gameObject.SetActive(x == randomBg));
    }
}	