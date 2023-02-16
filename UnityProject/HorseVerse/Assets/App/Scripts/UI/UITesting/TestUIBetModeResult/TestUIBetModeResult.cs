using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class TestUIBetModeResult : TestUIScript<UIBetModeResult, UIBetModeResult.Entity>
{
    protected override void OnGUI()
    {
        
        base.OnGUI();
        if (GUILayout.Button("showResultPanel"))
        {
            uiTest.showResultPanel().Forget();
        }
        
        if (GUILayout.Button("showMyResultPanel"))
        {
            uiTest.showMyResultPanel().Forget();
        }
    }
}