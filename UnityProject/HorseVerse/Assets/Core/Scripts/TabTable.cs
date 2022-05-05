using System;
using UnityEngine;
using UnityEngine.UI;

namespace Core{
    [Serializable]
    public class TabTable
    {
        public GameObject Container;
        public Button Button;
        [NonSerialized]
        public int Index;
    }
}