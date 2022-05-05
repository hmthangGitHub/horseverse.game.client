using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;
using UnityEngine;
using System.Reflection;
using UniRx;

namespace Core.MVVM
{
    // view base
    public abstract class ViewBase : MonoBehaviour
    {
        protected List<IDisposable> _bindingViews = new List<IDisposable>();

        // cache view model
        protected ViewModel _viewModel;
        public ViewModel ViewModel
        {
            get
            {
                return _viewModel;
            }
        }
        protected bool _isInited = false;
        //bool _isCleared = false;

        // all of view for ReactiveCollection
        private Dictionary<IList, List<ViewBase>> _viewsContains = new Dictionary<IList, List<ViewBase>>();

        // this view is created in Hiearachy or created by code
        public bool IsInitedByHierarchy;

        // add view model to view
        public virtual void BindViewModelToView(ViewModel viewModel, Action initView = null)
        {
            _isInited = false;
            if (_viewModel != null)
            {
                Clear();
            }
            //_isCleared = false;
            _viewModel = viewModel;
            BindViewModelToViewDone();
            Bind();
            _isInited = true;
            if (_viewModel.Visible != null) viewModel.Visible.Value = true;
            if (initView != null)
            {
                initView();
            }
            AfterBind();
        }

        protected virtual void BindViewModelToViewDone()
        {
        }

        // bind all ReactiveProperty, ReactiveCommand and Reactive Collection
        protected virtual void Bind()
        {
            BindProperty(ViewModel.Visible, (arg) => {
                if (!_isInited) return;

                if (arg)
                {
                    gameObject.SetActive(arg);
                    VisibleChanged(arg);
                }
                else
                {
                    VisibleChanged(arg);
                    gameObject.SetActive(arg);
                }
            });
            BindCommand(ViewModel.FindGameObjectCommand, FindGameObjectExecuted);
            BindCommand(ViewModel.DestroyGameObjectCommand, DestroyGameObjectExecuted);
        }

        void UnBindOnView()
        {
            for (var i = 0; i < _bindingViews.Count; i++)
            {
                _bindingViews[i].Dispose();
            }
            _bindingViews.Clear();
        }

        public void Clear()
        {
            //if (_isCleared) return;
            UnBindOnView();
            foreach (var item in _viewsContains)
            {
                if (item.Value.Count > 0)
                {
                    for (int i = 0; i < item.Value.Count; i++)
                    {
                        var view = item.Value[i];
                        view.ViewModel.Destroy();
                    }
                    item.Value.Clear();
                }
                item.Key.Clear();
            }
            _viewModel.ClearViewModelReference();
            _viewsContains.Clear();
            //_isCleared = false;
        }

        // bind done
        protected virtual void AfterBind()
        {

        }

        private void Awake()
        {
            // if is created in hierachy
            if (IsInitedByHierarchy && _viewModel == null)
            {
                // create view model
                var viewModel = CreateViewModelWhenInitedByHierarchy();
                // add view model to view
                BindViewModelToView(viewModel);
            }
            OnAwake();
        }

        private void Start()
        {

            OnStart();
        }

        private void Update()
        {
            OnUpdate();
        }

        void OnDestroy()
        {
            //if (_isCleared || _viewModel == null) return;

            //Debug.Log("Destroy: " + GetType().ToString() + ";" + gameObject.GetInstanceID());
            //Clear();
            //ReceiveDestroy();
        }

        public void Destroy()
        {
            ViewModel.Disposables.Clear();
            Clear();
            ReceiveDestroy();
            ViewModel.Dispose();
            if (IsAddToPool())
            {
                ObjectPoolManager.Instance.Add(gameObject.name, gameObject);
            }
            else
            {
                Debug.Log("destroy: " + gameObject.name);
                DestroyImmediate(gameObject);
            }
        }

        protected virtual void OnAwake()
        {

        }

        protected virtual void OnStart()
        {

        }

        protected virtual void OnUpdate()
        {

        }

        protected virtual void ReceiveDestroy()
        {

        }

        // create view model when this view is created in hierachy
        public virtual ViewModel CreateViewModelWhenInitedByHierarchy()
        {
            return null;
        }

        protected virtual void VisibleChanged(bool arg)
        {

        }

        // find game object
        void FindGameObjectExecuted(FindGameObjectCommand arg)
        {
            if (arg.Callback != null && arg.Type != null
               && (arg.Type == GetType() || GetType().IsSubclassOf(arg.Type)))
            {
                arg.Callback(gameObject);
            }
        }

        void DestroyGameObjectExecuted(DestroyGameObjectCommand command)
        {
            Destroy();
        }

        protected virtual bool IsAddToPool()
        {
            return false;
        }

        // waiting for seconds, after that will handle action
        protected IEnumerator WaitingForSeconds(float seconds, Action action)
        {
            yield return new WaitForSeconds(seconds);
            action();
        }

        // create new view at view
        public T InstantiateView<T>(ViewModel viewModel, GameObject obj, GameObject parent = null) where T : ViewBase
        {
            return transform.InstantiateView<T>(viewModel, obj, parent);
        }

        /// <summary>
        /// bind collection when add, remove and clear
        /// </summary>
        /// <param name="items"> rewactive collection</param>
        /// <param name="createView">create view</param>
        /// <param name="added">added calback</param>
        /// <param name="removed"> removed call back</param>
        /// <typeparam name="T">The 1st type parameter.</typeparam>
        protected void BindCollection<T>(ReactiveCollection<T> items, Func<ViewModel, ViewBase> createView, Action<ViewBase> added, Action<ViewBase> removed) where T : ViewModel, new()
        {
            List<ViewBase> views = new List<ViewBase>();
            _viewsContains.Add(items, views);
            // bind when add
            _bindingViews.Add(items.ObserveAdd().Subscribe(x => {
                var viewModel = x.Value as ViewModel;
                if (viewModel != null)
                {
                    ViewBase viewBase = createView(viewModel);
                    if (viewBase != null)
                    {
                        views.Add(viewBase);
                        added(viewBase);
                    }
                }
            }));
            // bind when remove
            _bindingViews.Add(items.ObserveRemove().Subscribe(x =>
            {
                var viewModel = x.Value as ViewModel;
                if (viewModel != null)
                {
                    int index = views.FindIndex(x1 => x1.ViewModel == viewModel);
                    if (index >= 0)
                    {
                        var viewBase = views[index];
                        views.RemoveAt(index);
                        removed(viewBase);
                    }
                }
            }));
            // bind when clear
            _bindingViews.Add(items.ObserveReset().Subscribe(_ =>
            {
                for (int i = 0; i < views.Count; i++)
                {
                    var view = views[i];
                    views.RemoveAt(i--);
                    removed(view);
                }
            }));
        }

        // bind property
        public void BindProperty<T>(ReactiveProperty<T> property, Action<T> action)
        {
            _bindingViews.Add(property.Skip(1).Subscribe(action));
        }

        // bind command
        public void BindCommand<T>(ReactiveCommand<T> command, Action<T> action)
        {
            _bindingViews.Add(command.Subscribe(action));
        }

        public void BindCommand(ReactiveCommand command, Action action)
        {
            _bindingViews.Add(command.Subscribe(x => action()));
        }
    }
}