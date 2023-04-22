using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using DG.Tweening;

public class Player : MonoBehaviour
{
    public int playerIndex;
    public float stdDeg;
    public float tarDeg;
    public float stdRadius;
    public float tarRadius;
    bool isInputted;
    bool isEnable;
    public GameObject playerSide;
    public GameObject playerCenter;
    SpriteRenderer playerSideRenderer;
    SpriteRenderer playerCenterRenderer;
    public Sprite playerSideSprite;
    WorldInfo beforeWorldInfo;
    public WorldInfo worldInfo;

    float tweenDeg;
    float tweenRadius;
    float tweenRotation;
    Vector2 tweenTotalScale;
    Vector2 tweenSideClickScale;
    Color tweenSideColor;
    Color tweenCenterColor;

    public float curDeg;
    public float curRadius;
    public float rotation;
    public Vector2 totalScale;
    public Color sideColor;
    public Color centerColor;

    public Sequence sideScaleTweener;
    public Sequence moveTweener;
    public Sequence radiusTweener;
    public Sequence totalScaleTweener;
    public Sequence rotationTweener;
    public Sequence colorTweener;

    Handy handy;
    PlayGameManager playGM;
    void OnEnable()
    {
        playGM = PlayGameManager.Property;
        handy = Handy.Property;
        playerSideRenderer = playerSide.GetComponent<SpriteRenderer>();
        playerCenterRenderer = playerCenter.GetComponent<SpriteRenderer>();
        tweenRadius = playGM.GetWorldInfo(playerIndex, playGM.closestNoteIndex[playerIndex] - 1).playerInfo[playerIndex].tarRadiusTween.value;
        isEnable = true;
    }
    void Update()
    {
        // InfoViewer.Property.SetInfo(this.name, nameof(sideColor.a), () => sideColor.a, playerIndex);
        // InfoViewer.Property.SetInfo(this.name, nameof(scale), () => scale, playerIndex);
        worldInfo = playGM.GetWorldInfo(playerIndex, playGM.closestNoteIndex[playerIndex]);
        beforeWorldInfo = playGM.GetWorldInfo(playerIndex, playGM.closestNoteIndex[playerIndex] - 1);
        if (isEnable)
        {
            isEnable = false;
        }
        UpdateDegs();
        stdRadius = beforeWorldInfo.playerInfo[playerIndex].tarRadiusTween.value;
        tarRadius = worldInfo.playerInfo[playerIndex].tarRadiusTween.value;
        if (!handy.compareValue_int.CompareWithBeforeValue(this.name, nameof(Update), nameof(playGM.closestNoteIndex), playGM.closestNoteIndex[playerIndex], playerIndex))
        {
            InitTween();
            isInputted = false;
            handy.compareValue_int.SetValueForCompare(this.name, nameof(Update), nameof(playGM.closestNoteIndex), playGM.closestNoteIndex[playerIndex], playerIndex);
        }
        UpdateTweenValue();
        if (playGM.isBreakUpdate() && playGM.countDownScript.isCountDown)
            return;
        SetPlayerRenderer();
        if (!isInputted)
        {
            if (!playGM.GetIsKeyDown(playerIndex) && !playGM.GetIsKeyPress(playerIndex) && playGM.GetJudgmentValue(playerIndex) > playGM.judgmentRange[playerIndex])
                curDeg = stdDeg;
        }
        SetPlayerTransform();

        if (playGM.GetIsKeyDown(playerIndex))
        {
            handy.StartCoroutine(SetSideClickScaleTweenerDelayed(Vector2.one * 0.8f, 0.15f, 0f));
        }
        else if (!playGM.GetIsKeyPress(playerIndex) && playGM.GetIsKeyUp(playerIndex))
        {
            if (sideScaleTweener != null)
                handy.StartCoroutine(SetSideClickScaleTweenerDelayed(Vector2.one, 0.15f, 0.15f - sideScaleTweener.Elapsed()));
        }

        if (playGM.countDownScript.isCountDown)
            return;
        if (!isInputted)
        {
            if (playGM.GetIsKeyDown(playerIndex) && playGM.GetJudgmentValue(playerIndex) <= playGM.judgmentRange[playerIndex])
            {
                int dir = worldInfo.playerInfo[playerIndex].moveDir;
                tarDeg += dir * tarDeg < dir * curDeg ? dir * 360f : 0f;
                handy.TryKillSequence(playGM.closestNoteScripts[playerIndex].fadeTweener);
                handy.TryKillSequence(playGM.closestNoteScripts[playerIndex].radiusTweener);
                // playGM.closestNoteScripts[playerIndex].TryKillFadeTweener(true);
                // playGM.closestNoteScripts[playerIndex].TryKillRadiusTweener(true);
                if (!playGM.closestNoteScripts[playerIndex].needInput)
                {
                    playGM.closestNoteScripts[playerIndex].ActNeedInput();
                }
                StartCoroutine(CheckInputtingKeys(playGM.closestNoteIndex[playerIndex]));
                handy.TryKillSequence(moveTweener);
                // TryKillMoveTweener();
                moveTweener = DOTween.Sequence()
                .AppendInterval(playGM.GetNoteWaitSecs(playerIndex, playGM.closestNoteIndex[playerIndex]) * Mathf.Clamp(-handy.GetSign0IsMin(playGM.closestNoteScripts[playerIndex].holdElapsedSecs) * playGM.GetJudgmentValue(playerIndex), 0f, playGM.judgmentRange[playerIndex]))
                .Append(DOTween.To(() => stdDeg, (d) => tweenDeg = d, tarDeg,
                playGM.GetNoteLengthSecs(playerIndex, playGM.closestNoteIndex[playerIndex]) * (1f - Mathf.Clamp(handy.GetSign0IsMin(playGM.closestNoteScripts[playerIndex].holdElapsedSecs) * playGM.GetJudgmentValue(playerIndex), 0f, playGM.judgmentRange[playerIndex])))
                .SetEase(worldInfo.playerInfo[playerIndex].degTween.ease));
                isInputted = true;
            }
        }
    }
    public void UpdateDegs()
    {
        stdDeg = playGM.GetStartDeg(playerIndex, playGM.closestNoteIndex[playerIndex]);
        tarDeg = playGM.GetEndDeg(playerIndex, playGM.closestNoteIndex[playerIndex]);
    }
    void SetPlayerTransform()
    {
        if (!handy.compareValue_int.CompareWithBeforeValue(this.name, nameof(SetPlayerTransform), nameof(playGM.closestNoteIndex), playGM.closestNoteIndex[playerIndex], playerIndex))
        {
            handy.TryKillSequence(totalScaleTweener);
            totalScaleTweener = DOTween.Sequence()
            .Append(DOTween.To(() => tweenTotalScale, (s) => tweenTotalScale = s, worldInfo.playerInfo[playerIndex].scaleTween.value, worldInfo.playerInfo[playerIndex].scaleTween.duration)
            .SetEase(worldInfo.playerInfo[playerIndex].scaleTween.ease));

            handy.TryKillSequence(rotationTweener);
            rotationTweener = DOTween.Sequence()
            .Append(DOTween.To(() => tweenRotation, (r) => tweenRotation = r, handy.GetCorrectDegMaxIs0(worldInfo.playerInfo[playerIndex].rotationTween.value), worldInfo.playerInfo[playerIndex].rotationTween.duration)
            .SetEase(worldInfo.playerInfo[playerIndex].rotationTween.ease));

            handy.compareValue_int.SetValueForCompare(this.name, nameof(SetPlayerTransform), nameof(playGM.closestNoteIndex), playGM.closestNoteIndex[playerIndex], playerIndex);
        }
        transform.position = handy.GetCircularPos(curDeg, curRadius, worldInfo.centerInfo.posTween.value);
        transform.rotation = Quaternion.Euler(0, 0, handy.GetCorrectDegMaxIs0(-rotation));
        playerSide.transform.localScale = tweenSideClickScale;
        transform.localScale = totalScale;
    }
    void SetPlayerRenderer()
    {
        playerSideSprite = Resources.Load<Sprite>("Image/Play/Player/" + worldInfo.noteInfo.sideImageName);
        playerSideRenderer.sprite = playerSideSprite;
        if (!handy.compareValue_int.CompareWithBeforeValue(this.name, nameof(SetPlayerRenderer), nameof(playGM.closestNoteIndex), playGM.closestNoteIndex[playerIndex], playerIndex))
        {
            handy.TryKillSequence(colorTweener);
            colorTweener = DOTween.Sequence()
            .Append(DOTween.To(() => tweenSideColor, (c) => tweenSideColor = c, worldInfo.playerInfo[playerIndex].sideColorTween.value, worldInfo.playerInfo[playerIndex].sideColorTween.duration)
            .SetEase(worldInfo.playerInfo[playerIndex].sideColorTween.ease))

            .Join(DOTween.To(() => tweenCenterColor, (c) => tweenCenterColor = c, worldInfo.playerInfo[playerIndex].centerColorTween.value, worldInfo.playerInfo[playerIndex].centerColorTween.duration)
            .SetEase(worldInfo.playerInfo[playerIndex].centerColorTween.ease));
            // playerSideRenderer.color = playGM.GetColor01WithPlayerIndex(handy.GetColor01(worldInfo.playerInfo[myPlayerIndex].sideColor), myPlayerIndex);
            // playerCenterRenderer.color = playGM.GetColor01WithPlayerIndex(handy.GetColor01(worldInfo.playerInfo[myPlayerIndex].centerColor), myPlayerIndex);
            handy.compareValue_int.SetValueForCompare(this.name, nameof(SetPlayerRenderer), nameof(playGM.closestNoteIndex), playGM.closestNoteIndex[playerIndex], playerIndex);
        }
        playerSideRenderer.color = sideColor;
        playerCenterRenderer.color = centerColor;
    }
    void UpdateTweenValue()
    {
        curDeg = handy.GetCorrectDegMaxIs0(tweenDeg);
        curRadius = tweenRadius;
        rotation = handy.GetCorrectDegMaxIs0(tweenRotation + curDeg);
        totalScale = tweenTotalScale;
        sideColor = playGM.GetColor01WithPlayerIndex(tweenSideColor, playerIndex);
        centerColor = playGM.GetColor01WithPlayerIndex(tweenCenterColor, playerIndex);
    }
    void InitTween(){
        tweenDeg = handy.GetCorrectDegMaxIs0(playGM.GetStartDeg(playerIndex, playGM.closestNoteIndex[playerIndex]));
        tweenRadius = beforeWorldInfo.playerInfo[playerIndex].tarRadiusTween.value;
        tweenRotation = handy.GetCorrectDegMaxIs0(beforeWorldInfo.playerInfo[playerIndex].rotationTween.value);
        tweenTotalScale = beforeWorldInfo.playerInfo[playerIndex].scaleTween.value;
        tweenSideColor = beforeWorldInfo.playerInfo[playerIndex].sideColorTween.value;
        tweenCenterColor = beforeWorldInfo.playerInfo[playerIndex].centerColorTween.value;
        tweenSideClickScale = Vector2.one;
    }
    /* public void TryKillMoveTweener(bool isComplete = true)
    {
        if (moveTweener != null)
        {
            moveTweener.Kill(isComplete);
            moveTweener = null;
        }
    } */
    public void SetRadiusTweener(float tarRadius, float duration, AnimationCurve ease)
    {
        radiusTweener = DOTween.Sequence()
        .Append(DOTween.To(() => tweenRadius, r => tweenRadius = r, tarRadius, duration)
        .SetEase(ease));
    }
    /* public void SetSideScaleTweener(Vector2 tarScale, float duration, float prependInterval = 0f)
    {
        StartCoroutine(SetSideScaleTweenerDelayed(tarScale, duration, prependInterval));
    }
    public void SideScaleToOrig()
    {
        if (sideScaleTweener != null)
            SetSideScaleTweener(worldInfo.playerInfo[playerIndex].scaleTween.value, 0.15f, 0.15f - sideScaleTweener.Elapsed());
    } */
    public IEnumerator SetSideClickScaleTweenerDelayed(Vector2 tarScale, float duration, float prependInterval)
    {
        yield return new WaitForSeconds(prependInterval);
        handy.TryKillSequence(sideScaleTweener);
        sideScaleTweener = DOTween.Sequence()
        .Append(DOTween.To(() => tweenSideClickScale, (s) => tweenSideClickScale = s, tarScale, duration));
    }
    /* void TryKillRadiusTweener(bool isComplete = true)
    {
        if (radiusTweener != null)
        {
            radiusTweener.Kill(isComplete);
            radiusTweener = null;
        }
    } */
    /* void TryKillSideScaleTweener(bool isComplete = true)
    {
        if (sideScaleTweener != null)
        {
            sideScaleTweener.Kill(isComplete);
            sideScaleTweener = null;
        }
    } */
    IEnumerator CheckInputtingKeys(int eachNoteIndex)
    {
        while (playGM.GetIsKeyPress(playerIndex) && playGM.GetNoteScript(playerIndex, eachNoteIndex).noteLengthSecs != 0f && playGM.closestNoteIndex[playerIndex] == eachNoteIndex)
        {
            yield return null;
        }
        if (playGM.GetNote(playerIndex, eachNoteIndex).activeSelf)
        {
            playGM.GetNoteScript(playerIndex, eachNoteIndex).StopNote();
        }
    }
}
