using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TweenManager;

[ExecuteInEditMode]
public class ToggleCtrler : MonoBehaviour
{
    [SerializeField]
    Vector2 toggleScale01;
    [SerializeField]
    Vector2 highlightedScale01 = new Vector2(272f, 144f);
    RectTransform toggleRect;
    RectTransform labelRect;
    RectTransform CheckmarkRect;
    [SerializeField]
    GameObject toggle;
    [SerializeField]
    GameObject label;
    [SerializeField]
    GameObject Checkmark;
    [SerializeField]
    Toggle toggleComponent;
    float toggleOnScale = 1f;
    TweeningInfo toggleOnScaleInfo;
    void Awake()
    {
        toggleScale01 = Vector2.zero;
    }
    void LateUpdate()
    {
        if (toggleOnScaleInfo == null)
            toggleOnScaleInfo = new TweeningInfo(new TweenInfo<float>(1f, 0.65f, AnimationCurve.Linear(0f, 0f, 1f, 1f)), 0.1f);
        if (!TweenMethod.IsInfoNull(toggleOnScaleInfo))
            toggleOnScale = ((TweenerInfo<float>)toggleOnScaleInfo).curValue;
        if (toggle == null)
            toggle = gameObject;
        toggleRect = toggle.GetComponent<RectTransform>();

        if (label != null)
            if (labelRect == null)
                labelRect = label.GetComponent<RectTransform>();

        if (Checkmark != null)
            if (CheckmarkRect == null)
                CheckmarkRect = Checkmark.GetComponent<RectTransform>();
        if (label != null)
        {
            if (labelRect != null)
            {
                if (toggleRect != null)
                {
                    toggleRect.sizeDelta = Handy.GetCorrectedVector(Vector2.Scale(toggleScale01, highlightedScale01 - (Vector2.one * 110f)) + (Vector2.one * 110f), 110f, float.MaxValue);
                    if (Checkmark != null)
                    {
                        if (CheckmarkRect != null)
                        {
                            CheckmarkRect.sizeDelta = toggleRect.sizeDelta;
                            CheckmarkRect.localScale = Vector2.one * toggleOnScale;
                            labelRect.localScale = CheckmarkRect.localScale;
                        }
                    }
                }
            }
        }
    }
    public void ClickToggle(ToggleCtrler toggleCtrler)
    {
        if (toggleCtrler.toggleComponent.isOn)
            TweenMethod.TrySetForward(toggleCtrler.toggleOnScaleInfo);
        else
            TweenMethod.TrySetBackward(toggleCtrler.toggleOnScaleInfo);
        TweenMethod.TryPlayTween(toggleCtrler.toggleOnScaleInfo);
    }
}
