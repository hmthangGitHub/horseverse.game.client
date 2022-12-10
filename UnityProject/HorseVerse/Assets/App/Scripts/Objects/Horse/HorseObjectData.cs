using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HorseObjectData : MonoBehaviour
{
    [System.Serializable]
    public class ReferenceMaterial
    {
        public int MaterialIndex;
        public Renderer RefRenderer;
    }

    [Header("Color Area")]
    [SerializeField] List<ReferenceMaterial> m_Color_1;
    [SerializeField] List<ReferenceMaterial> m_Color_2;
    [SerializeField] List<ReferenceMaterial> m_Color_3;
    [SerializeField] List<ReferenceMaterial> m_Color_4;

    public List<ReferenceMaterial> Color_1 => m_Color_1;
    public List<ReferenceMaterial> Color_2 => m_Color_2;
    public List<ReferenceMaterial> Color_3 => m_Color_3;
    public List<ReferenceMaterial> Color_4 => m_Color_4;
    
    public void SetColor(Color c1, Color c2, Color c3, Color c4)
    {
        SetColor(Color_1, c1);
        SetColor(Color_2, c2);
        SetColor(Color_3, c3);
        SetColor(Color_4, c4);
    }

    private void SetColor(List<ReferenceMaterial> materials, Color color)
    {
        if(materials != null && materials.Count > 0)
        {
            for (int i = 0; i < materials.Count; i++)
            {
                var mat = materials[i].RefRenderer.materials[materials[i].MaterialIndex];
                if(mat != null)
                {
                    mat.SetColor("_BaseColor", color);
                    mat.SetColor("_Color", color);
                }
            }
        }
    }
}
