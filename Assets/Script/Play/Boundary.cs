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

    public Color coverColor;
    public Color lineColor;
    public Vector2 scale;
    public Vector2 pos;

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
        beforeWorldInfo = playGM.GetWorldInfo(playGM.worldInfoIndex - 1);
        worldInfo = playGM.GetWorldInfo(playGM.worldInfoIndex);
        if (isAwake)
        {
            tweenCoverColor = worldInfo.boundaryInfo.coverColorTween.value == null ? worldInfo.cameraInfo.BGColorTween.value : (Color)worldInfo.boundaryInfo.coverColorTween.value;
            tweenLineColor = beforeWorldInfo.boundaryInfo.lineColorTween.value;
            tweenScale = beforeWorldInfo.boundaryInfo.scaleTween.value;
            tweenPos = null == worldInfo.boundaryInfo.posTween.value ? worldInfo.centerInfo.posTween.value : (Vector3)worldInfo.boundaryInfo.posTween.value;
            isAwake = false;
        }
        if (!handy.compareValue_int.CompareWithBeforeValue(this.name, nameof(Update), nameof(playGM.worldInfoIndex), playGM.worldInfoIndex))
        {
            handy.TryKillSequence(coverColorTweener);
            coverColorTweener = DOTween.Sequence()
            .Append(DOTween.To(() => tweenCoverColor, (c) => tweenCoverColor = c
            , worldInfo.boundaryInfo.coverColorTween.value == null ? worldInfo.cameraInfo.BGColorTween.value : (Color)worldInfo.boundaryInfo.coverColorTween.value
            , worldInfo.boundaryInfo.coverColorTween.duration))
            .SetEase(worldInfo.boundaryInfo.coverColorTween.ease);

            handy.TryKillSequence(lineColorTweener);
            lineColorTweener = DOTween.Sequence()
            .Append(DOTween.To(() => tweenLineColor, (c) => tweenLineColor = c, worldInfo.boundaryInfo.lineColorTween.value, worldInfo.boundaryInfo.lineColorTween.duration))
            .SetEase(worldInfo.boundaryInfo.lineColorTween.ease);

            handy.TryKillSequence(scaleTweener);
            scaleTweener = DOTween.Sequence()
            .Append(DOTween.To(() => tweenScale, (s) => tweenScale = s, worldInfo.boundaryInfo.scaleTween.value / worldInfo.cameraInfo.sizeTween.value, worldInfo.boundaryInfo.scaleTween.duration))
            .SetEase(worldInfo.boundaryInfo.scaleTween.ease);

            handy.TryKillSequence(posTweener);
            posTweener = DOTween.Sequence()
            .Append(DOTween.To(() => tweenPos, (p) => tweenPos = p, null == worldInfo.boundaryInfo.posTween.value ? worldInfo.centerInfo.posTween.value : (Vector3)worldInfo.boundaryInfo.posTween.value, worldInfo.boundaryInfo.posTween.duration))
            .SetEase(worldInfo.boundaryInfo.posTween.ease);

            handy.compareValue_int.SetValueForCompare(this.name, nameof(Update), nameof(playGM.worldInfoIndex), playGM.worldInfoIndex);
        }
        coverColor = tweenCoverColor;
        lineColor = tweenLineColor;
        scale = tweenScale;
        pos = tweenPos;

        boundaryCoverImage.color = coverColor;
        boundaryLineImage.color = lineColor;
        transform.localScale = scale; boundaryCover.transform.localScale = new Vector2(1f / scale.x, 1f / scale.y);
        transform.localPosition = pos; boundaryCover.transform.localPosition = -pos;
    }
}
