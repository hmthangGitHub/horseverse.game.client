using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Core
{
    public class LayerManager : Singleton<LayerManager>
    {
        public Transform RootLayer;
        public Transform RootPopup;
        public GameObject LoadingGO;
        public GameObject LoadingGOBG;
        public RectTransform RootUI;
        public float WaitingForFade;
        public float WaitingForTranlate;
        [SerializeField]
        public string RootLayerPath;
        Vector2 _size = Vector2.zero;
        public Vector2 Size
        {
            get
            {
                if (_size == Vector2.zero)
                {
                    _size = new Vector2(RootUI.rect.width, RootUI.rect.height);
                }
                return _size;
            }
        }

        private bool _isLoading = false;
        public bool IsLoading
        {
            get
            {
                return _isLoading;
            }
        }
        public void ShowLoading()
        {
            _isLoading = true;
            if (LoadingGOBG != null) LoadingGOBG.SetActive(true);
            LoadingGO.SetActive(_isLoading);
        }
        public void ShowLoadingHaveNoIcon()
        {
            _isLoading = true;
            if(LoadingGOBG != null) LoadingGOBG.SetActive(false);
            LoadingGO.SetActive(_isLoading);
        }
        public void HideLoading()
        {
            _isLoading = false;
            LoadingGO.SetActive(false);
        }
        private bool _useCached = false;

        protected LayerManager()
        {
            _layersStack = new List<SceneState>();
            _scenesCache = new List<BaseLayer>();
            _popupsCache = new List<BasePopup>();
            _popupsStack = new List<ElementParameter>();
        }

        public BaseLayer CurrentLayer { get; private set; }

        List<SceneState> _layersStack;
        List<BaseLayer> _scenesCache;
        List<BasePopup> _popupsCache;
        List<ElementParameter> _popupsStack;

        // Use this for initialization
        void Start()
        {
            StartCoroutine(WaitingForStartScene());
        }

        IEnumerator WaitingForStartScene()
        {
            if (string.IsNullOrEmpty(RootLayerPath)) yield break;
            yield return null;
            CreateLayer(new ElementParameter(RootLayerPath, TransitionType.None));
        }

        public void Reset()
        {

        }

        BaseLayer GetLayer(string name)
        {
            return _scenesCache.FirstOrDefault(x => x.LayerParameter.Name == name);
        }

        BaseLayer GetLayer<T>() where T : MonoBehaviour
        {
            var nameElement = typeof(T).ToString();
            return _scenesCache.FirstOrDefault(x => x.LayerParameter.Name == nameElement);
        }

        public BaseLayer GetParentOfCurrentLayer()
        {
            //if (_layersStack.Count > 0)
            //{
            //    SceneState sceneState = _layersStack[_layersStack.Count - 1];
            //    return sceneState.Parent;
            //}
            if (_layersStack.Count > 1)
            {
                SceneState screenState = _layersStack[_layersStack.Count - 2];
                return screenState.Owner;
            }
            return null;
        }

        void Push(BaseLayer baseLayer)
        {
            SceneState sceneState = new SceneState(baseLayer);
            CurrentLayer = baseLayer;
            _layersStack.Add(sceneState);
            baseLayer.transform.SetAsLastSibling();
            baseLayer.Show();
            if (_useCached)
            {
                if (!_scenesCache.Contains(CurrentLayer))
                {
                    _scenesCache.Add(CurrentLayer);
                }
            }
        }

        public void Pop()
        {
            if (_layersStack.Count > 1)
            {
                CurrentLayer.Hide();
                if (_useCached)
                {
                    if (!_scenesCache.Contains(CurrentLayer))
                    {
                        _scenesCache.Add(CurrentLayer);
                    }
                }
                _layersStack.RemoveAt(_layersStack.Count - 1);
                var currentState = _layersStack[_layersStack.Count - 1];
                CurrentLayer = currentState.Owner;
                CurrentLayer.transform.SetAsLastSibling();
            }
            else
            {
                Debug.Log("Is root!!!");
            }
        }

        public void PopToLayer<T>() where T : BaseLayer
        {
            var nameOfGO = typeof(T).ToString();
            var indexLayer = _layersStack.FindLastIndex(x => x.OwenerName == nameOfGO);
            if (indexLayer >= 0)
            {
                int count = (_layersStack.Count - 1) - indexLayer - 1;
                for (int i = 0; i < count; i++)
                {
                    _layersStack.RemoveAt(indexLayer + 1);
                }
                // hide current layer
                CurrentLayer.Hide();
                _layersStack.RemoveAt(_layersStack.Count - 1);
                // get next layer form stack
                var currentState = _layersStack[_layersStack.Count - 1];

                // set current layer
                CurrentLayer = currentState.Owner;
                // set at last index of layer container
                CurrentLayer.transform.SetAsLastSibling();
            }
            else
            {
                Debug.Log("Is root!!!");
            }
        }

        public BaseLayer CreateLayer(ElementParameter layerParameter)
        {
            BaseLayer scene = GetLayer(layerParameter.Name);
            if (scene != null)
            {
                scene.gameObject.SetActive(true);
                scene.transform.SetParent(RootLayer);
                scene.transform.localScale = Vector3.one;
                Push(scene);
                return scene;
            }
            GameObject obj = null;
            obj = Instantiate(Resources.Load(layerParameter.Path)) as GameObject;
            var canvasGroup = obj.GetComponent<CanvasGroup>();
            if (canvasGroup == null)
            {
                obj.AddComponent<CanvasGroup>();
            }
            scene = obj.GetComponent<BaseLayer>();
            scene.LayerParameter = layerParameter;
            scene.Transition = layerParameter.TransitionType;
            scene.transform.SetParent(RootLayer);
            scene.transform.localScale = Vector3.one;
            Push(scene);
            return scene;
        }

        public BasePopupInLayer CreatePopupInLayer(ElementParameter layerParameter, BaseLayer baseLayer)
        {
            GameObject obj = null;
            BasePopupInLayer layerBase = null;
            obj = Instantiate(Resources.Load(layerParameter.Path)) as GameObject;
            var canvasGroup = obj.GetComponent<CanvasGroup>();
            if (canvasGroup == null)
            {
                obj.AddComponent<CanvasGroup>();
            }
            layerBase = obj.GetComponent<BasePopupInLayer>();
            layerBase.baseLayer = baseLayer;
            layerBase.LayerParameter = layerParameter;
            layerBase.Transition = layerParameter.TransitionType;
            layerBase.transform.SetParent(baseLayer.transform);
            baseLayer.PushPopup(layerBase);
            layerBase.transform.localScale = Vector3.one;
            layerBase.Show();
            return layerBase;
        }

        BasePopup GetPopup(string name)
        {
            return _popupsCache.FirstOrDefault(x => x.LayerParameter.Name == name);
        }

        BasePopup GetPopup<T>() where T : MonoBehaviour
        {
            var nameElement = typeof(T).ToString();
            return _popupsCache.FirstOrDefault(x => x.LayerParameter.Name == nameElement);
        }

        void PushPopup(BasePopup basePopup)
        {
            basePopup.Show();
            if (_useCached)
            {
                if (!_popupsCache.Contains(basePopup))
                {
                    _popupsCache.Add(basePopup);
                }
            }
        }

        public BasePopup CreatePopupSystem(ElementParameter layerParameter)
        {
            return CreateBasePopupSystem(layerParameter);
        }

        public void CreatePopupSystemStack(ElementParameter layerParameter)
        {
            if (_popupsStack.Count == 0)
            {
                _popupsStack.Add(layerParameter);
                CreateBasePopupSystem(layerParameter);
            }
            else
            {
                _popupsStack.Add(layerParameter);
            }
        }

        public void PopupSystemCloseDone()
        {
            //if (_popupsStack.Count > 0)
            //{
            //    _popupsStack.RemoveAt(0);
            //    if (_popupsStack.Count > 0)
            //    {
            //        CreateBasePopupSystem(_popupsStack[0]);
            //    }
            //}
        }

        BasePopup CreateBasePopupSystem(ElementParameter layerParameter)
        {
            BasePopup popup = GetPopup(layerParameter.Name);
            if (popup != null)
            {
                popup.gameObject.SetActive(true);
                popup.transform.SetParent(RootPopup);
                popup.transform.localScale = Vector3.one;
                PushPopup(popup);
            }
            else
            {
                GameObject obj = null;
                obj = Instantiate(Resources.Load(layerParameter.Path)) as GameObject;
                var canvasGroup = obj.GetComponent<CanvasGroup>();
                if (canvasGroup == null)
                {
                    obj.AddComponent<CanvasGroup>();
                }
                popup = obj.GetComponent<BasePopup>();
                popup.LayerParameter = layerParameter;
                popup.Transition = layerParameter.TransitionType;
                popup.transform.SetParent(RootPopup);
                popup.transform.localScale = Vector3.one;
                PushPopup(popup);
            }
            return popup;
        }

        public void HideDone(BaseElement baseElement)
        {
            if (_useCached)
            {
                baseElement.gameObject.SetActive(false);
            }
            else
            {
                Destroy(baseElement.gameObject);
            }
        }
    }

    public class SceneState
    {
        private BaseLayer _owner = null;
        public BaseLayer Owner
        {
            get
            {
                if (_owner == null)
                {
                    var obj = GameObject.Instantiate(Resources.Load(OwnerParameter.Path)) as GameObject;
                    var canvasGroup = obj.GetComponent<CanvasGroup>();
                    if (canvasGroup == null)
                    {
                        obj.AddComponent<CanvasGroup>();
                    }
                    var scene = obj.GetComponent<BaseLayer>();
                    scene.LayerParameter = OwnerParameter;
                    scene.Transition = OwnerParameter.TransitionType;
                    scene.transform.SetParent(LayerManager.Instance.RootLayer);
                    scene.transform.localScale = Vector3.one;
                    _owner = scene;
                }
                return _owner;
            }
            private set
            {
                _owner = value;
            }
        }

        public ElementParameter OwnerParameter { get; private set; }
        public string OwenerName { get; private set; }

        public SceneState(BaseLayer owner)
        {
            Owner = owner;
            OwenerName = owner.GetType().ToString();
            OwnerParameter = owner.LayerParameter;
        }
    }

    public class PopupState
    {
        private BaseLayer _root;
        private BasePopupInLayer _owner = null;
        public BasePopupInLayer Owner
        {
            get
            {
                if (_owner == null)
                {
                    var obj = GameObject.Instantiate(Resources.Load(OwnerParameter.Path)) as GameObject;
                    var canvasGroup = obj.GetComponent<CanvasGroup>();
                    if (canvasGroup == null)
                    {
                        obj.AddComponent<CanvasGroup>();
                    }
                    var popup = obj.GetComponent<BasePopupInLayer>();
                    popup.baseLayer = _root;
                    popup.LayerParameter = OwnerParameter;
                    popup.Transition = OwnerParameter.TransitionType;
                    popup.transform.SetParent(_root.transform);
                    popup.transform.localScale = Vector3.one;
                    _owner = popup;
                }
                return _owner;
            }
            private set
            {
                _owner = value;
            }
        }

        public ElementParameter OwnerParameter { get; private set; }
        public string OwenerName { get; private set; }

        public PopupState(BasePopupInLayer owner, BaseLayer baseLayer)
        {
            Owner = owner;
            OwenerName = owner.GetType().ToString();
            OwnerParameter = owner.LayerParameter;
            _root = baseLayer;
        }
    }
}
