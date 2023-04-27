using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using DG.Tweening;
using TweenValue;

public class Player : MonoBehaviour
{
    public int playerIndex;
    public float stdDeg;
    public float tarDeg;
    public float stdRadius;
    public float tarRadius;
    bool isDegTweened;
    bool isEnable;
    public GameObject playerSide;
    public GameObject playerCenter;
    SpriteRenderer playerSideRenderer;
    SpriteRenderer playerCenterRenderer;
    public Sprite playerSideSprite;
    WorldInfo beforeWorldInfo;
    WorldInfo worldInfo;
    WorldInfo nextWorldInfo;

    // float tweenDeg;
    // float tweenRadius;
    // float tweenRotation;
    // Vector2 tweenTotalScale;
    // Vector2 tweenSideClickScale;
    // Color tweenSideColor;
    // Color tweenCenterColor;

    public float curDeg;
    public float rotation;
    public float curRadius;
    public float sideClickScale;
    public Vector2 totalScale;
    public Vector2 sideScale;
    public Vector2 centerScale;
    public Color sideColor;
    public Color centerColor;

    // public Sequence sideScaleTweener;
    // public Sequence moveTweener;
    // public Sequence radiusTweener;
    // public Sequence totalScaleTweener;
    // public Sequence rotationTweener;
    // public Sequence colorTweener;
    public TweeningInfo degInfo;
    public TweeningInfo rotationInfo;
    public TweeningInfo radiusTweener;
    public TweeningInfo totalScaleInfo;
    public TweeningInfo sideClickScaleInfo;
    public TweeningInfo sideScaleInfo;
    public TweeningInfo centerScaleInfo;
    public TweeningInfo sideColorInfo;
    public TweeningInfo centerColorInfo;

    Handy handy;
    PlayGameManager playGM;
    void OnEnable()
    {
        playGM = PlayGameManager.Property;
        handy = Handy.Property;
        playerSideRenderer = playerSide.GetComponent<SpriteRenderer>();
        playerCenterRenderer = playerCenter.GetComponent<SpriteRenderer>();
        // tweenRadius = playGM.GetWorldInfo(playerIndex, playGM.closestNoteIndex[playerIndex] - 1).playerInfo[playerIndex].tarRadiusTween.tweenEndValue;
        isEnable = true;
    }
    void Update()
    {
        // InfoViewer.Property.SetInfo(this.name, nameof(sideColor.a), () => sideColor.a, playerIndex);
        // InfoViewer.Property.SetInfo(this.name, nameof(scale), () => scale, playerIndex);
        worldInfo = playGM.GetWorldInfo(playerIndex, playGM.closestNoteIndex[playerIndex]);
        beforeWorldInfo = playGM.GetWorldInfo(playerIndex, playGM.closestNoteIndex[playerIndex] - 1);
        nextWorldInfo = playGM.GetWorldInfo(playerIndex, playGM.closestNoteIndex[playerIndex] + 1);
        if (isEnable)
        {
            isEnable = false;
        }
        if (!handy.compareValue_int.CompareWithBeforeValue(this.name, nameof(Update), nameof(playGM.closestNoteIndex), playGM.closestNoteIndex[playerIndex], playerIndex))
        {
            handy.TryKillTween(degInfo);
            degInfo = new TweeningInfo(worldInfo.playerInfo[playerIndex].degTween);

            handy.TryKillTween(rotationInfo);
            rotationInfo = new TweeningInfo(worldInfo.playerInfo[playerIndex].rotationTween);

            handy.TryKillTween(radiusTweener);
            radiusTweener = new TweeningInfo(worldInfo.playerInfo[playerIndex].radiusTween);

            handy.TryKillTween(totalScaleInfo);
            totalScaleInfo = new TweeningInfo(worldInfo.playerInfo[playerIndex].totalScaleTween);

            handy.TryKillTween(sideClickScaleInfo);
            sideClickScaleInfo = new TweeningInfo(new TweenInfo<float>(1f, 0.8f, 0.15f, AnimationCurve.Linear(0f, 0f, 1f, 1f)));

            handy.TryKillTween(sideScaleInfo);
            sideScaleInfo = new TweeningInfo(worldInfo.playerInfo[playerIndex].sideScaleTween);

            handy.TryKillTween(centerScaleInfo);
            centerScaleInfo = new TweeningInfo(worldInfo.playerInfo[playerIndex].centerScaleTween);

            handy.TryKillTween(sideColorInfo);
            sideColorInfo = new TweeningInfo(worldInfo.playerInfo[playerIndex].sideColorTween);

            handy.TryKillTween(centerColorInfo);
            centerColorInfo = new TweeningInfo(worldInfo.playerInfo[playerIndex].centerColorTween);

            handy.PlayTweens(rotationInfo, totalScaleInfo, sideScaleInfo, centerScaleInfo, sideColorInfo, centerColorInfo);

            stdDeg = playGM.GetStartDeg(playerIndex, playGM.closestNoteIndex[playerIndex]);
            tarDeg = playGM.GetEndDeg(playerIndex, playGM.closestNoteIndex[playerIndex]);

            stdRadius = beforeWorldInfo.playerInfo[playerIndex].radiusTween.endValue;
            tarRadius = worldInfo.playerInfo[playerIndex].radiusTween.endValue;

            // InitTween();

            isDegTweened = false;

            handy.compareValue_int.SetValueForCompare(this.name, nameof(Update), nameof(playGM.closestNoteIndex), playGM.closestNoteIndex[playerIndex], playerIndex);
        }
        UpdateTweenValue();
        SetPlayerRenderer();
        SetPlayerTransform();
        if (playGM.isPause)
            return;
        /* if (!isInputted)
        {
            if (!playGM.GetIsKeyDown(playerIndex) && !playGM.GetIsKeyPress(playerIndex) && playGM.GetJudgmentValue(playerIndex) > playGM.judgmentRange[playerIndex])
                curDeg = stdDeg;
        } */

        if (playGM.GetIsKeyDown(playerIndex))
        {
            handy.StartCoroutine(SetSideClickScaleTweenerDelayed(true));
        }
        else if (!playGM.GetIsKeyPress(playerIndex) && playGM.GetIsKeyUp(playerIndex))
        {
            if (handy.IsInfoNull(sideClickScaleInfo))
                handy.StartCoroutine(SetSideClickScaleTweenerDelayed(false, 0.15f - sideClickScaleInfo.tweener.Elapsed()));
        }

        if (!isDegTweened)
        {
            if (playGM.closestNoteScripts[playerIndex].isNeedInput)
            {
                int dir = worldInfo.playerInfo[playerIndex].moveDir;
                tarDeg += dir * tarDeg < dir * curDeg ? dir * 360f : 0f;

                StartCoroutine(CheckHoldingKey(playGM.closestNoteIndex[playerIndex]));
                degInfo.tweener.Goto(Mathf.Clamp(playGM.closestNoteScripts[playerIndex].toleranceSecsEndWait, 0f, float.MaxValue));
                handy.PlayTweens(degInfo);
                isDegTweened = true;
            }
        }
        /* if (!isInputted)
        {
            if (playGM.GetIsKeyDown(playerIndex) && playGM.GetJudgmentValue(playerIndex) <= playGM.judgmentRange[playerIndex])
            {
                int dir = worldInfo.playerInfo[playerIndex].moveDir;
                tarDeg += dir * tarDeg < dir * curDeg ? dir * 360f : 0f;

                handy.TryKillTween(playGM.closestNoteScripts[playerIndex].colorAInfo);
                handy.TryKillTween(playGM.closestNoteScripts[playerIndex].curRadiusInfo);

                if (!playGM.closestNoteScripts[playerIndex].isClick)
                {
                    playGM.closestNoteScripts[playerIndex].ActNeedInput();
                }
                StartCoroutine(CheckInputtingKeys(playGM.closestNoteIndex[playerIndex]));
                handy.WaitCodeUntil(() => playGM.closestNoteScripts[playerIndex].isNeedInput, () =>
                {
                    degInfo.tweener.Goto(Mathf.Clamp(playGM.closestNoteScripts[playerIndex].toleranceSecsEndWait, 0f, float.MaxValue));
                    handy.PlayTweens(degInfo);
                });
                isInputted = true;
            }
        } */
    }
    void SetPlayerTransform()
    {
        // if (!handy.compareValue_int.CompareWithBeforeValue(this.name, nameof(SetPlayerTransform), nameof(playGM.closestNoteIndex), playGM.closestNoteIndex[playerIndex], playerIndex))
        // {
        //     totalScaleTweener.Play();
        //     sideScaleTweener.Play();
        //     centerScaleTweener.Play();
        //     // handy.TryKillTween(totalScaleTweener);
        //     // totalScaleTweener = new TweeningInfo(worldInfo.playerInfo[playerIndex].totalScaleTween);
        //     /* totalScaleTweener = DOTween.Sequence()
        //     .Append(DOTween.To(() => tweenTotalScale, (s) => tweenTotalScale = s, worldInfo.playerInfo[playerIndex].scaleTween.tweenEndValue, worldInfo.playerInfo[playerIndex].scaleTween.duration)
        //     .SetEase(worldInfo.playerInfo[playerIndex].scaleTween.ease)); */

        //     // handy.TryKillTween(rotationTweener);
        //     // rotationTweener = new TweeningInfo(worldInfo.playerInfo[playerIndex].rotationTween);
        //     /* rotationTweener = DOTween.Sequence()
        //     .Append(DOTween.To(() => tweenRotation, (r) => tweenRotation = r, handy.GetCorrectDegMaxIs0(worldInfo.playerInfo[playerIndex].rotationTween.tweenEndValue), worldInfo.playerInfo[playerIndex].rotationTween.duration)
        //     .SetEase(worldInfo.playerInfo[playerIndex].rotationTween.ease)); */

        //     handy.compareValue_int.SetValueForCompare(this.name, nameof(SetPlayerTransform), nameof(playGM.closestNoteIndex), playGM.closestNoteIndex[playerIndex], playerIndex);
        // }
        transform.position = handy.GetCircularPos(curDeg, curRadius, worldInfo.centerInfo.posTween.endValue);
        transform.rotation = Quaternion.Euler(0, 0, rotation);
        transform.localScale = totalScale;
        playerSide.transform.localScale = sideScale * sideClickScale;
        playerCenter.transform.localScale = centerScale;
    }
    void SetPlayerRenderer()
    {
        playerSideSprite = Resources.Load<Sprite>("Image/Play/Player/" + worldInfo.noteInfo.sideImageName);
        playerSideRenderer.sprite = playerSideSprite;
        // if (!handy.compareValue_int.CompareWithBeforeValue(this.name, nameof(SetPlayerRenderer), nameof(playGM.closestNoteIndex), playGM.closestNoteIndex[playerIndex], playerIndex))
        // {
        //     sideColorTweener.Play();
        //     centerColorTweener.Play();
        //     // handy.TryKillTween(sideColorTweener);
        //     // sideColorTweener = new TweeningInfo(worldInfo.playerInfo[playerIndex].sideColorTween);
        //     /* colorTweener = DOTween.Sequence()
        //     .Append(DOTween.To(() => tweenSideColor, (c) => tweenSideColor = c, worldInfo.playerInfo[playerIndex].sideColorTween.tweenEndValue, worldInfo.playerInfo[playerIndex].sideColorTween.duration)
        //     .SetEase(worldInfo.playerInfo[playerIndex].sideColorTween.ease)) */

        //     // handy.TryKillTween(centerColorTweener);
        //     // centerColorTweener = new TweeningInfo(worldInfo.playerInfo[playerIndex].centerColorTween);
        //     /* .Join(DOTween.To(() => tweenCenterColor, (c) => tweenCenterColor = c, worldInfo.playerInfo[playerIndex].centerColorTween.tweenEndValue, worldInfo.playerInfo[playerIndex].centerColorTween.duration)
        //     .SetEase(worldInfo.playerInfo[playerIndex].centerColorTween.ease)); */
        //     // playerSideRenderer.color = playGM.GetColor01WithPlayerIndex(handy.GetColor01(worldInfo.playerInfo[myPlayerIndex].sideColor), myPlayerIndex);
        //     // playerCenterRenderer.color = playGM.GetColor01WithPlayerIndex(handy.GetColor01(worldInfo.playerInfo[myPlayerIndex].centerColor), myPlayerIndex);
        //     handy.compareValue_int.SetValueForCompare(this.name, nameof(SetPlayerRenderer), nameof(playGM.closestNoteIndex), playGM.closestNoteIndex[playerIndex], playerIndex);
        // }
        playerSideRenderer.color = sideColor;
        playerCenterRenderer.color = centerColor;
    }
    void UpdateTweenValue()
    {
        curDeg = handy.GetCorrectDegMaxIs0((float)degInfo.curValue);
        rotation = handy.GetCorrectDegMaxIs0((float)rotationInfo.curValue);
        curRadius = (float)radiusTweener.curValue;
        totalScale = (Vector2)totalScaleInfo.curValue;
        sideClickScale = (float)sideClickScaleInfo.curValue;
        sideScale = (Vector2)sideScaleInfo.curValue;
        centerScale = (Vector2)centerScaleInfo.curValue;
        sideColor = playGM.GetColor01WithPlayerIndex((Color)sideColorInfo.curValue, playerIndex);
        centerColor = playGM.GetColor01WithPlayerIndex((Color)centerColorInfo.curValue, playerIndex);
    }
    /* void InitTween()
    {
        tweenDeg = handy.GetCorrectDegMaxIs0(playGM.GetStartDeg(playerIndex, playGM.closestNoteIndex[playerIndex]));
        tweenRadius = beforeWorldInfo.playerInfo[playerIndex].tarRadiusTween.tweenEndValue;
        tweenRotation = handy.GetCorrectDegMaxIs0(beforeWorldInfo.playerInfo[playerIndex].rotationTween.tweenEndValue);
        tweenTotalScale = beforeWorldInfo.playerInfo[playerIndex].scaleTween.tweenEndValue;
        tweenSideColor = beforeWorldInfo.playerInfo[playerIndex].sideColorTween.tweenEndValue;
        tweenCenterColor = beforeWorldInfo.playerInfo[playerIndex].centerColorTween.tweenEndValue;
        tweenSideClickScale = Vector2.one;
    } */
    /* public void TryKillMoveTweener(bool isComplete = true)
    {
        if (moveTweener != null)
        {
            moveTweener.Kill(isComplete);
            moveTweener = null;
        }
    } */

    // public void SetRadiusTweener(/* float tarRadius, float duration, AnimationCurve ease */)
    // {
    //     radiusTweener.Play();
    //     /* radiusTweener = DOTween.Sequence()
    //     .Append(DOTween.To(() => stdRadius, r => tweenRadius = r, tarRadius, duration)
    //     .SetEase(ease)); */
    // }

    /* public void SetSideScaleTweener(Vector2 tarScale, float duration, float prependInterval = 0f)
    {
        StartCoroutine(SetSideScaleTweenerDelayed(tarScale, duration, prependInterval));
    }
    public void SideScaleToOrig()
    {
        if (sideScaleTweener != null)
            SetSideScaleTweener(worldInfo.playerInfo[playerIndex].scaleTween.value, 0.15f, 0.15f - sideScaleTweener.Elapsed());
    } */
    public IEnumerator SetSideClickScaleTweenerDelayed(bool isForward, float prependInterval = 0f)
    {
        yield return new WaitForSeconds(prependInterval);
        sideClickScaleInfo.tweener.Complete(true);
        if (!isForward)
        {
            sideClickScaleInfo.tweener.PlayBackwards();
        }
        sideClickScaleInfo.tweener.Play();
        /* handy.TryKillSequence(sideScaleTweener);
        sideScaleTweener = DOTween.Sequence()
        .Append(DOTween.To(() => tweenSideClickScale, (s) => tweenSideClickScale = s, tarScale, duration)); */
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
    IEnumerator CheckHoldingKey(int eachNoteIndex)
    {
        if (playGM.GetNoteScript(playerIndex, eachNoteIndex).noteLengthSecs != 0f)
        {
            while (playGM.GetIsKeyPress(playerIndex) && playGM.closestNoteIndex[playerIndex] == eachNoteIndex)
            {
                yield return null;
            }
            if (playGM.GetNote(playerIndex, eachNoteIndex).activeSelf)
            {
                playGM.GetNoteScript(playerIndex, eachNoteIndex).StopNote();
            }
        }
    }
}
