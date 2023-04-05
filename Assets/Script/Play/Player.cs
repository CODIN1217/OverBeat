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
    GameManager GM;
    void Awake()
    {
        GM = GameManager.Property;
        handy = Handy.Property;
        playerSideRenderer = playerSide.GetComponent<SpriteRenderer>();
        playerCenterRenderer = playerCenter.GetComponent<SpriteRenderer>();
        curRadius = handy.GetWorldInfo(GameManager.Property.closestNoteIndex[myPlayerIndex] - 1).playerInfo[myPlayerIndex].tarRadius;
    }
    void Update()
    {
        // if (handy.GetWorldInfo().PlayerInfo.Index == myPlayerIndex)
        worldInfo = handy.GetWorldInfo(GameManager.Property.closestNoteIndex[myPlayerIndex]);
        // tarDeg = handy.GetNextDeg(myPlayerIndex, worldInfo);
        SetPlayerRenderer();
        stdRadius = handy.GetWorldInfo(GameManager.Property.closestNoteIndex[myPlayerIndex] - 1).playerInfo[myPlayerIndex].tarRadius;
        tarRadius = worldInfo.playerInfo[myPlayerIndex].tarRadius;
        UpdateDegs();
        // curRadius = stdRadius;
        curDeg = handy.GetCorrectDegMaxIs0(curDeg);
        // if(curDeg == 0f)
        // handy.WriteLog(GameManager.Property.worldInfoIndex);
        SetPlayerTransform();
        if (GameManager.Property.GetIsKeyDown(myPlayerIndex) && !GameManager.Property.isPause)
        {
            SetSideScaleTweener(worldInfo.playerInfo[myPlayerIndex].scale * 0.8f, 0.15f);
        }
        else if (!GameManager.Property.GetIsKeyPress(myPlayerIndex) && GameManager.Property.GetIsKeyUp(myPlayerIndex))
        {
            SideScaleToOrig();
        }
        if (handy.GetJudgmentValue(myPlayerIndex) >= 1f)
            isInputted = false;

        if (GameManager.Property.isPause)
            return;
        if (!isInputted)
        {
            if (GameManager.Property.GetIsProperKeyDown(myPlayerIndex) && handy.GetJudgmentValue(myPlayerIndex) <= handy.judgmentRange[myPlayerIndex])
            {
                tarDeg += worldInfo.playerInfo[myPlayerIndex].moveDir * tarDeg < worldInfo.playerInfo[myPlayerIndex].moveDir * curDeg ? worldInfo.playerInfo[myPlayerIndex].moveDir * 360f : 0f;
                handy.closestNoteScripts[myPlayerIndex].TryKillFadeTweener(true);
                handy.closestNoteScripts[myPlayerIndex].TryKillRadiusTweener(true);
                if (!handy.closestNoteScripts[myPlayerIndex].needInput)
                {
                    handy.closestNoteScripts[myPlayerIndex].ActToNeedInput();
                }
                StartCoroutine(CheckInputtingKeys(GameManager.Property.closestNoteIndex[myPlayerIndex]));
                TryKillMoveTweener();
                moveTweener = DOTween.Sequence()
                .AppendInterval(handy.GetNoteWaitTime(myPlayerIndex, GM.closestNoteIndex[myPlayerIndex]) * Mathf.Clamp(-handy.GetSign0IsMin(handy.closestNoteScripts[myPlayerIndex].elapsedTimeWhenNeedInput) * handy.GetJudgmentValue(myPlayerIndex), 0f, handy.judgmentRange[myPlayerIndex]))
                .Append(DOTween.To(() => stdDeg, (d) => curDeg = d, tarDeg,
                handy.GetNoteLengthTime(myPlayerIndex, GM.closestNoteIndex[myPlayerIndex]) * (1f - Mathf.Clamp(handy.GetSign0IsMin(handy.closestNoteScripts[myPlayerIndex].elapsedTimeWhenNeedInput) * handy.GetJudgmentValue(myPlayerIndex), 0f, handy.judgmentRange[myPlayerIndex])))
                .SetEase(worldInfo.playerInfo[myPlayerIndex].degEase))
                .SetUpdate(true);
                isInputted = true;
            }
            else
            {
                curDeg = stdDeg;
            }
        }
        /* else if (GameManager.Property.GetIsKeyDown(myPlayerIndex))
        {
            GameManager.Property.SetMissJudgment(myPlayerIndex);
        } */
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
        playerSideSprite = Resources.Load<Sprite>("Image/Play/Player/" + worldInfo.noteInfo[myPlayerIndex].sideImageName);
        playerSideRenderer.sprite = playerSideSprite;
        playerSideRenderer.color = handy.GetColor01(worldInfo.playerInfo[myPlayerIndex].sideColor);
        playerCenterRenderer.color = handy.GetColor01(worldInfo.playerInfo[myPlayerIndex].centerColor);
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
            SetSideScaleTweener(worldInfo.playerInfo[myPlayerIndex].scale, 0.15f, 0.15f - sideScaleTweener.Elapsed());
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
    IEnumerator CheckInputtingKeys(int worldInfoIndex)
    {
        while (GameManager.Property.GetIsProperKeyPress(myPlayerIndex, handy.GetWorldInfo(worldInfoIndex).noteInfo[myPlayerIndex].startDegIndex) && handy.GetNoteScript(myPlayerIndex, worldInfoIndex).noteLengthTime != 0f/*  && handy.GetElapsedTimeWhenNeedInput01(myPlayerIndex) < 1f */)
        {
            yield return null;
        }
        // if (/* handy.GetElapsedTimeWhenNeedInput01(myPlayerIndex) < 1f &&  */handy.GetNoteScript(myPlayerIndex, worldInfoIndex).noteLengthTime != 0f)
        // {
        if (handy.GetNote(myPlayerIndex, worldInfoIndex).activeSelf)
        {
            handy.GetNoteScript(myPlayerIndex, worldInfoIndex).ActiveNextNote();
            // handy.judgmentGenScript.SetJudgmentText(myPlayerIndex, GameManager.Property.GetJudgment(myPlayerIndex, 1f - handy.GetElapsedTimeWhenNeedInput01(myPlayerIndex, worldInfoIndex)));
            handy.GetNoteScript(myPlayerIndex, worldInfoIndex).StopNote();
        }
        // }
        /* else if (handy.closestNoteScripts[myPlayerIndex].noteLengthTime != 0f)
        {
            handy.judgmentGenScript.SetJudgmentText(myPlayerIndex, JudgmentType.Perfect);
        } */
    }
}
