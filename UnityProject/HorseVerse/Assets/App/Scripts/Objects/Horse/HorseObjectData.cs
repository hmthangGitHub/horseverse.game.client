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
}
