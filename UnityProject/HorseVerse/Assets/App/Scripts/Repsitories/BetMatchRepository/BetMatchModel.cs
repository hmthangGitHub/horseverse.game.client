using System.Collections;
using System.Collections.Generic;
using io.hverse.game.protogen;
using UnityEngine;

public class BetMatchModel
{
    public long BetMatchId { get; set; }
    public long BetMatchTimeStamp { get; set; }
    public MatchStatus MatchStatus { get; set; }
    public long TimeToNextMatch { get; set; }
    public RaceScript RaceScript { get; set; }
}
