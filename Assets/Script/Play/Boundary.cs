using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TweenValue;

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

    public Color coverColor;
    public Color lineColor;
    public Vector2 scale;
    public Vector2 pos;

    public TweeningInfo coverColorTweener;
    public TweeningInfo lineColorTweener;
    public TweeningInfo scaleTweener;
    public TweeningInfo posTweener;

    WorldInfo beforeWorldInfo;
    void Awake()
    {
        playGM = PlayGameManager.Property;
        handy = Handy.Property;
    }
    void Update()
    {
        if (playGM.isBreakUpdate())
            return;
        beforeWorldInfo = playGM.GetWorldInfo(playGM.worldInfoIndex - 1);
        worldInfo = playGM.GetWorldInfo(playGM.worldInfoIndex);
        if (!handy.compareValue_int.CompareWithBeforeValue(this.name, nameof(Update), nameof(playGM.worldInfoIndex), playGM.worldInfoIndex))
        {
            handy.TryKillTween(coverColorTweener);
            coverColorTweener = new TweeningInfo(worldInfo.boundaryInfo.coverColorTween);

            handy.TryKillTween(lineColorTweener);
            lineColorTweener = new TweeningInfo(worldInfo.boundaryInfo.lineColorTween);

            handy.TryKillTween(scaleTweener);
            scaleTweener = new TweeningInfo(worldInfo.boundaryInfo.scaleTween);

            handy.TryKillTween(posTweener);
            posTweener = new TweeningInfo(worldInfo.boundaryInfo.posTween);

            handy.PlayTweens(coverColorTweener, lineColorTweener, scaleTweener, posTweener);

            handy.compareValue_int.SetValueForCompare(this.name, nameof(Update), nameof(playGM.worldInfoIndex), playGM.worldInfoIndex);
        }
        UpdateTweenValue();

        boundaryCoverImage.color = coverColor;
        boundaryLineImage.color = lineColor;
        transform.localScale = scale; boundaryCover.transform.localScale = new Vector2(1f / scale.x, 1f / scale.y);
        transform.localPosition = pos; boundaryCover.transform.localPosition = -pos;
    }
    void UpdateTweenValue()
    {
        coverColor = (Color)coverColorTweener.curValue;
        lineColor = (Color)lineColorTweener.curValue;
        scale = (Vector2)scaleTweener.curValue;
        pos = (Vector2)posTweener.curValue;
    }
}
