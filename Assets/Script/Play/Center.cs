using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class Center : MonoBehaviour
{
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
        centerImage.fillAmount = Mathf.Lerp(centerImage.fillAmount, playGM.HP01, Time.deltaTime * 4f);
        if (!handy.CompareWithBeforeValue(this.name, nameof(Update), nameof(playGM.curWorldInfoIndex), playGM.curWorldInfoIndex))
        {
            transform.DOScale(worldInfo.centerInfo.scale, worldInfo.centerInfo.scaleTween.duration).SetEase(worldInfo.centerInfo.scaleTween.ease);
            transform.DOLocalMove(worldInfo.centerInfo.pos, worldInfo.centerInfo.posTween.duration).SetEase(worldInfo.centerInfo.posTween.ease);
            centerImage.DOColor(handy.GetColor01(worldInfo.centerInfo.color), worldInfo.centerInfo.colorTween.duration).SetEase(worldInfo.centerInfo.colorTween.ease);
            handy.SetValueForCompare(this.name, nameof(Update), nameof(playGM.curWorldInfoIndex), playGM.curWorldInfoIndex);
        }
        // transform.position = worldInfo.centerInfo.pos;
        // centerColor = handy.GetColor01(worldInfo.centerInfo.color);
        // centerImage.color = centerColor;
    }
}
