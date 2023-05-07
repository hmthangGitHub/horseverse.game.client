using System.Collections;
using System.Collections.Generic;
using System.Linq;
using io.hverse.game.protogen;
using UnityEngine;

public class HorseRaceInfoFactory
{
    private readonly IDIContainer container;
    private MasterHorseContainer masterHorseContainer;
    private MasterHorseContainer MasterHorseContainer => masterHorseContainer ??= container.Inject<MasterHorseContainer>();

    public HorseRaceInfoFactory(IDIContainer container)
    {
        this.container = container;
    }

    public HorseRaceInfo[] GetHorseRaceInfos(RaceScript responseRaceScript)
    {
        return responseRaceScript.Phases.SelectMany(x =>
                x.HorseStats.Select((stat, i) => (stat: stat, horseId: stat.HorseId, start: x.Start, end: x.End)))
            .GroupBy(x => x.horseId)
            .Select(x =>
            {
                var horseInfo = responseRaceScript.HorseInfos.First(info => info.NftId == x.Key);
                var masterHorse = MasterHorseContainer.FromColorToMasterHorse(horseInfo.ColorType);
                return new HorseRaceInfo()
                {
                    DelayTime = x.First().stat.DelayTime,
                    RaceSegments = x.Select(info => new RaceSegmentTime()
                                    {
                                        CurrentLane = info.stat.LaneStart,
                                        ToLane = info.stat.LaneEnd,
                                        Time = info.stat.Time,
                                        Percentage = (float)(info.end) / responseRaceScript.TotalLength
                                    })
                                    .ToArray(),
                    MeshInformation = new MasterHorseMeshInformation()
                    {
                        masterHorseId = masterHorse.MasterHorseId,
                        //color1 = HorseRepository.GetColorFromHexCode(horseInfo.Color1),
                        //color2 = HorseRepository.GetColorFromHexCode(horseInfo.Color2),
                        //color3 = HorseRepository.GetColorFromHexCode(horseInfo.Color3),
                        //color4 = HorseRepository.GetColorFromHexCode(horseInfo.Color4),
                    },
                    Name = horseInfo.Name,
                    NftHorseId = horseInfo.NftId,
                    PowerBonus = horseInfo.Bms,
                    PowerRatio = 1,
                    SpeedBonus = horseInfo.Mms,
                    SpeedRatio = 1,
                    TechnicallyBonus = horseInfo.Acceleration,
                    TechnicallyRatio = 1,
                    Rarity = horseInfo.Rarity,
                    Type = horseInfo.HorseType,
                    Level = horseInfo.Level,
                };
            })
            .OrderBy(x => x.RaceSegments.First().CurrentLane)
            .ToArray();
    }
}
