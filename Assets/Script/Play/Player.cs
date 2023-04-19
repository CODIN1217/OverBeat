using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using DG.Tweening;

public class Player : MonoBehaviour
{
    public int playerIndex;
    public float stdDeg;
    public float curDeg;
    public float tarDeg;
    public float stdRadius;
    public float curRadius;
    public float tarRadius;
    float tweenRotationZ;
    bool isInputted;
    bool isEnable;
    public GameObject playerSide;
    public GameObject playerCenter;
    SpriteRenderer playerSideRenderer;
    SpriteRenderer playerCenterRenderer;
    public Sprite playerSideSprite;
    Vector2 tweenScale;
    Vector2 sideClickScale;
    Color tweenSideColor;
    Color tweenCenterColor;
    WorldInfo beforeWorldInfo;
    WorldInfo worldInfo;
    Sequence sideScaleTweener;
    Sequence moveTweener;
    Sequence radiusTweener;
    Sequence scaleTweener;
    Sequence rotationZTweener;
    Sequence colorTweener;
    Handy handy;
    PlayGameManager playGM;
    void OnEnable()
    {
        playGM = PlayGameManager.Property;
        handy = Handy.Property;
        playerSideRenderer = playerSide.GetComponent<SpriteRenderer>();
        playerCenterRenderer = playerCenter.GetComponent<SpriteRenderer>();
        curRadius = playGM.GetWorldInfo(playerIndex, playGM.closestNoteIndex[playerIndex] - 1).playerInfo[playerIndex].tarRadius;
        isEnable = true;
    }
    void Update()
    {
        InfoViewer.Property.SetInfo(this.name, nameof(curDeg), () => curDeg, playerIndex);
        InfoViewer.Property.SetInfo(this.name, nameof(stdDeg), () => stdDeg, playerIndex);
        if (playGM.isBreakUpdate() && !playGM.countDownScript.isCountDown)
            return;
        worldInfo = playGM.GetWorldInfo(playerIndex, playGM.closestNoteIndex[playerIndex]);
        beforeWorldInfo = playGM.GetWorldInfo(playerIndex, playGM.closestNoteIndex[playerIndex] - 1);
        SetPlayerRenderer();
        stdRadius = playGM.GetWorldInfo(playerIndex, playGM.closestNoteIndex[playerIndex] - 1).playerInfo[playerIndex].tarRadius;
        tarRadius = worldInfo.playerInfo[playerIndex].tarRadius;
        UpdateDegs();
        if (isEnable)
        {
            tweenRotationZ = beforeWorldInfo.playerInfo[playerIndex].rotation;
            tweenScale = beforeWorldInfo.playerInfo[playerIndex].scale;
            tweenSideColor = beforeWorldInfo.playerInfo[playerIndex].sideColor;
            tweenCenterColor = beforeWorldInfo.playerInfo[playerIndex].centerColor;
            sideClickScale = Vector2.one;
            isEnable = false;
        }
        if (!isInputted)
        {
            if (!playGM.GetIsProperKeyDown(playerIndex) && playGM.GetJudgmentValue(playerIndex) > playGM.judgmentRange[playerIndex])
                curDeg = stdDeg;
        }
        curDeg = handy.GetCorrectDegMaxIs0(curDeg);
        SetPlayerTransform();
        if (playGM.GetIsKeyDown(playerIndex))
        {
            SetSideScaleTweener(worldInfo.playerInfo[playerIndex].scale * 0.8f, 0.15f);
        }
        else if (!playGM.GetIsKeyPress(playerIndex) && playGM.GetIsKeyUp(playerIndex))
        {
            SideScaleToOrig();
        }
        if (playGM.countDownScript.isCountDown)
            return;
        if (!handy.compareValue_int.CompareWithBeforeValue(this.name, nameof(Update), nameof(playGM.closestNoteIndex), playGM.closestNoteIndex[playerIndex], playerIndex))
        {
            isInputted = false;
            handy.compareValue_int.SetValueForCompare(this.name, nameof(Update), nameof(playGM.closestNoteIndex), playGM.closestNoteIndex[playerIndex], playerIndex);
        }
        if (!isInputted)
        {
            if (playGM.GetIsProperKeyDown(playerIndex) && playGM.GetJudgmentValue(playerIndex) <= playGM.judgmentRange[playerIndex])
            {
                tarDeg += worldInfo.playerInfo[playerIndex].moveDir * tarDeg < worldInfo.playerInfo[playerIndex].moveDir * curDeg ? worldInfo.playerInfo[playerIndex].moveDir * 360f : 0f;
                playGM.closestNoteScripts[playerIndex].TryKillFadeTweener(true);
                playGM.closestNoteScripts[playerIndex].TryKillRadiusTweener(true);
                if (!playGM.closestNoteScripts[playerIndex].needInput)
                {
                    playGM.closestNoteScripts[playerIndex].ActNeedInput();
                }
                StartCoroutine(CheckInputtingKeys(playGM.closestNoteIndex[playerIndex]));
                TryKillMoveTweener();
                moveTweener = DOTween.Sequence()
                .AppendInterval(playGM.GetNoteWaitSecs(playerIndex, playGM.closestNoteIndex[playerIndex]) * Mathf.Clamp(-handy.GetSign0IsMin(playGM.closestNoteScripts[playerIndex].holdElapsedSecs) * playGM.GetJudgmentValue(playerIndex), 0f, playGM.judgmentRange[playerIndex]))
                .Append(DOTween.To(() => stdDeg, (d) => curDeg = d, tarDeg,
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
            handy.TryKillSequence(scaleTweener);
            scaleTweener = DOTween.Sequence()
            .Append(DOTween.To(() => tweenScale, (s) => tweenScale = s, worldInfo.playerInfo[playerIndex].scale, worldInfo.playerInfo[playerIndex].scaleTween.duration)
            .SetEase(worldInfo.playerInfo[playerIndex].scaleTween.ease));

            handy.TryKillSequence(rotationZTweener);
            rotationZTweener = DOTween.Sequence()
            .Append(DOTween.To(() => tweenRotationZ, (z) => tweenRotationZ = z, handy.GetCorrectDegMaxIs0(worldInfo.playerInfo[playerIndex].rotation), worldInfo.playerInfo[playerIndex].rotationTween.duration)
            .SetEase(worldInfo.playerInfo[playerIndex].rotationTween.ease));

            handy.compareValue_int.SetValueForCompare(this.name, nameof(SetPlayerTransform), nameof(playGM.closestNoteIndex), playGM.closestNoteIndex[playerIndex], playerIndex);
        }
        transform.position = handy.GetCircularPos(curDeg, curRadius, worldInfo.centerInfo.pos);
        transform.rotation = Quaternion.Euler(0, 0, handy.GetCorrectDegMaxIs0(-(tweenRotationZ + curDeg)));
        playerSide.transform.localScale = sideClickScale;
        transform.localScale = tweenScale;
    }
    void SetPlayerRenderer()
    {
        playerSideSprite = Resources.Load<Sprite>("Image/Play/Player/" + worldInfo.noteInfo.sideImageName);
        playerSideRenderer.sprite = playerSideSprite;
        if (!handy.compareValue_int.CompareWithBeforeValue(this.name, nameof(SetPlayerRenderer), nameof(playGM.closestNoteIndex), playGM.closestNoteIndex[playerIndex], playerIndex))
        {
            handy.TryKillSequence(colorTweener);
            colorTweener = DOTween.Sequence()
            .Append(DOTween.To(() => tweenSideColor, (c) => tweenSideColor = c, playGM.GetColor01WithPlayerIndex(worldInfo.playerInfo[playerIndex].sideColor, playerIndex), worldInfo.playerInfo[playerIndex].sideColorTween.duration)
            .SetEase(worldInfo.playerInfo[playerIndex].sideColorTween.ease))

            .Join(DOTween.To(() => tweenCenterColor, (c) => tweenCenterColor = c, playGM.GetColor01WithPlayerIndex(worldInfo.playerInfo[playerIndex].centerColor, playerIndex), worldInfo.playerInfo[playerIndex].centerColorTween.duration)
            .SetEase(worldInfo.playerInfo[playerIndex].centerColorTween.ease));
            // playerSideRenderer.color = playGM.GetColor01WithPlayerIndex(handy.GetColor01(worldInfo.playerInfo[myPlayerIndex].sideColor), myPlayerIndex);
            // playerCenterRenderer.color = playGM.GetColor01WithPlayerIndex(handy.GetColor01(worldInfo.playerInfo[myPlayerIndex].centerColor), myPlayerIndex);
            handy.compareValue_int.SetValueForCompare(this.name, nameof(SetPlayerRenderer), nameof(playGM.closestNoteIndex), playGM.closestNoteIndex[playerIndex], playerIndex);
        }
        playerSideRenderer.color = tweenSideColor;
        playerCenterRenderer.color = tweenCenterColor;
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
    public void SetSideScaleTweener(Vector2 tarScale, float duration, float prependInterval = 0f)
    {
        StartCoroutine(SetSideScaleTweener_delayed(tarScale, duration, prependInterval));
    }
    public void SideScaleToOrig()
    {
        if (sideScaleTweener != null)
            SetSideScaleTweener(worldInfo.playerInfo[playerIndex].scale, 0.15f, 0.15f - sideScaleTweener.Elapsed());
    }
    IEnumerator SetSideScaleTweener_delayed(Vector2 tarScale, float duration, float prependInterval)
    {
        yield return new WaitForSeconds(prependInterval);
        TryKillSideScaleTweener();
        sideScaleTweener = DOTween.Sequence().Append(DOTween.To(() => sideClickScale, (s) => sideClickScale = s, tarScale, duration))/* .SetUpdate(true) */;
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
        while (playGM.GetIsProperKeyPress(playerIndex, playGM.GetWorldInfo(playerIndex, eachNoteIndex).noteInfo.startDegIndex) && playGM.GetNoteScript(playerIndex, eachNoteIndex).noteLengthSecs != 0f)
        {
            yield return null;
        }
        if (playGM.GetNote(playerIndex, eachNoteIndex).activeSelf)
        {
            playGM.GetNoteScript(playerIndex, eachNoteIndex).StopNote();
        }
    }
}
