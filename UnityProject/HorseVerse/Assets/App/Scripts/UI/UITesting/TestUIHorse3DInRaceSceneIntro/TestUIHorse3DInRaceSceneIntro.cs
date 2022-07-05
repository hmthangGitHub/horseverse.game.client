using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestUIHorse3DInRaceSceneIntro : TestUIScript<UIHorse3DInRaceSceneIntro, UIHorse3DInRaceSceneIntro.Entity>
{
    public string[] modelPaths;
    private int currentIndex = 0;

    protected override void OnGUI()
    {
        base.OnGUI();
        if (GUILayout.Button("Next"))
        { 
            currentIndex++;
            currentIndex %=modelPaths.Length;
            entity.horseModelLoader.horse = modelPaths[currentIndex];
            uiTest.horseModelLoader.SetEntity(entity.horseModelLoader);
        }
    }
}