using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ServerDefine", menuName = "Settings/ServerDefine", order = 1)]
public class ServerDefine : ScriptableObject
{
    [System.Serializable]
    public struct Server {
        public string Host;
        public int Port;
    }

    [SerializeField] Server local;
    [SerializeField] Server dev;
    [SerializeField] Server staging;
    [SerializeField] Server production;

    public Server Local => local;
    public Server Dev => dev;
    public Server Staging => staging;
    public Server Production => production;


}
