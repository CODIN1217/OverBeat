using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using DG.Tweening;

public class Player : MonoBehaviour
{
    public int myPlayerIndex;
    public float stdDeg;
    public float curDeg;
    public float tarDeg;
    public float stdRadius;
    public float curRadius;
    public float tarRadius;
    bool isInputted;
    public GameObject playerSide;
    public GameObject playerCenter;
    SpriteRenderer playerSideRenderer;
    SpriteRenderer playerCenterRenderer;
    public Sprite playerSideSprite;
    WorldInfo worldInfo;
    Sequence sideScaleTweener;
    Sequence moveTweener;
    Sequence radiusTweener;
    Handy handy;
    void Awake()
    {
        handy = Handy.Property;
        worldInfo = handy.GetWorldInfo();
        playerSideRenderer = playerSide.GetComponent<SpriteRenderer>();
        playerCenterRenderer = playerCenter.GetComponent<SpriteRenderer>();
        stdDeg = handy.GetNextDeg();
        stdRadius = handy.GetWorldInfo(handy.noteIndex - 1).playerTarRadius[myPlayerIndex];
        curRadius = stdRadius;
        tarRadius = handy.GetWorldInfo().playerTarRadius[myPlayerIndex];
    }
    void Update()
    {
        // if (handy.GetWorldInfo().playerIndex == myPlayerIndex)
        worldInfo = handy.GetWorldInfo();
        tarDeg = handy.GetNextDeg(worldInfo);
        SetPlayerRenderer();
        handy.GetCorrectDegMaxIs0(curDeg);
        SetPlayerTransform();
        if (GameManager.Property.GetIsKeyDown() && !GameManager.Property.isPause)
        {
            SetSideScaleTweener(worldInfo.playerScale[myPlayerIndex] * 0.8f, 0.15f);
        }
        else if (!GameManager.Property.GetIsKeyPress() && GameManager.Property.GetIsKeyUp())
        {
            SideScaleToOrig();
        }
        if (handy.GetJudgmentValue() >= 1f)
            isInputted = false;

        if (GameManager.Property.isPause)
            return;
        if (handy.GetWorldInfo().playerIndex == myPlayerIndex)
        {
            if (GameManager.Property.GetIsProperKeyDown(myPlayerIndex))
            {
                if (handy.GetJudgmentValue() <= handy.judgmentRange && !isInputted)
                {
                    tarDeg += worldInfo.direction[myPlayerIndex] * tarDeg < worldInfo.direction[myPlayerIndex] * curDeg ? worldInfo.direction[myPlayerIndex] * 360f : 0f;
                    handy.closestNoteScript.TryKillFadeTweener(true);
                    handy.closestNoteScript.TryKillRadiusTweener(true);
                    if (!handy.closestNoteScript.needInput)
                    {
                        handy.closestNoteScript.ActToNeedInput();
                    }
                    StartCoroutine(CheckInputtingKeys(handy.GetWorldInfo(handy.noteIndex - 1).nextDegIndex[myPlayerIndex]));
                    TryKillMoveTweener();
                    moveTweener = DOTween.Sequence()
                    .AppendInterval(handy.GetNoteWaitTime() * Mathf.Clamp(-handy.GetSign0IsMin(handy.closestNoteScript.elapsedTimeWhenNeedInput) * handy.GetJudgmentValue(), 0f, handy.judgmentRange))
                    .Append(DOTween.To(() => stdDeg, (d) => curDeg = d, tarDeg,
                    handy.GetNoteLengthTime() * (1f - Mathf.Clamp(handy.GetSign0IsMin(handy.closestNoteScript.elapsedTimeWhenNeedInput) * handy.GetJudgmentValue(), 0f, handy.judgmentRange)))
                    .SetEase(worldInfo.playerMoveEaseType[myPlayerIndex]));
                    isInputted = true;
                }
            }
            else if (GameManager.Property.GetIsKeyDown())
            {
                GameManager.Property.SetMissJudgment();
            }
        }
    }
    void SetPlayerTransform()
    {
        transform.position = handy.GetCircularPos(curDeg, curRadius, worldInfo.centerPos);
        transform.localScale = worldInfo.playerScale[myPlayerIndex];
        transform.rotation = Quaternion.Euler(0f, 0f, handy.GetCorrectDegMaxIs0(-(worldInfo.playerRotation[myPlayerIndex] + curDeg)));
    }
    void SetPlayerRenderer()
    {
        playerSideSprite = Resources.Load<Sprite>("Image/Play/Player/" + worldInfo.SideImageName);
        playerSideRenderer.sprite = playerSideSprite;
        playerSideRenderer.color = handy.GetColor01(worldInfo.playerSideColor[myPlayerIndex]);
        playerCenterRenderer.color = handy.GetColor01(worldInfo.playerCenterColor[myPlayerIndex]);
    }
    public void TryKillMoveTweener(bool isComplete = true)
    {
        if (moveTweener != null)
        {
            moveTweener.Kill(isComplete);
            moveTweener = null;
        }
    }
    public void SetRadiusTweener(float tarRadius, float duration, AnimationCurve easeType)
    {
        TryKillRadiusTweener();
        radiusTweener = DOTween.Sequence().Append(DOTween.To(() => curRadius, r => curRadius = r, tarRadius, duration).SetEase(easeType)).SetUpdate(true);
    }
    public void SetSideScaleTweener(Vector3 tarScale, float duration, float prependInterval = 0f)
    {
        StartCoroutine(SetSideScaleTweener_delayed(tarScale, duration, prependInterval));
    }
    public void SideScaleToOrig()
    {
        if (sideScaleTweener != null)
            SetSideScaleTweener(worldInfo.playerScale[myPlayerIndex], 0.15f, 0.15f - sideScaleTweener.Elapsed());
    }
    IEnumerator SetSideScaleTweener_delayed(Vector3 tarScale, float duration, float prependInterval)
    {
        yield return new WaitForSecondsRealtime(prependInterval);
        TryKillSideScaleTweener();
        sideScaleTweener = DOTween.Sequence().Append(playerSide.transform.DOScale(tarScale, duration)).SetUpdate(true);
    }
    void TryKillRadiusTweener(bool isComplete = true)
    {
        if (radiusTweener != null)
        {
            radiusTweener.Kill(isComplete);
            radiusTweener = null;
        }
    }
    void TryKillSideScaleTweener(bool isComplete = true)
    {
        if (sideScaleTweener != null)
        {
            sideScaleTweener.Kill(isComplete);
            sideScaleTweener = null;
        }
    }
    IEnumerator CheckInputtingKeys(int nextDegIndex)
    {
        while (GameManager.Property.GetIsProperKeyPress(myPlayerIndex, nextDegIndex) && GameManager.Property.elapsedTimeWhenNeedInput01 < 1f)
        {
            yield return null;
        }
        if (GameManager.Property.elapsedTimeWhenNeedInput01 < 1f && handy.closestNoteScript.noteLengthTime != 0f)
        {
            GameManager.Property.SetMissJudgment();
            handy.closestNoteScript.StopNote();
        }
        else if (handy.closestNoteScript.noteLengthTime != 0f)
        {
            handy.judgmentGenScript.SetJudgmentText(JudgmentType.Perfect);
        }
    }
}
