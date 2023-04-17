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
    bool isEnable;
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
    PlayGameManager playGM;
    void OnEnable()
    {
        playGM = PlayGameManager.Property;
        handy = Handy.Property;
        playerSideRenderer = playerSide.GetComponent<SpriteRenderer>();
        playerCenterRenderer = playerCenter.GetComponent<SpriteRenderer>();
        curRadius = playGM.GetWorldInfo(myPlayerIndex, playGM.closestNoteIndex[myPlayerIndex] - 1).playerInfo[myPlayerIndex].tarRadius;
        isEnable = true;
    }
    void Update()
    {
        if (playGM.isBreakUpdate())
            return;
        worldInfo = playGM.GetWorldInfo(myPlayerIndex, playGM.closestNoteIndex[myPlayerIndex]);
        SetPlayerRenderer();
        stdRadius = playGM.GetWorldInfo(myPlayerIndex, playGM.closestNoteIndex[myPlayerIndex] - 1).playerInfo[myPlayerIndex].tarRadius;
        tarRadius = worldInfo.playerInfo[myPlayerIndex].tarRadius;
        UpdateDegs();
        if (isEnable)
        {
            curDeg = stdDeg;
            isEnable = false;
        }
        curDeg = handy.GetCorrectDegMaxIs0(curDeg);
        SetPlayerTransform();
        if (playGM.GetIsKeyDown(myPlayerIndex))
        {
            SetSideScaleTweener(worldInfo.playerInfo[myPlayerIndex].scale * 0.8f, 0.15f);
        }
        else if (!playGM.GetIsKeyPress(myPlayerIndex) && playGM.GetIsKeyUp(myPlayerIndex))
        {
            SideScaleToOrig();
        }
        if (playGM.countDownScript.isCountDown)
            return;
        if (playGM.GetJudgmentValue(myPlayerIndex) >= 1f)
            isInputted = false;
        if (!isInputted)
        {
            if (playGM.GetIsProperKeyDown(myPlayerIndex) && playGM.GetJudgmentValue(myPlayerIndex) <= playGM.judgmentRange[myPlayerIndex])
            {
                tarDeg += worldInfo.playerInfo[myPlayerIndex].moveDir * tarDeg < worldInfo.playerInfo[myPlayerIndex].moveDir * curDeg ? worldInfo.playerInfo[myPlayerIndex].moveDir * 360f : 0f;
                playGM.closestNoteScripts[myPlayerIndex].TryKillFadeTweener(true);
                playGM.closestNoteScripts[myPlayerIndex].TryKillRadiusTweener(true);
                if (!playGM.closestNoteScripts[myPlayerIndex].needInput)
                {
                    playGM.closestNoteScripts[myPlayerIndex].ActNeedInput();
                }
                StartCoroutine(CheckInputtingKeys(playGM.closestNoteIndex[myPlayerIndex]));
                TryKillMoveTweener();
                moveTweener = DOTween.Sequence()
                .AppendInterval(playGM.GetNoteWaitSecs(myPlayerIndex, playGM.closestNoteIndex[myPlayerIndex]) * Mathf.Clamp(-handy.GetSign0IsMin(playGM.closestNoteScripts[myPlayerIndex].elapsedSecsWhenNeedInput) * playGM.GetJudgmentValue(myPlayerIndex), 0f, playGM.judgmentRange[myPlayerIndex]))
                .Append(DOTween.To(() => stdDeg, (d) => curDeg = d, tarDeg,
                playGM.GetNoteLengthSecs(myPlayerIndex, playGM.closestNoteIndex[myPlayerIndex]) * (1f - Mathf.Clamp(handy.GetSign0IsMin(playGM.closestNoteScripts[myPlayerIndex].elapsedSecsWhenNeedInput) * playGM.GetJudgmentValue(myPlayerIndex), 0f, playGM.judgmentRange[myPlayerIndex])))
                .SetEase(worldInfo.playerInfo[myPlayerIndex].degTween.ease));
                isInputted = true;
            }
            else
            {
                curDeg = stdDeg;
            }
        }
    }
    public void UpdateDegs()
    {
        stdDeg = playGM.GetStartDeg(myPlayerIndex, playGM.closestNoteIndex[myPlayerIndex]);
        tarDeg = playGM.GetEndDeg(myPlayerIndex, playGM.closestNoteIndex[myPlayerIndex]);
    }
    void SetPlayerTransform()
    {
        transform.position = handy.GetCircularPos(curDeg, curRadius, worldInfo.centerInfo.pos);
        if (!handy.CompareWithBeforeValue(this.name, nameof(SetPlayerTransform), nameof(playGM.curWorldInfoIndex), playGM.curWorldInfoIndex))
        {
            transform.DOScale(worldInfo.playerInfo[myPlayerIndex].scale, worldInfo.playerInfo[myPlayerIndex].scaleTween.duration)
            .SetEase(worldInfo.playerInfo[myPlayerIndex].scaleTween.ease);
            transform.DORotate(Vector3.forward * handy.GetCorrectDegMaxIs0(-(worldInfo.playerInfo[myPlayerIndex].rotation + curDeg)), worldInfo.playerInfo[myPlayerIndex].rotationTween.duration)
            .SetEase(worldInfo.playerInfo[myPlayerIndex].rotationTween.ease);
            // transform.localScale = worldInfo.playerInfo[myPlayerIndex].scale;
            // transform.rotation = Quaternion.Euler(0f, 0f, handy.GetCorrectDegMaxIs0(-(worldInfo.playerInfo[myPlayerIndex].rotation + curDeg)));
            handy.SetValueForCompare(this.name, nameof(SetPlayerTransform), nameof(playGM.curWorldInfoIndex), playGM.curWorldInfoIndex);
        }
    }
    void SetPlayerRenderer()
    {
        playerSideSprite = Resources.Load<Sprite>("Image/Play/Player/" + worldInfo.noteInfo.sideImageName);
        playerSideRenderer.sprite = playerSideSprite;
        if (!handy.CompareWithBeforeValue(this.name, nameof(SetPlayerRenderer), nameof(playGM.curWorldInfoIndex), playGM.curWorldInfoIndex))
        {
            playerSideRenderer.DOColor(playGM.GetColor01WithPlayerIndex(handy.GetColor01(worldInfo.playerInfo[myPlayerIndex].sideColor), myPlayerIndex), worldInfo.playerInfo[myPlayerIndex].sideColorTween.duration)
            .SetEase(worldInfo.playerInfo[myPlayerIndex].sideColorTween.ease);
            playerCenterRenderer.DOColor(playGM.GetColor01WithPlayerIndex(handy.GetColor01(worldInfo.playerInfo[myPlayerIndex].centerColor), myPlayerIndex), worldInfo.playerInfo[myPlayerIndex].centerColorTween.duration)
            .SetEase(worldInfo.playerInfo[myPlayerIndex].centerColorTween.ease);
            // playerSideRenderer.color = playGM.GetColor01WithPlayerIndex(handy.GetColor01(worldInfo.playerInfo[myPlayerIndex].sideColor), myPlayerIndex);
            // playerCenterRenderer.color = playGM.GetColor01WithPlayerIndex(handy.GetColor01(worldInfo.playerInfo[myPlayerIndex].centerColor), myPlayerIndex);
            handy.SetValueForCompare(this.name, nameof(SetPlayerRenderer), nameof(playGM.curWorldInfoIndex), playGM.curWorldInfoIndex);
        }
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
        radiusTweener = DOTween.Sequence().Append(DOTween.To(() => curRadius, r => curRadius = r, tarRadius, duration).SetEase(easeType))/* .SetUpdate(true) */;
    }
    public void SetSideScaleTweener(Vector3 tarScale, float duration, float prependInterval = 0f)
    {
        StartCoroutine(SetSideScaleTweener_delayed(tarScale, duration, prependInterval));
    }
    public void SideScaleToOrig()
    {
        if (sideScaleTweener != null)
            SetSideScaleTweener(worldInfo.playerInfo[myPlayerIndex].scale, 0.15f, 0.15f - sideScaleTweener.Elapsed());
    }
    IEnumerator SetSideScaleTweener_delayed(Vector3 tarScale, float duration, float prependInterval)
    {
        yield return new WaitForSecondsRealtime(prependInterval);
        TryKillSideScaleTweener();
        sideScaleTweener = DOTween.Sequence().Append(playerSide.transform.DOScale(tarScale, duration))/* .SetUpdate(true) */;
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
    IEnumerator CheckInputtingKeys(int eachNoteIndex)
    {
        while (playGM.GetIsProperKeyPress(myPlayerIndex, playGM.GetWorldInfo(myPlayerIndex, eachNoteIndex).noteInfo.startDegIndex) && playGM.GetNoteScript(myPlayerIndex, eachNoteIndex).noteLengthSecs != 0f)
        {
            yield return null;
        }
        if (playGM.GetNote(myPlayerIndex, eachNoteIndex).activeSelf)
        {
            playGM.GetNoteScript(myPlayerIndex, eachNoteIndex).StopNote();
        }
    }
}
