using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using DG.Tweening;
using TweenManager;

public class Player : MonoBehaviour, ITweener, PlayManager.ITweenerInPlay, IGameObject
{
    public int playerIndex;
    public bool isInit;
    public GameObject playerSide;
    public GameObject playerCenter;
    public SpriteRenderer playerSideRenderer;
    public SpriteRenderer playerCenterRenderer;
    public Sprite playerSideSprite;
    WorldInfo worldInfo;

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

    public float sideClickScale;

    public TweeningInfo sideClickScaleInfo;

    PlayManager PM;

    void Awake()
    {
        PM = PlayManager.Property;
        InitGameObjectScript();
        playerSideRenderer = playerSide.GetComponent<SpriteRenderer>();
        playerCenterRenderer = playerCenter.GetComponent<SpriteRenderer>();
    }
    void OnEnable()
    {
        InitSideClickScaleInfo();
    }
    void Update()
    {
        if (PM.isStop || PM.isPause)
            return;

        if (PM.GetIsKeyDown(playerIndex))
        {
            SetSideClickScaleTweener(false);
        }
        else if (!PM.GetIsKeyPress(playerIndex) && PM.GetIsKeyUp(playerIndex))
        {
            SetSideClickScaleTweener(true);
        }
    }
    public void InitTween()
    {
        isInit = true;

        worldInfo = PM.GetWorldInfo(PM.worldInfoIndex);

        playerSideSprite = Resources.Load<Sprite>("Image/Play/Player/" + worldInfo.noteInfo.sideImageName);

        // TweenMethod.TryKillTween(radiusInfo);
        radiusInfo = new TweeningInfo(worldInfo.playerInfo[playerIndex].radiusTween, PM.GetNoteHoldSecs(worldInfo));

        // TweenMethod.TryKillTween(degInfo);
        degInfo = new TweeningInfo(PM.CorrectDegTween(worldInfo.playerInfo[playerIndex].degTween, worldInfo.playerInfo[playerIndex].degDir), PM.GetNoteHoldSecs(worldInfo));

        // TweenMethod.TryKillTween(rotationInfo);
        rotationInfo = new TweeningInfo(worldInfo.playerInfo[playerIndex].rotationTween, PM.GetNoteHoldSecs(worldInfo));

        // TweenMethod.TryKillTween(totalScaleInfo);
        totalScaleInfo = new TweeningInfo(worldInfo.playerInfo[playerIndex].totalScaleTween, PM.GetNoteHoldSecs(worldInfo));

        // TweenMethod.TryKillTween(sideScaleInfo);
        sideScaleInfo = new TweeningInfo(worldInfo.playerInfo[playerIndex].sideScaleTween, PM.GetNoteHoldSecs(worldInfo));

        // TweenMethod.TryKillTween(centerScaleInfo);
        centerScaleInfo = new TweeningInfo(worldInfo.playerInfo[playerIndex].centerScaleTween, PM.GetNoteHoldSecs(worldInfo));

        // TweenMethod.TryKillTween(sideColorInfo);
        sideColorInfo = new TweeningInfo(worldInfo.playerInfo[playerIndex].sideColorTween, PM.GetNoteHoldSecs(worldInfo));

        // TweenMethod.TryKillTween(centerColorInfo);
        centerColorInfo = new TweeningInfo(worldInfo.playerInfo[playerIndex].centerColorTween, PM.GetNoteHoldSecs(worldInfo));
    }
    public void UpdateTweenValue()
    {
        curDeg = Handy.Math.DegMethod.CorrectDegMaxIs0(((TweenerInfo<float>)degInfo).curValue);
        rotation = Handy.Math.DegMethod.CorrectDegMaxIs0(-(((TweenerInfo<float>)rotationInfo).curValue + curDeg));
        curRadius = ((TweenerInfo<float>)radiusInfo).curValue;
        totalScale = ((TweenerInfo<Vector2>)totalScaleInfo).curValue;
        sideClickScale = ((TweenerInfo<float>)sideClickScaleInfo).curValue;
        sideScale = ((TweenerInfo<Vector2>)sideScaleInfo).curValue;
        centerScale = ((TweenerInfo<Vector2>)centerScaleInfo).curValue;
        sideColor = PM.GetColor01WithPlayerIndex(((TweenerInfo<Color>)sideColorInfo).curValue, playerIndex);
        centerColor = PM.GetColor01WithPlayerIndex(((TweenerInfo<Color>)centerColorInfo).curValue, playerIndex);
    }
    public void PlayWaitTween() { }
    public void PlayHoldTween()
    {
        TweenMethod.PlayTweens(
            radiusInfo,
            degInfo,
            rotationInfo,
            totalScaleInfo,
            sideScaleInfo,
            centerScaleInfo,
            sideColorInfo,
            centerColorInfo);
    }
    public void TryKillTween()
    {
        TweenMethod.TryKillTweens(radiusInfo,
        degInfo,
        rotationInfo,
        totalScaleInfo,
        sideScaleInfo,
        centerScaleInfo,
        sideColorInfo,
        centerColorInfo);
        
        isInit = false;
    }
    public void GotoTween(float toSecs)
    {
        if (isInit)
        {
            degInfo.Goto(toSecs);
            rotationInfo.Goto(toSecs);
            totalScaleInfo.Goto(toSecs);
            sideScaleInfo.Goto(toSecs);
            centerScaleInfo.Goto(toSecs);
            sideColorInfo.Goto(toSecs);
            centerColorInfo.Goto(toSecs);
        }
    }
    public void InitGameObjectScript()
    {
        PM.AddGO(this).AddTweenerGO(this).AddTweenerInPlayGO(this);
    }
    public void UpdateTransform()
    {
        transform.position = Handy.Transform.PosMethod.GetCircularPos(curDeg, curRadius, PM.centerScript.pos);
        transform.rotation = Quaternion.Euler(0, 0, rotation);
        transform.localScale = totalScale;
        playerSide.transform.localScale = sideScale * sideClickScale;
        playerCenter.transform.localScale = centerScale;
    }
    public void UpdateRenderer()
    {
        playerSideRenderer.sprite = playerSideSprite;
        playerSideRenderer.color = sideColor;
        playerCenterRenderer.color = centerColor;
    }
    public void InitSideClickScaleInfo()
    {
        TweenMethod.TryKillTween(sideClickScaleInfo);
        sideClickScaleInfo = new TweeningInfo(new TweenInfo<float>(1f, 0.8f, AnimationCurve.Linear(0f, 0f, 1f, 1f)), 0.15f);
    }
    public void SetSideClickScaleTweener(bool IsBackwards)
    {
        PM.StartCoroutine(SetSideClickScaleTweenerCo(IsBackwards));
    }
    IEnumerator SetSideClickScaleTweenerCo(bool IsBackwards)
    {
        yield return new WaitUntil(() => !sideClickScaleInfo.IsPlaying());
        if (IsBackwards)
        {
            sideClickScaleInfo.SetBackward();
        }
        else
        {
            sideClickScaleInfo.SetForward();
        }
        sideClickScaleInfo.Play();
    }
}
