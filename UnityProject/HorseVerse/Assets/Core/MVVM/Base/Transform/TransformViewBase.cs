using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Core.MVVM;

namespace Core.MVVM
{
    public class TransformViewBase : ViewBase
    {

        public TransformViewModel Transform
        {
            get
            {
                return _viewModel as TransformViewModel;
            }
        }

        public override ViewModel CreateViewModelWhenInitedByHierarchy()
        {
            return MVVM.CreateViewModel<TransformViewModel>();
        }

        protected override void Bind()
        {
            base.Bind();

            BindProperty(Transform.Position, PositionChanged);
            BindProperty(Transform.EulerAngles, EulerAnglesChanged);
            BindProperty(Transform.Scale, ScaleChanged);
            BindProperty(Transform.ResourceName, ResourceNameChanged);

            BindCommand(Transform.UpdatePositionImediatelyCommand, UpdatePositionImediatelyExecuted);
            BindCommand(Transform.UpdateScaleImediatelyCommand, UpdateScaleImediatelyExecuted);
            BindCommand(Transform.UpdateEulerAnglesImediatelyCommand, UpdateEulerAnglesImediatelyExecuted);
            BindCommand(Transform.PauseCommand, PauseExecuted);
        }

        public virtual void ScaleChanged(Vector3 arg)
        {
        }

        public virtual void PositionChanged(Vector3 arg)
        {
        }

        public virtual void EulerAnglesChanged(Vector3 arg)
        {
        }

        public virtual void ResourceNameChanged(string arg)
        {
        }

        public virtual void UpdatePositionImediatelyExecuted(Vector3 arg)
        {
        }

        public virtual void UpdateEulerAnglesImediatelyExecuted(Vector3 arg)
        {
        }

        public virtual void UpdateScaleImediatelyExecuted(Vector3 arg)
        {
        }

        public virtual void PauseExecuted(bool arg)
        {
        }
    }
}
