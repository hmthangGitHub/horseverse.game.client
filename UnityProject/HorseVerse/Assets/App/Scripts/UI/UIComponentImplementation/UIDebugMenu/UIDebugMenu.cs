using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class UIDebugMenu : PopupEntity<UIDebugMenu.Entity>
{
	[System.Serializable]
    public class Entity
    {
	    public UIDebugMenuItemList.Entity debugMenuItemList;
	    public ButtonComponent.Entity closeBtn;
    }
    
    public UIDebugMenuItemList debugMenuItemList;
    public FormattedTextComponent title;
    public ButtonComponent closeBtn;
    public ButtonComponent backBtn;

    private string currentPath = string.Empty;
	private IOrderedEnumerable<UIDebugMenuItem.Entity> sortedEntities;
    
    protected override void OnSetEntity()
    {
	    sortedEntities = this.entity.debugMenuItemList.entities.OrderBy(x => x.debugMenu);
	    UpdateDebugMenus();
	    closeBtn.SetEntity(this.entity.closeBtn);
	    backBtn.SetEntity(() =>
	    {
		    var subMenus = currentPath.Split('/')
			    .Where(x => !string.IsNullOrEmpty(x))
			    .ToArray();
		    currentPath = string.Join("/", subMenus.Take(subMenus.Length - 1));
		    currentPath += subMenus.Length > 1 ? "/" : string.Empty;
		    UpdateDebugMenus();
	    });
    }

    private void UpdateDebugMenus()
    {
	    var entities = sortedEntities.Where(x => x.debugMenu.StartsWith(currentPath))
		    .Select(x =>
		    {
			    var subMenus = !string.IsNullOrEmpty(currentPath) 
				    ? x.debugMenu.Replace(currentPath, string.Empty).Split('/')
				    : x.debugMenu.Split('/');
			    
			    subMenus = subMenus.Where(x => !string.IsNullOrEmpty(x)).ToArray();
			    
			    return (subMenu : subMenus.FirstOrDefault() + (subMenus.Length == 1 ? string.Empty : "/"), entity : x);
		    })
		    .GroupBy(x => x.subMenu)
		    .Select(x => x.First())
		    .Select(x =>
		    {
			    return new UIDebugMenuItem.Entity()
			    {
				    debugMenu = x.subMenu,
				    debugMenuBtn = new ButtonComponent.Entity(() =>
				    {
					    if (!x.subMenu.Contains("/"))
					    {
						    x.entity.debugMenuBtn.onClickEvent.Invoke();
					    }
					    else
					    {
						    currentPath += x.subMenu;
						    UpdateDebugMenus();    
					    }
					    
				    })
			    };
		    }).ToArray();
	    debugMenuItemList.SetEntity(new UIDebugMenuItemList.Entity()
	    {
		    entities = entities
	    });
	    
	    title.SetEntity($"Debug/{currentPath}");
    }
}	