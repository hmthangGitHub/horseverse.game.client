using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;

public class ReplayLabelAnimation : MonoBehaviour
{
    public TextMeshProUGUI label;

    private void OnEnable()
    {
        label.DOFade(0.0f, 0.5f)
             .SetLoops(-1, LoopType.Yoyo);
    }
}
