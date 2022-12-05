using UniRx.Operators;
using UnityEngine;

public static class MasterHorseContainerExtensions
{
    public static HorseMeshInformation GetHorseMeshInformation(this MasterHorseContainer masterHorseContainer,
                                                               MasterHorseMeshInformation masterHorseMeshInformation,
                                                               HorseModelMode horseModelMode)
    {
        return new HorseMeshInformation()
        {
            color1 = masterHorseMeshInformation.color1,
            color2 = masterHorseMeshInformation.color2,
            color3 = masterHorseMeshInformation.color3,
            color4 = masterHorseMeshInformation.color4,
            horseModelPath = GetModelPathFromHorseModelMode(
                masterHorseContainer.MasterHorseIndexer[masterHorseMeshInformation.masterHorseId], horseModelMode)
        };
    }

    private static string GetModelPathFromHorseModelMode(MasterHorse masterHorse, HorseModelMode horseModelMode)
    {
        return horseModelMode switch

        {
            HorseModelMode.Menu => masterHorse.ModelPath,
            HorseModelMode.Race => masterHorse.RaceModeModelPath,
            HorseModelMode.Intro => masterHorse.IntroRaceModeModelPath,
            _ => masterHorse.ModelPath
        };
    }
}

public enum HorseModelMode
{
    Menu,
    Intro,
    Race
}

public class MasterHorseMeshInformation
{
    public long masterHorseId;
    public Color color1;
    public Color color2;
    public Color color3;
    public Color color4;
}