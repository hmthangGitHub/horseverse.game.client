using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Core.MVVM;
using UniRx;

namespace Core.MVVM.UI
{
    public class ItemDragViewModelBase : ItemBasicViewModel
    {
        public ReactiveCommand<Vector3> OnDropCommand;

        public override void InitViewModel()
        {
            base.InitViewModel();
            OnDropCommand = new ReactiveCommand<Vector3>();
        }

        #region command

        #endregion

        public override void Dispose()
        {
            base.Dispose();

            OnDropCommand.Dispose();
        }
    }
}
