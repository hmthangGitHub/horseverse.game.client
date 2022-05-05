using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Core.MVVM;
using UniRx;
using System;

namespace Core.MVVM.UI
{
    public class SetDragLogicForItemCommand : ICommand
    {
        public RectTransform UI { get; set; }
        public List<RectTransform> List { get; set; }
        public bool IsDrag { get; set; }
        public bool HasClone { get; set; }
        public Vector2 Size { get; set; }
        public Action<RectTransform, object> Callback { get; set; }
        public Action CallbackNoContainInList { get; set; }
        public float TimeToCheckDrag { get; set; }

        public SetDragLogicForItemCommand()
        {
            List = new List<RectTransform>();
            HasClone = true;
            IsDrag = true;
            TimeToCheckDrag = 0.5f;
        }
    }

    public class ItemBasicViewModelBase : ViewModel
    {
        public ReactiveCommand OnClickCommand;
        public ReactiveCommand SelectCommand;
        public ReactiveCommand UnSelectCommand;
        public ReactiveCommand<SetDragLogicForItemCommand> SetDragLogicForItemCommand;

        public override void InitViewModel()
        {
            base.InitViewModel();
            OnClickCommand = new ReactiveCommand();
            SelectCommand = new ReactiveCommand();
            UnSelectCommand = new ReactiveCommand();
            SetDragLogicForItemCommand = new ReactiveCommand<SetDragLogicForItemCommand>();
        }

        #region command

        #endregion

        public override void Dispose()
        {
            base.Dispose();

            OnClickCommand.Dispose();
            SelectCommand.Dispose();
            UnSelectCommand.Dispose();
            SetDragLogicForItemCommand.Dispose();
        }
    }
}

