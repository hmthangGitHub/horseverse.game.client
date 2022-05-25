using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIMainMenu : PopupEntity<UIMainMenu.Entity>
{
    [Serializable]
    public class Entity
    {
        public ButtonComponent.Entity breedingBtn;
        public ButtonComponent.Entity stableBtn;
        public ButtonComponent.Entity betmodeBtn;
        public ButtonComponent.Entity libraryBtn;
        public ButtonComponent.Entity playBtn;
        public ButtonComponent.Entity inventoryBtn;
    }

    public ButtonComponent breedingBtn;
    public ButtonComponent stableBtn;
    public ButtonComponent betmodeBtn;
    public ButtonComponent libraryBtn;
    public ButtonComponent playBtn;
    public ButtonComponent inventoryBtn;

    protected override void OnSetEntity()
    {
        breedingBtn.SetEntity(this.entity.breedingBtn);
        stableBtn.SetEntity(this.entity.stableBtn);
        betmodeBtn.SetEntity(this.entity.betmodeBtn);
        libraryBtn.SetEntity(this.entity.libraryBtn);
        playBtn.SetEntity(this.entity.playBtn);
        inventoryBtn.SetEntity(this.entity.inventoryBtn);
    }
}
