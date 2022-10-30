using Newtonsoft.Json;
using System;

[Serializable]
public partial class MasterHorseTrainingProperty
{
	[JsonProperty]
	private long master_horse_training_property_id;
	public long MasterHorseTrainingPropertyId => master_horse_training_property_id;

	[JsonProperty]
	private float jump_velocity;
	public float JumpVelocity => jump_velocity;

	[JsonProperty]
	private float horizontal_velocity;
	public float HorizontalVelocity => horizontal_velocity;

	[JsonProperty]
	private float forward_velocity;
	public float ForwardVelocity => forward_velocity;

	[JsonProperty]
	private float fall_gravity_multiplier;
	public float FallGravityMultiplier => fall_gravity_multiplier;

	[JsonProperty]
	private float fall_air_time_min;
	public float FallAirTimeMin => fall_air_time_min;

	[JsonProperty]
	private float fall_air_time_max;
	public float FallAirTimeMax => fall_air_time_max;

	[JsonProperty]
	private float block_padding;
	public float BlockPadding => block_padding;

	[JsonProperty]
	private float block_spacing;
	public float BlockSpacing => block_spacing;

	[JsonProperty]
	private int block_numbers_min;
	public int BlockNumbersMin => block_numbers_min;

	[JsonProperty]
	private int block_numbers_max;
	public int BlockNumbersMax => block_numbers_max;

	[JsonProperty]
	private float run_camera_rotation;
	public float RunCameraRotation => run_camera_rotation;

	[JsonProperty]
	private float fall_camera_rotation;
	public float FallCameraRotation => fall_camera_rotation;

	[JsonProperty]
	private float jumping_point;
	public float JumpingPoint => jumping_point;

	[JsonProperty]
	private float landing_point;
	public float LandingPoint => landing_point;

}
