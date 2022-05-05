using System.Collections;
using System.Collections.Generic;
using System;
using System.ComponentModel;
using System.Reflection;
using System.Linq;
using UniRx;
using UnityEngine;

namespace Core.MVVM
{

    // view model base
    public abstract class ViewModel
    {
        public ReactiveProperty<bool> Visible;
        public ReactiveCommand<FindGameObjectCommand> FindGameObjectCommand;
        public ReactiveCommand<DestroyGameObjectCommand> DestroyGameObjectCommand;

        private CompositeDisposable _disposables = new CompositeDisposable();
        public CompositeDisposable Disposables
        {
            get
            {
                return _disposables;
            }
        }

        private List<ViewModel> _viewModelsReference = new List<ViewModel>();

        public virtual void InitViewModel()
        {
            Visible = new ReactiveProperty<bool>();

            FindGameObjectCommand = new ReactiveCommand<FindGameObjectCommand>();
            DestroyGameObjectCommand = new ReactiveCommand<DestroyGameObjectCommand>();
        }

        public void Destroy()
        {
            DestroyGameObjectCommand.Execute(new Core.MVVM.DestroyGameObjectCommand());
        }

        public virtual void Dispose()
        {
            Visible.Dispose();
            FindGameObjectCommand.Dispose();
            DestroyGameObjectCommand.Dispose();
        }

        public void AddViewModelReference(ViewModel viewModel)
        {
            _viewModelsReference.Add(viewModel);
        }

        public void RemoveViewModelReference(ViewModel viewModel)
        {
            _viewModelsReference.Remove(viewModel);
        }

        public void ClearViewModelReference()
        {
            for (int i = 0; i < _viewModelsReference.Count; i++)
            {
                var viewModel = _viewModelsReference[i];
                viewModel.Destroy();
            }
            _viewModelsReference.Clear();
        }

        protected void BindCollection<T>(ReactiveCollection<T> items, Action<ViewModel> added, Action<ViewModel> removed) where T : ViewModel, new()
        {
            List<ViewModel> viewModels = new List<ViewModel>();
            // bind when add
            items.ObserveAdd().Subscribe(x =>
            {
                var viewModel = x.Value as ViewModel;
                if (viewModel != null)
                {
                    added(viewModel);
                    viewModels.Add(viewModel);
                }
            });
            // bind when remove
            items.ObserveRemove().Subscribe(x =>
            {
                var viewModel = x.Value as ViewModel;
                if (viewModel != null)
                {
                    removed(viewModel);
                    viewModels.Remove(viewModel);
                }
            });
            // bind when clear
            items.ObserveReset().Subscribe(_ =>
            {
                for (int i = 0; i < viewModels.Count; i++)
                {
                    var viewModel = viewModels[i];
                    removed(viewModel);
                    viewModels.RemoveAt(i--);
                }
            });
        }
    }
}

