using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class SceneryObjectAnimation : MonoBehaviour
{
    private Vector2 duration = new Vector2(10, 15);
    private Vector2 rotateDuration = new Vector2(40, 100);
    private Vector2 moveRange = new Vector2(1, 4);
    private Vector2 delayRange = new Vector2(0.0f, 1.0f);
    private Tween tween;

    private void Start()
    {
        transform
            .DOLocalRotate(new Vector3(0, 360.0f), rotateDuration.Random(), RotateMode.FastBeyond360)
            .SetEase(Ease.Linear)
            .SetLoops(-1)
            .SetDelay(delayRange.Random());
    }
}
