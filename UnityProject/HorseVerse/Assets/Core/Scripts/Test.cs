using System.Collections;
using System.Collections.Generic;
using Core.Behavior.Movement;
using Core.MVVM;
using UnityEngine;
using System;
using Core.Behavior;

namespace Core.Behavior{
    public class Test : MonoBehaviour
    {
        public GameObject transformPrefab;

        List<TransformViewModel> transforms = new List<TransformViewModel>();

        // Use this for initialization
        void Start()
        {
            //TestBasic();
            //TestSelf();
            //TestSelfAndLowerPriority();
            //TestSendAndReceiveEvent();
            StartCoroutine(WaitingFor(1.0f, TestMoveAndRotate));
        }

        IEnumerator WaitingFor(float time, Action callback){
            yield return new WaitForSeconds(time);
            callback();
        }

        void TestBasic(){
            TransformViewModel transformViewModel = MVVM.MVVM.CreateViewModel<TransformViewModel>();
            TransformView transformView = MVVM.MVVM.InstantiateView<TransformView>(transform, transformViewModel, transformPrefab, gameObject);

            transforms.Add(transformViewModel);

            transformViewModel.Tree = new BehaviorTree();

            transformViewModel.Tree.AddVariable("RepeatForever", new ShareBoolBehavior { Value = true });
            transformViewModel.Tree.AddVariable("EndOnFailure", new ShareBoolBehavior { Value = false });

            transformViewModel.Tree.RegisterEvent<bool>("ChangeEndOnFailure", (result) =>
            {
                Debug.Log("ChangeEndOnFailure: " + result);
                var variable = transformViewModel.Tree.GetVariable<ShareBoolBehavior>("EndOnFailure");
                variable.Value = result;
            });

            var parallelTask = new ParallelTask()
                                        .AddTask(new FailureTask()
                                                 .AddTask(new InvertTask()
                                                        .AddTask(new InvertTask()
                                                                .AddTask(new SequenceTask()
                                                                         .AddTask(new WaitingTimeTask {  Time = new ShareFloatBehavior { Value = 2.5f } })
                                                                         .AddTask(new SendEventTask { Key = new ShareStringBehavior("ChangeEndOnFailure") })
                                                                        ))))
                                        .AddTask(new SequenceTask()
                                                 .AddTask(new WaitingTimeTask {  Time = new ShareFloatBehavior { Value = 1.5f } }));
            //transformViewModel.Tree.AddStartTask(parallelTask);
            transformViewModel.Tree.AddStartTask(new RepeatTask
            {
                RepeatForever = transformViewModel.Tree.GetVariable<ShareBoolBehavior>("RepeatForever"),
                EndOnFailure = transformViewModel.Tree.GetVariable<ShareBoolBehavior>("EndOnFailure")
            }
                                        .AddTask(parallelTask));
            transformViewModel.Tree.StartBehaviour();
        }

        void TestSelf(){
            TransformViewModel transformViewModel = MVVM.MVVM.CreateViewModel<TransformViewModel>();
            TransformView transformView = MVVM.MVVM.InstantiateView<TransformView>(transform, transformViewModel, transformPrefab, gameObject);

            transforms.Add(transformViewModel);

            transformViewModel.Tree = new BehaviorTree();

            transformViewModel.Tree.AddVariable("Test1", new ShareBoolBehavior { Value = false });
            var selectorTask = new SelectorTask{AbortType = AbortType.Self}
                .AddTask(new CompareWithShareBoolTask { ShareValue = transformViewModel.Tree.GetVariable<ShareBoolBehavior>("Test1"), CompareValue = new ShareBoolBehavior(true) })
                .AddTask(new WaitingTimeTask {  Time = new ShareFloatBehavior { Value = 30.0f } });

            transformViewModel.Tree.AddStartTask(selectorTask);
            transformViewModel.Tree.StartBehaviour();
        }

        void TestSelfAndLowerPriority()
        {
            TransformViewModel transformViewModel = MVVM.MVVM.CreateViewModel<TransformViewModel>();
            TransformView transformView = MVVM.MVVM.InstantiateView<TransformView>(transform, transformViewModel, transformPrefab, gameObject);

            transforms.Add(transformViewModel);

            transformViewModel.Tree = new BehaviorTree();

            transformViewModel.Tree.AddVariable("Test1", new ShareBoolBehavior { Value = false });
            transformViewModel.Tree.AddVariable("Test2", new ShareBoolBehavior { Value = false });
            var selectorTask = new SelectorTask { AbortType = AbortType.Self }
                .AddTask(new CompareWithShareBoolTask { ShareValue = transformViewModel.Tree.GetVariable<ShareBoolBehavior>("Test1"), CompareValue = new ShareBoolBehavior(true) })
                .AddTask(new SequenceTask { AbortType = AbortType.LowerPriority }
                         .AddTask(new CompareWithShareBoolTask { ShareValue = transformViewModel.Tree.GetVariable<ShareBoolBehavior>("Test2"), CompareValue = new ShareBoolBehavior(true) })
                         .AddTask(new WaitingTimeTask {  Time = new ShareFloatBehavior { Value = 2.0f } }))
                .AddTask(new WaitingTimeTask {  Time = new ShareFloatBehavior { Value = 30.0f } });

            transformViewModel.Tree.AddStartTask(selectorTask);
            transformViewModel.Tree.StartBehaviour();
        }

        void TestSendAndReceiveEvent(){
            TransformViewModel transformViewModel = MVVM.MVVM.CreateViewModel<TransformViewModel>();
            TransformView transformView = MVVM.MVVM.InstantiateView<TransformView>(transform, transformViewModel, transformPrefab, gameObject);

            transforms.Add(transformViewModel);

            transformViewModel.Tree = new BehaviorTree();

            var selectorTask = new SelectorTask { AbortType = AbortType.Self }
                .AddTask(new HasReceiveEventTask { Key = new ShareStringBehavior("ReceiveEventTask1") })
                .AddTask(new SequenceTask { AbortType = AbortType.LowerPriority }
                         .AddTask(new HasReceiveEventTask { Key = new ShareStringBehavior("ReceiveEventTask2") })
                         .AddTask(new SendEventTask { Key =new ShareStringBehavior("ReceiveEventTask1") })
                         .AddTask(new WaitingTimeTask {  Time = new ShareFloatBehavior { Value = 2.0f } }))
                .AddTask(new WaitingTimeTask {  Time = new ShareFloatBehavior { Value = 30.0f } });

            transformViewModel.Tree.AddStartTask(selectorTask);
            transformViewModel.Tree.StartBehaviour();
        }

        internal void TestMoveAndRotate()
        {
            // player
            {
                TransformViewModel transformViewModel = MVVM.MVVM.CreateViewModel<TransformViewModel>();
                TransformView transformView = MVVM.MVVM.InstantiateView<TransformView>(transform, transformViewModel, transformPrefab, gameObject);

                transforms.Add(transformViewModel);

                transformViewModel.Tree = new BehaviorTree();

                Vector3 eulerAngle = new Vector3(0, 180, 0);
                var direct = MathCustom.GetDirectionFromEulerAngle(eulerAngle);

                //Targets = transformViewModel.Tree.GetVariable<ShareListTransformViewModelBehavior>("Enemy"),

                transformViewModel.Tree.AddVariable("Speed", new ShareFloatBehavior { Value = 14 });
                transformViewModel.Tree.AddVariable("Direct", new ShareVector3Behavior { Value = direct });
                transformViewModel.Tree.AddVariable("Enemy", new ShareListTransformViewModelBehavior { Value = new List<TransformViewModel>() });
                transformViewModel.Tree.AddVariable("Target", new ShareTransformViewModelBehavior {  });
                var selectorTask = new SelectorTask { AbortType = AbortType.Self }
                    .AddTask(new HasReceiveEventTask { Key = new ShareStringBehavior("ReceiveEventTask1") })
                    .AddTask(new SequenceTask { AbortType = AbortType.LowerPriority }
                             .AddTask(new CanSeeObjectTask
                             {
                                 Target = transformViewModel.Tree.GetVariable<ShareTransformViewModelBehavior>("Target"),
                                 Targets = transformViewModel.Tree.GetVariable<ShareListTransformViewModelBehavior>("Enemy"),
                                 FieldOfView = new ShareFloatBehavior { Value = 360 },
                                 Range = new ShareFloatBehavior { Value = 6 }
                             }).AddTask(new ChaseTask
                             {
                                 TargetPosition = new ShareVector3Behavior { Value = new Vector3(0, 0, 0) },
                                 Target = transformViewModel.Tree.GetVariable<ShareTransformViewModelBehavior>("Target"),
                                 Speed = transformViewModel.Tree.GetVariable<ShareFloatBehavior>("Speed"),
                                 BufferDistance = new ShareFloatBehavior { Value = 1 },
                                 SpeedRotate = new ShareFloatBehavior { Value = 720.0f },
                             }))
                    .AddTask(new SequenceTask { AbortType = AbortType.None }
                             .AddTask(new RotationTask
                             {
                                 SpeedRotate = new ShareFloatBehavior { Value = 360.0f },
                                 Direct = transformViewModel.Tree.GetVariable<ShareVector3Behavior>("Direct")
                             })
                             .AddTask(new MoveTowardTask
                             {
                                 TargetPosition = new ShareVector3Behavior { Value = new Vector3(0, 0, 8) },
                                 Speed = transformViewModel.Tree.GetVariable<ShareFloatBehavior>("Speed")
                             })
                             .AddTask(new RotationTask
                             {
                                 SpeedRotate = new ShareFloatBehavior { Value = 360.0f },
                                 Direct = new ShareVector3Behavior { Value = new Vector3(0, 0, 1) }
                             }).AddTask(new MoveTowardTask
                             {
                                 TargetPosition = new ShareVector3Behavior { Value = new Vector3(0, 0, -4) },
                                 Speed = transformViewModel.Tree.GetVariable<ShareFloatBehavior>("Speed")
                             }))
                    .AddTask(new WaitingTimeTask {  Time = new ShareFloatBehavior { Value = 0.0f } });
                var repeatTask = new RepeatTask { RepeatForever = new ShareBoolBehavior { Value = true } };
                repeatTask.AddTask(selectorTask);
                transformViewModel.Tree.AddStartTask(repeatTask);
                transformViewModel.Tree.StartBehaviour();
            }
            //enemy
            {
                TransformViewModel transformViewModel = MVVM.MVVM.CreateViewModel<TransformViewModel>();
                TransformView transformView = MVVM.MVVM.InstantiateView<TransformView>(transform, transformViewModel, transformPrefab, gameObject);
                transformViewModel.Position.Value = new Vector3(0, 0, 12);

                transforms.Add(transformViewModel);

                transformViewModel.Tree = new BehaviorTree();

                Vector3 eulerAngle = new Vector3(0, 90, 0);
                var direct = MathCustom.GetDirectionFromEulerAngle(eulerAngle);

                transformViewModel.Tree.AddVariable("Speed", new ShareFloatBehavior { Value = 10 });
                transformViewModel.Tree.AddVariable("Direct", new ShareVector3Behavior { Value = direct });
                var repeatTask = new RepeatTask { RepeatForever = new ShareBoolBehavior { Value = true } };
                repeatTask.AddTask(new SequenceTask { AbortType = AbortType.LowerPriority }
                             .AddTask(new RotationTask
                             {
                                 SpeedRotate = new ShareFloatBehavior { Value = 90.0f },
                                 Direct = transformViewModel.Tree.GetVariable<ShareVector3Behavior>("Direct")
                             })
                             .AddTask(new MoveTowardTask
                             {
                                 TargetPosition = new ShareVector3Behavior { Value = new Vector3(12, 0, 12) },
                                 Speed = transformViewModel.Tree.GetVariable<ShareFloatBehavior>("Speed")
                             })
                             .AddTask(new RotationTask
                             {
                                 SpeedRotate = new ShareFloatBehavior { Value = 180.0f },
                                 Direct = new ShareVector3Behavior { Value = new Vector3(-1, 0, 0) }
                             }).AddTask(new MoveTowardTask
                             {
                                 TargetPosition = new ShareVector3Behavior { Value = new Vector3(-12, 0, 12) },
                                 Speed = transformViewModel.Tree.GetVariable<ShareFloatBehavior>("Speed")
                             }));

                transformViewModel.Tree.AddStartTask(repeatTask);
                transformViewModel.Tree.StartBehaviour();
            }
            {
                var enemy = transforms[0].Tree.GetVariable<ShareListTransformViewModelBehavior>("Enemy");
                enemy.Value.Add(transforms[1]);
            }
        }

        // Update is called once per frame
        void Update()
        {
            if(transforms.Count > 0){
                for (int i = 0; i < transforms.Count; i++){
                    var transformVM = transforms[i];
                    transformVM.Update(Time.deltaTime);
                }

                if (Input.GetKeyUp(KeyCode.P))
                {
                    transforms[0].Tree.SendEvent<bool>("ChangeEndOnFailure", true);
                }
                if (Input.GetKeyUp(KeyCode.A))
                {
                    var test1 = transforms[0].Tree.GetVariable<ShareBoolBehavior>("Test1");
                    if (test1 != null)
                    {
                        test1.Value = !test1.Value;
                    }
                }
                if (Input.GetKeyUp(KeyCode.S))
                {
                    var test2 = transforms[0].Tree.GetVariable<ShareBoolBehavior>("Test2");
                    if (test2 != null)
                    {
                        test2.Value = !test2.Value;
                    }
                }
                if (Input.GetKeyUp(KeyCode.D))
                {
                    transforms[0].Tree.SendEvent("ReceiveEventTask1");
                }
                if (Input.GetKeyUp(KeyCode.F))
                {
                    transforms[0].Tree.SendEvent("ReceiveEventTask2");
                }
            }
        }
    }
}

