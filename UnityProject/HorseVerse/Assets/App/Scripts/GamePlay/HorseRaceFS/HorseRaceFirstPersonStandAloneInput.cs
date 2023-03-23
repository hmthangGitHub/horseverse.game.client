using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HorseRaceFirstPersonStandAloneInput : MonoBehaviour
{
    [SerializeField] private HorseRaceFirstPersonPlayerController horseRaceFirstPersonController;

    public HorseRaceFirstPersonPlayerController HorseRaceFirstPersonController
    {
        get => horseRaceFirstPersonController;
        set => horseRaceFirstPersonController = value;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            HorseRaceFirstPersonController.MoveHorizontal(-1);
        }
        if (Input.GetKeyDown(KeyCode.D))
        {
            HorseRaceFirstPersonController.MoveHorizontal(1);
        }
        if (Input.GetKeyUp(KeyCode.A))
        {
            HorseRaceFirstPersonController.MoveHorizontal(1);
        }
        if (Input.GetKeyUp(KeyCode.D))
        {
            HorseRaceFirstPersonController.MoveHorizontal(-1);
        }
        if (Input.GetKeyUp(KeyCode.LeftShift))
        {
            HorseRaceFirstPersonController.Sprint();
        }
    }
}
