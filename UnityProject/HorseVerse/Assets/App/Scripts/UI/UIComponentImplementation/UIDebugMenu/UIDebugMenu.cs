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
	    public ButtonComponent.Entity openBtn;
	    public UIDebugMenuState.State states;
    }
    
    public UIDebugMenuItemList debugMenuItemList;
    public FormattedTextComponent title;
    public ButtonComponent closeBtn;
    public ButtonComponent openBtn;
    public ButtonComponent backBtn;
    public UIDebugMenuState states;
    public CanvasGroup collapseCanvasGroup;

    private string currentPath = string.Empty;
	private IOrderedEnumerable<UIDebugMenuItem.Entity> sortedEntities;
    
    protected override void OnSetEntity()
    {
	    sortedEntities = this.entity.debugMenuItemList.entities.OrderBy(x => x.debugMenu);
	    UpdateDebugMenus();
	    closeBtn.SetEntity(this.entity.closeBtn);
	    openBtn.SetEntity(this.entity.openBtn);
	    backBtn.SetEntity(() =>
	    {
		    if (!string.IsNullOrEmpty(currentPath))
		    {
				 var subMenus = currentPath.Split('/')
                 			    .Where(x => !string.IsNullOrEmpty(x))
                 			    .ToArray();
                 		    currentPath = string.Join("/", subMenus.Take(subMenus.Length - 1));
                 		    currentPath += subMenus.Length > 1 ? "/" : string.Empty;
                 		    UpdateDebugMenus();   
		    }
		    else
		    {
			    closeBtn.button.onClick.Invoke();
		    }
	    });
	    states.SetEntity(this.entity.states);	
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

    public void RefreshMenu()
    {
	    sortedEntities = this.entity.debugMenuItemList.entities.OrderBy(x => x.debugMenu);
	    UpdateDebugMenus();
    }

    public void ShowDebugMenuAsVisible(bool isInvisible)
    {
	    collapseCanvasGroup.alpha = isInvisible ? 0.0f : 1.0f;
    }
}	