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
    
    public TweenValue.TweeningInfo coverColorInfo;
    public TweenValue.TweeningInfo lineColorInfo;
    public TweenValue.TweeningInfo scaleInfo;
    public TweenValue.TweeningInfo posInfo;

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
            handy.TryKillSequence(coverColorInfo);
            coverColorInfo = new TweenValue.TweeningInfo(worldInfo.boundaryInfo.coverColorTween);

            handy.TryKillSequence(lineColorInfo);
            lineColorInfo = new TweenValue.TweeningInfo(worldInfo.boundaryInfo.lineColorTween);

            handy.TryKillSequence(scaleInfo);
            scaleInfo = new TweenValue.TweeningInfo(worldInfo.boundaryInfo.scaleTween);

            handy.TryKillSequence(posInfo);
            posInfo = new TweenValue.TweeningInfo(worldInfo.boundaryInfo.posTween);

            handy.compareValue_int.SetValueForCompare(this.name, nameof(Update), nameof(playGM.worldInfoIndex), playGM.worldInfoIndex);
        }

        boundaryCoverImage.color = (Color)coverColorInfo.curValue;
        boundaryLineImage.color = (Color)lineColorInfo.curValue;
        transform.localScale = (Vector2)scaleInfo.curValue; boundaryCover.transform.localScale = new Vector2(1f / ((Vector2)scaleInfo.curValue).x, 1f / ((Vector2)scaleInfo.curValue).y);
        transform.localPosition = (Vector2)posInfo.curValue; boundaryCover.transform.localPosition = -(Vector2)posInfo.curValue;
    }
}
