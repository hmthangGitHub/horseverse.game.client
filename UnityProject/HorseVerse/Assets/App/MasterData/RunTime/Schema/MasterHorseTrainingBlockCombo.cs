using Newtonsoft.Json;
using System;

[Serializable]
public partial class MasterHorseTrainingBlockCombo
{
	[JsonProperty]
	private long master_horse_training_block_id;
	public long MasterHorseTrainingBlockId => master_horse_training_block_id;

	[JsonProperty]
	private string name;
	public string Name => name;

	[JsonProperty]
	private string master_horse_training_block_ids;
	public string MasterHorseTrainingBlockIds => master_horse_training_block_ids;

	[JsonProperty]
	private string obstacles;
	public string Obstacles => obstacles;

	[JsonProperty]
	private string coins;
	public string Coins => coins;

}
