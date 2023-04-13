using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Accuracy : MonoBehaviour
{
    float accuracy01;
    TextMeshProUGUI accuracyTMP;
    Handy handy;
    PlayGameManager playGM;
    void Awake() {
        handy = Handy.Property;
        playGM = PlayGameManager.Property;
        accuracyTMP = GetComponent<TextMeshProUGUI>();
    }
    void Update()
    {
        accuracy01 = playGM.accuracy01;
        accuracyTMP.text = accuracy01.ToString("0%");
    }
}
