public class UserDataModel
{
    public long UserId { get; set; }
    public long Coin { get; set; }
    public string UserName { get; set; }
    public int AccountType { get; set; }
    public int MaxEnergy { get; set; }
    public int Energy { get; set; }
    public long CurrentHorseNftId { get; set; }
    public long TraningTimeStamp { get; set; }
    public int Level { get; set; }
    public int Exp { get; set; }
    public int NextLevelExp { get; set; }
    public int DailyRacingNumberLeft { get; set; }


    public UserDataModel Clone()
    {
        return (UserDataModel)this.MemberwiseClone();
    }
}