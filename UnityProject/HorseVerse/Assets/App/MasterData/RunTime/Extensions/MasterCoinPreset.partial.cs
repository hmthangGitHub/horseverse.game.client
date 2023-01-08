using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;

public partial class MasterCoinPreset
{
#if ENABLE_MASTER_RUN_TIME_EDIT
    public static MasterCoinPreset Instantiate(string masterCoinPresetId, Coin coin)
    {
        return new MasterCoinPreset()
        {
            master_coin_preset_id = masterCoinPresetId,
            coin = JsonConvert.SerializeObject(coin).Replace(",", "...")
        };
    }
#endif
    public Coin CoinObject => JsonConvert.DeserializeObject<Coin>(MasterHorseTrainingBlockCombo.FormatCustomData(Coin));
}
