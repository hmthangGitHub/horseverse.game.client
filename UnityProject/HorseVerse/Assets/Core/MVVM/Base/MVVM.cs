using UnityEngine;
using System.Collections;

namespace Core.MVVM
{
    public static class MVVM
    {

        // create view model
        public static T CreateViewModel<T>() where T : ViewModel, new()
        {
            T t = new T();
            t.InitViewModel();
            return t;
        }
        public static ViewModel CreateViewModel(System.Type type)
        {
            var vm = System.Activator.CreateInstance(type) as ViewModel;
            vm.InitViewModel();
            return vm;
        }

        // create view by view model
        public static T InstantiateView<T>(this MonoBehaviour source, ViewModel viewModel, GameObject obj, GameObject parent = null) where T : ViewBase
        {
            return source.transform.InstantiateView<T>(viewModel, obj, null);
        }

        // create view by view model
        public static T InstantiateView<T>(this Transform source, ViewModel viewModel, GameObject obj, GameObject parent = null) where T : ViewBase
        {
            GameObject gameObj = null;
            var view = ObjectPoolManager.Instance.Get(obj.name);
            if (parent != null)
            {
                if (view == null)
                {
                    gameObj = Object.Instantiate(obj, parent.transform);
                    gameObj.name = obj.name;
                }
                else
                {
                    gameObj = view.gameObject;
                    gameObj.transform.SetParent(parent.transform);
                }
            }
            else
            {
                if (view == null)
                {
                    gameObj = Object.Instantiate(obj);
                    gameObj.name = obj.name;
                }
                else
                {
                    gameObj = view.gameObject;
                }
            }
            gameObj.transform.localScale = Vector3.one;
            gameObj.transform.localPosition = Vector3.zero;
            gameObj.transform.localEulerAngles = Vector3.zero;
            T t = gameObj.GetComponent<T>();
            if (t == null)
            {
                t = gameObj.AddComponent<T>();
            }
            if (viewModel == null)
            {
                viewModel = t.CreateViewModelWhenInitedByHierarchy();
            }
            t.BindViewModelToView(viewModel);
            return t;
        }
    }
}


