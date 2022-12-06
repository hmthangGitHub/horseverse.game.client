using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIComponentBetAmouthsContainer : UIComponent<UIComponentBetAmouthsContainer.Entity>
{
    [System.Serializable]
    public class Entity
    {
        public int totalBetAmouth;
        public ButtonComponent.Entity cancelBtn;
        public UIComponentBetAmouthList.Entity betAmounths;
    }

    public FormattedTextComponent totalBetAmouth;
    public ButtonComponent cancelBtn;
    public UIComponentBetAmouthList betAmounths;

    protected override void OnSetEntity()
    {
        totalBetAmouth.SetEntity(this.entity.totalBetAmouth);
        cancelBtn.SetEntity(this.entity.cancelBtn);
        betAmounths.SetEntity(this.entity.betAmounths);
    }

    public void SetTotalBetAmouth(int amouth)
    {
        this.entity.totalBetAmouth = amouth;
        totalBetAmouth.SetEntity(this.entity.totalBetAmouth);
    }
}	