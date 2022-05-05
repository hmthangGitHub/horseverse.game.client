using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Core.MVVM;

namespace Core.MVVM
{
    public class TransformView : TransformViewBase
    {
        protected GameObject _go;
        protected List<Animator> animators = new List<Animator>();
        protected List<Material> materials = new List<Material>();
        protected List<ParticleSystem> particles = new List<ParticleSystem>();

        protected int _preTick = -1;
        public override ViewModel CreateViewModelWhenInitedByHierarchy()
        {
            return MVVM.CreateViewModel<TransformViewModel>();
        }

        protected override sealed void BindViewModelToViewDone()
        {
            if (IsInitedByHierarchy)
            {
                Transform.NextTargetPosition = transform.position;
                Transform.Position.Value = transform.position;
                Transform.EulerAngles.Value = transform.eulerAngles;
                Transform.Scale.Value = transform.localScale;
            }
        }

        protected override void OnUpdate()
        {
            if (!Transform.IsRunning) return;

            Quaternion toRotate = Quaternion.Euler(Transform.EulerAngles.Value);
            transform.rotation = Quaternion.Lerp(transform.rotation, toRotate, Time.deltaTime * 20);
            //Debug.Log("OnUpdate: " + Actor.NextTargetPosition + ";" + Time.time);
            var nextDirection = (Transform.NextTargetPosition - Transform.Position.Value).normalized;
            var tickInterval = Transform.TickInterval;
            if(tickInterval <= 0){
                tickInterval = 0.05f;
            }
            var speed = (Transform.NextTargetPosition - Transform.Position.Value).magnitude / tickInterval;
			//transform.position = transform.position + nextDirection * Actor.Speed.Value * Time.deltaTime;
            if (_preTick == BattleManager.Instance.BattleViewModel.Tick)
            {
                transform.position = transform.position + nextDirection * speed * Time.deltaTime;
            }
            else
            {
                _preTick = BattleManager.Instance.BattleViewModel.Tick;
                transform.position = Transform.Position.Value + nextDirection * speed * BattleManager.Instance.BattleView.CurrentTickCooldown;
            }
        }

		public override void PositionChanged(Vector3 arg)
        {
            transform.position = arg;
        }

        public override void EulerAnglesChanged(Vector3 arg)
        {
            transform.eulerAngles = arg;
        }

        public override void ScaleChanged(Vector3 arg)
        {
            transform.localScale = arg;
        }

        public override void UpdatePositionImediatelyExecuted(Vector3 arg)
        {
            transform.position = arg;
        }

        public override void UpdateScaleImediatelyExecuted(Vector3 arg)
        {
            transform.localScale = arg;
        }

        public override void UpdateEulerAnglesImediatelyExecuted(Vector3 arg)
        {
            transform.eulerAngles = arg;
        }

		public override void PauseExecuted(bool arg)
		{
            animators.SetSpeed(arg ? 0 : 1);
            particles.SetPause(arg);
		}

		protected override void ReceiveDestroy()
		{
            base.ReceiveDestroy();
            if(_go != null)
            {
                if (!IsAddToPool())
                {
                    Destroy(_go);
                }
                else
                {
                    ObjectPoolManager.Instance.Add(_go.name, _go);
                }
            }
            _go = null;
            animators.Clear();
            particles.Clear();
		}
	}
}
