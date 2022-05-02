using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HorseMasterDataContainer
{
    static string[] horseModelPath = new string[]
    {
        "Horses/Horse_Black",
        "Horses/Horse_Black_Tobiano_pinto",
        "Horses/Horse_Brown",
        "Horses/Horse_Buckskin",
        "Horses/Horse_GraysRoans",
        "Horses/Horse_Palomino",
        "Horses/Horse_palomino_overo_pinto",
        "Horses/Horse_White",
    };

    public static IReadOnlyList<string> HorseModelPaths => horseModelPath;
}
