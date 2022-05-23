using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIComponentHorsePosition : UIComponent<UIComponentHorsePosition.Entity>
{
    [Serializable]
    public class Entity
    {
        public int top;
    }

    public FormattedTextComponent top;
    protected override void OnSetEntity()
    {
        top.SetEntity(AddOrdinal(this.entity.top));
    }

    public static string AddOrdinal(int num)
    {
        if (num <= 0) return num.ToString();

        switch (num % 100)
        {
            case 11:
            case 12:
            case 13:
                return num + "th";
        }

        switch (num % 10)
        {
            case 1:
                return num + "st";
            case 2:
                return num + "nd";
            case 3:
                return num + "rd";
            default:
                return num + "th";
        }
    }

}
