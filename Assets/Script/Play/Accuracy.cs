using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Accuracy : MonoBehaviour
{
    float accuracy01;
    TextMeshProUGUI accuracyTMP;
    PlayManager PM;
    void Awake()
    {
        PM = PlayManager.Member;
        accuracyTMP = GetComponent<TextMeshProUGUI>();
    }
    void Update()
    {
        if (PM.isPause)
            return;
        accuracy01 = PM.accuracy01;
        accuracyTMP.text = accuracy01.ToString("0%");
    }
}
