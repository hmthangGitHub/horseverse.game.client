using Newtonsoft.Json;
using System;

[Serializable]
public partial class MasterTrainingBlockDistribute
{
	[JsonProperty]
	private long master_training_block_distribute_id;
	public long MasterTrainingBlockDistributeId => master_training_block_distribute_id;

	[JsonProperty]
	private int difficulty;
	public int Difficulty => difficulty;

	[JsonProperty]
	private int master_horse_training_block_group_id;
	public int MasterHorseTrainingBlockGroupId => master_horse_training_block_group_id;

	[JsonProperty]
	private int weight;
	public int Weight => weight;

}
