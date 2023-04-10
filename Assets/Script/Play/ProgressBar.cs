using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ProgressBar : MonoBehaviour
{
    float progress01;
    public GameObject ProgressBarBG;
    public GameObject ProgressBarOutline;
    Image ProgressBarBGImage;
    Handy handy;
    GameManager GM;
    void Awake() {
        handy = Handy.Property;
        GM = GameManager.Property;
        ProgressBarBGImage = ProgressBarBG.GetComponent<Image>();
    }
    void Update()
    {
        progress01 = GM.progress01;
        ProgressBarBGImage.fillAmount = Mathf.Lerp(ProgressBarBGImage.fillAmount, progress01, Time.unscaledDeltaTime * 4f);
    }
}
