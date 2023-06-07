using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

[ExecuteInEditMode]
public class TextCtrler : MonoBehaviour
{
    public string stdText;
    public GameObject target;
    [Range(0, 1)]
    public float contentFillAmount = 1f;
    ComText comText;

    void Update()
    {
        if (target == null)
            target = gameObject;
            comText = target.GetComponent<ComText>();
        if (comText != null)
        {
            if (stdText == null)
                stdText = comText.text;
            comText.text = stdText.Substring(0, Mathf.CeilToInt((float)stdText.Length * contentFillAmount));
        }
    }
}
