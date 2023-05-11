using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TweenManager;

public class Boundary : MonoBehaviour, ITweenerInfo
{
    Handy handy;
    WorldInfo worldInfo;
    public GameObject boundaryLine;
    public GameObject boundaryCover;
    public GameObject boundaryMask;
    public Image boundaryLineImage;
    public Image boundaryCoverImage;
    public Image boundaryMaskImage;
    PlayManager PM;

    public Color coverColor;
    public Color lineColor;
    public Vector2 scale;
    public Vector2 pos;

    public TweeningInfo coverColorInfo;
    public TweeningInfo lineColorInfo;
    public TweeningInfo scaleInfo;
    public TweeningInfo posInfo;

    // WorldInfo beforeWorldInfo;
    void Awake()
    {
        PM = PlayManager.Property;
        handy = Handy.Property;
        PM.initTweenEvent += InitTween;
        PM.playHoldTweenEvent += PlayHoldTween;
    }
    void Update()
    {
        if (PM.isBreakUpdate())
            return;
        // beforeWorldInfo = playGM.GetWorldInfo(playGM.worldInfoIndex - 1);
        /* if (!handy.compareValue_int.CompareWithBeforeValue(this.name, nameof(Update), nameof(playGM.worldInfoIndex), playGM.worldInfoIndex))
        {
            InitTween();

            PlayTween();

            handy.compareValue_int.SetValueForCompare(this.name, nameof(Update), nameof(playGM.worldInfoIndex), playGM.worldInfoIndex);
        } */
        UpdateTweenValue();

        boundaryCoverImage.color = coverColor;
        boundaryLineImage.color = lineColor;
        transform.localScale = scale; boundaryCover.transform.localScale = new Vector2(1f / scale.x, 1f / scale.y);
        transform.localPosition = pos; boundaryCover.transform.localPosition = -pos;
    }
    public void InitTween()
    {
        worldInfo = PM.GetWorldInfo(PM.worldInfoIndex);
        
        handy.TryKillTween(coverColorInfo);
        coverColorInfo = new TweeningInfo(worldInfo.boundaryInfo.coverColorTween, PM.GetHoldNoteSecs(PM.worldInfoIndex)/* , null, () => playGM.baseCameraScript.BGColor */);

        handy.TryKillTween(posInfo);
        posInfo = new TweeningInfo(worldInfo.boundaryInfo.posTween, PM.GetHoldNoteSecs(PM.worldInfoIndex)/* , null, () => playGM.baseCameraScript.pos */);

        handy.TryKillTween(lineColorInfo);
        lineColorInfo = new TweeningInfo(worldInfo.boundaryInfo.lineColorTween, PM.GetHoldNoteSecs(PM.worldInfoIndex));

        handy.TryKillTween(scaleInfo);
        scaleInfo = new TweeningInfo(worldInfo.boundaryInfo.scaleTween, PM.GetHoldNoteSecs(PM.worldInfoIndex));
    }
    public void UpdateTweenValue()
    {
        coverColor = ((TweenerInfo<Color>)coverColorInfo).curValue;
        lineColor = ((TweenerInfo<Color>)lineColorInfo).curValue;
        scale = ((TweenerInfo<Vector2>)scaleInfo).curValue;
        pos = ((TweenerInfo<Vector2>)posInfo).curValue;
    }
    public void PlayWaitTween(){}
    public void PlayHoldTween()
    {
        handy.PlayTweens(coverColorInfo, lineColorInfo, scaleInfo, posInfo);
    }
}
