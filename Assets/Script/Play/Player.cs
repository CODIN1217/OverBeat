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
    GameManager GM;
    void OnEnable()
    {
        GM = GameManager.Property;
        handy = Handy.Property;
        playerSideRenderer = playerSide.GetComponent<SpriteRenderer>();
        playerCenterRenderer = playerCenter.GetComponent<SpriteRenderer>();
        curRadius = handy.GetWorldInfo(myPlayerIndex, GM.closestNoteIndex[myPlayerIndex] - 1).playerInfo[myPlayerIndex].tarRadius;
        isEnable = true;
    }
    void Update()
    {
        // if (handy.GetWorldInfo().PlayerInfo.Index == myPlayerIndex)
        if (GM.isBreakUpdate())
            return;
        worldInfo = handy.GetWorldInfo(myPlayerIndex, GM.closestNoteIndex[myPlayerIndex]);
        // tarDeg = handy.GetNextDeg(myPlayerIndex, worldInfo);
        SetPlayerRenderer();
        stdRadius = handy.GetWorldInfo(myPlayerIndex, GM.closestNoteIndex[myPlayerIndex] - 1).playerInfo[myPlayerIndex].tarRadius;
        tarRadius = worldInfo.playerInfo[myPlayerIndex].tarRadius;
        UpdateDegs();
        if (isEnable)
        {
            curDeg = stdDeg;
            isEnable = false;
        }
        curDeg = handy.GetCorrectDegMaxIs0(curDeg);
        // if(curDeg == 0f)
        // handy.WriteLog(GM.worldInfoIndex);
        SetPlayerTransform();
        if (GM.GetIsKeyDown(myPlayerIndex))
        {
            SetSideScaleTweener(worldInfo.playerInfo[myPlayerIndex].scale * 0.8f, 0.15f);
        }
        else if (!GM.GetIsKeyPress(myPlayerIndex) && GM.GetIsKeyUp(myPlayerIndex))
        {
            SideScaleToOrig();
        }
        // GM.SetUpdateSequence(moveTweener);
        // GM.SetUpdateSequence(radiusTweener);
        // GM.SetUpdateSequence(sideScaleTweener);
        if (handy.GetJudgmentValue(myPlayerIndex) >= 1f)
            isInputted = false;

        if (!isInputted)
        {
            if (GM.GetIsProperKeyDown(myPlayerIndex) && handy.GetJudgmentValue(myPlayerIndex) <= handy.judgmentRange[myPlayerIndex])
            {
                tarDeg += worldInfo.playerInfo[myPlayerIndex].moveDir * tarDeg < worldInfo.playerInfo[myPlayerIndex].moveDir * curDeg ? worldInfo.playerInfo[myPlayerIndex].moveDir * 360f : 0f;
                handy.closestNoteScripts[myPlayerIndex].TryKillFadeTweener(true);
                handy.closestNoteScripts[myPlayerIndex].TryKillRadiusTweener(true);
                if (!handy.closestNoteScripts[myPlayerIndex].needInput)
                {
                    handy.closestNoteScripts[myPlayerIndex].ActNeedInput();
                }
                StartCoroutine(CheckInputtingKeys(GM.closestNoteIndex[myPlayerIndex]));
                TryKillMoveTweener();
                moveTweener = DOTween.Sequence()
                .AppendInterval(handy.GetNoteWaitSecs(myPlayerIndex, GM.closestNoteIndex[myPlayerIndex]) * Mathf.Clamp(-handy.GetSign0IsMin(handy.closestNoteScripts[myPlayerIndex].elapsedSecsWhenNeedInput) * handy.GetJudgmentValue(myPlayerIndex), 0f, handy.judgmentRange[myPlayerIndex]))
                .Append(DOTween.To(() => stdDeg, (d) => curDeg = d, tarDeg,
                handy.GetNoteLengthSecs(myPlayerIndex, GM.closestNoteIndex[myPlayerIndex]) * (1f - Mathf.Clamp(handy.GetSign0IsMin(handy.closestNoteScripts[myPlayerIndex].elapsedSecsWhenNeedInput) * handy.GetJudgmentValue(myPlayerIndex), 0f, handy.judgmentRange[myPlayerIndex])))
                .SetEase(worldInfo.playerInfo[myPlayerIndex].degEase))
                /* .SetUpdate(true) */;
                isInputted = true;
            }
            else
            {
                curDeg = stdDeg;
            }
        }
        /* else if (GM.GetIsKeyDown(myPlayerIndex))
        {
            GM.SetMissJudgment(myPlayerIndex);
        } */
        /* if(!GM.isEnd && GM.isPause){
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
    public void UpdateDegs()
    {
        stdDeg = handy.GetStartDeg(myPlayerIndex, GM.closestNoteIndex[myPlayerIndex]);
        tarDeg = handy.GetEndDeg(myPlayerIndex, GM.closestNoteIndex[myPlayerIndex]);
    }
    void SetPlayerTransform()
    {
        transform.position = handy.GetCircularPos(curDeg, curRadius, worldInfo.centerInfo.pos);
        transform.localScale = worldInfo.playerInfo[myPlayerIndex].scale;
        transform.rotation = Quaternion.Euler(0f, 0f, handy.GetCorrectDegMaxIs0(-(worldInfo.playerInfo[myPlayerIndex].rotation + curDeg)));
    }
    void SetPlayerRenderer()
    {
        playerSideSprite = Resources.Load<Sprite>("Image/Play/Player/" + worldInfo.noteInfo.sideImageName);
        playerSideRenderer.sprite = playerSideSprite;
        playerSideRenderer.color = handy.GetColor01WithPlayerIndex(handy.GetColor01(worldInfo.playerInfo[myPlayerIndex].sideColor), myPlayerIndex);
        playerCenterRenderer.color = handy.GetColor01WithPlayerIndex(handy.GetColor01(worldInfo.playerInfo[myPlayerIndex].centerColor), myPlayerIndex);
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
        while (GM.GetIsProperKeyPress(myPlayerIndex, handy.GetWorldInfo(myPlayerIndex, eachNoteIndex).noteInfo.startDegIndex) && handy.GetNoteScript(myPlayerIndex, eachNoteIndex).noteLengthSecs != 0f)
        {
            yield return null;
        }
        if (handy.GetNote(myPlayerIndex, eachNoteIndex).activeSelf)
        {
            handy.GetNoteScript(myPlayerIndex, eachNoteIndex).StopNote();
        }
    }
}
