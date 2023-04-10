using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Accuracy : MonoBehaviour
{
    float accuracy01;
    TextMeshProUGUI accuracyTMP;
    Handy handy;
    GameManager GM;
    void Awake() {
        handy = Handy.Property;
        GM = GameManager.Property;
        accuracyTMP = GetComponent<TextMeshProUGUI>();
    }
    void Update()
    {
        accuracy01 = GM.accuracy01;
        accuracyTMP.text = accuracy01.ToString("0%");
    }
}
