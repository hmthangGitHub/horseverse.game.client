using Newtonsoft.Json;
using System;

[Serializable]
public partial class MasterTrainingModularBlock
{
	[JsonProperty]
	private string master_training_modular_block_id;
	public string MasterTrainingModularBlockId => master_training_modular_block_id;

	[JsonProperty]
	private MasterTrainingModularBlockType master_training_modular_block_type;
	public MasterTrainingModularBlockType MasterTrainingModularBlockType => master_training_modular_block_type;

}
