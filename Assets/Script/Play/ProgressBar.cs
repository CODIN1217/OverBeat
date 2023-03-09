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
    void Awake() {
        ProgressBarBGImage = ProgressBarBG.GetComponent<Image>();
    }
    void Update()
    {
        progress01 = GameManager.Property.progress01;
        ProgressBarBGImage.fillAmount = Mathf.Lerp(ProgressBarBGImage.fillAmount, progress01, Time.unscaledDeltaTime * 4f);
    }
}
