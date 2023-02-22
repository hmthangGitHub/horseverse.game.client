using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BetMatchDataContext
{
    public long BetMatchId { get; set; }
    public int TotalBetWin { get; set; }
}


public class BetMatchFullDataContext
{
    public long BetMatchId { get; set; }
    public BetMatchDataDetail[] Record { get; set; }
}

public class BetMatchDataDetail
{
    public int pool_1 { get; set; }
    public int pool_2 { get; set; }
    public bool doubleBet { get; set; }
    public float rate { get; set; }
    public int betMoney { get; set; }
    public int winMoney { get; set; }
}

public class BetHistoryRecord
{
}
