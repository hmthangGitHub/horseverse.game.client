using Newtonsoft.Json;
using System;

[Serializable]
public partial class MasterTrainingDifficulty
{
	[JsonProperty]
	private long master_training_difficulty_id;
	public long MasterTrainingDifficultyId => master_training_difficulty_id;

	[JsonProperty]
	private int difficulty;
	public int Difficulty => difficulty;

	[JsonProperty]
	private int required_score;
	public int RequiredScore => required_score;

	[JsonProperty]
	private float forward_velocity;
	public float ForwardVelocity => forward_velocity;

}
