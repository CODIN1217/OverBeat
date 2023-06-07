using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class ToggleCtrler : MonoBehaviour
{
    RectTransform toggleCtrlerRect;
    RectTransform toggleRect;
    RectTransform labelRect;
    [SerializeField]
    GameObject toggle;
    [SerializeField]
    GameObject label;
    void Update()
    {
        toggleCtrlerRect = GetComponent<RectTransform>();
        if (toggle != null)
            toggleRect = toggle.GetComponent<RectTransform>();
        if (label != null)
        {
            labelRect = label.GetComponent<RectTransform>();
            if (labelRect != null)
            {
                if (toggleRect != null)
                    toggleRect.sizeDelta = labelRect.sizeDelta;
                if(toggleCtrlerRect != null)
                toggleCtrlerRect.sizeDelta = Handy.GetCorrectedVector(toggleRect.sizeDelta, 60f, float.MaxValue);
            }
        }
    }
}
