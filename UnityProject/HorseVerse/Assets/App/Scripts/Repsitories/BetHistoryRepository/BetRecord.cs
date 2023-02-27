using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BetRecord
{
    public long MatchId { get; set; }
    public long TimeStamp { get; set; }
    public int FirstHorseIndex { get; set; }
    public string FirstHorseName { get; set; }
    public int SecondHorseIndex { get; set; }
    public string SecondHorseName { get; set; }
}
