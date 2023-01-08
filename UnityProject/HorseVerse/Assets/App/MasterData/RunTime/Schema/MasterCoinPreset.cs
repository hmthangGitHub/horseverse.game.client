using Newtonsoft.Json;
using System;

[Serializable]
public partial class MasterCoinPreset
{
	[JsonProperty]
	private string master_coin_preset_id;
	public string MasterCoinPresetId => master_coin_preset_id;

	[JsonProperty]
	private string coin;
	public string Coin => coin;

}
