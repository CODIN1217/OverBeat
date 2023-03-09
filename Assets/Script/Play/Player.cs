using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using DG.Tweening;

public class Player : MonoBehaviour
{
    public float stdDeg;
    public float curDeg;
    public float tarDeg;
    public float radius;
    public bool isInputted;
    // bool isInputting;
    public PlayMode mode;
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
        playerSideRenderer = playerSide.GetComponent<SpriteRenderer>();
        playerCenterRenderer = playerCenter.GetComponent<SpriteRenderer>();
        stdDeg = handy.GetNextDeg(0);
        radius = handy.GetWorldInfo(0).playerRadius;
    }
    void Update()
    {
        worldInfo = handy.GetWorldInfo(handy.noteIndex);
        mode = worldInfo.mode;
        tarDeg = handy.GetNextDeg(worldInfo);
        SetPlayerRenderer();
        handy.GetCorrectDegMaxIs0(curDeg);
        SetPlayerTransform();
        /* if (GameManager.Property.numberOfInputKeys > 0 && !GameManager.Property.isPause)
        {
            SetSideScaleTweener(worldInfo.playerScale * 0.8f, 0.15f);
        } */
        if (GameManager.Property.GetIsKeyDown() && !GameManager.Property.isPause)
        {
            SetSideScaleTweener(worldInfo.playerScale * 0.8f, 0.15f);
        }
        else if (!GameManager.Property.GetIsKeyPress() && GameManager.Property.GetIsKeyUp())
        {
            SideScaleToOrig();
        }
        if (handy.GetJudgmentValue() >= 1f)
            isInputted = false;

        if (GameManager.Property.isPause)
            return;
        // if (GameManager.Property.numberOfInputtingKeys == 0)
        //     isInputting = false;

        // if (!isInputted)
        // {
        //     if (GameManager.Property.numberOfInputKeys > 0)
        //     {
        //         if (mode == PlayMode.TelePort)
        //             curDeg = tarDeg;

        //         isInputted = true;
        //     }
        // }
        if (GameManager.Property.GetIsProperKeyDown())
        {
            if (handy.GetJudgmentValue() <= handy.judgmentRange && !isInputted)
            {
                tarDeg += worldInfo.direction * tarDeg < worldInfo.direction * curDeg ? worldInfo.direction * 360f : 0f;
                handy.closestNoteScript.TryKillFadeTweener(true);
                handy.closestNoteScript.TryKillRadiusTweener(true);
                // Debug.Log(handy.GetJudgmentValue());
                if (!handy.closestNoteScript.needInput)
                {
                    // handy.WriteLog(handy.GetJudgmentValue(), GameManager.Property.elapsedTimeWhenNeedlessInput01, GameManager.Property.elapsedTimeWhenNeedInput01, handy.closestNote.transform.position - transform.position, handy.closestNoteScript.curRadius);
                    handy.closestNoteScript.ActToNeedInput();
                }
                StartCoroutine(CheckInputtingKeys(handy.GetWorldInfo(handy.noteIndex - 1).nextDegIndex));
                TryKillMoveTweener();
                moveTweener = DOTween.Sequence()
                .AppendInterval(handy.GetNoteWaitTime() * Mathf.Clamp(-handy.GetSign0IsMin(handy.closestNoteScript.elapsedTimeWhenNeedInput) * handy.GetJudgmentValue(), 0f, handy.judgmentRange))
                .Append(DOTween.To(() => stdDeg, (d) => curDeg = d, tarDeg,
                handy.GetNoteLengthTime() * (1f - Mathf.Clamp(handy.GetSign0IsMin(handy.closestNoteScript.elapsedTimeWhenNeedInput) * handy.GetJudgmentValue(), 0f, handy.judgmentRange)))
                .SetEase(worldInfo.playerMoveEaseType));
                isInputted = true;
            }
        }
        else if (GameManager.Property.GetIsKeyDown())
        {
            GameManager.Property.SetMissJudgment();
        }
    }
    void SetPlayerTransform()
    {
        transform.position = handy.GetCircularPos(curDeg, radius, worldInfo.centerPos);
        transform.localScale = worldInfo.playerScale;
        transform.rotation = Quaternion.Euler(0f, 0f, handy.GetCorrectDegMaxIs0(-(worldInfo.playerRotation + curDeg)));
    }
    void SetPlayerRenderer()
    {
        playerSideSprite = Resources.Load<Sprite>("Image/Play/Player/" + worldInfo.SideImageName);
        playerSideRenderer.sprite = playerSideSprite;
        playerSideRenderer.color = worldInfo.playerSideColor;
        playerCenterRenderer.color = worldInfo.playerCenterColor;
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
        radiusTweener = DOTween.Sequence().Append(DOTween.To(() => radius, r => radius = r, tarRadius, duration).SetEase(easeType)).SetUpdate(true);
    }
    /* public void SetRadiusTweener(float? tarRadius = null, float? duration = null, AnimationCurve easeType = null)
    {
        if(tarRadius == null)
        tarRadius = WorldInfo.playerRadius;
        if(duration == null)
        duration = handy.closestNoteScript.noteLengthTime;
        if(easeType == null)
        easeType = WorldInfo.playerRadiusEaseType;
        TryKillRadiusTweener();
        radiusTweener = DOTween.Sequence().Append(DOTween.To(() => radius, r => radius = r, (float)tarRadius, (float)duration).SetEase(easeType)).SetUpdate(true);
    } */
    public void SetSideScaleTweener(Vector3 tarScale, float duration, float prependInterval = 0f)
    {
        StartCoroutine(SetSideScaleTweener_delayed(tarScale, duration, prependInterval));
    }
    public void SideScaleToOrig()
    {
        if (sideScaleTweener != null)
            SetSideScaleTweener(worldInfo.playerScale, 0.15f, 0.15f - sideScaleTweener.Elapsed());
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
        while (GameManager.Property.GetIsProperKeyPress(nextDegIndex) && GameManager.Property.elapsedTimeWhenNeedInput01 < 1f)
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
