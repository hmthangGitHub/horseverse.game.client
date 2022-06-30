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

}
