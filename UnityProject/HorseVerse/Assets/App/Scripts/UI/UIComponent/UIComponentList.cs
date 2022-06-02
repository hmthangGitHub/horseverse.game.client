using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIComponentList<Template, TemplateEntity> : UIComponent<UIComponentList<Template, TemplateEntity>.Entity> where TemplateEntity : new()
                                                                                                                   where Template : UIComponent<TemplateEntity>
{
    [Serializable]
    public partial class Entity
    {
        public TemplateEntity[] entities;
    }

    private void Awake()
    {
        template.gameObject.SetActive(false);
        
    }

    public Template template;
    public List<Template> instanceList = new List<Template>();

    protected override void OnSetEntity()
    {
        foreach (var instance in instanceList)
        {
            GameObject.Destroy(instance.gameObject);
        }
        instanceList.Clear();
        foreach (var item in this.entity.entities)
        {
            var instance = CreateTemplate();
            instance.gameObject.SetActive(true);
            instance.SetEntity(item);
            instanceList.Add(instance);
        }
    }

    private Template CreateTemplate()
    {
        return Instantiate<Template> (template, template.transform.parent);
    }
}
