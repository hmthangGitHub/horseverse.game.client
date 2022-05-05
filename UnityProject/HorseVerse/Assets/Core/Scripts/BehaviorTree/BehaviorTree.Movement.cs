using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Core.MVVM;
using UnityEngine.AI;
using System;
using FixRound;

namespace Core.Behavior.Movement
{
    public abstract class MovementTask : ActionTask{
        public ShareFloatBehavior SpeedRotate { get; set; } // 360/s

        protected void CheckRotate(Vector3 directTarget, float speedRotate, Action rotateFinish = null){
            var direct = MathCustom.GetDirectionFromEulerAngle(Transform.EulerAngles.Value);
            var angle = MathCustom.GetAngleFromNagative180To180(direct, directTarget);
            var nextAngle = speedRotate * TickInterval * (angle >= 0 ? 1 : -1);
            if (Mathf.Abs(angle) <= Mathf.Abs(nextAngle))
            {
				nextAngle = angle;
                var nextDirect = MathCustom.GetDirectionWithAngleAndCurrentDirection(nextAngle, direct);
                Transform.EulerAngles.Value = MathCustom.GetEulerAngleFromDirect(nextDirect);
                if(rotateFinish != null){
                    rotateFinish();
                }
            }
            else
            {
                var nextDirect = MathCustom.GetDirectionWithAngleAndCurrentDirection(nextAngle, direct);
                Transform.EulerAngles.Value = MathCustom.GetEulerAngleFromDirect(nextDirect);
            }
        }
    }
    public sealed class RotationTask : MovementTask{
        
        public ShareVector3Behavior Direct { get; set; }
        public ShareTransformViewModelBehavior Target { get; set; }

        public RotationTask(){
            Direct = new ShareVector3Behavior();
            SpeedRotate = new ShareFloatBehavior();
            SpeedRotate.Value = 360;
            Target = new ShareTransformViewModelBehavior();
            Target.Value = null;
        }

        protected override void OnStart()
        {
            base.OnStart();
            
            CheckRotate(GetDirect(), SpeedRotate.Value, () =>
            {
                TaskStatus = TaskStatus.Success;
            });
        }

		protected override void OnUpdate()
		{
            CheckRotate(GetDirect(), SpeedRotate.Value, () =>
            {
                TaskStatus = TaskStatus.Success;
            });
		}

        private Vector3 GetDirect()
        {
            var direct = Direct.Value;
            if (Target != null && Target.Value != null)
            {
                direct = Vector3.Normalize(Target.Value.Position.Value - Transform.Position.Value);
            }
            return direct;
        }
	}

    // no nav mesh
    public sealed class MoveTowardTask : MovementTask
    {
        public ShareVector3Behavior TargetPosition { get; set; }
        public ShareFloatBehavior Speed { get; set; }
        public ShareFloatBehavior BufferDistance { get; set; }
        public ShareBoolBehavior CanRotate { get; set; }
        public ShareBoolBehavior LimitDistance { get; set; }

        private float _distance = 0;

        public MoveTowardTask()
        {
            TargetPosition = new ShareVector3Behavior();
            Speed = new ShareFloatBehavior();
            Speed.Value = 2;
            BufferDistance = new ShareFloatBehavior();
            BufferDistance.Value = 0.3f;
            CanRotate = new ShareBoolBehavior();
            CanRotate.Value = true;
            SpeedRotate = new ShareFloatBehavior();
            SpeedRotate.Value = 360;
            LimitDistance = new ShareBoolBehavior();
            LimitDistance.Value = false;
        }

		protected override void OnStart()
		{
            TaskStatus = TaskStatus.Running;
            if (LimitDistance.Value){
                _distance = Vector3.Distance(TargetPosition.Value, Transform.Position.Value);
            }
            if(TargetPosition.Value != Transform.Position.Value){
                Vector3 targetPosition = TargetPosition.Value;
                var position = Transform.Position.Value;
                var direct = Vector3.Normalize(targetPosition - position);
                Transform.NextTargetPosition = position + direct * Speed.Value * Transform.TickInterval;
                if (LimitDistance.Value)
                {
                    _distance -= Vector3.Distance(Transform.NextTargetPosition, position);
                }
                if (CanRotate.Value)
                {
                    CheckRotate(Vector3.Normalize(targetPosition - Transform.Position.Value), SpeedRotate.Value);
                }
            }

            //Debug.Log("OnStart: " + Transform.NextTargetPosition + ";" + Time.time);
		}

		protected override void OnUpdate()
        {
            Vector3 targetPosition = TargetPosition.Value;
            var prePosition = Transform.Position.Value;
            var position = Transform.NextTargetPosition;
            Transform.Position.Value = position;
            var preDirect = Vector3.Normalize(position - prePosition);

            if(CanRotate.Value){
                CheckRotate(Vector3.Normalize(targetPosition - Transform.Position.Value), SpeedRotate.Value);
            }


            var distance = Vector3.Distance(targetPosition, position);
            var direct = Vector3.Normalize(targetPosition - position);
            var dot = Vector3.Dot(preDirect, direct);
            if(LimitDistance.Value){
                if (_distance <= 0.1f)
                {
                    _distance = 0;
                    Transform.Position.Value = Transform.Position.Value;
                    Transform.NextTargetPosition = Transform.Position.Value;
                    TaskStatus = TaskStatus.Success;
                    return;
                }
            } else{
                //Debug.Log("dot: " + dot + ";preDirect: " + preDirect + ";direct: " + direct + ";" + distance);
                if (distance <= BufferDistance.Value || dot < 0)
                {
                    //Debug.Log("Move Done: " + Time.time);
                    Transform.Position.Value = Transform.Position.Value + direct * (BufferDistance.Value - distance);
                    Transform.NextTargetPosition = Transform.Position.Value;
                    TaskStatus = TaskStatus.Success;
                    return;
                }
            }

            Transform.NextTargetPosition = position + direct * Speed.Value * TickInterval;
            if (LimitDistance.Value)
            {
                _distance -= Vector3.Distance(position, Transform.NextTargetPosition);
            }
            //Debug.Log("Move : " + Transform.NextTargetPosition + ";" + Time.time);
        }
    }

    public class MovementNavMeshTask : MovementTask{
        public ShareVector3Behavior TargetPosition { get; set; }
        public ShareFloatBehavior Speed { get; set; }
        public ShareFloatBehavior Accelerate { get; set; }
        public ShareFloatBehavior BufferDistance { get; set; }
        public ShareBoolBehavior CanRotate { get; set; }
        public ShareBoolBehavior IsOneLine { get; set; }
        protected NavMeshPath _path;

        protected float _speed = 0;
        protected float _currentTime = 0;

        public MovementNavMeshTask() : base()
        {
            TargetPosition = new ShareVector3Behavior();
            Speed = new ShareFloatBehavior();
            Speed.Value = 2;
            Accelerate = new ShareFloatBehavior();
            Accelerate.Value = 100;
            BufferDistance = new ShareFloatBehavior();
            BufferDistance.Value = 0.1f;
            CanRotate = new ShareBoolBehavior();
            CanRotate.Value = true;
            SpeedRotate = new ShareFloatBehavior();
            SpeedRotate.Value = 360;
            IsOneLine = new ShareBoolBehavior();
            IsOneLine.Value = true;

            _path = new NavMeshPath();
        }

        protected void InitAccelerate()
        {
            _currentTime = 0;
            _speed = Speed.Value;
            if (Accelerate.Value < 100)
            {
                _speed = _speed * (Accelerate.Value / 100f);
            }
        }

        protected void HandleAccelerate()
        {
            _currentTime += TickInterval;
            if (_currentTime >= 0.1f)
            {
                _currentTime = 0;
                _speed *= 1.1f;
                if (_speed > Speed.Value)
                {
                    _speed = Speed.Value;
                }
            }
        }

        protected override void OnStart()
        {
            TaskStatus = TaskStatus.Running;
            InitAccelerate();
            Vector3 targetPosition = GetTargetPosition();
            if (targetPosition != Transform.Position.Value)
            {
                //Vector3 targetPosition = TargetPosition.Value;
                //var position = Transform.Position.Value;
                //if (!HasPath(position, targetPosition))
                //{
                //    if (CanRotate.Value)
                //    {
                //        CheckRotate(Vector3.Normalize(targetPosition - Transform.Position.Value), SpeedRotate.Value / 2);
                //    }
                //}
                //var position = Transform.NextTargetPosition;
                //if (HasPath(position, targetPosition))
                //{
                   
                //}
                //
                StartMove();
            }
        }

        protected override void OnUpdate()
        {
            HandleAccelerate();
            Vector3 targetPosition = GetTargetPosition();
            var position = Transform.NextTargetPosition;
            Transform.Position.Value = position;
            if (!HasPath(position, targetPosition)){
                Transform.NextTargetPosition = Transform.Position.Value;
                StopMove();
                TaskStatus = TaskStatus.Success;
                return;
            }
        }

        protected bool HasPath(Vector3 sourcePosition, Vector3 targetPosition){
            var bufferDistance = BufferDistance.Value;
            NavMesh.CalculatePath(sourcePosition, targetPosition, NavMesh.AllAreas, _path);
            List<Vector3> nodes = new List<Vector3>();
            nodes.AddRange(_path.corners);
            if (bufferDistance > 0)
            {
                for (int i = nodes.Count - 1; i > 0; i--)
                {
                    var position1 = nodes[i];
                    var position2 = nodes[i - 1];
                    var distance = FloatValue.Round(Vector3.Distance(position2, position1));
                    if (distance > bufferDistance)
                    {
                        Vector3 direction = Vector3Value.Round(Vector3.Normalize(position1 - position2));
                        Vector3 newPosition = Vector3Value.Round(position2 + direction * (distance - bufferDistance));
                        nodes[i] = newPosition;
                        break;
                    }
                    else
                    {
                        bufferDistance -= distance;
                        nodes.RemoveAt(i);
                    }
                }
            }
            if (nodes.Count > 1)
            {
                if (Accelerate.Value < 100)
                {
                    var length = 0f;
                    for (int i = 0, j = 1; i < nodes.Count - 1; i++, j++)
                    {
                        length += FloatValue.Round(Vector3.Distance(nodes[i], nodes[j]));
                    }
                    if (length < 3)
                    {
                        _speed /= 1.1f;
                        _speed = FloatValue.Round(_speed);
                        var minSpeed = FloatValue.Round(Speed.Value * (Accelerate.Value / 100f));
                        if (_speed < minSpeed)
                        {
                            _speed = minSpeed;
                        }
                    }
                }
                var maxDistance = FloatValue.Round(_speed * TickInterval);
                Vector3 nextPosition = Vector3.zero;
                Vector3 nextDirection = Vector3.one;
                bool canFinish = false;
                var isOneLine = IsOneLine.Value;
                for (int i = 0; i < nodes.Count - 1; i++)
                {
                    var position1 = nodes[i];
                    var position2 = nodes[i + 1];
                    canFinish = false;
                    var distance = FloatValue.Round(Vector3.Distance(position2, position1));
                    if (maxDistance <= distance)
                    {
                        nextDirection = Vector3.Normalize(position2 - position1);
                        nextPosition = position1 + nextDirection * maxDistance;
                        break;
                    }
                    else
                    {
                        nextDirection = Vector3.Normalize(position2 - position1);
                        nextPosition = position2;
                        maxDistance -= distance;
                        canFinish = true;
                    }
                    if (isOneLine)
                    {
                        canFinish = false;
                        break;
                    }
                }

                if (CanRotate.Value)
                {
                    CheckRotate(!isOneLine ? nextDirection : (Vector3Value.NormalizeIgnoreY(targetPosition, sourcePosition)), SpeedRotate.Value);
                }
                Transform.NextTargetPosition = nextPosition;
                return !canFinish;
            }
            if (IsOneLine.Value)
            {
                if (CanRotate.Value)
                {
                    CheckRotate(Vector3Value.NormalizeIgnoreY(targetPosition, sourcePosition), SpeedRotate.Value / 2);
                }
            }
            return false;
        }

        protected virtual Vector3 GetTargetPosition(){
            return TargetPosition.Value;
        }

        protected virtual void StartMove()
        {

        }

        protected virtual void StopMove()
        {

        }
    }

    public sealed class ChaseTask : MovementNavMeshTask {
        public ShareTransformViewModelBehavior Target { get; set; }
        public ShareStringBehavior CallbackWhenMoveDone { get; set; }
        public ShareStringBehavior CallbackWhenStartMove { get; set; }

        public ShareFloatBehavior RadiusToCheckSeeTarget { get; set; }
        public ShareFloatBehavior RadiusToCheckRaycast { get; set; }

        public ChaseTask() : base()
        {
            Target = new ShareTransformViewModelBehavior();
            CallbackWhenMoveDone = new ShareStringBehavior();
            CallbackWhenStartMove = new ShareStringBehavior();
            RadiusToCheckSeeTarget = new ShareFloatBehavior();
            RadiusToCheckSeeTarget.Value = -1;
            RadiusToCheckRaycast = new ShareFloatBehavior();
            RadiusToCheckRaycast.Value = -1;
        }

        protected override void OnStart()
        {
            base.OnStart();
            CheckStopMove();
        }

        protected override void OnUpdate()
        {
            base.OnUpdate();
            CheckStopMove();
        }

        protected override Vector3 GetTargetPosition()
		{
            if(Target.Value == null){
                return base.GetTargetPosition();
            }
            return Target.Value.Position.Value;
		}

        protected override void StartMove()
        {
            if (!string.IsNullOrEmpty(CallbackWhenStartMove.Value))
            {
                _tree.SendEvent(CallbackWhenStartMove.Value);
            }
        }

        protected override void StopMove()
        {
            if (!string.IsNullOrEmpty(CallbackWhenMoveDone.Value))
            {
                _tree.SendEvent(CallbackWhenMoveDone.Value);
            }
        }

        private void CheckStopMove()
        {
            if (TaskStatus == TaskStatus.Running)
            {
                if(RadiusToCheckSeeTarget == null || RadiusToCheckRaycast == null)
                {
                    return;
                }
                var radiusToCheckSeeTarget = RadiusToCheckSeeTarget.Value;
                var radiusToCheckRaycast = RadiusToCheckRaycast.Value;
                radiusToCheckSeeTarget = radiusToCheckSeeTarget > radiusToCheckRaycast ? radiusToCheckRaycast : radiusToCheckSeeTarget;
                var target = Target.Value;
                if (radiusToCheckSeeTarget > 0 && radiusToCheckSeeTarget > Vector3Value.Distance(Transform.Position.Value, target.Position.Value))
                {
                    bool hitENV = false;
                    var startPosition = Transform.Position.Value + new Vector3(0, Transform.Radius + 0.5f, 0);
                    var startDirection = (target.Position.Value - Transform.Position.Value).normalized;
                    Ray ray = new Ray();
                    ray.origin = startPosition;
                    ray.direction = startDirection;
                    RaycastHit raycastHit;
                    var d1 = radiusToCheckRaycast;
                    var d2 = 0f;
                    if (Physics.Raycast(ray, out raycastHit, d1))
                    {
                        d2 = Vector3.Distance(raycastHit.point, ray.origin);
                        hitENV = true;
                        var actorPosition = target.Position.Value;
                        var actorPositionTemp = actorPosition;
                        actorPositionTemp.y = startPosition.y;
                        var d = Vector3.Distance(actorPositionTemp, startPosition);
                        if (MathCustom.CheckInStraight(startDirection,
                                                    startPosition,
                                                    actorPositionTemp,
                                                    target.Radius,
                                                    radiusToCheckSeeTarget,
                                                    0.1f))
                        {
                            if (raycastHit.collider != null && d2 < d)
                            {
                                //Debug.Log("hit env");
                            }
                            else
                            {
                                hitENV = false;
                            }
                        }
                    }

                    if (!hitENV)
                    {
                        Transform.NextTargetPosition = Transform.Position.Value;
                        StopMove();
                        TaskStatus = TaskStatus.Success;
                    }
                }
            }
        }
    }

    public class WanderTask : MovementNavMeshTask
    {
        public ShareFloatBehavior MinDistance { get; set; }
        public ShareFloatBehavior MaxDistance { get; set; }
        public ShareFloatBehavior MinTimeToWaitToContinueWander { get; set; }
        public ShareFloatBehavior MaxTimeToWaitToContinueWander { get; set; }
        public ShareIntBehavior NumberRetry { get; set; }
        public ShareFloatBehavior MaxAngle { get; set; }
        public ShareFloatBehavior AverageAngle { get; set; }
        public ShareStringBehavior CallbackWhenMoveDone { get; set; }
        public ShareStringBehavior CallbackWhenStartMove { get; set; }

        protected float _waitToContinueWander = 0;


        public WanderTask() : base()
        {
            MinDistance = new ShareFloatBehavior();
            MinDistance.Value = 2;
            MaxDistance = new ShareFloatBehavior();
            MaxDistance.Value = 5;
            MaxAngle = new ShareFloatBehavior();
            MaxAngle.Value = 360;
            AverageAngle = new ShareFloatBehavior();
            AverageAngle.Value = 20f;
            MinTimeToWaitToContinueWander = new ShareFloatBehavior();
            MinTimeToWaitToContinueWander.Value = 1;
            MaxTimeToWaitToContinueWander = new ShareFloatBehavior();
            MaxTimeToWaitToContinueWander.Value = 3;
            NumberRetry = new ShareIntBehavior();
            NumberRetry.Value = 1;
            IsOneLine.Value = false;
            CallbackWhenMoveDone = new ShareStringBehavior();
            CallbackWhenStartMove = new ShareStringBehavior();
        }

        protected override void OnStart()
        {
            if (_waitToContinueWander > 0) return;

            FindNewTargetPosition();
            //Vector3 targetPosition = TargetPosition.Value;
            //var position = Transform.Position.Value;
            //HasPath(position, targetPosition);
        }

        protected override void OnUpdate()
        {
            if (_waitToContinueWander > 0)
            {
                _waitToContinueWander -= TickInterval;
                if (_waitToContinueWander <= 0)
                {
                    FindNewTargetPosition();
                }
                return;
            }
            HandleAccelerate();
            Vector3 targetPosition = TargetPosition.Value;
            var position = Transform.NextTargetPosition;
            Transform.Position.Value = position;
            if (!HasPath(position, targetPosition))
            {
                StartWaitingToContinueWander();
                HandleWhenFinishMove();
                return;
            }

            if (Transform.NextTargetPosition == Transform.Position.Value)
            {
                StartWaitingToContinueWander();
                HandleWhenFinishMove();
            }
        }

        void StartWaitingToContinueWander()
        {
            _waitToContinueWander = _tree.RandomRange(MinTimeToWaitToContinueWander.Value, MaxTimeToWaitToContinueWander.Value);
            StopMove();
        }

        protected virtual void FindNewTargetPosition()
        {
            InitAccelerate();

            int n = NumberRetry.Value;             if (n > 16)             {                 n = 16;             }             n = n <= 0 ? 1 : n;

            var maxAngle = MaxAngle.Value;
            var averageAngle = AverageAngle.Value;
            var angleCount = (int)(maxAngle / averageAngle) + 1;
            List<float> angles = new List<float>();
            for (int i = 0; i < angleCount; i++)
            {
                angles.Add(averageAngle * i - maxAngle / 2);
            }

            Vector3 targetPosition = Vector3.zero;
            float distance = 0;
            bool validatePosition = false;

            var minDistance = MinDistance.Value;
            if (minDistance > MaxDistance.Value)
            {
                minDistance = MaxDistance.Value / 2;
            }
            distance = _tree.RandomRange(minDistance, MaxDistance.Value);
            var currentPosition = Transform.Position.Value;
            while (!validatePosition && n > 0)
            {
                var index = _tree.RandomRange(0, angles.Count);                 var angle = angles[index];                 angles.RemoveAt(index);
                var direction = MathCustom.GetDirectionWithAngleAndCurrentDirection(angle, Transform.Direction);
                targetPosition = Transform.Position.Value + direction * distance;
                if (ConditionFindTargetPosition(targetPosition))
                {
                    NavMeshHit hit;
                    validatePosition = NavMesh.SamplePosition(targetPosition, out hit, float.MaxValue, NavMesh.AllAreas);

                    NavMesh.CalculatePath(currentPosition, targetPosition, NavMesh.AllAreas, _path);                     validatePosition = _path.corners.Length > 1;                     if (!validatePosition)
                    {                         targetPosition = currentPosition + direction * 3;                         NavMesh.CalculatePath(currentPosition, targetPosition, NavMesh.AllAreas, _path);                         validatePosition = _path.corners.Length > 1;                     }
                }
                n--;
            }
            if (validatePosition)
            {
                TargetPosition.Value = targetPosition;
                StartMove();
                HandleWhenStartMove();
            }
            else
            {
                HandleWhenFindFailTargetPosition();
            }
        }

        protected override void StartMove()
        {
            if (!string.IsNullOrEmpty(CallbackWhenStartMove.Value))
            {
                //Debug.Log("CallbackWhenStartMove");
                _tree.SendEvent(CallbackWhenStartMove.Value);
            }
        }

        protected override void StopMove()
        {
            if (!string.IsNullOrEmpty(CallbackWhenMoveDone.Value))
            {
                //Debug.Log("CallbackWhenMoveDone");
                _tree.SendEvent(CallbackWhenMoveDone.Value);
            }
        }

        protected virtual bool ConditionFindTargetPosition(Vector3 targetPosition)
        {
            return true;
        }

        protected virtual void HandleWhenFindFailTargetPosition()
        {
            TaskStatus = TaskStatus.Failure;
        }

        protected virtual void HandleWhenFinishMove()
        {

        }

        protected virtual void HandleWhenStartMove()
        {

        }
    }

    public class HikeInCirclesTask : WanderTask {
        public ShareVector3Behavior InitPosition { get; set; }
        public ShareFloatBehavior Radius { get; set; }


        public HikeInCirclesTask() :base(){
            InitPosition = new ShareVector3Behavior();
            InitPosition.Value = TargetPosition.Value;
            Radius = new ShareFloatBehavior();
            Radius.Value = 5;
        }

        protected override bool ConditionFindTargetPosition(Vector3 targetPosition){
            var distanceTemp = Vector3.Distance(targetPosition, InitPosition.Value);
            return distanceTemp <= Radius.Value;
        }

        protected override void HandleWhenFindFailTargetPosition(){
            TargetPosition.Value = InitPosition.Value;
            var position = Transform.Position.Value;
            HasPath(position, TargetPosition.Value);
        }
	}

    public sealed class WithinDistanceTask : ConditionTask{
        public ShareTransformViewModelBehavior Target { get; set; }
        public ShareFloatBehavior MaxDistance { get; set; }

        public WithinDistanceTask() {
            Target = new ShareTransformViewModelBehavior();
            MaxDistance = new ShareFloatBehavior();
        }

        protected internal override bool IsPassCondition()
        {
            return CanPass();
        }

        bool CanPass()
        {
            if(Target.Value != null){
                var distance = Vector3Value.DistanceIgnoreY(Transform.Position.Value, Target.Value.Position.Value);
                return distance <= MaxDistance.Value;
            }
            return false;
        }
    }

    public sealed class RotateAroundTask : ActionTask
    {
        public ShareFloatBehavior Rotate { get; set; }
        public ShareFloatBehavior Duration { get; set; }

        private float currentRotate = 0;

        public RotateAroundTask()
        {
            Rotate = new ShareFloatBehavior();
            Rotate.Value = 360;
            Duration = new ShareFloatBehavior();
            Duration.Value = 1f;
        }

		protected override void OnStart()
		{
            base.OnStart();
            currentRotate = 0;
		}

		protected override void OnUpdate()
		{
            var maxRotate = Rotate.Value < 0 ? (Rotate.Value * -1) : Rotate.Value;
            if (maxRotate > 0 && Duration.Value > 0)
            {
                var nextRotate = maxRotate * TickInterval / Duration.Value;
                Vector3 eulerAngles = Vector3.zero;
                if (currentRotate + nextRotate >= maxRotate)
                {
                    eulerAngles = Transform.EulerAngles.Value;
                    eulerAngles.y += (maxRotate - currentRotate);
                    Transform.EulerAngles.Value = eulerAngles;
                    TaskStatus = TaskStatus.Success;
                    return;
                }
                eulerAngles = Transform.EulerAngles.Value;
                eulerAngles.y += nextRotate;
                Transform.EulerAngles.Value = eulerAngles;
                currentRotate += nextRotate;
            }
		}

		protected override void OnEnd()
		{
            base.OnEnd();
            currentRotate = 0;
		}
	}

    public sealed class CanSeeObjectTask : ConditionTask
    {
        public ShareListTransformViewModelBehavior Targets { get; set; }
        public ShareTransformViewModelBehavior Target { get; set; }
        public ShareFloatBehavior FieldOfView { get; set; }
        public ShareFloatBehavior Range { get; set; }

        public CanSeeObjectTask()
        {
            Target = new ShareTransformViewModelBehavior();
            Targets = new ShareListTransformViewModelBehavior();
            FieldOfView = new ShareFloatBehavior();
            Range = new ShareFloatBehavior();
        }

        protected internal override bool IsPassCondition()
        {
            return CanPass();
        }

        bool CanPass(){
            if (Target.Value != null)
            {

                if (CanSee(Target.Value))
                {
                    return true;
                }
                else
                {
                    Target.Value = null;
                }
            }

            if (Targets.Value != null && Targets.Value.Count > 0)
            {
                for (int i = 0; i < Targets.Value.Count; i++)
                {
                    var target = Targets.Value[i];
                    if (CanSee(target))
                    {
                        Target.Value = target;
                        return true;
                    }

                }
            }
            return false;
        }

        bool CanSee(TransformViewModel target){
            if (!target.IsAlive) return false;
            var distance = Vector3.Distance(target.Position.Value, Transform.Position.Value);
            var radius = target.Radius;
            if (Range.Value >= distance - radius){
                var direct = MathCustom.GetDirectionFromEulerAngle(Transform.EulerAngles.Value);
                var direct1 = Vector3.Normalize(target.Position.Value - Transform.Position.Value);
                var angle = Vector3.Angle(direct, direct1);
                var result = FieldOfView.Value / 2 >= angle;
                return result;
            }
            return false;
        }
	}
}
