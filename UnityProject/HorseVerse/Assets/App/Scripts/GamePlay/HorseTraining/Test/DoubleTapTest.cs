using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Lean.Touch;
using UnityEngine;
using UnityEngine.UIElements;
using Image = UnityEngine.UI.Image;

public class DoubleTapTest : MonoBehaviour
{
    [SerializeField] private LeanFingerTap doubleTap;
    [SerializeField] private Image image;
    [SerializeField] private LeanFingerUp touchUp;

    private double lastTap;

    private enum LastTouch
    {
        None,
        Left,
        Right
    }

    private LastTouch lastTouch;

    // Start is called before the first frame update
    void Start()
    {
        touchUp.OnFinger.AddListener(finger =>
        {

            var currentTouch = LastTouch.None;
            if (finger.Up && finger.StartScreenPosition.x < Screen.width / 2)
            {
                currentTouch = LastTouch.Left;
            }
            else if (finger.Up && finger.StartScreenPosition.x > Screen.width / 2)
            {
                currentTouch = LastTouch.Right;
            }

            if (lastTouch != LastTouch.None && lastTouch != currentTouch && Time.realtimeSinceStartup - lastTap < 0.2f)
            {
                image.DOKill(true);
                image.DOFade(1.0f, 0.0f, 0.2f);
            }

            lastTap = Time.realtimeSinceStartup;
            lastTouch = currentTouch;
        });
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
