using Newtonsoft.Json;
using System;

[Serializable]
public partial class MasterLocalize
{
	[JsonProperty]
	private string master_localization_id;
	public string MasterLocalizationId => master_localization_id;

	[JsonProperty]
	private string EN;
	public string En => EN;

	[JsonProperty]
	private string VI;
	public string Vi => VI;

}
