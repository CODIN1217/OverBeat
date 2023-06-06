using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TweenManager;

public class PauseController : MonoBehaviour
{
    PlayManager PM;
    CanvasGroup PauseGroup;
    TweeningInfo PauseGroupAlphaInfo;
    void Awake()
    {
        PM = PlayManager.Member;
        PauseGroup = GetComponent<CanvasGroup>();
        PauseGroupAlphaInfo = new TweeningInfo(new TweenInfo<float>(0f, 1f, AnimationCurve.Linear(0f, 0f, 1f, 1f)), 0.3f);
    }
    void OnEnable()
    {
        TweenMethod.TryPlayTween(PauseGroupAlphaInfo);
    }
    void Update()
    {
        PauseGroup.alpha = ((TweenerInfo<float>)PauseGroupAlphaInfo).curValue;
        if (!PM.isPause)
            gameObject.SetActive(false);
    }
    void OnDisable()
    {
        PauseGroupAlphaInfo.Goto(0f);
        PauseGroup.alpha = 0f;
    }
}
