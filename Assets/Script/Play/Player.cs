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
        worldInfo = handy.GetWorldInfo(myPlayerIndex);
        playerSideRenderer = playerSide.GetComponent<SpriteRenderer>();
        playerCenterRenderer = playerCenter.GetComponent<SpriteRenderer>();
        stdDeg = handy.GetNextDeg(myPlayerIndex);
        stdRadius = handy.GetWorldInfo(myPlayerIndex, handy.noteIndexes[myPlayerIndex] - 1).PlayerInfo.TarRadius;
        curRadius = stdRadius;
        tarRadius = handy.GetWorldInfo(myPlayerIndex).PlayerInfo.TarRadius;
    }
    void Update()
    {
        // if (handy.GetWorldInfo().PlayerInfo.Index == myPlayerIndex)
        worldInfo = handy.GetWorldInfo(myPlayerIndex);
        tarDeg = handy.GetNextDeg(worldInfo);
        SetPlayerRenderer();
        handy.GetCorrectDegMaxIs0(curDeg);
        SetPlayerTransform();
        if (GameManager.Property.GetIsKeyDown(myPlayerIndex) && !GameManager.Property.isPause)
        {
            SetSideScaleTweener(worldInfo.PlayerInfo.Scale * 0.8f, 0.15f);
        }
        else if (!GameManager.Property.GetIsKeyPress(myPlayerIndex) && GameManager.Property.GetIsKeyUp(myPlayerIndex))
        {
            SideScaleToOrig();
        }
        if (handy.GetJudgmentValue(myPlayerIndex) >= 1f)
            isInputted = false;

        if (GameManager.Property.isPause)
            return;
        if (handy.GetWorldInfo(myPlayerIndex).PlayerInfo.Index == myPlayerIndex)
        {
            if (GameManager.Property.GetIsProperKeyDown(myPlayerIndex))
            {
                if (handy.GetJudgmentValue(myPlayerIndex) <= handy.judgmentRanges[myPlayerIndex] && !isInputted)
                {
                    tarDeg += worldInfo.PlayerInfo.MoveDir * tarDeg < worldInfo.PlayerInfo.MoveDir * curDeg ? worldInfo.PlayerInfo.MoveDir * 360f : 0f;
                    handy.closestNoteScripts[myPlayerIndex].TryKillFadeTweener(true);
                    handy.closestNoteScripts[myPlayerIndex].TryKillRadiusTweener(true);
                    if (!handy.closestNoteScripts[myPlayerIndex].needInput)
                    {
                        handy.closestNoteScripts[myPlayerIndex].ActToNeedInput();
                    }
                    StartCoroutine(CheckInputtingKeys(handy.GetWorldInfo(myPlayerIndex, handy.noteIndexes[myPlayerIndex] - 1).NoteInfo.NextDegIndex));
                    TryKillMoveTweener();
                    moveTweener = DOTween.Sequence()
                    .AppendInterval(handy.GetNoteWaitTime(myPlayerIndex) * Mathf.Clamp(-handy.GetSign0IsMin(handy.closestNoteScripts[myPlayerIndex].elapsedTimeWhenNeedInput) * handy.GetJudgmentValue(myPlayerIndex), 0f, handy.judgmentRanges[myPlayerIndex]))
                    .Append(DOTween.To(() => stdDeg, (d) => curDeg = d, tarDeg,
                    handy.GetNoteLengthTime(myPlayerIndex) * (1f - Mathf.Clamp(handy.GetSign0IsMin(handy.closestNoteScripts[myPlayerIndex].elapsedTimeWhenNeedInput) * handy.GetJudgmentValue(myPlayerIndex), 0f, handy.judgmentRanges[myPlayerIndex])))
                    .SetEase(worldInfo.PlayerInfo.DegEase));
                    isInputted = true;
                }
            }
            else if (GameManager.Property.GetIsKeyDown(myPlayerIndex))
            {
                GameManager.Property.SetMissJudgment(myPlayerIndex);
            }
        }
    }
    void SetPlayerTransform()
    {
        transform.position = handy.GetCircularPos(curDeg, curRadius, worldInfo.CenterInfo.Pos);
        transform.localScale = worldInfo.PlayerInfo.Scale;
        transform.rotation = Quaternion.Euler(0f, 0f, handy.GetCorrectDegMaxIs0(-(worldInfo.PlayerInfo.Rotation + curDeg)));
    }
    void SetPlayerRenderer()
    {
        playerSideSprite = Resources.Load<Sprite>("Image/Play/Player/" + worldInfo.NoteInfo.SideImageName);
        playerSideRenderer.sprite = playerSideSprite;
        playerSideRenderer.color = handy.GetColor01(worldInfo.PlayerInfo.SideColor);
        playerCenterRenderer.color = handy.GetColor01(worldInfo.PlayerInfo.CenterColor);
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
            SetSideScaleTweener(worldInfo.PlayerInfo.Scale, 0.15f, 0.15f - sideScaleTweener.Elapsed());
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
        while (GameManager.Property.GetIsProperKeyPress(myPlayerIndex, nextDegIndex) && handy.GetElapsedTimeWhenNeedInput01(myPlayerIndex) < 1f)
        {
            yield return null;
        }
        if (handy.GetElapsedTimeWhenNeedInput01(myPlayerIndex) < 1f && handy.closestNoteScripts[myPlayerIndex].noteLengthTime != 0f)
        {
            GameManager.Property.SetMissJudgment(myPlayerIndex);
            handy.closestNoteScripts[myPlayerIndex].StopNote();
        }
        else if (handy.closestNoteScripts[myPlayerIndex].noteLengthTime != 0f)
        {
            handy.judgmentGenScript.SetJudgmentText(myPlayerIndex, JudgmentType.Perfect);
        }
    }
}
