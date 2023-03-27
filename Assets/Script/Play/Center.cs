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
        worldInfo = handy.GetWorldInfo();
        HP01 = GameManager.Property.HP01;
        centerImage.fillAmount = Mathf.Lerp(centerImage.fillAmount, HP01, Time.unscaledDeltaTime * 4f);
        transform.localScale = worldInfo.CenterInfo.Scale;
        transform.position = worldInfo.CenterInfo.Pos;
        centerColor = handy.GetColor01(worldInfo.CenterInfo.Color);
        centerImage.color = centerColor;
    }
}
