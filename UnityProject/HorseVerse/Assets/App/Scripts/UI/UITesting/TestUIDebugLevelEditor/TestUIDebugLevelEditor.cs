using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestUIDebugLevelEditor : TestUIScript<UIDebugLevelEditor, UIDebugLevelEditor.Entity>
{
    public UIDebugLevelEditorPopUp.Entity popUpEntity;
    protected override void OnGUI()
    {
        base.OnGUI();                                                                                                                                            
        if (GUILayout.Button("SetPopup"))
        {
            popUpEntity.closeBtn.onClickEvent.AddListener(() => uiTest.isPopUpVisible.SetEntity(false));
            uiTest.isPopUpVisible.SetEntity(true);
            uiTest.SetPopUpEntity(popUpEntity);
        }
    }
}