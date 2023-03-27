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
        playerSideRenderer = playerSide.GetComponent<SpriteRenderer>();
        playerCenterRenderer = playerCenter.GetComponent<SpriteRenderer>();
        worldInfo = handy.GetWorldInfo();
        stdDeg = handy.GetNextDeg(myPlayerIndex);
        stdRadius = handy.GetWorldInfo(GameManager.Property.worldInfoIndex - 1).PlayerInfo[myPlayerIndex].TarRadius;
        curRadius = stdRadius;
        tarRadius = handy.GetWorldInfo().PlayerInfo[myPlayerIndex].TarRadius;
    }
    void Update()
    {
        // if (handy.GetWorldInfo().PlayerInfo.Index == myPlayerIndex)
        worldInfo = handy.GetWorldInfo((int?)myPlayerIndex);
        tarDeg = handy.GetNextDeg(myPlayerIndex, worldInfo);
        SetPlayerRenderer();
        handy.GetCorrectDegMaxIs0(curDeg);
        SetPlayerTransform();
        if (GameManager.Property.GetIsKeyDown(myPlayerIndex) && !GameManager.Property.isPause)
        {
            SetSideScaleTweener(worldInfo.PlayerInfo[myPlayerIndex].Scale * 0.8f, 0.15f);
        }
        else if (!GameManager.Property.GetIsKeyPress(myPlayerIndex) && GameManager.Property.GetIsKeyUp(myPlayerIndex))
        {
            SideScaleToOrig();
        }
        if (handy.GetJudgmentValue(myPlayerIndex) >= 1f)
            isInputted = false;

        if (GameManager.Property.isPause)
            return;
        if (GameManager.Property.GetIsProperKeyDown(myPlayerIndex))
        {
            if (handy.GetJudgmentValue(myPlayerIndex) <= handy.judgmentRange && !isInputted)
            {
                tarDeg += worldInfo.PlayerInfo[myPlayerIndex].MoveDir * tarDeg < worldInfo.PlayerInfo[myPlayerIndex].MoveDir * curDeg ? worldInfo.PlayerInfo[myPlayerIndex].MoveDir * 360f : 0f;
                handy.closestNoteScripts[myPlayerIndex].TryKillFadeTweener(true);
                handy.closestNoteScripts[myPlayerIndex].TryKillRadiusTweener(true);
                if (!handy.closestNoteScripts[myPlayerIndex].needInput)
                {
                    handy.closestNoteScripts[myPlayerIndex].ActToNeedInput();
                }
                StartCoroutine(CheckInputtingKeys(handy.GetWorldInfo(GameManager.Property.worldInfoIndex - 1).NoteInfo[myPlayerIndex].NextDegIndex));
                TryKillMoveTweener();
                moveTweener = DOTween.Sequence()
                .AppendInterval(handy.GetNoteWaitTime(myPlayerIndex) * Mathf.Clamp(-handy.GetSign0IsMin(handy.closestNoteScripts[myPlayerIndex].elapsedTimeWhenNeedInput) * handy.GetJudgmentValue(myPlayerIndex), 0f, handy.judgmentRange))
                .Append(DOTween.To(() => stdDeg, (d) => curDeg = d, tarDeg,
                handy.GetNoteLengthTime(myPlayerIndex) * (1f - Mathf.Clamp(handy.GetSign0IsMin(handy.closestNoteScripts[myPlayerIndex].elapsedTimeWhenNeedInput) * handy.GetJudgmentValue(myPlayerIndex), 0f, handy.judgmentRange)))
                .SetEase(worldInfo.PlayerInfo[myPlayerIndex].DegEase))
                .SetUpdate(true);
                isInputted = true;
            }
        }
        else if (GameManager.Property.GetIsKeyDown(myPlayerIndex))
        {
            GameManager.Property.SetMissJudgment(myPlayerIndex);
        }
        /* if(!GameManager.Property.isEnd && GameManager.Property.isPause){
            sideScaleTweener.Pause();
            moveTweener.Pause();
            radiusTweener.Pause();
        }
        else{
            sideScaleTweener.Play();
            moveTweener.Play();
            radiusTweener.Play();
        } */

    }
    void SetPlayerTransform()
    {
        transform.position = handy.GetCircularPos(curDeg, curRadius, worldInfo.CenterInfo.Pos);
        transform.localScale = worldInfo.PlayerInfo[myPlayerIndex].Scale;
        transform.rotation = Quaternion.Euler(0f, 0f, handy.GetCorrectDegMaxIs0(-(worldInfo.PlayerInfo[myPlayerIndex].Rotation + curDeg)));
    }
    void SetPlayerRenderer()
    {
        playerSideSprite = Resources.Load<Sprite>("Image/Play/Player/" + worldInfo.NoteInfo[myPlayerIndex].SideImageName);
        playerSideRenderer.sprite = playerSideSprite;
        playerSideRenderer.color = handy.GetColor01(worldInfo.PlayerInfo[myPlayerIndex].SideColor);
        playerCenterRenderer.color = handy.GetColor01(worldInfo.PlayerInfo[myPlayerIndex].CenterColor);
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
            SetSideScaleTweener(worldInfo.PlayerInfo[myPlayerIndex].Scale, 0.15f, 0.15f - sideScaleTweener.Elapsed());
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
