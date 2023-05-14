using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using DG.Tweening;
using TweenManager;

public class Player : MonoBehaviour, ITweener, PlayManager.ITweenerInPlay, IGameObject
{
    public int playerIndex;
    public GameObject playerSide;
    public GameObject playerCenter;
    public SpriteRenderer playerSideRenderer;
    public SpriteRenderer playerCenterRenderer;
    public Sprite playerSideSprite;
    WorldInfo worldInfo;

    public float curDeg;
    public float rotation;
    public float curRadius;
    public float sideClickScale;
    public Vector2 totalScale;
    public Vector2 sideScale;
    public Vector2 centerScale;
    public Color sideColor;
    public Color centerColor;
    public TweeningInfo degInfo;
    public TweeningInfo rotationInfo;
    public TweeningInfo radiusInfo;
    public TweeningInfo totalScaleInfo;
    public TweeningInfo sideClickScaleInfo;
    public TweeningInfo sideScaleInfo;
    public TweeningInfo centerScaleInfo;
    public TweeningInfo sideColorInfo;
    public TweeningInfo centerColorInfo;

    Handy handy;
    PlayManager PM;

    void Awake()
    {
        PM = PlayManager.Property;
        handy = Handy.Property;
        PM.AddGO(this).AddTweenerGO(this).AddTweenerInPlayGO(this);
    }
    void OnEnable()
    {
        playerSideRenderer = playerSide.GetComponent<SpriteRenderer>();
        playerCenterRenderer = playerCenter.GetComponent<SpriteRenderer>();

        handy.TryKillTween(sideClickScaleInfo);
        sideClickScaleInfo = new TweeningInfo(new TweenInfo<float>(1f, 0.8f, AnimationCurve.Linear(0f, 0f, 1f, 1f)), 0.15f);

        // PM.initTweenEvent += InitTween;
        // PM.playHoldTweenEvent += PlayHoldTween;
    }
    void Update()
    {
        // UpdateTweenValue();
        if (PM.isPause)
            return;

        if (PM.GetIsKeyDown(playerIndex))
        {
            handy.StartCoroutine(SetSideClickScaleTweener(false));
        }
        else if (!PM.GetIsKeyPress(playerIndex) && PM.GetIsKeyUp(playerIndex))
        {
            handy.StartCoroutine(SetSideClickScaleTweener(true));
        }
    }
    /* void LateUpdate()
    {
        UpdateTransform();
        UpdateRenderer();
    } */
    public void InitTween()
    {
        worldInfo = PM.GetWorldInfo(PM.worldInfoIndex);

        playerSideSprite = Resources.Load<Sprite>("Image/Play/Player/" + worldInfo.noteInfo.sideImageName);

        handy.TryKillTween(radiusInfo);
        radiusInfo = new TweeningInfo(worldInfo.playerInfo[playerIndex].radiusTween, PM.GetHoldNoteSecs(worldInfo));

        handy.TryKillTween(degInfo);
        degInfo = new TweeningInfo(PM.CorrectDegTween(worldInfo.playerInfo[playerIndex].degTween, worldInfo.playerInfo[playerIndex].degDir), PM.GetHoldNoteSecs(worldInfo));

        handy.TryKillTween(rotationInfo);
        rotationInfo = new TweeningInfo(worldInfo.playerInfo[playerIndex].rotationTween, PM.GetHoldNoteSecs(worldInfo));

        handy.TryKillTween(totalScaleInfo);
        totalScaleInfo = new TweeningInfo(worldInfo.playerInfo[playerIndex].totalScaleTween, PM.GetHoldNoteSecs(worldInfo));

        handy.TryKillTween(sideScaleInfo);
        sideScaleInfo = new TweeningInfo(worldInfo.playerInfo[playerIndex].sideScaleTween, PM.GetHoldNoteSecs(worldInfo));

        handy.TryKillTween(centerScaleInfo);
        centerScaleInfo = new TweeningInfo(worldInfo.playerInfo[playerIndex].centerScaleTween, PM.GetHoldNoteSecs(worldInfo));

        handy.TryKillTween(sideColorInfo);
        sideColorInfo = new TweeningInfo(worldInfo.playerInfo[playerIndex].sideColorTween, PM.GetHoldNoteSecs(worldInfo));

        handy.TryKillTween(centerColorInfo);
        centerColorInfo = new TweeningInfo(worldInfo.playerInfo[playerIndex].centerColorTween, PM.GetHoldNoteSecs(worldInfo));
    }
    public void UpdateTweenValue()
    {
        curDeg = handy.CorrectDegMaxIs0(((TweenerInfo<float>)degInfo).curValue);
        rotation = handy.CorrectDegMaxIs0(-(((TweenerInfo<float>)rotationInfo).curValue + curDeg));
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
        handy.PlayTweens(
            radiusInfo,
            degInfo,
            rotationInfo,
            totalScaleInfo,
            sideScaleInfo,
            centerScaleInfo,
            sideColorInfo,
            centerColorInfo);
    }
    public void UpdateTransform()
    {
        transform.position = handy.GetCircularPos(curDeg, curRadius, PM.centerScript.pos);
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
    public IEnumerator SetSideClickScaleTweener(bool IsBackwards)
    {
        yield return new WaitUntil(() => !sideClickScaleInfo.tweener.IsPlaying());
        if (IsBackwards)
        {
            sideClickScaleInfo.tweener.PlayBackwards();
        }
        else
        {
            sideClickScaleInfo.tweener.PlayForward();
        }
    }
}
