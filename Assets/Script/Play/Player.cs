using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using DG.Tweening;
using TweenValue;

public class Player : MonoBehaviour
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
    public TweeningInfo radiusTweener;
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
    }
    void Update()
    {
        NotePrefab closestNoteScript = playGM.closestNoteScripts[playerIndex];
        worldInfo = closestNoteScript.tarPlayerIndex == -1 && closestNoteScript.myEachNoteIndex == -1 ? playGM.GetWorldInfo(0) : playGM.GetWorldInfo(playerIndex, playGM.closestNoteIndex[playerIndex]);
        if (!handy.compareValue_int.CompareWithBeforeValue(this.name, nameof(Update), nameof(playGM.closestNoteIndex), playGM.closestNoteIndex[playerIndex], playerIndex))
        {
            handy.TryKillTween(radiusTweener);
            radiusTweener = new TweeningInfo(worldInfo.playerInfo[playerIndex].radiusTween, playGM.GetHoldNoteSecs(playerIndex, playGM.closestNoteIndex[playerIndex]));

            handy.TryKillTween(degInfo);
            degInfo = new TweeningInfo(worldInfo.playerInfo[playerIndex].degTween, playGM.GetHoldNoteSecs(playerIndex, playGM.closestNoteIndex[playerIndex]));

            handy.TryKillTween(rotationInfo);
            rotationInfo = new TweeningInfo(worldInfo.playerInfo[playerIndex].rotationTween, playGM.GetHoldNoteSecs(playerIndex, playGM.closestNoteIndex[playerIndex]));

            handy.TryKillTween(totalScaleInfo);
            totalScaleInfo = new TweeningInfo(worldInfo.playerInfo[playerIndex].totalScaleTween, playGM.GetHoldNoteSecs(playerIndex, playGM.closestNoteIndex[playerIndex]));

            handy.TryKillTween(sideScaleInfo);
            sideScaleInfo = new TweeningInfo(worldInfo.playerInfo[playerIndex].sideScaleTween, playGM.GetHoldNoteSecs(playerIndex, playGM.closestNoteIndex[playerIndex]));

            handy.TryKillTween(centerScaleInfo);
            centerScaleInfo = new TweeningInfo(worldInfo.playerInfo[playerIndex].centerScaleTween, playGM.GetHoldNoteSecs(playerIndex, playGM.closestNoteIndex[playerIndex]));

            handy.TryKillTween(sideColorInfo);
            sideColorInfo = new TweeningInfo(worldInfo.playerInfo[playerIndex].sideColorTween, playGM.GetHoldNoteSecs(playerIndex, playGM.closestNoteIndex[playerIndex]));

            handy.TryKillTween(centerColorInfo);
            centerColorInfo = new TweeningInfo(worldInfo.playerInfo[playerIndex].centerColorTween, playGM.GetHoldNoteSecs(playerIndex, playGM.closestNoteIndex[playerIndex]));

            handy.TryKillTween(sideClickScaleInfo);
            sideClickScaleInfo = new TweeningInfo(new TweenInfo<float>(1f, 0.8f, AnimationCurve.Linear(0f, 0f, 1f, 1f)), 0.15f);

            handy.compareValue_int.SetValueForCompare(this.name, nameof(Update), nameof(playGM.closestNoteIndex), playGM.closestNoteIndex[playerIndex], playerIndex);
        }
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
    void UpdateTweenValue()
    {
        curDeg = handy.GetCorrectDegMaxIs0((float)degInfo.curValue);
        rotation = handy.GetCorrectDegMaxIs0(-((float)rotationInfo.curValue + curDeg));
        curRadius = (float)radiusTweener.curValue;
        totalScale = (Vector2)totalScaleInfo.curValue;
        sideClickScale = (float)sideClickScaleInfo.curValue;
        sideScale = (Vector2)sideScaleInfo.curValue;
        centerScale = (Vector2)centerScaleInfo.curValue;
        sideColor = playGM.GetColor01WithPlayerIndex((Color)sideColorInfo.curValue, playerIndex);
        centerColor = playGM.GetColor01WithPlayerIndex((Color)centerColorInfo.curValue, playerIndex);
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
