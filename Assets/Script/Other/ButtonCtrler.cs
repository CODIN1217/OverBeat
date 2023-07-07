using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TweenManager;

[ExecuteInEditMode]
public class ButtonCtrler : MonoBehaviour
{
    [SerializeField]
    Vector2 buttonScale01;
    [SerializeField]
    Vector2 highlightedScale01 = new Vector2(272f, 144f);
    RectTransform buttonRect;
    RectTransform TMPRect;
    [SerializeField]
    GameObject button;
    [SerializeField]
    GameObject TMP;
    void Reset()
    {
        buttonScale01 = Vector2.zero;
    }
    void LateUpdate()
    {
        if (button == null)
            button = gameObject;
        buttonRect = button.GetComponent<RectTransform>();

        if (TMP != null)
            if (TMPRect == null)
                TMPRect = TMP.GetComponent<RectTransform>();
        if (TMP != null)
        {
            if (TMPRect != null)
            {
                if (buttonRect != null)
                {
                    buttonRect.sizeDelta = Handy.GetCorrectedVector(Vector2.Scale(buttonScale01, highlightedScale01 - (Vector2.one * 110f)) + (Vector2.one * 110f), 110f, float.MaxValue);
                }
            }
        }
    }
}
