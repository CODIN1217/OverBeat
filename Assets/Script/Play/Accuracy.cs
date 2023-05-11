using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Accuracy : MonoBehaviour
{
    float accuracy01;
    TextMeshProUGUI accuracyTMP;
    Handy handy;
    PlayManager PM;
    void Awake() {
        handy = Handy.Property;
        PM = PlayManager.Property;
        accuracyTMP = GetComponent<TextMeshProUGUI>();
    }
    void Update()
    {
        accuracy01 = PM.accuracy01;
        accuracyTMP.text = accuracy01.ToString("0%");
    }
}
