using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TweenManager;

public class Boundary : MonoBehaviour, ITweener, PlayManager.ITweenerInPlay, IGameObject
{
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
    void Awake()
    {
        PM = PlayManager.Property;
        InitGameObjectScript();
    }
    public void InitTween()
    {
        worldInfo = PM.GetWorldInfo(PM.worldInfoIndex);

        // TweenMethod.TryKillTween(coverColorInfo);
        coverColorInfo = new TweeningInfo(worldInfo.boundaryInfo.coverColorTween, PM.GetHoldNoteSecs(PM.worldInfoIndex)/* , null, () => playGM.baseCameraScript.BGColor */);

        // TweenMethod.TryKillTween(posInfo);
        posInfo = new TweeningInfo(worldInfo.boundaryInfo.posTween, PM.GetHoldNoteSecs(PM.worldInfoIndex)/* , null, () => playGM.baseCameraScript.pos */);

        // TweenMethod.TryKillTween(lineColorInfo);
        lineColorInfo = new TweeningInfo(worldInfo.boundaryInfo.lineColorTween, PM.GetHoldNoteSecs(PM.worldInfoIndex));

        // TweenMethod.TryKillTween(scaleInfo);
        scaleInfo = new TweeningInfo(worldInfo.boundaryInfo.scaleTween, PM.GetHoldNoteSecs(PM.worldInfoIndex));
    }
    public void UpdateTweenValue()
    {
        coverColor = ((TweenerInfo<Color>)coverColorInfo).curValue;
        lineColor = ((TweenerInfo<Color>)lineColorInfo).curValue;
        scale = ((TweenerInfo<Vector2>)scaleInfo).curValue;
        pos = ((TweenerInfo<Vector2>)posInfo).curValue;
    }
    public void PlayWaitTween() { }
    public void PlayHoldTween()
    {
        TweenMethod.PlayTweens(coverColorInfo, lineColorInfo, scaleInfo, posInfo);
    }
    public void TryKillTween()
    {
        TweenMethod.TryKillTweens(coverColorInfo, posInfo, lineColorInfo, scaleInfo);
    }
    public void InitGameObjectScript()
    {
        PM.AddGO(this).AddTweenerGO(this).AddTweenerInPlayGO(this);
    }
    public void UpdateTransform()
    {
        transform.localScale = scale; boundaryCover.transform.localScale = new Vector2(1f / scale.x, 1f / scale.y);
        transform.localPosition = pos; boundaryCover.transform.localPosition = -pos;
    }
    public void UpdateRenderer()
    {
        boundaryCoverImage.color = coverColor;
        boundaryLineImage.color = lineColor;
    }
}
