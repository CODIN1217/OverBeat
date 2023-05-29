using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TweenManager;

public class Center : MonoBehaviour, PlayManager.ITweenerInPlay, IGameObject, IScript, ITweener
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
    public TweeningInfo[] GetTweens()
    {
        return Handy.GetArray(scaleInfo, posInfo, colorInfo);
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
        TweenMethod.TryPlayTween(GetTweens());
    }
    public void TryKillTween()
    {
        TweenMethod.TryKillTween(GetTweens());

        isInit = false;
    }
    public void GotoTween(float toSecs)
    {
        if (isInit)
        {
            foreach (var T in GetTweens())
            {
                T.Goto(toSecs);
            }
        }
    }
    public void InitScript()
    {
        PM.AddGO(this).AddTweenerInPlayGO(this).AddScript(this).AddTweener(this);
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
