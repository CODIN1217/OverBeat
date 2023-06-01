using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TweenManager;

public class PauseButton : MonoBehaviour
{
    bool isMouseOn;
    float scale;
    TweeningInfo scaleInfo;
    PlayManager PM;
    void Awake()
    {
        PM = PlayManager.Member;
        scaleInfo = new TweeningInfo(new TweenInfo<float>(1f, 1.3f, AnimationCurve.Linear(0f, 0f, 1f, 1f)), 0.5f);
        scale = 1f;
    }
    void Update()
    {
        if (PM.isPause)
        {
            Ray2D ray = new Ray2D(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);
            RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction);
            if (!isMouseOn)
            {
                if (hit.transform != null)
                {
                    if (hit.transform.gameObject == gameObject)
                    {
                        TweenMethod.TrySetForward(scaleInfo);
                        TweenMethod.TryPlayTween(scaleInfo);
                        isMouseOn = true;
                    }
                }
            }
            else
            {
                if (hit.transform == null)
                {
                    TweenMethod.TrySetBackward(scaleInfo);
                    TweenMethod.TryPlayTween(scaleInfo);
                    isMouseOn = false;
                }
            }
            transform.localScale = Vector2.one * scale;
        }
    }
}
