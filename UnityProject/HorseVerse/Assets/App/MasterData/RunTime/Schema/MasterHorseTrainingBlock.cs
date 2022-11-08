using Newtonsoft.Json;
using System;

[Serializable]
public partial class MasterHorseTrainingBlock
{
	[JsonProperty]
	private long master_horse_training_block_id;
	public long MasterHorseTrainingBlockId => master_horse_training_block_id;

	[JsonProperty]
	private string name;
	public string Name => name;

	[JsonProperty]
	private MasterHorseTrainingLaneType master_horse_training_lane_type_left;
	public MasterHorseTrainingLaneType MasterHorseTrainingLaneTypeLeft => master_horse_training_lane_type_left;

	[JsonProperty]
	private int custom_value_left;
	public int CustomValueLeft => custom_value_left;

	[JsonProperty]
	private MasterHorseTrainingLaneType master_horse_training_lane_type_mid;
	public MasterHorseTrainingLaneType MasterHorseTrainingLaneTypeMid => master_horse_training_lane_type_mid;

	[JsonProperty]
	private int custom_value_mid;
	public int CustomValueMid => custom_value_mid;

	[JsonProperty]
	private MasterHorseTrainingLaneType master_horse_training_lane_type_right;
	public MasterHorseTrainingLaneType MasterHorseTrainingLaneTypeRight => master_horse_training_lane_type_right;

	[JsonProperty]
	private int custom_value_right;
	public int CustomValueRight => custom_value_right;

}
