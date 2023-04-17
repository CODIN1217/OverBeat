using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class Boundary : MonoBehaviour
{
    Handy handy;
    WorldInfo worldInfo;
    public GameObject boundaryLine;
    public GameObject boundaryCover;
    public GameObject boundaryMask;
    public Image boundaryLineImage;
    public Image boundaryCoverImage;
    public Image boundaryMaskImage;
    PlayGameManager playGM;
    void Awake()
    {
        playGM = PlayGameManager.Property;
        handy = Handy.Property;
    }
    void Update()
    {
        if (playGM.isBreakUpdate())
            return;
        worldInfo = playGM.GetWorldInfo(playGM.curWorldInfoIndex);
        if (!handy.CompareWithBeforeValue(this.name, nameof(Update), nameof(playGM.curWorldInfoIndex), playGM.curWorldInfoIndex))
        {
            boundaryCoverImage.DOColor(worldInfo.boundaryInfo.coverColor == null ? handy.GetColor01(worldInfo.cameraInfo.BGColor) : handy.GetColor01((Color)worldInfo.boundaryInfo.coverColor),
            worldInfo.boundaryInfo.coverColorTween.duration)
            .SetEase(worldInfo.boundaryInfo.coverColorTween.ease);
            boundaryLineImage.DOColor(handy.GetColor01(worldInfo.boundaryInfo.lineColor), worldInfo.boundaryInfo.lineColorTween.duration)
            .SetEase(worldInfo.boundaryInfo.lineColorTween.ease);
            transform.DOScale(worldInfo.boundaryInfo.scale / worldInfo.cameraInfo.size, worldInfo.boundaryInfo.scaleTween.duration)
            .SetEase(worldInfo.boundaryInfo.scaleTween.ease);
            transform.DOLocalMove(null == worldInfo.boundaryInfo.pos ? worldInfo.centerInfo.pos : (Vector3)worldInfo.boundaryInfo.pos, worldInfo.boundaryInfo.posTween.duration)
            .SetEase(worldInfo.boundaryInfo.posTween.ease);
            handy.SetValueForCompare(this.name, nameof(Update), nameof(playGM.curWorldInfoIndex), playGM.curWorldInfoIndex);
        }
    }
}
