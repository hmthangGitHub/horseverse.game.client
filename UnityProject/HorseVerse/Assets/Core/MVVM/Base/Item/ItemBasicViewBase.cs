using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Core.MVVM;

namespace Core.MVVM.UI
{
    public class ItemBasicViewBase : ViewBase
    {

        public ItemBasicViewModel ItemBasic
        {
            get
            {
                return _viewModel as ItemBasicViewModel;
            }
        }

        public override ViewModel CreateViewModelWhenInitedByHierarchy()
        {
            return MVVM.CreateViewModel<ItemBasicViewModel>();
        }

        protected override void Bind()
        {
            base.Bind();
            BindCommand(ItemBasic.SelectCommand, SelectExecuted);
            BindCommand(ItemBasic.UnSelectCommand, UnSelectExecuted);
            BindCommand(ItemBasic.SetDragLogicForItemCommand, SetDragLogicForItemExecuted);
        }

        #region property
        #endregion

        #region command
        protected virtual void SelectExecuted()
        {

        }
        protected virtual void UnSelectExecuted()
        {

        }
        protected virtual void SetDragLogicForItemExecuted(SetDragLogicForItemCommand command)
        {

        }
        #endregion
    }
}
