using Newtonsoft.Json;
using System;

[Serializable]
public partial class MasterHorse
{
	[JsonProperty]
	private long master_horse_id;
	public long MasterHorseId => master_horse_id;

	[JsonProperty]
	private string model_path;
	public string ModelPath => model_path;

	[JsonProperty]
	private string name;
	public string Name => name;

}
