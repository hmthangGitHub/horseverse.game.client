using System.Collections;
using System.Collections.Generic;
using Core.MVVM.UI;
using Core.MVVM;
using UnityEngine;

namespace Core.Behavior.UI
{
    public class TaskViewBase : ItemDragView
    {
        public TaskViewModel Task{
            get{
                return (TaskViewModel)_viewModel;
            }
        }

        public override ViewModel CreateViewModelWhenInitedByHierarchy()
        {
            return MVVM.MVVM.CreateViewModel<TaskViewModel>();
        }

		protected override void Bind()
		{
            base.Bind();
            BindProperty(Task.IsValidate, IsValidateChanged);
            BindCommand(Task.InitTaskCommand, InitTaskExecuted);
		}

        public virtual void IsValidateChanged(bool arg){
            
        }

        public virtual void InitTaskExecuted(InitTaskCommand command){
            
        }
	}
}
