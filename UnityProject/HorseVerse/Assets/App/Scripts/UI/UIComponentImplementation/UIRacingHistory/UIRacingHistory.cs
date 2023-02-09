using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIRacingHistory : PopupEntity<UIRacingHistory.Entity>
{
    [System.Serializable]
    public class Entity
    {
        public UIComponentHistoryRecordList.Entity historyContainer;
        public UIComponentToggle.Entity traditionalTab;
        public UIComponentToggle.Entity stableVsStableTab;
        public UIComponentToggle.Entity rankTab;
        public UIComponentToggle.Entity tournamentTab;
    }

    public UIComponentHistoryRecordList historyContainer;
    public UIComponentToggle traditionalTab;
    public UIComponentToggle stableVsStableTab;
    public UIComponentToggle rankTab;
    public UIComponentToggle tournamentTab;

    protected override void OnSetEntity()
    {
        historyContainer.SetEntity(this.entity.historyContainer);
        traditionalTab.SetEntity(this.entity.traditionalTab);
        stableVsStableTab.SetEntity(this.entity.stableVsStableTab);
        rankTab.SetEntity(this.entity.rankTab);
        tournamentTab.SetEntity(this.entity.tournamentTab);
    }
}