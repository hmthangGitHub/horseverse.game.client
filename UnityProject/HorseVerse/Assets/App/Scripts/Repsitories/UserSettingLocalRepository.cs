using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UserSettingLocalRepository
{
    private static string IsSkipConfirmBetKey => $"{Application.productName}_IsSkipConfirmBet";
    // public static bool IsSkipConfirmBet 
    // { 
    //     get => PlayerPrefs.GetInt(IsSkipConfirmBetKey, 0) == 1;
    //     set => PlayerPrefs.SetInt(IsSkipConfirmBetKey, value == true ? 1 : 0);
    // }
    
    public static bool IsSkipConfirmBet { get; set; } // TODO temporary

    public static MasterDataModel MasterDataModel { get; set; } = new MasterDataModel();
}
