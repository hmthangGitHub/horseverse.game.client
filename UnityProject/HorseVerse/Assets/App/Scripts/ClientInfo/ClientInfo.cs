using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "ClientVersion", menuName = "HVerse/ClientVersion", order = 1)]
public class ClientInfo : ScriptableObject
{
    public enum Enviroment
    {
        Development,
        Staging,
        Production
    }

    public enum AssetBuildMode
    {
        Streaming,
        Remote
    }

    [Serializable]
    public class EnviromentInfo
    {
        public Enviroment Enviroment;
        public string Version;
        public string AssetVersion;
        public AssetBuildMode LastBuildInfo;
    }

    [SerializeField]
    private Enviroment currentEnviroment;
    [SerializeField]
    private List<EnviromentInfo> EnviromentInfos;

    public Enviroment CurrentEnviroment { get => currentEnviroment; set => currentEnviroment = value; }
#if !UNITY_EDITOR
    public string Version => EnviromentInfos.FirstOrDefault(x => x.Enviroment == CurrentEnviroment).Version;
    public string AssetVersion => EnviromentInfos.FirstOrDefault(x => x.Enviroment == CurrentEnviroment).AssetVersion;
#else
    public EnviromentInfo this[Enviroment enviroment] => EnviromentInfos.FirstOrDefault(x => x.Enviroment == enviroment);
    
    public string Version 
    {   get => this[CurrentEnviroment].Version;
        set => this[CurrentEnviroment].Version = value;
    }

    public string AssetVersion
    {
        get => this[CurrentEnviroment].AssetVersion;
        set => this[CurrentEnviroment].AssetVersion = value;
    }

    public AssetBuildMode LastAssetBuildMode
    {
        get => this[CurrentEnviroment].LastBuildInfo;
        set => this[CurrentEnviroment].LastBuildInfo = value;
    }
#endif
}
