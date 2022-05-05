namespace Core.MVVM{
    using System;
    using UnityEngine;

    // command base(all of command will inherit it)
    public abstract class ICommand
    {
        //public ViewModel Sender {get; set;}
    }

    // command to find GameObject
    public sealed class FindGameObjectCommand : ICommand
    {
        public Type Type { get; set; }
        public Action<GameObject> Callback { get; set; }
    }

    // command to find GameObject
    public sealed class DestroyGameObjectCommand : ICommand
    {

    }

    public sealed class InitEditorToolLayerCommand : ICommand
    {

    }
    
}