using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using Core.MVVM;
using Core.MVVM.UI;

namespace Core.Behavior.UI{

    public class InitTaskCommand : ICommand {
        
    }

    public class TaskViewModelBase : ItemDragViewModel
    {
        public ReactiveProperty<bool> IsValidate;
        public ReactiveCommand<InitTaskCommand> InitTaskCommand;

		public override void InitViewModel()
		{
            base.InitViewModel();
            IsValidate = new ReactiveProperty<bool>();
            InitTaskCommand = new ReactiveCommand<InitTaskCommand>();
		}

        public virtual void InitTask(InitTaskCommand command){
            
        }

		public override void Dispose()
		{
            base.Dispose();
            IsValidate.Dispose();
            InitTaskCommand.Dispose();
		}
	}
}
