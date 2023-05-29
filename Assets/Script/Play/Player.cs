using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using DG.Tweening;
using TweenManager;

public class Player : MonoBehaviour, PlayManager.ITweenerInPlay, IGameObject, IScript, ITweener
{
    public int playerIndex;
    public bool isInit;
    public GameObject playerSide;
    public GameObject playerCenter;
    public SpriteRenderer playerSideRenderer;
    public SpriteRenderer playerCenterRenderer;
    public Sprite playerSideSprite;
    LevelInfo levelInfo;

    public float curDeg;
    public float rotation;
    public float curRadius;
    public Vector2 totalScale;
    public Vector2 sideScale;
    public Vector2 centerScale;
    public Color sideColor;
    public Color centerColor;

    public TweeningInfo degInfo;
    public TweeningInfo rotationInfo;
    public TweeningInfo radiusInfo;
    public TweeningInfo totalScaleInfo;
    public TweeningInfo sideScaleInfo;
    public TweeningInfo centerScaleInfo;
    public TweeningInfo sideColorInfo;
    public TweeningInfo centerColorInfo;

    public float sideSubScale;

    public TweeningInfo sideSubScaleInfo;

    PlayManager PM;

    void Awake()
    {
        PM = PlayManager.Member;
        InitScript();
        playerSideRenderer = playerSide.GetComponent<SpriteRenderer>();
        playerCenterRenderer = playerCenter.GetComponent<SpriteRenderer>();
    }
    void OnEnable()
    {
        InitSideSubScaleInfo();
    }
    void Update()
    {
        if (PM.isStop || PM.isPause)
        {
            if (PM.isGameOver)
            {
                TweenMethod.TryPauseTween(GetTweens());
                TweenMethod.TryKillTween(sideSubScaleInfo);
                sideSubScaleInfo = new TweeningInfo(new TweenInfo<float>(sideSubScale, 0f, AnimationCurve.Linear(0f, 0f, 1f, 1f)), 1.5f);
            }
            return;
        }

        if (PM.GetIsKeyDown(playerIndex))
        {
            SetSideSubScaleTweener(false);
        }
        else if (!PM.GetIsKeyPress(playerIndex) && PM.GetIsKeyUp(playerIndex))
        {
            SetSideSubScaleTweener(true);
        }
    }
    public TweeningInfo[] GetTweens()
    {
        return Handy.GetArray(
            radiusInfo,
            degInfo,
            rotationInfo,
            totalScaleInfo,
            sideScaleInfo,
            centerScaleInfo,
            sideColorInfo,
            centerColorInfo);
    }
    public void InitTween()
    {
        isInit = true;

        levelInfo = PM.GetLevelInfo(PM.levelInfoIndex);

        playerSideSprite = Resources.Load<Sprite>("Image/Play/Player/" + levelInfo.noteInfo.sideImageName);

        radiusInfo = new TweeningInfo(levelInfo.playerInfo[playerIndex].radiusTween, PM.GetNoteHoldSecs(levelInfo));

        degInfo = new TweeningInfo(PM.CorrectDegTween(levelInfo.playerInfo[playerIndex].degTween, levelInfo.playerInfo[playerIndex].degDir), PM.GetNoteHoldSecs(levelInfo));

        rotationInfo = new TweeningInfo(levelInfo.playerInfo[playerIndex].rotationTween, PM.GetNoteHoldSecs(levelInfo));

        totalScaleInfo = new TweeningInfo(levelInfo.playerInfo[playerIndex].totalScaleTween, PM.GetNoteHoldSecs(levelInfo));

        sideScaleInfo = new TweeningInfo(levelInfo.playerInfo[playerIndex].sideScaleTween, PM.GetNoteHoldSecs(levelInfo));

        centerScaleInfo = new TweeningInfo(levelInfo.playerInfo[playerIndex].centerScaleTween, PM.GetNoteHoldSecs(levelInfo));

        sideColorInfo = new TweeningInfo(levelInfo.playerInfo[playerIndex].sideColorTween, PM.GetNoteHoldSecs(levelInfo));

        centerColorInfo = new TweeningInfo(levelInfo.playerInfo[playerIndex].centerColorTween, PM.GetNoteHoldSecs(levelInfo));
    }
    public void UpdateTweenValue()
    {
        curDeg = Handy.GetCorrectedDegMaxIs0(((TweenerInfo<float>)degInfo).curValue);
        rotation = Handy.GetCorrectedDegMaxIs0(-(((TweenerInfo<float>)rotationInfo).curValue + curDeg));
        curRadius = ((TweenerInfo<float>)radiusInfo).curValue;
        totalScale = ((TweenerInfo<Vector2>)totalScaleInfo).curValue;
        sideSubScale = ((TweenerInfo<float>)sideSubScaleInfo).curValue;
        sideScale = ((TweenerInfo<Vector2>)sideScaleInfo).curValue;
        centerScale = ((TweenerInfo<Vector2>)centerScaleInfo).curValue;
        sideColor = PM.GetColor01WithPlayerIndex(((TweenerInfo<Color>)sideColorInfo).curValue, playerIndex);
        centerColor = PM.GetColor01WithPlayerIndex(((TweenerInfo<Color>)centerColorInfo).curValue, playerIndex);
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
        transform.position = Handy.GetCircularPos(curDeg, curRadius, PM.centerScript.pos);
        transform.rotation = Quaternion.Euler(0, 0, rotation);
        transform.localScale = totalScale;
        playerSide.transform.localScale = sideScale * sideSubScale;
        playerCenter.transform.localScale = centerScale;
    }
    public void UpdateRenderer()
    {
        playerSideRenderer.sprite = playerSideSprite;
        playerSideRenderer.color = sideColor;
        playerCenterRenderer.color = centerColor;
    }
    public void InitSideSubScaleInfo()
    {
        TweenMethod.TryKillTween(sideSubScaleInfo);
        sideSubScaleInfo = new TweeningInfo(new TweenInfo<float>(1f, 0.8f, AnimationCurve.Linear(0f, 0f, 1f, 1f)), 0.15f);
    }
    public void SetSideSubScaleTweener(bool IsBackwards)
    {
        PM.StartCoroutine(SetSideSubScaleTweenerCo(IsBackwards));
    }
    IEnumerator SetSideSubScaleTweenerCo(bool IsBackwards)
    {
        yield return new WaitUntil(() => !sideSubScaleInfo.IsPlaying());
        if (IsBackwards)
        {
            TweenMethod.TrySetBackward(sideSubScaleInfo);
        }
        else
        {
            TweenMethod.TrySetForward(sideSubScaleInfo);
        }
        TweenMethod.TryPlayTween(sideSubScaleInfo);
    }
}
