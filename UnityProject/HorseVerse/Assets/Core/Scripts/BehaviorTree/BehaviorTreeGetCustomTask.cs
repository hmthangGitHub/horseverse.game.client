using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Core.Behavior.UI{
    public static class BehaviorTreeGetCustomTask
    {
        public static List<Type> GetTypes(){
            List<Type> types = new List<Type>();
            types.Add(typeof(IsForcing));
            types.Add(typeof(CanMove));
            types.Add(typeof(PerformAttack));
			types.Add(typeof(MoveToTargetPosition));
            return types;
        }
    }
}

