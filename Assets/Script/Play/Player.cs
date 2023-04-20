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
    Vector2 sideClickScale;
    WorldInfo beforeWorldInfo;
    public WorldInfo worldInfo;

    public float tweenDeg;
    public float tweenRadius;
    float tweenRotationZ;
    Vector2 tweenScale;
    Color tweenSideColor;
    Color tweenCenterColor;

    public float curDeg;
    public float curRadius;
    float rotation;
    Vector2 scale;
    Color sideColor;
    Color centerColor;

    public Sequence sideScaleTweener;
    public Sequence moveTweener;
    public Sequence radiusTweener;
    public Sequence scaleTweener;
    public Sequence rotationZTweener;
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
        InfoViewer.Property.SetInfo(this.name, nameof(tweenDeg), () => tweenDeg, playerIndex);
        InfoViewer.Property.SetInfo(this.name, nameof(stdDeg), () => stdDeg, playerIndex);
        if (playGM.isBreakUpdate() && !playGM.countDownScript.isCountDown)
            return;
        worldInfo = playGM.GetWorldInfo(playerIndex, playGM.closestNoteIndex[playerIndex]);
        beforeWorldInfo = playGM.GetWorldInfo(playerIndex, playGM.closestNoteIndex[playerIndex] - 1);
        SetPlayerRenderer();
        stdRadius = playGM.GetWorldInfo(playerIndex, playGM.closestNoteIndex[playerIndex] - 1).playerInfo[playerIndex].tarRadiusTween.value;
        tarRadius = worldInfo.playerInfo[playerIndex].tarRadiusTween.value;
        UpdateDegs();
        if (isEnable)
        {
            tweenRotationZ = beforeWorldInfo.playerInfo[playerIndex].rotationTween.value;
            tweenScale = beforeWorldInfo.playerInfo[playerIndex].scaleTween.value;
            tweenSideColor = beforeWorldInfo.playerInfo[playerIndex].sideColorTween.value;
            tweenCenterColor = beforeWorldInfo.playerInfo[playerIndex].centerColorTween.value;
            sideClickScale = Vector2.one;
            isEnable = false;
        }
        if (!isInputted)
        {
            if (!playGM.GetIsKeyDown(playerIndex) && !playGM.GetIsKeyPress(playerIndex) && playGM.GetJudgmentValue(playerIndex) > playGM.judgmentRange[playerIndex])
                tweenDeg = stdDeg;
        }
        tweenDeg = handy.GetCorrectDegMaxIs0(tweenDeg);
        SetPlayerTransform();

        if (playGM.GetIsKeyDown(playerIndex))
        {
            StartCoroutine(SetSideScaleTweenerDelayed(worldInfo.playerInfo[playerIndex].scaleTween.value * 0.8f, 0.15f, 0f));
            // SetSideScaleTweener(worldInfo.playerInfo[playerIndex].scaleTween.value * 0.8f, 0.15f);
        }
        else if (!playGM.GetIsKeyPress(playerIndex) && playGM.GetIsKeyUp(playerIndex))
        {
            StartCoroutine(SetSideScaleTweenerDelayed(worldInfo.playerInfo[playerIndex].scaleTween.value, 0.15f, 0.15f - sideScaleTweener.Elapsed()));
            // SideScaleToOrig();
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
            if (playGM.GetIsKeyDown(playerIndex) && playGM.GetJudgmentValue(playerIndex) <= playGM.judgmentRange[playerIndex])
            {
                tarDeg += worldInfo.playerInfo[playerIndex].moveDir * tarDeg < worldInfo.playerInfo[playerIndex].moveDir * tweenDeg ? worldInfo.playerInfo[playerIndex].moveDir * 360f : 0f;
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
            handy.TryKillSequence(scaleTweener);
            scaleTweener = DOTween.Sequence()
            .Append(DOTween.To(() => tweenScale, (s) => tweenScale = s, worldInfo.playerInfo[playerIndex].scaleTween.value, worldInfo.playerInfo[playerIndex].scaleTween.duration)
            .SetEase(worldInfo.playerInfo[playerIndex].scaleTween.ease));

            handy.TryKillSequence(rotationZTweener);
            rotationZTweener = DOTween.Sequence()
            .Append(DOTween.To(() => tweenRotationZ, (z) => tweenRotationZ = z, handy.GetCorrectDegMaxIs0(worldInfo.playerInfo[playerIndex].rotationTween.value), worldInfo.playerInfo[playerIndex].rotationTween.duration)
            .SetEase(worldInfo.playerInfo[playerIndex].rotationTween.ease));

            handy.compareValue_int.SetValueForCompare(this.name, nameof(SetPlayerTransform), nameof(playGM.closestNoteIndex), playGM.closestNoteIndex[playerIndex], playerIndex);
        }
        transform.position = handy.GetCircularPos(tweenDeg, tweenRadius, worldInfo.centerInfo.posTween.value);
        transform.rotation = Quaternion.Euler(0, 0, handy.GetCorrectDegMaxIs0(-(tweenRotationZ + tweenDeg)));
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
            .Append(DOTween.To(() => tweenSideColor, (c) => tweenSideColor = c, playGM.GetColor01WithPlayerIndex(worldInfo.playerInfo[playerIndex].sideColorTween.value, playerIndex), worldInfo.playerInfo[playerIndex].sideColorTween.duration)
            .SetEase(worldInfo.playerInfo[playerIndex].sideColorTween.ease))

            .Join(DOTween.To(() => tweenCenterColor, (c) => tweenCenterColor = c, playGM.GetColor01WithPlayerIndex(worldInfo.playerInfo[playerIndex].centerColorTween.value, playerIndex), worldInfo.playerInfo[playerIndex].centerColorTween.duration)
            .SetEase(worldInfo.playerInfo[playerIndex].centerColorTween.ease));
            // playerSideRenderer.color = playGM.GetColor01WithPlayerIndex(handy.GetColor01(worldInfo.playerInfo[myPlayerIndex].sideColor), myPlayerIndex);
            // playerCenterRenderer.color = playGM.GetColor01WithPlayerIndex(handy.GetColor01(worldInfo.playerInfo[myPlayerIndex].centerColor), myPlayerIndex);
            handy.compareValue_int.SetValueForCompare(this.name, nameof(SetPlayerRenderer), nameof(playGM.closestNoteIndex), playGM.closestNoteIndex[playerIndex], playerIndex);
        }
        playerSideRenderer.color = tweenSideColor;
        playerCenterRenderer.color = tweenCenterColor;
    }
    void UpdateTween()
    {
        curDeg = tweenDeg;
        curRadius = tweenRadius;
        rotation = tweenRotationZ;
        scale = tweenScale;
        sideColor = tweenSideColor;
        centerColor = tweenCenterColor;
    }
    /* public void TryKillMoveTweener(bool isComplete = true)
    {
        if (moveTweener != null)
        {
            moveTweener.Kill(isComplete);
            moveTweener = null;
        }
    } */
    /* public void SetRadiusTweener(float tarRadius, float duration, AnimationCurve easeType)
    {
        handy.TryKillSequence(radiusTweener);
        // TryKillRadiusTweener();
        radiusTweener = DOTween.Sequence().Append(DOTween.To(() => curRadius, r => curRadius = r, tarRadius, duration).SetEase(easeType));
    } */
    /* public void SetSideScaleTweener(Vector2 tarScale, float duration, float prependInterval = 0f)
    {
        StartCoroutine(SetSideScaleTweenerDelayed(tarScale, duration, prependInterval));
    }
    public void SideScaleToOrig()
    {
        if (sideScaleTweener != null)
            SetSideScaleTweener(worldInfo.playerInfo[playerIndex].scaleTween.value, 0.15f, 0.15f - sideScaleTweener.Elapsed());
    } */
    public IEnumerator SetSideScaleTweenerDelayed(Vector2 tarScale, float duration, float prependInterval)
    {
        if (sideScaleTweener != null)
        {
            yield return new WaitForSeconds(prependInterval);
            handy.TryKillSequence(sideScaleTweener);
            sideScaleTweener = DOTween.Sequence().Append(DOTween.To(() => sideClickScale, (s) => sideClickScale = s, tarScale, duration));
        }
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
