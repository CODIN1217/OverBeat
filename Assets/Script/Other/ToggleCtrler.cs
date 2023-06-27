using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TweenManager;

[ExecuteInEditMode]
public class ToggleCtrler : MonoBehaviour
{
    public Vector2 toggleScale01;
    // public bool isHighlighted;
    RectTransform toggleRect;
    // RectTransform BGRect;
    RectTransform labelRect;
    RectTransform CheckmarkRect;
    [SerializeField]
    GameObject toggle;
    // [SerializeField]
    // GameObject BG;
    [SerializeField]
    GameObject label;
    [SerializeField]
    GameObject Checkmark;
    [SerializeField]
    Toggle toggleComponent;
    float toggleOnScale = 1f;
    // TweeningInfo toggleOnScaleInfo;
    // [SerializeField]
    void Reset()
    {
        toggleScale01 = Vector2.zero;
    }
    void LateUpdate()
    {
        // if (toggleOnScaleInfo == null)
        //     toggleOnScaleInfo = new TweeningInfo(new TweenInfo<float>(1f, 0.65f, AnimationCurve.Linear(0f, 0f, 1f, 1f)), 0.1f);
        // if (!TweenMethod.IsInfoNull(toggleOnScaleInfo))
        //     toggleOnScale = ((TweenerInfo<float>)toggleOnScaleInfo).curValue;
        if (toggle == null)
            toggle = gameObject;
        toggleRect = toggle.GetComponent<RectTransform>();

        if (label != null)
            if (labelRect == null)
                labelRect = label.GetComponent<RectTransform>();

        if (Checkmark != null)
            if (CheckmarkRect == null)
                CheckmarkRect = Checkmark.GetComponent<RectTransform>();
                
        // toggleOnScale = animator.GetFloat("ToggleOnScale");
        // if (toggleComponent != null && !TweenMethod.IsInfoNull(toggleOnScaleInfo))
        // {
        //     bool isOn = toggleComponent.isOn;
        //     if (toggleComponent.isOn)
        //     {
        //         TweenMethod.TrySetForward(toggleOnScaleInfo);
        //     }
        //     else
        //     {
        //         TweenMethod.TrySetBackward(toggleOnScaleInfo);
        //     }
        // }
        if (label != null)
        {
            if (labelRect != null)
            {
                // labelRect.anchoredPosition = Vector2.Scale(new Vector2(labelRect.sizeDelta.x * -0.5f, 0f), toggleScale01);
                // labelRect.localScale = new Vector2(toggleScale01.x, 1f);
                if (toggleRect != null)
                {
                    toggleRect.sizeDelta = Handy.GetCorrectedVector(Vector2.Scale(toggleScale01, labelRect.sizeDelta - (Vector2.one * 110f) + Vector2.right * 75f) + (Vector2.one * 110f), 110f, float.MaxValue);
                    // toggleRect.sizeDelta = Handy.GetCorrectedVector(labelRect.sizeDelta - (Vector2.one * 110f) + (Vector2.right * 75f) + (Vector2.one * 110f), 110f, float.MaxValue);
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
    // public void PlayToggleTween()
    // {
    //     TweenMethod.TryPlayTween(toggleOnScaleInfo);
    // }
}
