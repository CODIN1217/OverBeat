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
    PlayGameManager playGM;
    void Awake()
    {
        playGM = PlayGameManager.Property;
        handy = Handy.Property;
        centerImage = GetComponent<Image>();
    }
    void Update()
    {
        if (playGM.isBreakUpdate())
            return;
        worldInfo = playGM.GetWorldInfo(playGM.curWorldInfoIndex);
        HP01 = playGM.HP01;
        centerImage.fillAmount = Mathf.Lerp(centerImage.fillAmount, HP01, Time.deltaTime * 4f);
        transform.localScale = worldInfo.centerInfo.scale;
        transform.position = worldInfo.centerInfo.pos;
        centerColor = handy.GetColor01(worldInfo.centerInfo.color);
        centerImage.color = centerColor;
    }
}
