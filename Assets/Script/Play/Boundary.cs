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
    Color tweenCoverColor;
    Color tweenLineColor;
    Vector2 tweenScale;
    Vector2 tweenPos;
    Sequence coverColorTweener;
    Sequence lineColorTweener;
    Sequence scaleTweener;
    Sequence posTweener;
    WorldInfo beforeWorldInfo;
    bool isAwake;
    void Awake()
    {
        playGM = PlayGameManager.Property;
        handy = Handy.Property;
        isAwake = true;
    }
    void Update()
    {
        if (playGM.isBreakUpdate())
            return;
        beforeWorldInfo = playGM.GetWorldInfo(playGM.curWorldInfoIndex - 1);
        worldInfo = playGM.GetWorldInfo(playGM.curWorldInfoIndex);
        if (isAwake)
        {
            tweenCoverColor = worldInfo.boundaryInfo.coverColor == null ? worldInfo.cameraInfo.BGColor :(Color)worldInfo.boundaryInfo.coverColor;
            tweenLineColor = beforeWorldInfo.boundaryInfo.lineColor;
            tweenScale = beforeWorldInfo.boundaryInfo.scale;
            tweenPos = null == worldInfo.boundaryInfo.pos ? worldInfo.centerInfo.pos : (Vector3)worldInfo.boundaryInfo.pos;
            isAwake = false;
        }
        if (!handy.compareValue_int.CompareWithBeforeValue(this.name, nameof(Update), nameof(playGM.curWorldInfoIndex), playGM.curWorldInfoIndex))
        {
            handy.TryKillSequence(coverColorTweener);
            coverColorTweener = DOTween.Sequence()
            .Append(DOTween.To(() => tweenCoverColor, (c) => tweenCoverColor = c
            , worldInfo.boundaryInfo.coverColor == null ? worldInfo.cameraInfo.BGColor :(Color)worldInfo.boundaryInfo.coverColor
            , worldInfo.boundaryInfo.coverColorTween.duration))
            .SetEase(worldInfo.boundaryInfo.coverColorTween.ease);

            handy.TryKillSequence(lineColorTweener);
            lineColorTweener = DOTween.Sequence()
            .Append(DOTween.To(() => tweenLineColor, (c) => tweenLineColor = c, worldInfo.boundaryInfo.lineColor, worldInfo.boundaryInfo.lineColorTween.duration))
            .SetEase(worldInfo.boundaryInfo.lineColorTween.ease);

            handy.TryKillSequence(scaleTweener);
            scaleTweener = DOTween.Sequence()
            .Append(DOTween.To(() => tweenScale, (s) => tweenScale = s, worldInfo.boundaryInfo.scale / worldInfo.cameraInfo.size, worldInfo.boundaryInfo.scaleTween.duration))
            .SetEase(worldInfo.boundaryInfo.scaleTween.ease);
            
            handy.TryKillSequence(posTweener);
            posTweener = DOTween.Sequence()
            .Append(DOTween.To(() => tweenPos, (p) => tweenPos = p, null == worldInfo.boundaryInfo.pos ? worldInfo.centerInfo.pos : (Vector3)worldInfo.boundaryInfo.pos, worldInfo.boundaryInfo.posTween.duration))
            .SetEase(worldInfo.boundaryInfo.posTween.ease);

            handy.compareValue_int.SetValueForCompare(this.name, nameof(Update), nameof(playGM.curWorldInfoIndex), playGM.curWorldInfoIndex);
        }
        boundaryCoverImage.color = tweenCoverColor;
        boundaryLineImage.color = tweenLineColor;
        transform.localScale = tweenScale;
        boundaryCover.transform.localScale = new Vector2(1f / tweenScale.x, 1f / tweenScale.y);
        transform.localPosition = tweenPos;
        boundaryCover.transform.localPosition = -tweenPos;
    }
}
