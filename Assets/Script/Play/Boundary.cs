using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TweenManager;

public class Boundary : MonoBehaviour, ITweener, PlayManager.ITweenerInPlay, IGameObject, IScript
{
    public bool isInit;
    LevelInfo levelInfo;
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
        PM = PlayManager.Member;
        InitScript();
    }
    public void InitTween()
    {
        isInit = true;

        levelInfo = PM.GetLevelInfo(PM.levelInfoIndex);

        coverColorInfo = new TweeningInfo(levelInfo.boundaryInfo.coverColorTween, PM.GetNoteHoldSecs(PM.levelInfoIndex));

        posInfo = new TweeningInfo(levelInfo.boundaryInfo.posTween, PM.GetNoteHoldSecs(PM.levelInfoIndex));

        lineColorInfo = new TweeningInfo(levelInfo.boundaryInfo.lineColorTween, PM.GetNoteHoldSecs(PM.levelInfoIndex));

        scaleInfo = new TweeningInfo(levelInfo.boundaryInfo.scaleTween, PM.GetNoteHoldSecs(PM.levelInfoIndex));
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
        TweenMethod.TryPlayTweens(coverColorInfo, lineColorInfo, scaleInfo, posInfo);
    }
    public void TryKillTween()
    {
        TweenMethod.TryKillTweens(coverColorInfo, posInfo, lineColorInfo, scaleInfo);

        isInit = false;
    }
    public void GotoTween(float toSecs)
    {
        if (isInit)
        {
            coverColorInfo.Goto(toSecs);
            posInfo.Goto(toSecs);
            lineColorInfo.Goto(toSecs);
            scaleInfo.Goto(toSecs);
        }
    }
    public void InitScript()
    {
        PM.AddGO(this).AddTweenerGO(this).AddTweenerInPlayGO(this).AddScript(this);
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
