using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class ToggleCtrler : MonoBehaviour
{
    public Vector2 toggleScale01;
    public bool isHighlighted;
    RectTransform toggleRect;
    RectTransform BGRect;
    RectTransform labelRect;
    [SerializeField]
    GameObject toggle;
    [SerializeField]
    GameObject BG;
    [SerializeField]
    GameObject label;
    void Reset()
    {
        toggleScale01 = Vector2.zero;
    }
    void Update()
    {
        if (toggle == null)
            toggle = gameObject;
        toggleRect = toggle.GetComponent<RectTransform>();
        if (label != null)
        {
            labelRect = label.GetComponent<RectTransform>();
            if (labelRect != null)
            {
                if (toggleRect != null)
                {
                    toggleRect.sizeDelta = Vector2.Scale(toggleScale01, labelRect.sizeDelta - (Vector2.one * 60f) + Vector2.right * 75f) + (Vector2.one * 60f);
                    if (BG != null)
                    {
                        BGRect = BG.GetComponent<RectTransform>();
                        if (BGRect != null)
                        {
                            BGRect.offsetMin = Vector2.zero;
                            BGRect.offsetMax = new Vector2(Mathf.Clamp(-(labelRect.sizeDelta.x + 15f), 60f - toggleRect.sizeDelta.x, labelRect.sizeDelta.x + 15f), 0f);
                        }
                    }
                }
            }
        }
    }
}
