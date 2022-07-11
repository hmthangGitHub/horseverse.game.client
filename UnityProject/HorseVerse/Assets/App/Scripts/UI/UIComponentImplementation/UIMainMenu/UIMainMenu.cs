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
        public ButtonComponent.Entity trainingBtn;
        public UIComponentMainMenuUserInfo.Entity userInfo;
        public UIComponentHorseBreedInfoAndDetail.Entity horseInfo;
    }

    public ButtonComponent breedingBtn;
    public ButtonComponent stableBtn;
    public ButtonComponent betmodeBtn;
    public ButtonComponent libraryBtn;
    public ButtonComponent playBtn;
    public ButtonComponent inventoryBtn;
    public ButtonComponent trainingBtn;
    public UIComponentMainMenuUserInfo userInfo;
    public UIComponentHorseBreedInfoAndDetail horseInfo;

    protected override void OnSetEntity()
    {
        breedingBtn.SetEntity(this.entity.breedingBtn);
        stableBtn.SetEntity(this.entity.stableBtn);
        betmodeBtn.SetEntity(this.entity.betmodeBtn);
        libraryBtn.SetEntity(this.entity.libraryBtn);
        playBtn.SetEntity(this.entity.playBtn);
        inventoryBtn.SetEntity(this.entity.inventoryBtn);
        trainingBtn.SetEntity(this.entity.trainingBtn);
        userInfo.SetEntity(this.entity.userInfo);
        horseInfo.SetEntity(this.entity.horseInfo);
    }
}
