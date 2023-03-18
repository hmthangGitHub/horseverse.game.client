using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HorseRaceFirstPersonStandAloneInput : MonoBehaviour
{
    [SerializeField] private HorseRaceFirstPersonPlayerController horseRaceFirstPersonController;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            horseRaceFirstPersonController.MoveHorizontal(-1);
        }
        if (Input.GetKeyDown(KeyCode.D))
        {
            horseRaceFirstPersonController.MoveHorizontal(1);
        }
        if (Input.GetKeyUp(KeyCode.A))
        {
            horseRaceFirstPersonController.MoveHorizontal(1);
        }
        if (Input.GetKeyUp(KeyCode.D))
        {
            horseRaceFirstPersonController.MoveHorizontal(-1);
        }
        if (Input.GetKeyUp(KeyCode.LeftShift))
        {
            horseRaceFirstPersonController.Sprint();
        }
    }
}
