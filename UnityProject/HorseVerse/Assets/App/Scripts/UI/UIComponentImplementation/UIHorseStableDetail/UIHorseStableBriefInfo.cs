using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIHorseStableBriefInfo : UIComponent<UIHorseStableBriefInfo.Entity>
{
	[System.Serializable]
    public class Entity
    {
	    public string horseName;
	    public ButtonComponent.Entity leftBtn;
	    public ButtonComponent.Entity rightBtn;
	    public UIHorseSexInfo.Sex sex;
	    public int age;
	    public UIComponentHorseElement.Element element;
	    public UIComponentHorseRankRequirement.Rarity rarity;
	    public bool showChangeHorseBtn;
    }

    public FormattedTextComponent horseName;
    public ButtonComponent leftBtn;
    public ButtonComponent rightBtn;
    public UIHorseSexInfo sex;
    public UIComponentEnumInt age;
    public UIComponentEnumInt femaleAge;
    public UIComponentHorseElement element;
    public UIComponentHorseRankRequirement rarity;
    public IsVisibleComponent showChangeHorseLeft;
    public IsVisibleComponent showChangeHorseRight;

    
    protected override void OnSetEntity()
    {
	    horseName.SetEntity(this.entity.horseName);
	    leftBtn.SetEntity(this.entity.leftBtn);
	    rightBtn.SetEntity(this.entity.rightBtn);
	    sex.SetEntity(this.entity.sex);
	    age.SetEntity(this.entity.age);
	    femaleAge.SetEntity(this.entity.age);
	    element.SetEntity(this.entity.element);
	    rarity.SetEntity(this.entity.rarity);
	    showChangeHorseLeft.SetEntity(this.entity.showChangeHorseBtn);
	    showChangeHorseRight.SetEntity(this.entity.showChangeHorseBtn);
    }
}	