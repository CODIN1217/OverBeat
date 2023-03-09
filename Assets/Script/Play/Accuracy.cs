using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Accuracy : MonoBehaviour
{
    float accuracy01;
    TextMeshProUGUI accuracyTMP;
    void Awake() {
        accuracyTMP = GetComponent<TextMeshProUGUI>();
    }
    void Update()
    {
        accuracy01 = GameManager.Property.accuracy01;
        accuracyTMP.text = accuracy01.ToString("0%");
    }
}
