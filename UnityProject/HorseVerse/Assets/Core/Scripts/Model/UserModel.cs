using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SQLite4Unity3d;
using System;

namespace CoreData
{
    public abstract class UserModel : BaseModel
    {
        public string Name { get; set; }
    }
}