using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

[ExecuteInEditMode]
public class ComText : MonoBehaviour
{
    public string text;
    public ColorClass color;
    public GameObject target;
    Text UI_text = null;
    TextMeshProUGUI UI_TMP = null;
    void Update()
    {
        if (target == null)
            target = gameObject;
        UI_TMP = target.GetComponent<TextMeshProUGUI>();
        UI_text = target.GetComponent<Text>();
        if (UI_TMP != null)
        {
            if (text == null)
                text = UI_TMP.text;
            if (color == null)
                color = (ColorClass)UI_TMP.color;
            UI_TMP.text = text;
            UI_TMP.color = (Color)color;
        }
        else if (UI_text != null)
        {
            if (text == null)
                text = UI_text.text;
            if (color == null)
                color = (ColorClass)UI_text.color;
            UI_text.text = text;
            UI_text.color = (Color)color;
        }
    }
}
