using System;

public class BetRateModel
{
    public int First { get; set; }
    public int Second { get; set; }
    public float Rate { get; set; }
    public int TotalBet { get; set; }
    
    public BetRateModel Clone()
    {
        return (BetRateModel)this.MemberwiseClone();
    }

}