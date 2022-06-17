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
    private string model_path;
    [JsonProperty]
    private string name;

    public long MasterHorseId => master_horse_id;
    public string ModelPath => model_path;
    public string Name => name;
}
