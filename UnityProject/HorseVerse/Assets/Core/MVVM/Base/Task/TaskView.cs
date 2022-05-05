using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Linq;

namespace Core.Behavior.UI
{
    public class TaskView : TaskViewBase
    {
        public Button linkBtn;
        public Button openBtn;
        public Button deleteBtn;
        public Text nameTxt;
        [SerializeField]
        GameObject validateTask;
        [SerializeField]
        Text abortTypeTxt;
        [SerializeField]
        RectTransform[] lines;
        [SerializeField]
        BehaviorTreeEditor behaviourTreeEditor;

        Vector2 _prePosition;
        Vector2 _rootTopPosition;
        public Vector2 RootTopPosition
        {
            get
            {
                return _rootTopPosition;
            }
        }
        Vector2 _rootBottomPosition;
        public Vector2 RootBottomPosition{
            get{
                return _rootBottomPosition;
            }
        }

        TaskView _parent = null;
        List<TaskView> _children = new List<TaskView>();

		public override void IsValidateChanged(bool arg)
		{
            base.IsValidateChanged(arg);
            validateTask.SetActive(!arg);
		}

		public override void InitTaskExecuted(InitTaskCommand command)
		{
            if (!behaviourTreeEditor.TaskViews.Contains(this))
            {
                behaviourTreeEditor.TaskViews.Add(this);
            }
            _prePosition = GetComponent<RectTransform>().anchoredPosition;
            var parentTask = Task.Task as ParentTask;
            if (parentTask == null)
            {
                Task.IsValidate.Value = true;
            }
            UpdatePosition(true);
            CheckUpdateAbortType();

            _callbackPointerEventData = (pointEventData, isEndPoint) =>
            {
                UpdatePosition(isEndPoint);
                if(isEndPoint){
                    if(_parent != null){
                        var compositeTask = _parent.Task.Task as CompositeTask;
                        if (compositeTask != null)
                        {
                            _parent._children.Sort((x, y) =>
                            {
                                return x.Task.Task.X.CompareTo(y.Task.Task.X);
                            });
                        }
                    }
                }
            };

            if (linkBtn == null) return;


            EventTrigger eventTrigger = linkBtn.gameObject.AddComponent<EventTrigger>();
            {
                var pointerDown = new EventTrigger.Entry();
                pointerDown.eventID = EventTriggerType.PointerDown;
                pointerDown.callback.AddListener((e) => {
                    //PointerEventData pointerEventData = e as PointerEventData;
                    //Vector2 localPosition = Vector2.zero;
                    //RectTransformUtility.ScreenPointToLocalPointInRectangle(UI, pointerEventData.position, pointerEventData.pressEventCamera, out localPosition);
                    //var direct = (localPosition - _rootPositionLine).normalized;
                    //var distance = Vector2.Distance(localPosition, _rootPositionLine);
                    //line.sizeDelta = new Vector2(line.sizeDelta.x, distance);
                    //var angle = MathCustom.GetAngleFromNagative180To180(direct, new Vector2(0, 1));
                    //line.localEulerAngles = new Vector3(0, 0, angle);
                });
                eventTrigger.triggers.Add(pointerDown);
            }
            {
                var pointerUp = new EventTrigger.Entry();
                pointerUp.eventID = EventTriggerType.PointerUp;
                pointerUp.callback.AddListener((e) => {
                    PointerEventData pointerEventData = e as PointerEventData;
                    TaskView taskSelected = null;
                    Vector3 localMousePosition = transform.parent.GetComponent<RectTransform>().InverseTransformPoint(Input.mousePosition);
                    for (int i = 0; i < behaviourTreeEditor.TaskViews.Count; i++)
                    {
                        var task = behaviourTreeEditor.TaskViews[i];
                        var rectTransformTemp = task.GetComponent<RectTransform>();
                        var anchoredPoint = rectTransformTemp.anchoredPosition;
                        var size = rectTransformTemp.sizeDelta;
                        // for overlay
                        if (task != this 
                            && anchoredPoint.x - size.x / 2 <= localMousePosition.x
                            && anchoredPoint.x + size.x / 2 >= localMousePosition.x
                            && anchoredPoint.y - size.y / 2 <= localMousePosition.y
                            && anchoredPoint.y + size.y / 2 >= localMousePosition.y)
                        {
                            taskSelected = task;
                        }

                        //Vector2 localMousePosition = rectTransformTemp.inve(Input.mousePosition);
                        //if (task != this && rectTransformTemp.rect.Contains(pointerEventData.position))
                        //{
                        //    taskSelected = task;
                        //}
                    }
                    if (taskSelected != null)
                    {
                        AddParent(taskSelected);
                    }
                    else
                    {
                        if (_parent != null)
                        {
                            RemoveParent();
                        }
                    }
                });
                eventTrigger.triggers.Add(pointerUp);
            }
		}

        protected void UpdatePosition(bool isInited = false){
            var rectTransform = GetComponent<RectTransform>();
            Vector2 newPosition = Vector2.zero;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(UI, rectTransform.position, null, out newPosition);
            _rootTopPosition = new Vector2(newPosition.x, newPosition.y + rectTransform.sizeDelta.y / 2);
            _rootBottomPosition = new Vector2(newPosition.x, newPosition.y - rectTransform.sizeDelta.y / 2);
            var anchoredPosition = rectTransform.anchoredPosition;
            Task.Task.X = anchoredPosition.x;
            Task.Task.Y = anchoredPosition.y;
            if(!isInited){
                if (_parent != null)
                {
                    UpdateLink();
                }
                var offset = rectTransform.anchoredPosition - _prePosition;
                for (int i = 0; i < _children.Count; i++)
                {
                    var child = _children[i];
                    var rectChild = child.GetComponent<RectTransform>();
                    rectChild.anchoredPosition = rectChild.anchoredPosition + offset;
                    child.UpdatePosition();
                }
            }
            _prePosition = rectTransform.anchoredPosition;
        }

        public void UpdateLink(){
            var topChild = _parent._children.FindMaxOrMin((x, y) => x.Task.Task.Y < y.Task.Task.Y);

            List<Vector2> vector2s = new List<Vector2>();
            vector2s.Add(_rootTopPosition);
            vector2s.Add(new Vector3(_rootTopPosition.x, topChild.RootTopPosition.y + 60));
            vector2s.Add(new Vector3(_parent.RootBottomPosition.x, topChild.RootTopPosition.y + 60));
            vector2s.Add(_parent.RootBottomPosition);
            var rectTransform = GetComponent<RectTransform>();
            for (int i = 0; i < vector2s.Count - 1; i++){
                var startPosition = vector2s[i];
                var targetPosition = vector2s[i + 1];
                var direct = (targetPosition - startPosition).normalized;
                var distance = Vector2.Distance(targetPosition, startPosition);
                var line = lines[i];
                line.gameObject.SetActive(true);
                line.anchoredPosition = new Vector2(startPosition.x - rectTransform.anchoredPosition.x, startPosition.y - rectTransform.anchoredPosition.y);
                line.sizeDelta = new Vector2(line.sizeDelta.x, distance);
                var angle = MathCustom.GetAngleFromNagative180To180(direct, new Vector2(0, 1));
                if(targetPosition.x > startPosition.x)
                {
                    angle *= -1;
                }
                line.localEulerAngles = new Vector3(0, 0, angle);
            }
        }

        protected void AddChild(TaskView child){
            _children.Add(child);
            Task.IsValidate.Value = true;
        }

        protected void RemoveChild(TaskView child)
        {
            _children.Remove(child);
            if(_children.Count == 0){
                Task.IsValidate.Value = false;
            }
        }

        public void AddParent(TaskView parent){
            var parentTask = parent.Task.Task as ParentTask;
            if(parentTask != null){
                if(_parent != null){
                    RemoveParent();
                }
                //Debug.Log("Add link");
                if(parent._children.Count > 0 && parentTask as CompositeTask == null){
                    var childTaskOfParent = behaviourTreeEditor.TaskViews.FirstOrDefault(x => x == parent._children[0]);
                    childTaskOfParent.RemoveParent();
                }
                parent.AddChild(this);
                _parent = parent;
                var compositeTask = _parent.Task.Task as CompositeTask;
                if (compositeTask != null)
                {
                    _parent._children.Sort((x, y) =>
                    {
                        return x.Task.Task.X.CompareTo(y.Task.Task.X);
                    });
                }
                UpdateLink();
            } else{
                Debug.Log("Can't add link");
            }
        }

        public void RemoveParent()
        {
            if (_parent == null) return;
            //Debug.Log("Remove link");
            _parent.RemoveChild(this);
            _parent = null;
            for (int i = 0; i < lines.Length; i++){
                var line = lines[i];
                line.gameObject.SetActive(false);
            }
        }

        public void RemoveChildren(){
            for (int i = 0; i < _children.Count; i++){
                var child = _children[i];
                child.RemoveParent();
            }
        }

        public void Generator(){
            var parentTask = Task.Task as ParentTask;
            if(parentTask != null){
                parentTask.Tasks.Clear();
            }
            for (int i = 0; i < _children.Count; i++){
                var child = _children[i];
                child.Generator();
                if (parentTask != null)
                {
                    parentTask.AddTask(child.Task.Task);
                }
            }
        }

        public void CheckUpdateAbortType(){
            var compositeTask = Task.Task as CompositeTask;
            if(compositeTask != null){
                if(compositeTask.AbortType == AbortType.None){
                    abortTypeTxt.gameObject.SetActive(false);
                } else{
                    abortTypeTxt.gameObject.SetActive(true);
                    abortTypeTxt.text = compositeTask.AbortType.ToString();
                }
            }
        }
	}
}
