using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SupportLanguage", menuName = "Localization/SupportLanguage")]
public class SupportLanguage : ScriptableObject
{
    [SerializeField] List<SystemLanguage> m_languages;

    public List<SystemLanguage> languages { get { return m_languages; } }
}
