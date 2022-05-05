using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SQLite4Unity3d;
using System;

namespace CoreData
{
    public abstract class MasterModel : BaseModel
    {
        public string Name { get; set; }
        public string IdOfName { get; set; }
        public string Description { get; set; }
    }
}