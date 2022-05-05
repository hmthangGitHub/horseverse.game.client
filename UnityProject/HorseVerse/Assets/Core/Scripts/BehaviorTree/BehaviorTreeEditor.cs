using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Core.MVVM;
using System;
using UniRx;
using UnityEngine.UI;
using SQLite4Unity3d;
using System.Linq;
using CoreData;
using System.IO;
using Core.Behavior;
using Core.MVVM.UI;

namespace Core.Behavior.UI
{
    public class BehaviorTreeEditor : MonoBehaviour
    {
        List<BehaviorTree.PropertyVariable> _listPropertyVariable = new List<BehaviorTree.PropertyVariable>();
        [SerializeField]
        string fileName;
        [HideInInspector]
        public List<TaskView> TaskViews = new List<TaskView>();
        public BehaviorTree BehaviourTree;
        public GameObject ItemMenuTaskPrefab;
        public GameObject ItemTaskPrefab;
        public GameObject Container;
        TaskView _rootTask;
        readonly float _scaleDefault = 1;
        [SerializeField]
        Slider sliderScale;

        [SerializeField]
        Button[] tabs;
        [SerializeField]
        GameObject taskViewDetailMenu;
        [SerializeField]
        GameObject tasksMenu;
        [SerializeField]
        GameObject propertiesMenu;
        GameObject _propertyContainer;
        protected Color colorSelected = new Color(1, 0, 0, 1);
        protected Color colorNormal = new Color(96f / 255, 96f / 255, 96f / 255, 1);
        [SerializeField]
        public GameObject popupDelete;
        [SerializeField]
        public Button yesBtn;
        [SerializeField]
        public Button noBtn;

        GameObject _taskContainer;
        GameObject _itemShareString;
        GameObject _itemShareInt;
        GameObject _itemShareFloat;
        GameObject _itemShareBool;
        GameObject _itemShareVector3;
        GameObject _itemShareTransform;
        GameObject _itemShareListTransform;
        GameObject _itemEnum;
        GameObject _itemString;

        GameObject _itemPropertyShareString;
        GameObject _itemPropertyShareInt;
        GameObject _itemPropertyShareFloat;
        GameObject _itemPropertyShareBool;
        GameObject _itemPropertyShareVector3;
        GameObject _itemPropertyShareTransform;
        GameObject _itemPropertyShareListTransform;

        protected List<Type> _types = new List<Type>();
		// Use this for initialization
		void Start()
        {
            _taskContainer = taskViewDetailMenu.FindGameObjectByName("Content", true);
            _itemShareString = _taskContainer.FindGameObjectByName("ItemShareStringBehavior", true);
            _itemShareInt = _taskContainer.FindGameObjectByName("ItemShareStringBehavior", true);
            _itemShareFloat = _taskContainer.FindGameObjectByName("ItemShareFloatBehavior", true);
            _itemShareBool = _taskContainer.FindGameObjectByName("ItemShareBoolBehavior", true);
            _itemShareVector3 = _taskContainer.FindGameObjectByName("ItemShareVector3Behavior", true);
            _itemShareTransform = _taskContainer.FindGameObjectByName("ItemShareTransformBehavior", true);
            _itemShareListTransform = _taskContainer.FindGameObjectByName("ItemShareListTransformBehavior", true);
            _itemEnum = _taskContainer.FindGameObjectByName("ItemEnum", true);
            _itemString = _taskContainer.FindGameObjectByName("ItemString", true);

            var scrollView = propertiesMenu.FindGameObjectByName("ScrollView", true);
            _propertyContainer = scrollView.FindGameObjectByName("Content", true);
            _itemPropertyShareString = _propertyContainer.FindGameObjectByName("ItemPropertyShareStringBehavior", true);
            _itemPropertyShareInt = _propertyContainer.FindGameObjectByName("ItemPropertyShareIntBehavior", true);
            _itemPropertyShareFloat = _propertyContainer.FindGameObjectByName("ItemPropertyShareFloatBehavior", true);
            _itemPropertyShareBool = _propertyContainer.FindGameObjectByName("ItemPropertyShareBoolBehavior", true);
            _itemPropertyShareVector3 = _propertyContainer.FindGameObjectByName("ItemPropertyShareVector3Behavior", true);
            _itemPropertyShareTransform = _propertyContainer.FindGameObjectByName("ItemPropertyShareTransformBehavior", true);
            _itemPropertyShareListTransform = _propertyContainer.FindGameObjectByName("ItemPropertyShareListTransformBehavior", true);

            List<Type> typesProperty = new List<Type>();
            typesProperty.Add(typeof(ShareIntBehavior));
            typesProperty.Add(typeof(ShareFloatBehavior));
            typesProperty.Add(typeof(ShareStringBehavior));
            typesProperty.Add(typeof(ShareBoolBehavior));
            typesProperty.Add(typeof(ShareVector3Behavior));
            typesProperty.Add(typeof(ShareTransformViewModelBehavior));
            typesProperty.Add(typeof(ShareListTransformViewModelBehavior));

            noBtn.onClick.AddListener(() =>
            {
                popupDelete.SetActive(false);
            });

            {
                var dropdown = propertiesMenu.GetComponentsInChildren<Dropdown>(true)[0];
                for (int i = 0; i < typesProperty.Count; i++)
                {
                    Dropdown.OptionData optionData = new Dropdown.OptionData();
                    var text = typesProperty[i].ToString();
                    var list = text.Split('.');
                    optionData.text = list[list.Length - 1];
                    dropdown.options.Add(optionData);
                }
                dropdown.value = -1;
                dropdown.value = 0;
                var inputFiled = propertiesMenu.GetComponentsInChildren<InputField>(true)[0];
                var buttonAdd = propertiesMenu.GetComponentsInChildren<Button>(true)[0];
                buttonAdd.onClick.AddListener(() =>
                {
                    var type = typesProperty[dropdown.value];
                    CreateProperty(type, inputFiled.text);
                });
            }


            for (int i = 0; i < tabs.Length; i++){
                var tabBtn = tabs[i];
                int index = i;
                tabBtn.onClick.AddListener(() =>
                {
                    OpenTabs(index);
                });
            }
            OpenTabs(0);

            BehaviourTree = new BehaviorTree();
            _types.Add(typeof(SelectorTask));
            _types.Add(typeof(SequenceTask));
            _types.Add(typeof(ParallelTask));
            _types.Add(typeof(ParallelSelectorTask));
            _types.Add(typeof(RepeatTask));
            _types.Add(typeof(CompareWithShareIntTask));
            _types.Add(typeof(CompareWithShareFloatTask));
            _types.Add(typeof(CompareWithShareBoolTask));
            _types.Add(typeof(CompareWithShareVector3Task));
            _types.Add(typeof(CompareWithShareTransformViewModelTask));
            _types.Add(typeof(HasReceiveEventTask));
            _types.Add(typeof(SendEventTask));
            _types.Add(typeof(WaitingTimeTask));
            _types.Add(typeof(LogTask));
            _types.Add(typeof(InvertTask));
            _types.Add(typeof(SuccessTask));
            _types.Add(typeof(FailureTask));
            _types.Add(typeof(UntilSuccessTask));
            _types.Add(typeof(Movement.CanSeeObjectTask));
            _types.Add(typeof(Movement.ChaseTask));
            _types.Add(typeof(Movement.HikeInCirclesTask));
            _types.Add(typeof(Movement.MovementNavMeshTask));
            _types.Add(typeof(Movement.MoveTowardTask));
            _types.Add(typeof(Movement.RotateAroundTask));
            _types.Add(typeof(Movement.RotationTask));
            _types.Add(typeof(Movement.WanderTask));
            _types.Add(typeof(Movement.WithinDistanceTask));
            // add custom
            var customTypes = BehaviorTreeGetCustomTask.GetTypes();
            for (int i = 0; i < customTypes.Count; i++){
                var type = customTypes[i];
                if(_types.Any(x => x.ToString() == type.ToString())){
                    continue;
                }
                _types.Add(type);
            }
            //

            List<ItemBasicView> menuItems = new List<ItemBasicView>();
            var rectTransform = Container.GetComponent<RectTransform>();
            for (int i = 0; i < _types.Count; i++)
            {
                var typeOfModelName = _types[i].ToString();
                var menuItem = transform.InstantiateView<ItemBasicView>(null, ItemMenuTaskPrefab, ItemMenuTaskPrefab.transform.parent.gameObject);
                menuItem.gameObject.SetActive(true);
                menuItems.Add(menuItem);
                var list = typeOfModelName.Split('.');
                menuItem.GetComponentInChildren<Text>().text = list[list.Length - 1];
                menuItem.ItemBasic.OnClickCommand.Subscribe(_ =>
                {
                    var obj = GameCommon.CreateObjectByClassName(typeOfModelName, "Assembly-CSharp");

                    CreateTaskView(obj as Task, true);
                });
            }

            sliderScale.onValueChanged.AddListener(value =>
            {
                var scale = _scaleDefault - value / 2;
                rectTransform.localScale = new Vector3(scale, scale, scale);
            });
            sliderScale.value = 0.5f;

            // create root
            var rootVM = MVVM.MVVM.CreateViewModel<TaskViewModel>();
            rootVM.Task = new ParentTask();
            var rootV = transform.InstantiateView<TaskView>(rootVM, ItemTaskPrefab, Container);
            rootV.gameObject.SetActive(true);
            rootV.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, 400);
            rootV.gameObject.name = "Root";
            rootV.nameTxt.text = "Root";
            rootV.Task.InitTaskCommand.Execute(new InitTaskCommand());
            rootV.linkBtn.gameObject.SetActive(false);
            rootV.openBtn.gameObject.SetActive(false);
            rootV.deleteBtn.gameObject.SetActive(false);
            _rootTask = rootV;

            var folder = Application.dataPath + "/Resources" + "/Behavior";
            var file = folder + "/" + fileName + ".txt";
            if (File.Exists(file))
            {
                var json = System.IO.File.ReadAllText(file);
                LoadData(json);
            }

            var searchTask = tasksMenu.FindGameObjectByName("SearchTask", true).GetComponent<InputField>();
            searchTask.onValueChanged.AddListener(value =>
            {
                for (int i = 0; i < menuItems.Count; i++){
                    var itemMenu = menuItems[i];
                    var nameText = itemMenu.GetComponentInChildren<Text>(true);
                    itemMenu.gameObject.SetActive(nameText.text.ToLower().Contains(value.ToLower()));
                }
            });
        }

        void OpenTabs(int index){
            for (int i = 0; i < tabs.Length; i++){
                var tabBtn = tabs[i];
                tabBtn.GetComponent<Image>().color = i == index ? colorSelected : colorNormal;
            }
            if (index == 0)
            {
                tasksMenu.SetActive(true);
                propertiesMenu.SetActive(false);
                taskViewDetailMenu.SetActive(false);
            }
            else if (index == 1)
            {
                propertiesMenu.SetActive(true);
                tasksMenu.SetActive(false);
                taskViewDetailMenu.SetActive(false);
            }
            else
            {
                taskViewDetailMenu.SetActive(true);
                tasksMenu.SetActive(false);
                propertiesMenu.SetActive(false);
            }
        }

        public void Save(){
            bool passValidate = true;
            for (int i = 0; i < TaskViews.Count; i++){
                var taskV = TaskViews[i];
                if(!taskV.Task.IsValidate.Value){
                    passValidate = false;
                    break;
                }
            }
            if(!passValidate){
                Debug.LogError("Please check validate!!!");
                return;
            }
            _rootTask.Generator();
            var parentTask = _rootTask.Task.Task as ParentTask;
            if (parentTask.Tasks.Count == 0)
            {
                return;
            }
            BehaviourTree = new BehaviorTree();
            BehaviourTree.ListPropertyVariable.AddRange(_listPropertyVariable);
            BehaviourTree.AddStartTask(parentTask.Tasks[0]);
            var json = BehaviourTree.GetJson();
            var folder = Application.dataPath + "/Resources" + "/Behavior";
            if (!Directory.Exists(folder)){
                Directory.CreateDirectory(folder);
            }
            System.IO.File.WriteAllText(folder + "/" + fileName + ".txt", json);
            json = System.IO.File.ReadAllText(folder + "/" + fileName + ".txt");
            BehaviorTree test = new BehaviorTree();
            test.ParseFromJson(json);
            Debug.Log(test.GetJson());
        }

        void LoadData(string json){
            BehaviourTree.ParseFromJson(json);
            Debug.Log("LoadData: \n" + json);
            for (int i = 0; i < BehaviourTree.ListPropertyVariable.Count; i++){
                var shareProperty = BehaviourTree.ListPropertyVariable[i];
                CreateProperty(shareProperty.Value.GetType(), shareProperty.Key, shareProperty.Value);
            }
            LoadParentTask(BehaviourTree, _rootTask);
        }

        void LoadParentTask(ParentTask task, TaskView parentTaskView){
            for (int i = 0; i < task.Tasks.Count; i++)
            {
                var child = task.Tasks[i];
                LoadTask(child, parentTaskView);
            }
        }

        TaskView CreateTaskView(Task task, bool isNewCreate = false){
            var taskVM = MVVM.MVVM.CreateViewModel<TaskViewModel>();
            taskVM.Task = task;
            var taskV = transform.InstantiateView<TaskView>(taskVM, ItemTaskPrefab, Container);
            taskV.gameObject.SetActive(true);
            var list = task.TypeNameOfModel.Split('.');
            taskV.gameObject.name = list[list.Length - 1];
            taskV.nameTxt.text = list[list.Length - 1];
            if (isNewCreate)
            {
                var parentRectTransform = Container.GetComponent<RectTransform>();
                taskV.GetComponent<RectTransform>().anchoredPosition = new Vector2(-parentRectTransform.anchoredPosition.x + 100, -parentRectTransform.anchoredPosition.y) / parentRectTransform.transform.localScale.x;
            }
            else
            {
                taskV.GetComponent<RectTransform>().anchoredPosition = new Vector2(task.X, task.Y);
            }
            taskV.openBtn.onClick.AddListener(() =>
            {
                OpenTabs(2);
                OpenTaskViewDetail(taskV);
            });
            taskV.deleteBtn.onClick.AddListener(() =>
            {
                popupDelete.SetActive(true);
                yesBtn.onClick.RemoveAllListeners();
                yesBtn.onClick.AddListener(() =>
                {
                    popupDelete.SetActive(false);
                    var parentTask = taskV.Task.Task as ParentTask;
                    if (parentTask != null)
                    {
                        taskV.RemoveChildren();
                    }
                    taskV.RemoveParent();
                    TaskViews.Remove(taskV);
                    Destroy(taskV.gameObject);
                });
            });
            taskV.Task.InitTaskCommand.Execute(new InitTaskCommand());
            return taskV;
        }

        void LoadTask(Task task, TaskView parentTaskView){
            var taskV = CreateTaskView(task);
            taskV.AddParent(parentTaskView);
            var childIsParentTask = task as ParentTask;
            if(childIsParentTask != null){
                LoadParentTask(childIsParentTask, taskV);
            }
        }

        public void OpenTaskViewDetail(TaskView taskView){
            taskViewDetailMenu.SetActive(true);
            // clear 
            var children = _taskContainer.GetComponentsInChildren<ItemBasicView>();
            foreach(var child in children){
                Destroy(child.gameObject);
            }
            var task = taskView.Task.Task;
            var type = task.GetType();
            var properties = type.GetProperties();
            foreach (var propertyInfo in properties)
            {
                var nameField = propertyInfo.Name;
                var firstString = nameField.Substring(0, 1).ToLower();
                var primaryKeySerialized = Attribute.GetCustomAttribute(propertyInfo, typeof(PrimaryKeyAttribute)) as PrimaryKeyAttribute;
                if (primaryKeySerialized != null)
                {
                    nameField = "_" + firstString + nameField.Substring(1);
                }
                else
                {
                    nameField = firstString + nameField.Substring(1);
                }
                var ignoreSerialized = Attribute.GetCustomAttribute(propertyInfo, typeof(IgnoreAttribute)) as IgnoreAttribute;
                var ignoreShowUISerialized = Attribute.GetCustomAttribute(propertyInfo, typeof(IgnoreShowUIAttribute)) as IgnoreShowUIAttribute;
                if (ignoreSerialized == null && ignoreShowUISerialized == null)
                {
                    if (propertyInfo.PropertyType == typeof(string))
                    {
                        var data = propertyInfo.GetValue(task, null) as string;
                        var itemV = transform.InstantiateView<ItemBasicView>(null, _itemString, _taskContainer);
                        itemV.gameObject.SetActive(true);
                        itemV.gameObject.transform.SetSiblingIndex(0);
                        itemV.GetComponentsInChildren<Text>()[0].text = nameField;
                        var inputField = itemV.GetComponentsInChildren<InputField>()[0];
                        inputField.text = data;
                        inputField.onValueChanged.AddListener(result =>
                        {
                            propertyInfo.SetValue(task, result, null);
                        });
                    }
                    else if (propertyInfo.PropertyType == typeof(ShareIntBehavior))
                    {
                        var data = propertyInfo.GetValue(task, null) as ShareIntBehavior;
                        if(data == null)
                        {
                            data = new ShareIntBehavior();
                            propertyInfo.SetValue(task, data);
                        }
                        var itemV = transform.InstantiateView<ItemBasicView>(null, _itemShareInt, _taskContainer);
                        var disablePanel = itemV.gameObject.FindGameObjectByName("Disable", true);
                        itemV.gameObject.SetActive(true);
                        itemV.gameObject.transform.SetSiblingIndex(0);
                        itemV.GetComponentsInChildren<Text>()[0].text = nameField;
                        var inputField = itemV.GetComponentsInChildren<InputField>()[0];
                        inputField.onValueChanged.AddListener(result =>
                        {
                            if (result == "")
                            {
                                result = "0";
                                inputField.text = result;
                            }
                            data.Value = int.Parse(result);
                        });
                        inputField.text = data.Value + "";


                        var dropdown = itemV.GetComponentsInChildren<Dropdown>()[0];
                        {
                            Dropdown.OptionData optionData = new Dropdown.OptionData();
                            optionData.text = "";
                            dropdown.options.Add(optionData);
                        }
                        var list = _listPropertyVariable.Where(x => x.Value as ShareIntBehavior != null).ToList();
                        for (int i = 0; i < list.Count; i++){
                            Dropdown.OptionData optionData = new Dropdown.OptionData();
                            optionData.text = list[i].Key;
                            dropdown.options.Add(optionData);
                        }

                        var current = dropdown.options.FindIndex(x => x.text == data.VariableKey);
                        if(current == -1){
                            current = 0;
                        }
                        dropdown.onValueChanged.AddListener((result) =>
                        {
                            disablePanel.SetActive(result > 0);
                            data.VariableKey = dropdown.options[result].text;
                            if(!string.IsNullOrEmpty(data.VariableKey)){
                                var shareIntBahaviour = list[result - 1].Value as ShareIntBehavior;
                                inputField.text = shareIntBahaviour.Value + "";
                            }
                        });
                        dropdown.value = current;
                    } else if (propertyInfo.PropertyType == typeof(ShareFloatBehavior))
                    {
                        var data = propertyInfo.GetValue(task, null) as ShareFloatBehavior;
                        if(data == null)
                        {
                            data = new ShareFloatBehavior();
                            propertyInfo.SetValue(task, data);
                        }
                        var itemV = transform.InstantiateView<ItemBasicView>(null, _itemShareFloat, _taskContainer);
                        var disablePanel = itemV.gameObject.FindGameObjectByName("Disable", true);
                        itemV.gameObject.SetActive(true);
                        itemV.gameObject.transform.SetSiblingIndex(0);
                        itemV.GetComponentsInChildren<Text>()[0].text = nameField;
                        var inputField = itemV.GetComponentsInChildren<InputField>()[0];
                        inputField.text = data.Value + "";
                        inputField.onValueChanged.AddListener(result =>
                        {
                            if (result == "")
                            {
                                result = "0";
                                inputField.text = result;
                            }
                            data.Value = float.Parse(result);
                        });


                        var dropdown = itemV.GetComponentsInChildren<Dropdown>()[0];
                        {
                            Dropdown.OptionData optionData = new Dropdown.OptionData();
                            optionData.text = "";
                            dropdown.options.Add(optionData);
                        }
                        var list = _listPropertyVariable.Where(x => x.Value as ShareFloatBehavior != null).ToList();
                        for (int i = 0; i < list.Count; i++)
                        {
                            Dropdown.OptionData optionData = new Dropdown.OptionData();
                            optionData.text = list[i].Key;
                            dropdown.options.Add(optionData);
                        }

                        var current = dropdown.options.FindIndex(x => x.text == data.VariableKey);
                        if (current == -1)
                        {
                            current = 0;
                        }
                        dropdown.onValueChanged.AddListener((result) =>
                        {
                            disablePanel.SetActive(result > 0);
                            data.VariableKey = dropdown.options[result].text;
                            if (!string.IsNullOrEmpty(data.VariableKey))
                            {
                                var shareFloatBahaviour = list[result - 1].Value as ShareFloatBehavior;
                                inputField.text = shareFloatBahaviour.Value + "";
                            }
                        });
                        dropdown.value = current;
                    } else if (propertyInfo.PropertyType == typeof(ShareStringBehavior))
                    {
                        var data = propertyInfo.GetValue(task, null) as ShareStringBehavior;
                        if(data == null)
                        {
                            data = new ShareStringBehavior();
                            propertyInfo.SetValue(task, data);
                        }
                        var itemV = transform.InstantiateView<ItemBasicView>(null, _itemShareString, _taskContainer);
                        var disablePanel = itemV.gameObject.FindGameObjectByName("Disable", true);
                        itemV.gameObject.SetActive(true);
                        itemV.gameObject.transform.SetSiblingIndex(0);
                        itemV.GetComponentsInChildren<Text>()[0].text = nameField;
                        var inputField = itemV.GetComponentsInChildren<InputField>()[0];
                        inputField.text = data.Value + "";
                        inputField.onValueChanged.AddListener(result =>
                        {
                            data.Value = result;
                        });

                        var dropdown = itemV.GetComponentsInChildren<Dropdown>()[0];
                        {
                            Dropdown.OptionData optionData = new Dropdown.OptionData();
                            optionData.text = "";
                            dropdown.options.Add(optionData);
                        }
                        var list = _listPropertyVariable.Where(x => x.Value as ShareStringBehavior != null).ToList();
                        for (int i = 0; i < list.Count; i++)
                        {
                            Dropdown.OptionData optionData = new Dropdown.OptionData();
                            optionData.text = list[i].Key;
                            dropdown.options.Add(optionData);
                        }

                        var current = dropdown.options.FindIndex(x => x.text == data.VariableKey);
                        if (current == -1)
                        {
                            current = 0;
                        }
                        dropdown.onValueChanged.AddListener((result) =>
                        {
                            disablePanel.SetActive(result > 0);
                            data.VariableKey = dropdown.options[result].text;
                            if (!string.IsNullOrEmpty(data.VariableKey))
                            {
                                var shareStringBahaviour = list[result - 1].Value as ShareStringBehavior;
                                inputField.text = shareStringBahaviour.Value;
                            }
                        });
                        dropdown.value = current;
                    } else if (propertyInfo.PropertyType == typeof(ShareBoolBehavior))
                    {
                        var data = propertyInfo.GetValue(task, null) as ShareBoolBehavior;
                        if(data == null)
                        {
                            data = new ShareBoolBehavior();
                            propertyInfo.SetValue(task, data);
                        }
                        var itemV = transform.InstantiateView<ItemBasicView>(null, _itemShareBool, _taskContainer);
                        var disablePanel = itemV.gameObject.FindGameObjectByName("Disable", true);
                        itemV.gameObject.SetActive(true);
                        itemV.gameObject.transform.SetSiblingIndex(0);
                        itemV.GetComponentsInChildren<Text>()[0].text = nameField;
                        var toogle = itemV.GetComponentsInChildren<Toggle>()[0];
                        toogle.isOn = data.Value;
                        toogle.onValueChanged.AddListener(result =>
                        {
                            data.Value = result;
                        });

                        var dropdown = itemV.GetComponentsInChildren<Dropdown>()[0];
                        {
                            Dropdown.OptionData optionData = new Dropdown.OptionData();
                            optionData.text = "";
                            dropdown.options.Add(optionData);
                        }
                        var list = _listPropertyVariable.Where(x => x.Value as ShareBoolBehavior != null).ToList();
                        for (int i = 0; i < list.Count; i++)
                        {
                            Dropdown.OptionData optionData = new Dropdown.OptionData();
                            optionData.text = list[i].Key;
                            dropdown.options.Add(optionData);
                        }

                        var current = dropdown.options.FindIndex(x => x.text == data.VariableKey);
                        if (current == -1)
                        {
                            current = 0;
                        }
                        dropdown.onValueChanged.AddListener((result) =>
                        {
                            disablePanel.SetActive(result > 0);
                            data.VariableKey = dropdown.options[result].text;
                            if (!string.IsNullOrEmpty(data.VariableKey))
                            {
                                var shareBoolBahaviour = list[result - 1].Value as ShareBoolBehavior;
                                toogle.isOn = shareBoolBahaviour.Value;
                            }
                        });
                        dropdown.value = current;
                    } else if (propertyInfo.PropertyType == typeof(ShareVector3Behavior))
                    {
                        var data = propertyInfo.GetValue(task, null) as ShareVector3Behavior;
                        if(data == null)
                        {
                            data = new ShareVector3Behavior();
                            propertyInfo.SetValue(task, data);
                        }
                        var itemV = transform.InstantiateView<ItemBasicView>(null, _itemShareVector3, _taskContainer);
                        var disablePanel = itemV.gameObject.FindGameObjectByName("Disable", true);
                        itemV.gameObject.SetActive(true);
                        itemV.gameObject.transform.SetSiblingIndex(0);
                        itemV.GetComponentsInChildren<Text>()[0].text = nameField;
                        var inputFieldX = itemV.GetComponentsInChildren<InputField>()[0];
                        inputFieldX.text = data.Value + "";
                        inputFieldX.onValueChanged.AddListener(result =>
                        {
                            if (result == "")
                            {
                                result = "0";
                                inputFieldX.text = result;
                            }
                            data.Value = new Vector3(float.Parse(result), data.Value.y, data.Value.z);
                        });
                        var inputFieldY = itemV.GetComponentsInChildren<InputField>()[1];
                        inputFieldY.text = data.Value + "";
                        inputFieldY.onValueChanged.AddListener(result =>
                        {
                            if (result == "")
                            {
                                result = "0";
                                inputFieldY.text = result;
                            }
                            data.Value = new Vector3(data.Value.x, float.Parse(result), data.Value.z);
                        });
                        var inputFieldZ = itemV.GetComponentsInChildren<InputField>()[2];
                        inputFieldZ.text = data.Value + "";
                        inputFieldZ.onValueChanged.AddListener(result =>
                        {
                            if (result == "")
                            {
                                result = "0";
                                inputFieldZ.text = result;
                            }
                            data.Value = new Vector3(data.Value.x, data.Value.y, float.Parse(result));
                        });


                        var dropdown = itemV.GetComponentsInChildren<Dropdown>()[0];
                        {
                            Dropdown.OptionData optionData = new Dropdown.OptionData();
                            optionData.text = "";
                            dropdown.options.Add(optionData);
                        }
                        var list = _listPropertyVariable.Where(x => x.Value as ShareVector3Behavior != null).ToList();
                        for (int i = 0; i < list.Count; i++)
                        {
                            Dropdown.OptionData optionData = new Dropdown.OptionData();
                            optionData.text = list[i].Key;
                            dropdown.options.Add(optionData);
                        }

                        var current = dropdown.options.FindIndex(x => x.text == data.VariableKey);
                        if (current == -1)
                        {
                            current = 0;
                        }
                        dropdown.onValueChanged.AddListener((result) =>
                        {
                            disablePanel.SetActive(result > 0);
                            data.VariableKey = dropdown.options[result].text;
                            if (!string.IsNullOrEmpty(data.VariableKey))
                            {
                                var shareVector3Bahaviour = list[result - 1].Value as ShareVector3Behavior;
                                inputFieldX.text = shareVector3Bahaviour.Value.x + "";
                                inputFieldY.text = shareVector3Bahaviour.Value.y + "";
                                inputFieldZ.text = shareVector3Bahaviour.Value.z + "";
                            }
                        });
                        dropdown.value = current;
                    } else if (propertyInfo.PropertyType == typeof(ShareTransformViewModelBehavior))
                    {
                        var data = propertyInfo.GetValue(task, null) as ShareTransformViewModelBehavior;
                        if(data == null)
                        {
                            data = new ShareTransformViewModelBehavior();
                            propertyInfo.SetValue(task, data);
                        }
                        var itemV = transform.InstantiateView<ItemBasicView>(null, _itemShareTransform, _taskContainer);
                        itemV.gameObject.SetActive(true);
                        itemV.gameObject.transform.SetSiblingIndex(0);
                        itemV.GetComponentsInChildren<Text>()[0].text = nameField;

                        var dropdown = itemV.GetComponentsInChildren<Dropdown>()[0];
                        {
                            Dropdown.OptionData optionData = new Dropdown.OptionData();
                            optionData.text = "";
                            dropdown.options.Add(optionData);
                        }
                        var list = _listPropertyVariable.Where(x => x.Value as ShareTransformViewModelBehavior != null).ToList();
                        for (int i = 0; i < list.Count; i++)
                        {
                            Dropdown.OptionData optionData = new Dropdown.OptionData();
                            optionData.text = list[i].Key;
                            dropdown.options.Add(optionData);
                        }

                        var current = dropdown.options.FindIndex(x => x.text == data.VariableKey);
                        if (current == -1)
                        {
                            current = 0;
                        }
                        dropdown.onValueChanged.AddListener((result) =>
                        {
                            data.VariableKey = dropdown.options[result].text;
                        });
                        dropdown.value = current;
                    } else if (propertyInfo.PropertyType == typeof(ShareListTransformViewModelBehavior))
                    {
                        var data = propertyInfo.GetValue(task, null) as ShareListTransformViewModelBehavior;
                        if(data == null)
                        {
                            data = new ShareListTransformViewModelBehavior();
                        }
                        var itemV = transform.InstantiateView<ItemBasicView>(null, _itemShareListTransform, _taskContainer);
                        itemV.gameObject.SetActive(true);
                        itemV.gameObject.transform.SetSiblingIndex(0);
                        itemV.GetComponentsInChildren<Text>()[0].text = nameField;

                        var dropdown = itemV.GetComponentsInChildren<Dropdown>()[0];
                        {
                            Dropdown.OptionData optionData = new Dropdown.OptionData();
                            optionData.text = "";
                            dropdown.options.Add(optionData);
                        }
                        var list = _listPropertyVariable.Where(x => x.Value as ShareListTransformViewModelBehavior != null).ToList();
                        for (int i = 0; i < list.Count; i++)
                        {
                            Dropdown.OptionData optionData = new Dropdown.OptionData();
                            optionData.text = list[i].Key;
                            dropdown.options.Add(optionData);
                        }

                        var current = dropdown.options.FindIndex(x => x.text == data.VariableKey);
                        if (current == -1)
                        {
                            current = 0;
                        }
                        dropdown.onValueChanged.AddListener((result) =>
                        {
                            data.VariableKey = dropdown.options[result].text;
                        });
                        dropdown.value = current;
                    } else if (propertyInfo.PropertyType.IsEnum)
                    {
                        var itemV = transform.InstantiateView<ItemBasicView>(null, _itemEnum, _taskContainer);
                        itemV.gameObject.SetActive(true);
                        itemV.gameObject.transform.SetSiblingIndex(0);
                        itemV.GetComponentsInChildren<Text>()[0].text = nameField;
                        var dropdown = itemV.GetComponentsInChildren<Dropdown>()[0];
                        var list = Enum.GetNames(propertyInfo.PropertyType);
                        for (int i = 0; i < list.Length; i++)
                        {
                            Dropdown.OptionData optionData = new Dropdown.OptionData();
                            optionData.text = list[i];
                            dropdown.options.Add(optionData);
                        }
                        var value = Enum.Parse(propertyInfo.PropertyType, propertyInfo.GetValue(task, null).ToString(), true);
                        var current = dropdown.options.FindIndex(x => x.text == value.ToString());
                        dropdown.value = -1;
                        dropdown.value = current;
                        dropdown.onValueChanged.AddListener((result) =>
                        {
                            propertyInfo.SetValue(task, Enum.Parse(propertyInfo.PropertyType, list[result], true), null);
                            if(propertyInfo.PropertyType == typeof(AbortType)){
                                taskView.CheckUpdateAbortType();
                            }
                        });
                    }
                }
            }
        }

        void CreateProperty(Type type, string key, SharePropertyBehavior sharePropertyBehaviour = null){
            if(string.IsNullOrEmpty(key)){
                Debug.LogError("Please input key");
                return;
            }
            if(_listPropertyVariable.Any(x => x.Key == key)){
                Debug.LogError("Existed key: " + key);
                return;
            }
            if(sharePropertyBehaviour == null){
                sharePropertyBehaviour = Activator.CreateInstance(type) as SharePropertyBehavior;
                sharePropertyBehaviour.VariableKey = key;
            }
            ItemBasicView itemV = null;
            BehaviorTree.PropertyVariable propertyVariable = new BehaviorTree.PropertyVariable();
            propertyVariable.Key = key;
            propertyVariable.Value = sharePropertyBehaviour;
            _listPropertyVariable.Add(propertyVariable);
            if(type == typeof(ShareIntBehavior)){
                var shareIntProperty = sharePropertyBehaviour as ShareIntBehavior;
                itemV = transform.InstantiateView<ItemBasicView>(null, _itemPropertyShareInt, _propertyContainer);
                itemV.gameObject.SetActive(true);
                itemV.GetComponentsInChildren<Text>()[0].text = key;
                var inputField = itemV.GetComponentsInChildren<InputField>()[0];
                inputField.text = shareIntProperty.Value.ToString();
                inputField.onValueChanged.AddListener(result =>
                {
                    if (result == "")
                    {
                        result = "0";
                        inputField.text = result;
                    }
                    shareIntProperty.Value = int.Parse(result);           
                });
            } else if (type == typeof(ShareFloatBehavior))
            {
                var shareFloatProperty = sharePropertyBehaviour as ShareFloatBehavior;
                itemV = transform.InstantiateView<ItemBasicView>(null, _itemPropertyShareFloat, _propertyContainer);
                itemV.gameObject.SetActive(true);
                itemV.GetComponentsInChildren<Text>()[0].text = key;
                var inputField = itemV.GetComponentsInChildren<InputField>()[0];
                inputField.text = shareFloatProperty.Value.ToString();
                inputField.onValueChanged.AddListener(result =>
                {
                    if (result == "")
                    {
                        result = "0";
                        inputField.text = result;
                    }
                    shareFloatProperty.Value = float.Parse(result);
                });
            } else if (type == typeof(ShareStringBehavior))
            {
                var shareStringProperty = sharePropertyBehaviour as ShareStringBehavior;
                itemV = transform.InstantiateView<ItemBasicView>(null, _itemPropertyShareString, _propertyContainer);
                itemV.gameObject.SetActive(true);
                itemV.GetComponentsInChildren<Text>()[0].text = key;
                var inputField = itemV.GetComponentsInChildren<InputField>()[0];
                inputField.text = shareStringProperty.Value;
                inputField.onValueChanged.AddListener(result =>
                {
                    shareStringProperty.Value = result;
                });
            } else if (type == typeof(ShareBoolBehavior))
            {
                var shareBoolProperty = sharePropertyBehaviour as ShareBoolBehavior;
                itemV = transform.InstantiateView<ItemBasicView>(null, _itemPropertyShareBool, _propertyContainer);
                itemV.gameObject.SetActive(true);
                itemV.GetComponentsInChildren<Text>()[0].text = key;
                var toggle = itemV.GetComponentsInChildren<Toggle>()[0];
                toggle.isOn = shareBoolProperty.Value;
                toggle.onValueChanged.AddListener(result =>
                {
                    shareBoolProperty.Value = result;
                });
            } else if (type == typeof(ShareVector3Behavior))
            {
                var shareVector3Property = sharePropertyBehaviour as ShareVector3Behavior;
                itemV = transform.InstantiateView<ItemBasicView>(null, _itemPropertyShareVector3, _propertyContainer);
                itemV.gameObject.SetActive(true);
                itemV.GetComponentsInChildren<Text>()[0].text = key;
                var inputFieldX = itemV.GetComponentsInChildren<InputField>()[0];
                inputFieldX.text = shareVector3Property.Value.x + "";
                inputFieldX.onValueChanged.AddListener(result =>
                {
                    shareVector3Property.Value = new Vector3(float.Parse(result), shareVector3Property.Value.y, shareVector3Property.Value.z);
                });
                var inputFieldY = itemV.GetComponentsInChildren<InputField>()[1];
                inputFieldY.text = shareVector3Property.Value.y + "";
                inputFieldY.onValueChanged.AddListener(result =>
                {
                    shareVector3Property.Value = new Vector3(shareVector3Property.Value.x, float.Parse(result), shareVector3Property.Value.z);
                });
                var inputFieldZ = itemV.GetComponentsInChildren<InputField>()[2];
                inputFieldZ.text = shareVector3Property.Value.z + "";
                inputFieldZ.onValueChanged.AddListener(result =>
                {
                    shareVector3Property.Value = new Vector3(shareVector3Property.Value.x, shareVector3Property.Value.y, float.Parse(result));
                });
            } else if (type == typeof(ShareTransformViewModelBehavior))
            {
                var shareTransformProperty = sharePropertyBehaviour as ShareTransformViewModelBehavior;
                itemV = transform.InstantiateView<ItemBasicView>(null, _itemPropertyShareTransform, _propertyContainer);
                itemV.gameObject.SetActive(true);
                itemV.GetComponentsInChildren<Text>()[0].text = key;
            } else if (type == typeof(ShareListTransformViewModelBehavior))
            {
                var shareListTransformProperty = sharePropertyBehaviour as ShareListTransformViewModelBehavior;
                itemV = transform.InstantiateView<ItemBasicView>(null, _itemPropertyShareListTransform, _propertyContainer);
                itemV.gameObject.SetActive(true);
                itemV.GetComponentsInChildren<Text>()[0].text = key;
            }
            var removeBtn = itemV.GetComponentsInChildren<Button>()[0];
            removeBtn.onClick.AddListener(() =>
            {
                popupDelete.SetActive(true);
                yesBtn.onClick.RemoveAllListeners();
                yesBtn.onClick.AddListener(() =>
                {
                    popupDelete.SetActive(false);
                    _listPropertyVariable.Remove(propertyVariable);
                    Destroy(itemV.gameObject);
                });
            });
        }

        public void CloseTaskViewDetail(){
            taskViewDetailMenu.SetActive(false);
        }
	}
}
