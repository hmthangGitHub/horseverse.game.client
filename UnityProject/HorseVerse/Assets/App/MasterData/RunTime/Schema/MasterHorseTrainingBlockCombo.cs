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

	[JsonProperty]
	private MasterTrainingBlockComboType master_training_block_combo_type;
	public MasterTrainingBlockComboType MasterTrainingBlockComboType => master_training_block_combo_type;

	[JsonProperty]
	private string master_training_modular_block_id_start;
	public string MasterTrainingModularBlockIdStart => master_training_modular_block_id_start;

	[JsonProperty]
	private string master_training_modular_block_id_end;
	public string MasterTrainingModularBlockIdEnd => master_training_modular_block_id_end;

	[JsonProperty]
	private int master_horse_training_block_combo_group_id;
	public int MasterHorseTrainingBlockComboGroupId => master_horse_training_block_combo_group_id;

}
