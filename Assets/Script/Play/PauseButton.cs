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
            // Handy.WriteLog(Camera.main.ScreenToWorldPoint(Input.mousePosition) * 100f);
            Ray2D ray = new Ray2D(Camera.main.ScreenToWorldPoint(Input.mousePosition) * 100f, Vector2.zero);
            RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction);
            if (!isMouseOn)
            {
                if (hit.collider != null)
                {
                    Handy.WriteLog(gameObject.name);
                    if (ReferenceEquals(hit.collider.gameObject, gameObject))
                    {
                        TweenMethod.TrySetForward(scaleInfo);
                        TweenMethod.TryPlayTween(scaleInfo);
                        isMouseOn = true;
                    }
                }
            }
            else
            {
                if (hit.collider == null)
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
