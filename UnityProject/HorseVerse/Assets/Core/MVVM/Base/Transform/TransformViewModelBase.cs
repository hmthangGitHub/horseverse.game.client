using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Core.MVVM;
using UniRx;

namespace Core.MVVM
{
    public class TransformViewModelBase : ViewModel
    {

        public ReactiveProperty<Vector3> Position;
        public ReactiveProperty<Vector3> EulerAngles;
        public ReactiveProperty<Vector3> Scale;

        public ReactiveProperty<string> ResourceName;

        public ReactiveCommand<Vector3> UpdatePositionImediatelyCommand;
        public ReactiveCommand<Vector3> UpdateScaleImediatelyCommand;
        public ReactiveCommand<Vector3> UpdateEulerAnglesImediatelyCommand;

        public ReactiveCommand<bool> PauseCommand;

        public override void InitViewModel()
        {
            base.InitViewModel();

            Position = new ReactiveProperty<Vector3>();
            EulerAngles = new ReactiveProperty<Vector3>();
            Scale = new ReactiveProperty<Vector3>();
            Scale.Value = Vector3.one;
            ResourceName = new ReactiveProperty<string>();

            UpdatePositionImediatelyCommand = new ReactiveCommand<Vector3>();
            UpdatePositionImediatelyCommand.Subscribe(UpdatePositionImediately);
            UpdateEulerAnglesImediatelyCommand = new ReactiveCommand<Vector3>();
            UpdateEulerAnglesImediatelyCommand.Subscribe(UpdateEulerAnglesImediately);
            UpdateScaleImediatelyCommand = new ReactiveCommand<Vector3>();
            UpdateScaleImediatelyCommand.Subscribe(UpdateScaleImediately);
            PauseCommand = new ReactiveCommand<bool>();
            PauseCommand.Subscribe(Pause);
        }

        #region command
        public virtual void UpdatePositionImediately(Vector3 arg)
        {
        }

        public virtual void UpdateEulerAnglesImediately(Vector3 arg)
        {
        }

        public virtual void UpdateScaleImediately(Vector3 arg)
        {
        }

        public virtual void Pause(bool arg)
        {

        }
        #endregion

        public override void Dispose()
        {
            base.Dispose();
            Position.Dispose();
            EulerAngles.Dispose();
            Scale.Dispose();
            ResourceName.Dispose();

            UpdatePositionImediatelyCommand.Dispose();
            UpdateEulerAnglesImediatelyCommand.Dispose();
            UpdateScaleImediatelyCommand.Dispose();
            PauseCommand.Dispose();
        }
    }
}
