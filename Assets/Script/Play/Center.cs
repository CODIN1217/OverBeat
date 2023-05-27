using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TweenManager;

public class Center : MonoBehaviour, ITweener, PlayManager.ITweenerInPlay, IGameObject, IScript
{
    public bool isInit;
    Image centerImage;
    LevelInfo levelInfo;
    PlayManager PM;

    public Vector2 scale;
    public Vector2 pos;
    public Color color;

    public TweeningInfo scaleInfo;
    public TweeningInfo posInfo;
    public TweeningInfo colorInfo;
    void Awake()
    {
        PM = PlayManager.Member;
        centerImage = GetComponent<Image>();
        InitScript();
    }
    void Update()
    {
        if (PM.isPause)
            return;
        centerImage.fillAmount = Mathf.Lerp(centerImage.fillAmount, PM.HP01, Time.deltaTime * 4f);
    }
    public void InitTween()
    {
        isInit = true;

        levelInfo = PM.GetLevelInfo(PM.levelInfoIndex);

        scaleInfo = new TweeningInfo(levelInfo.centerInfo.scaleTween, PM.GetNoteHoldSecs(PM.levelInfoIndex));

        posInfo = new TweeningInfo(levelInfo.centerInfo.posTween, PM.GetNoteHoldSecs(PM.levelInfoIndex));

        colorInfo = new TweeningInfo(levelInfo.centerInfo.colorTween, PM.GetNoteHoldSecs(PM.levelInfoIndex));
    }
    public void UpdateTweenValue()
    {
        scale = ((TweenerInfo<Vector2>)scaleInfo).curValue;
        pos = ((TweenerInfo<Vector2>)posInfo).curValue;
        color = ((TweenerInfo<Color>)colorInfo).curValue;
    }
    public void PlayWaitTween() { }
    public void PlayHoldTween()
    {
        TweenMethod.TryPlayTweens(scaleInfo, posInfo, colorInfo);
    }
    public void TryKillTween()
    {
        TweenMethod.TryKillTweens(scaleInfo, posInfo, colorInfo);

        isInit = false;
    }
    public void GotoTween(float toSecs)
    {
        if (isInit)
        {
            scaleInfo.Goto(toSecs);
            posInfo.Goto(toSecs);
            colorInfo.Goto(toSecs);
        }
    }
    public void InitScript()
    {
        PM.AddGO(this).AddTweenerGO(this).AddTweenerInPlayGO(this).AddScript(this);
    }
    public void UpdateTransform()
    {
        transform.localScale = scale;
        transform.localPosition = pos;
    }
    public void UpdateRenderer()
    {
        centerImage.color = color;
    }
}
