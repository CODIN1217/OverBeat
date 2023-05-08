using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using DG.Tweening;
using TweenManager;

public class Player : MonoBehaviour, ITweenerInfo
{
    public int playerIndex;
    public GameObject playerSide;
    public GameObject playerCenter;
    SpriteRenderer playerSideRenderer;
    SpriteRenderer playerCenterRenderer;
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
    PlayGameManager playGM;
    
    void OnEnable()
    {
        playGM = PlayGameManager.Property;
        handy = Handy.Property;
        playerSideRenderer = playerSide.GetComponent<SpriteRenderer>();
        playerCenterRenderer = playerCenter.GetComponent<SpriteRenderer>();

        handy.TryKillTween(sideClickScaleInfo);
        sideClickScaleInfo = new TweeningInfo(new TweenInfo<float>(1f, 0.8f, AnimationCurve.Linear(0f, 0f, 1f, 1f)), 0.15f);

        playGM.initTweenEvent += InitTween;
        playGM.playTweenEvent += PlayTween;
    }
    void Update()
    {
        UpdateTweenValue();
        if (playGM.isPause)
            return;

        if (playGM.GetIsKeyDown(playerIndex))
        {
            handy.StartCoroutine(SetSideClickScaleTweener(false));
        }
        else if (!playGM.GetIsKeyPress(playerIndex) && playGM.GetIsKeyUp(playerIndex))
        {
            handy.StartCoroutine(SetSideClickScaleTweener(true));
        }
    }
    void LateUpdate()
    {
        UpdatePlayerTransform();
        UpdatePlayerRenderer();
    }
    void UpdatePlayerTransform()
    {
        transform.position = handy.GetCircularPos(curDeg, curRadius, playGM.centerScript.pos);
        transform.rotation = Quaternion.Euler(0, 0, rotation);
        transform.localScale = totalScale;
        playerSide.transform.localScale = sideScale * sideClickScale;
        playerCenter.transform.localScale = centerScale;
    }
    void UpdatePlayerRenderer()
    {
        playerSideSprite = Resources.Load<Sprite>("Image/Play/Player/" + worldInfo.noteInfo.sideImageName);
        playerSideRenderer.sprite = playerSideSprite;
        playerSideRenderer.color = sideColor;
        playerCenterRenderer.color = centerColor;
    }
    public void InitTween()
    {
        worldInfo = playGM.GetWorldInfo(playGM.worldInfoIndex);

        handy.TryKillTween(radiusInfo);
        radiusInfo = new TweeningInfo(worldInfo.playerInfo[playerIndex].radiusTween, playGM.GetHoldNoteSecs(worldInfo));

        handy.TryKillTween(degInfo);
        TweenInfo<float> degTweenTemp = worldInfo.playerInfo[playerIndex].degTween.Clone();
        int degDir = worldInfo.playerInfo[playerIndex].degDir;
        degTweenTemp.endValue += degDir * degTweenTemp.startValue > degDir * degTweenTemp.endValue ? degDir * 360f : 0f;
        degInfo = new TweeningInfo(degTweenTemp, playGM.GetHoldNoteSecs(worldInfo));

        handy.TryKillTween(rotationInfo);
        rotationInfo = new TweeningInfo(worldInfo.playerInfo[playerIndex].rotationTween, playGM.GetHoldNoteSecs(worldInfo));

        handy.TryKillTween(totalScaleInfo);
        totalScaleInfo = new TweeningInfo(worldInfo.playerInfo[playerIndex].totalScaleTween, playGM.GetHoldNoteSecs(worldInfo));

        handy.TryKillTween(sideScaleInfo);
        sideScaleInfo = new TweeningInfo(worldInfo.playerInfo[playerIndex].sideScaleTween, playGM.GetHoldNoteSecs(worldInfo));

        handy.TryKillTween(centerScaleInfo);
        centerScaleInfo = new TweeningInfo(worldInfo.playerInfo[playerIndex].centerScaleTween, playGM.GetHoldNoteSecs(worldInfo));

        handy.TryKillTween(sideColorInfo);
        sideColorInfo = new TweeningInfo(worldInfo.playerInfo[playerIndex].sideColorTween, playGM.GetHoldNoteSecs(worldInfo));

        handy.TryKillTween(centerColorInfo);
        centerColorInfo = new TweeningInfo(worldInfo.playerInfo[playerIndex].centerColorTween, playGM.GetHoldNoteSecs(worldInfo));
    }
    public void UpdateTweenValue()
    {
        curDeg = handy.GetCorrectDegMaxIs0(((TweenerInfo<float>)degInfo).curValue);
        rotation = handy.GetCorrectDegMaxIs0(-(((TweenerInfo<float>)rotationInfo).curValue + curDeg));
        curRadius = ((TweenerInfo<float>)radiusInfo).curValue;
        totalScale = ((TweenerInfo<Vector2>)totalScaleInfo).curValue;
        sideClickScale = ((TweenerInfo<float>)sideClickScaleInfo).curValue;
        sideScale = ((TweenerInfo<Vector2>)sideScaleInfo).curValue;
        centerScale = ((TweenerInfo<Vector2>)centerScaleInfo).curValue;
        sideColor = playGM.GetColor01WithPlayerIndex(((TweenerInfo<Color>)sideColorInfo).curValue, playerIndex);
        centerColor = playGM.GetColor01WithPlayerIndex(((TweenerInfo<Color>)centerColorInfo).curValue, playerIndex);
    }
    public void PlayTween()
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
