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
    PlayGameManager playGM;

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
        playGM = PlayGameManager.Property;
        handy = Handy.Property;
        playGM.initTweenEvent += InitTween;
        playGM.playTweenEvent += PlayTween;
    }
    void Update()
    {
        if (playGM.isBreakUpdate())
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
        worldInfo = playGM.GetWorldInfo(playGM.worldInfoIndex);
        
        handy.TryKillTween(coverColorInfo);
        coverColorInfo = new TweeningInfo(worldInfo.boundaryInfo.coverColorTween, playGM.GetHoldNoteSecs(playGM.worldInfoIndex)/* , null, () => playGM.baseCameraScript.BGColor */);

        handy.TryKillTween(posInfo);
        posInfo = new TweeningInfo(worldInfo.boundaryInfo.posTween, playGM.GetHoldNoteSecs(playGM.worldInfoIndex)/* , null, () => playGM.baseCameraScript.pos */);

        handy.TryKillTween(lineColorInfo);
        lineColorInfo = new TweeningInfo(worldInfo.boundaryInfo.lineColorTween, playGM.GetHoldNoteSecs(playGM.worldInfoIndex));

        handy.TryKillTween(scaleInfo);
        scaleInfo = new TweeningInfo(worldInfo.boundaryInfo.scaleTween, playGM.GetHoldNoteSecs(playGM.worldInfoIndex));
    }
    public void UpdateTweenValue()
    {
        coverColor = ((TweenerInfo<Color>)coverColorInfo).curValue;
        lineColor = ((TweenerInfo<Color>)lineColorInfo).curValue;
        scale = ((TweenerInfo<Vector2>)scaleInfo).curValue;
        pos = ((TweenerInfo<Vector2>)posInfo).curValue;
    }
    public void PlayTween()
    {
        handy.PlayTweens(coverColorInfo, lineColorInfo, scaleInfo, posInfo);
    }
}
