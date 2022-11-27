using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HorseObjectPresenter 
{
    public static void SetColor(HorseObjectData data, Color c1, Color c2, Color c3, Color c4)
    {
        if(data != null)
        {
            SetColor(data.Color_1, c1);
            SetColor(data.Color_2, c2);
            SetColor(data.Color_3, c3);
            SetColor(data.Color_4, c4);
        }
    }

    private static void SetColor(List<HorseObjectData.ReferenceMaterial> materials, Color color)
    {
        if(materials != null && materials.Count > 0)
        {
            for (int i = 0; i < materials.Count; i++)
            {
                var m = materials[i];
                var mat = m.RefRenderer.materials[m.MaterialIndex];
                if(mat != null)
                {
                    mat.SetColor("_Color", color);
                }
            }
        }
    }
}
