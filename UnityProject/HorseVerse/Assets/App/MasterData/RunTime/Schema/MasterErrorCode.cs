using Newtonsoft.Json;
using System;

[Serializable]
public partial class MasterErrorCode
{
	[JsonProperty]
	private int master_error_code_id;
	public int MasterErrorCodeId => master_error_code_id;

	[JsonProperty]
	private string description_key;
	public string DescriptionKey => description_key;

	[JsonProperty]
	private bool handle;
	public bool Handle => handle;

	[JsonProperty]
	private bool sucess;
	public bool Sucess => sucess;

}
