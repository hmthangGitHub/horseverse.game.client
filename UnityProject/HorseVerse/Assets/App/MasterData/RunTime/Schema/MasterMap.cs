using Newtonsoft.Json;
using System;

[Serializable]
public partial class MasterMap
{
	[JsonProperty]
	private long master_map_id;
	public long MasterMapId => master_map_id;

	[JsonProperty]
	private string map_path;
	public string MapPath => map_path;

	[JsonProperty]
	private string map_name;
	public string MapName => map_name;

	[JsonProperty]
	private string map_settings;
	public string MapSettings => map_settings;

	[JsonProperty]
	private long master_map_phase_setting_group_id;
	public long MasterMapPhaseSettingGroupId => master_map_phase_setting_group_id;

}
