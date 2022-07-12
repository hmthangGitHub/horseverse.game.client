using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestUIScript<T, TEntity> : MonoBehaviour  where T : MonoBehaviour, IPopupEntity<TEntity>, IUIComponent<TEntity>
                                                       where TEntity : new()
{
    public T uiTest;
    public TEntity entity;

    protected virtual void OnGUI()
    {
        GUI.skin.button.fontSize = 30;
        if (GUILayout.Button("Set Entity"))
        {
            SetEntity();
        }

        if (GUILayout.Button("In"))
        {
            uiTest.In().Forget();
        }

        if (GUILayout.Button("Out"))
        {
            uiTest.Out().Forget();
        }
    }

    public virtual void SetEntity()
    {
        uiTest.SetEntity(entity);
    }
}
