using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIComponentBetSlotNumber : UIComponent<UIComponentBetSlotNumber.Entity>
{
    [System.Serializable]
    public class Entity
    {
        public int Number;
    }

    public FormattedTextComponent Number;
    public Image Background; 
    public List<Sprite> BackgroundSprites;

    protected override void OnSetEntity()
    {
        Number.SetEntity(this.entity.Number);
        var index = (this.entity.Number - 1) < BackgroundSprites.Count ? (this.entity.Number - 1) : 0;
        var sprite = BackgroundSprites[index];
        Background.sprite = sprite;
    }
}
