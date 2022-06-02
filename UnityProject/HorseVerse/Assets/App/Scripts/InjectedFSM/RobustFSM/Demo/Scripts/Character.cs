using System.Collections.Generic;
using UnityEngine;

namespace Assets.SimpleFSM.Demo.Scripts
{
    [RequireComponent(typeof(CharacterFSM))]
    public  class Character : MonoBehaviour
    {
        public int Speed = 3;
        public List<Transform> PatrolPoints = new List<Transform>();

        public CharacterFSM FSM { get; set; }
        public Transform Target { get; set; }

        private void Start()
        {
            FSM = GetComponent<CharacterFSM>();
            FSM.Owner = this;
        }
    }
}
