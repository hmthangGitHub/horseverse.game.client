using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SQLite4Unity3d;
using System;
using UniRx;

namespace CoreData
{
    public abstract class BaseModel : OriginalModel
    {
        [PrimaryKey]
        public string Id { get; set; }

        protected int _hashCode = -1;
        public override int GetHashCode()
        {
            if (_hashCode == -1)
            {
                _hashCode = Id.GetHashCode();
                if (_hashCode == -1)
                {
                    _hashCode = Guid.NewGuid().GetHashCode();
                }
            }
            return _hashCode;
        }

        #region binding
        public ReactiveCommand OnChangedCommand = new ReactiveCommand();

        public void UnbindOnChanged()
        {
            OnChangedCommand.Dispose();
        }
        #endregion
    }
}