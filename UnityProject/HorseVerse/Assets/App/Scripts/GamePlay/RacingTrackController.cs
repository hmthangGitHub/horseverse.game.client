using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class RacingTrackController : MonoBehaviour
{
    public MeshRenderer startGate;

    public void PlayStartAnimation()
    {
        DOTween.Sequence()
            .Append(startGate.material.DOFade(0.0f, 0.5f))
            .Append(startGate.material.DOFade(1.0f, 0.0f))
            .SetLoops(6);
    }
}
