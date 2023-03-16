using System.Collections;
using System.Collections.Generic;
using Lean.Touch;
using UnityEngine;

public class HorseRaceFirstPersonInput : MonoBehaviour
{
    [SerializeField] private LeanFingerUpdate touchUpdate;
    [SerializeField] private LeanFingerUp touchUp;
    [SerializeField] private HorseRaceFirstPersonPlayerController horseRaceFirstPersonController;

    void Start()
    {
        touchUp.OnFinger.AddListener(HandleCancelTouch);
        touchUpdate.OnFinger.AddListener(HandleTouch);
    }

    private void HandleCancelTouch(LeanFinger finger)
    {
        var halfScreenWidth = Screen.width / 2;

        if (finger.ScreenPosition.x < halfScreenWidth)
        {
            horseRaceFirstPersonController.MoveHorizontal(1);
        }
        else if (finger.ScreenPosition.x > halfScreenWidth)
        {
            horseRaceFirstPersonController.MoveHorizontal(-1);
        }
    }

    private void HandleTouch(LeanFinger finger)
    {
        var halfScreenWidth = Screen.width / 2;

        if (finger.ScreenPosition.x < halfScreenWidth)
        {
            horseRaceFirstPersonController.MoveHorizontal(-1);
        }
        else if (finger.ScreenPosition.x > halfScreenWidth)
        {
            horseRaceFirstPersonController.MoveHorizontal(1);
        }
    }
}
