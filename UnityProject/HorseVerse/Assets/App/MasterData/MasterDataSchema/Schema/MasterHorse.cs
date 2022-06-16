using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class MasterHorse
{
    [JsonProperty]
    private long master_horse_id;
    [JsonProperty]
    public string master_horse_model_path;
    public long MasterHorseId => master_horse_id;
    public string ModelPath => master_horse_model_path;
}
