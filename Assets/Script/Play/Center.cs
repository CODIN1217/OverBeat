using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TweenManager;

public class Center : MonoBehaviour, ITweener, PlayManager.ITweenerInPlay, IGameObject
{
    public bool isInit;
    Image centerImage;
    WorldInfo worldInfo;
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
        InitGameObjectScript();
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

        worldInfo = PM.GetWorldInfo(PM.worldInfoIndex);
        
        scaleInfo = new TweeningInfo(worldInfo.centerInfo.scaleTween, PM.GetNoteHoldSecs(PM.worldInfoIndex));
        
        posInfo = new TweeningInfo(worldInfo.centerInfo.posTween, PM.GetNoteHoldSecs(PM.worldInfoIndex));
        
        colorInfo = new TweeningInfo(worldInfo.centerInfo.colorTween, PM.GetNoteHoldSecs(PM.worldInfoIndex));
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
        TweenMethod.PlayTweens(scaleInfo, posInfo, colorInfo);
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
    public void InitGameObjectScript()
    {
        PM.AddGO(this).AddTweenerGO(this).AddTweenerInPlayGO(this);
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
