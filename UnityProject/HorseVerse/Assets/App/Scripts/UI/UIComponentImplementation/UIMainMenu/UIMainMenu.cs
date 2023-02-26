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
        public UIComponentHorseBreedInfoAndDetail.Entity horseInfo;
        public int highScore;
        public int totalScoreBonus;
        public bool totalScoreBonusVisible;
    }

    public ButtonComponent breedingBtn;
    public ButtonComponent stableBtn;
    public ButtonComponent betmodeBtn;
    public ButtonComponent libraryBtn;
    public ButtonComponent playBtn;
    public ButtonComponent inventoryBtn;
    public ButtonComponent trainingBtn;
    public UIComponentHorseBreedInfoAndDetail horseInfo;
    public FormattedTextComponent highScore;
    public FormattedTextComponent totalScoreBonus;
    public IsVisibleComponent totalScoreBonusVisible;

    protected override void OnSetEntity()
    {
        breedingBtn.SetEntity(this.entity.breedingBtn);
        stableBtn.SetEntity(this.entity.stableBtn);
        betmodeBtn.SetEntity(this.entity.betmodeBtn);
        libraryBtn.SetEntity(this.entity.libraryBtn);
        playBtn.SetEntity(this.entity.playBtn);
        inventoryBtn.SetEntity(this.entity.inventoryBtn);
        trainingBtn.SetEntity(this.entity.trainingBtn);
        horseInfo.SetEntity(this.entity.horseInfo);
        highScore.SetEntity(this.entity.highScore);
        totalScoreBonus.SetEntity(this.entity.totalScoreBonus);
        totalScoreBonusVisible.SetEntity(this.entity.totalScoreBonusVisible);
    }
}
