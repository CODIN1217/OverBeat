using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class Center : MonoBehaviour
{
    float HP01;
    Image centerImage;
    Color centerColor;
    Sequence colorTweener;
    WorldInfo worldInfo;
    Handy handy;
    void Awake()
    {
        handy = Handy.Property;
        centerImage = GetComponent<Image>();
    }
    void Update()
    {
        worldInfo = handy.GetWorldInfo(handy.noteIndex);
        HP01 = GameManager.Property.HP01;
        centerImage.fillAmount = Mathf.Lerp(centerImage.fillAmount, HP01, Time.unscaledDeltaTime * 4f);
        transform.localScale = worldInfo.centerScale;
        transform.position = worldInfo.centerPos;
        centerColor = worldInfo.centerColor;
        centerImage.color = centerColor;
    }
    /* public void TryKillColorTweener(bool isComplete = true)
    {
        if (colorTweener != null)
        {
            colorTweener.Kill(isComplete);
            colorTweener = null;
        }
    } */
}
