using Newtonsoft.Json;
using System;

[Serializable]
public partial class MasterTrainingTrap
{
    [JsonProperty]
    private string master_training_trap_id;
    public string MasterTrainingTrapId => master_training_trap_id;

}
