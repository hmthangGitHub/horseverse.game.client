using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Core.MVVM;

namespace Core.MVVM.UI
{
    public class ItemDragViewBase : ItemBasicView
    {

        public ItemDragViewModel ItemDrag
        {
            get
            {
                return _viewModel as ItemDragViewModel;
            }
        }

        public override ViewModel CreateViewModelWhenInitedByHierarchy()
        {
            return MVVM.CreateViewModel<ItemDragViewModel>();
        }

        protected override void Bind()
        {
            base.Bind();
        }

        #region property

        #endregion

        #region command

        #endregion
    }
}