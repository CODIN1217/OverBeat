using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Diagnostics;
using System;
using Debug = UnityEngine.Debug;
using DG.Tweening;

public class NotePrefab : MonoBehaviour
{
    public bool isInputted;
    public int myEachNoteIndex;
    public int tarPlayerIndex;
    int curPathIndex;
    public float curDeg;
    float stdDeg;
    float tarDeg;
    public float stdRadius;
    public float tarRadius;
    float longNoteCurRadius;
    public float holdElapsedSecs;
    public float waitElapsedSecs;
    public float waitElapsedSecs01;
    public float holdElapsedSecs01;
    public float noteWaitSecs;
    public float noteLengthSecs;
    public float toleranceSecsEndWait;
    float dottedLineLength;
    bool isBeforeAwake;
    bool isAfterAwake;
    public bool isDisable;
    public bool isStop;
    public bool needInput;
    public Sprite startNoteSprite;
    public Sprite endNoteSprite;
    GameObject startNote;
    GameObject processNote;
    GameObject endNote;
    GameObject tarPlayer;
    Player tarPlayerScript;
    GameObject nextNote;
    Vector2 centerPos;
    List<Vector3> myPathPoses;
    List<Vector3> myLocalPathPoses;
    List<Vector3> myProcessPathPoses;

    /* float tweenRadius;
    float tweenTotalRotation;
    float tweenStartRotation;
    float tweenEndRotation;
    float tweenFade;
    Color tweenStartColor;
    Color tweenProcessStartColor;
    Color tweenProcessEndColor;
    Color tweenEndColor;

    public float curRadius;
    public float totalRotation;
    public float startRotation;
    public float endRotation;
    public float fadeValue;
    public Color startColor;
    public Color processStartColor;
    public Color processEndColor;
    public Color endColor;

    public Sequence radiusTweener;
    public Sequence fadeTweener;
    public Sequence colorTweener;
    public Sequence rotationTweener; */
    public TweenValue.TweeningInfo curRadiusInfo;
    public TweenValue.TweeningInfo totalRotationInfo;
    public TweenValue.TweeningInfo startRotationInfo;
    public TweenValue.TweeningInfo endRotationInfo;
    public TweenValue.TweeningInfo appearInfo;
    public TweenValue.TweeningInfo startColorInfo;
    public TweenValue.TweeningInfo processStartColorInfo;
    public TweenValue.TweeningInfo processEndColorInfo;
    public TweenValue.TweeningInfo endColorInfo;

    SpriteRenderer startNoteRenderer;
    LineRenderer processNoteRenderer;
    DottedLine processNoteDottedLine;
    SpriteRenderer endNoteRenderer;
    Stopwatch myElapsedTime;
    NotePrefab nextNoteScript;
    public WorldInfo beforeWorldInfo;
    public WorldInfo afterWorldInfo;
    public WorldInfo worldInfo;
    Handy handy;
    public float toleranceSecsWhenAwake;
    PlayGameManager playGM;
    void Awake()
    {
        playGM = PlayGameManager.Property;
        myElapsedTime = new Stopwatch();
        isBeforeAwake = true;
        isAfterAwake = true;
    }
    void Update()
    {
        myElapsedTime.Stop();
        // UpdateTweenValue();
        if (playGM.isBreakUpdate() && playGM.countDownScript.isCountDown)
            return;
        if (isBeforeAwake)
        {
            isBeforeAwake = false;
        }
        SetNoteTransform();
        SetNotePartsTransform();
        SetNoteRenderer();
        if (playGM.countDownScript.isCountDown)
            return;
        SetElapsedSecs01();
        if (isAfterAwake)
        {
            curRadiusInfo = new TweenValue.TweeningInfo(worldInfo.noteInfo.waitRadiusTween);
            
            totalRotationInfo = new TweenValue.TweeningInfo(worldInfo.noteInfo.totalRotationTween, (r) => handy.GetCorrectDegMaxIs0(r + stdDeg));
            startRotationInfo = new TweenValue.TweeningInfo(worldInfo.noteInfo.startRotationTween, (r) => handy.GetCorrectDegMaxIs0(r + curDeg));
            endRotationInfo = new TweenValue.TweeningInfo(worldInfo.noteInfo.endRotationTween, (r) => handy.GetCorrectDegMaxIs0(r + tarDeg));

            appearInfo = new TweenValue.TweeningInfo(worldInfo.noteInfo.appearTween, (a) => {if(isDisable) return 0f; else return a;});
            
            startColorInfo = new TweenValue.TweeningInfo(worldInfo.noteInfo.startColorTween, (c) => playGM.GetColor01WithPlayerIndex(c, tarPlayerIndex));
            processStartColorInfo = new TweenValue.TweeningInfo(worldInfo.noteInfo.processStartColorTween, (c) => playGM.GetColor01WithPlayerIndex(c, tarPlayerIndex));
            processEndColorInfo = new TweenValue.TweeningInfo(worldInfo.noteInfo.processEndColorTween, (c) => playGM.GetColor01WithPlayerIndex(c, tarPlayerIndex));
            endColorInfo = new TweenValue.TweeningInfo(worldInfo.noteInfo.endColorTween, (c) => playGM.GetColor01WithPlayerIndex(c, tarPlayerIndex));
            /* handy.TryKillSequence(rotationTweener);
            rotationTweener = DOTween.Sequence()
            .Append(DOTween.To(() => tweenTotalRotation, (r) => tweenTotalRotation = r, handy.GetCorrectDegMaxIs0(worldInfo.noteInfo.totalRotationTween.tweenEndValue), worldInfo.noteInfo.totalRotationTween.duration)
            .SetEase(worldInfo.noteInfo.totalRotationTween.ease))

            .Join(DOTween.To(() => tweenStartRotation, (r) => tweenStartRotation = r, handy.GetCorrectDegMaxIs0(worldInfo.noteInfo.startRotationTween.tweenEndValue), worldInfo.noteInfo.startRotationTween.duration)
            .SetEase(worldInfo.noteInfo.startRotationTween.ease))

            .Join(DOTween.To(() => tweenEndRotation, (r) => tweenEndRotation = r, handy.GetCorrectDegMaxIs0(worldInfo.noteInfo.endRotationTween.tweenEndValue), worldInfo.noteInfo.endRotationTween.duration)
            .SetEase(worldInfo.noteInfo.endRotationTween.ease));


            fadeTweener = DOTween.Sequence()
            .Append(DOTween.To(() => tweenFade, c => tweenFade = c, 1f, Mathf.Clamp(noteWaitSecs * 0.3f - toleranceSecsWhenAwake, 0f, float.MaxValue))
            .SetEase(worldInfo.noteInfo.appearTween.ease));
            // SetFadeTweener(colorAlpha, 1f, Mathf.Clamp(noteWaitSecs * 0.3f - toleranceSecsWhenAwake, 0f, float.MaxValue), worldInfo.noteInfo.appearTween.ease);

            handy.TryKillSequence(colorTweener);
            colorTweener = DOTween.Sequence()
            .Append(DOTween.To(() => tweenStartColor, (c) => tweenStartColor = c, worldInfo.noteInfo.startColorTween.tweenEndValue, worldInfo.noteInfo.startColorTween.duration)
            .SetEase(worldInfo.noteInfo.startColorTween.ease))

            .Join(DOTween.To(() => tweenProcessStartColor, (c) => tweenProcessStartColor = c,
            worldInfo.noteInfo.processStartColorTween.tweenEndValue,
            worldInfo.noteInfo.processStartColorTween.duration)
            .SetEase(worldInfo.noteInfo.processStartColorTween.ease))

            .Join(DOTween.To(() => tweenProcessEndColor, (c) => tweenProcessEndColor = c,
            worldInfo.noteInfo.processEndColorTween.tweenEndValue,
            worldInfo.noteInfo.processEndColorTween.duration)
            .SetEase(worldInfo.noteInfo.processEndColorTween.ease))

            .Join(DOTween.To(() => tweenEndColor, (c) => tweenEndColor = c, worldInfo.noteInfo.endColorTween.tweenEndValue, worldInfo.noteInfo.endColorTween.duration)
            .SetEase(worldInfo.noteInfo.endColorTween.ease));

            handy.TryKillSequence(radiusTweener);
            radiusTweener = DOTween.Sequence()
            .Append(DOTween.To(() => tweenRadius, r => tweenRadius = r, tarRadius, Mathf.Clamp(noteWaitSecs - toleranceSecsWhenAwake, 0f, float.MaxValue))
            .SetEase(worldInfo.noteInfo.waitRadiusTween.ease));
            // SetRadiusTweener(curRadius, tarRadius, Mathf.Clamp(noteWaitSecs - toleranceSecsWhenAwake, 0f, float.MaxValue), worldInfo.noteInfo.waitRadiusTween.ease); */
            myElapsedTime.Reset();
            myElapsedTime.Stop();
            isAfterAwake = false;
        }
        myElapsedTime.Start();
        if (!needInput)
        {
            waitElapsedSecs = toleranceSecsWhenAwake + myElapsedTime.ElapsedMilliseconds * 0.001f;
            if (waitElapsedSecs >= noteWaitSecs)
            {
                ActNeedInput();
            }
        }
        else if (needInput && !isStop)
        {
            holdElapsedSecs = toleranceSecsEndWait + myElapsedTime.ElapsedMilliseconds * 0.001f;
            if (holdElapsedSecs >= noteLengthSecs)
            {
                isDisable = true;
            }
        }
        if (isDisable)
        {
            if (!isStop)
            {
                if ((holdElapsedSecs01 > playGM.judgmentRange[tarPlayerIndex] || myEachNoteIndex == 0))
                    StopNote();
            }
        }
    }
    public void InitNote()
    {
        handy = Handy.Property;
        startNote = transform.GetChild(0).gameObject;
        processNote = transform.GetChild(1).gameObject;
        endNote = transform.GetChild(2).gameObject;
        startNoteRenderer = startNote.transform.GetChild(0).GetComponent<SpriteRenderer>();
        processNoteRenderer = processNote.GetComponent<LineRenderer>();
        processNoteDottedLine = processNote.GetComponent<DottedLine>();
        endNoteRenderer = endNote.transform.GetChild(0).GetComponent<SpriteRenderer>();
        myPathPoses = new List<Vector3>();
        myLocalPathPoses = new List<Vector3>();
        myProcessPathPoses = new List<Vector3>();

        beforeWorldInfo = playGM.GetWorldInfo(tarPlayerIndex, myEachNoteIndex - 1);
        worldInfo = playGM.GetWorldInfo(tarPlayerIndex, myEachNoteIndex);
        afterWorldInfo = playGM.GetWorldInfo(tarPlayerIndex, myEachNoteIndex + 1);
        tarPlayer = playGM.GetPlayer(tarPlayerIndex);
        tarPlayerScript = playGM.GetPlayerScript(tarPlayerIndex);
        stdRadius = beforeWorldInfo.playerInfo[tarPlayerIndex].tarRadiusTween.tweenEndValue;
        tarRadius = worldInfo.playerInfo[tarPlayerIndex].tarRadiusTween.tweenEndValue;
        centerPos = worldInfo.centerInfo.posTween.tweenEndValue;
        stdDeg = playGM.GetStartDeg(tarPlayerIndex, myEachNoteIndex);
        tarDeg = playGM.GetEndDeg(tarPlayerIndex, myEachNoteIndex);
        nextNote = playGM.GetNote(tarPlayerIndex, myEachNoteIndex + 1);
        nextNoteScript = playGM.GetNoteScript(tarPlayerIndex, myEachNoteIndex + 1);
        startNoteSprite = Resources.Load<Sprite>("Image/Play/Player/" + worldInfo.noteInfo.sideImageName);
        endNoteSprite = Resources.Load<Sprite>("Image/Play/Player/" + worldInfo.noteInfo.sideImageName);
        // InitTween();
        SetNoteTransform();
        if (noteLengthSecs != 0f)
        {
            for (int i = 0; i <= (int)(100f * noteLengthSecs); i++)
            {
                myPathPoses.Add(handy.GetCircularPos
                (handy.GetCorrectDegMaxIs0(stdDeg + worldInfo.playerInfo[tarPlayerIndex].moveDir * handy.GetDistanceDeg(tarDeg, stdDeg, false, worldInfo.playerInfo[tarPlayerIndex].moveDir) * worldInfo.playerInfo[tarPlayerIndex].degTween.ease.Evaluate((float)i / Mathf.Floor(100f * noteLengthSecs)))
                , beforeWorldInfo.playerInfo[tarPlayerIndex].tarRadiusTween.tweenEndValue + (worldInfo.playerInfo[tarPlayerIndex].tarRadiusTween.tweenEndValue - beforeWorldInfo.playerInfo[tarPlayerIndex].tarRadiusTween.tweenEndValue) * worldInfo.playerInfo[tarPlayerIndex].tarRadiusTween.ease.Evaluate((float)i / Mathf.Floor(100f * noteLengthSecs))
                + GetLongNoteCurRadius((float)i / Mathf.Floor(100f * noteLengthSecs))));
            }
        }
        else
        {
            myPathPoses.Add(handy.GetCircularPos(stdDeg, tarRadius, centerPos));
        }
        SetNotePartsTransform();
        SetNoteRenderer();
        SetElapsedSecs01();
    }
    /* void UpdateTweenValue()
    {
        curRadius = tweenRadius;
        totalRotation = handy.GetCorrectDegMaxIs0(tweenTotalRotation + stdDeg);
        startRotation = handy.GetCorrectDegMaxIs0(tweenStartRotation + curDeg);
        endRotation = handy.GetCorrectDegMaxIs0(tweenEndRotation + tarDeg);
        fadeValue = tweenFade;
        startColor = playGM.GetColor01WithPlayerIndex(tweenStartColor, tarPlayerIndex);
        processStartColor = playGM.GetColor01WithPlayerIndex(tweenProcessStartColor, tarPlayerIndex);
        processEndColor = playGM.GetColor01WithPlayerIndex(tweenProcessEndColor, tarPlayerIndex);
        endColor = playGM.GetColor01WithPlayerIndex(tweenEndColor, tarPlayerIndex);
    } */
    /* void InitTween(){
        tweenRadius = worldInfo.noteInfo.waitRadiusTween.tweenEndValue;
        tweenTotalRotation = handy.GetCorrectDegMaxIs0(beforeWorldInfo.noteInfo.totalRotationTween.tweenEndValue);
        tweenStartRotation = handy.GetCorrectDegMaxIs0(beforeWorldInfo.noteInfo.startRotationTween.tweenEndValue);
        tweenEndRotation = handy.GetCorrectDegMaxIs0(beforeWorldInfo.noteInfo.endRotationTween.tweenEndValue);
        tweenFade = 0f;
        tweenStartColor = beforeWorldInfo.noteInfo.startColorTween.tweenEndValue;
        tweenProcessStartColor = beforeWorldInfo.noteInfo.processStartColorTween.tweenEndValue;
        tweenProcessEndColor = beforeWorldInfo.noteInfo.processEndColorTween.tweenEndValue;
        tweenEndColor = beforeWorldInfo.noteInfo.endColorTween.tweenEndValue;
    } */
    public void SetElapsedSecs01()
    {
        if (noteWaitSecs != 0f)
            waitElapsedSecs01 = Mathf.Clamp01(Mathf.Clamp(waitElapsedSecs, 0f, noteWaitSecs) / noteWaitSecs);
        else
            waitElapsedSecs01 = 0f;

        if (noteLengthSecs != 0f)
            holdElapsedSecs01 = Mathf.Clamp01(Mathf.Clamp(holdElapsedSecs, 0f, noteLengthSecs) / noteLengthSecs);
        else
            holdElapsedSecs01 = Mathf.Clamp01(Mathf.Clamp(holdElapsedSecs, 0f, playGM.GetNoteWaitSecs(tarPlayerIndex, myEachNoteIndex + 1)) / playGM.GetNoteWaitSecs(tarPlayerIndex, myEachNoteIndex + 1));
    }
    public void ActNeedInput()
    {
        toleranceSecsEndWait = waitElapsedSecs - noteWaitSecs;
        handy.PlayCodeWaitForSecs(Mathf.Clamp(-toleranceSecsEndWait, 0f, noteWaitSecs), () => { playGM.worldInfoIndex += (int)Mathf.Clamp01(myEachNoteIndex); });
        waitElapsedSecs = noteWaitSecs;

        handy.TryKillSequence(tarPlayerScript.radiusTweener);
        tarPlayerScript.SetRadiusTweener(worldInfo.playerInfo[tarPlayerIndex].tarRadiusTween.tweenEndValue, Mathf.Clamp(noteLengthSecs - toleranceSecsEndWait, 0f, (noteLengthSecs == 0f ? 0f : float.MaxValue)), worldInfo.playerInfo[tarPlayerIndex].tarRadiusTween.ease);
        /* tarPlayerScript.radiusTweener = DOTween.Sequence()
        .Append(DOTween.To(() => tarPlayerScript.tweenRadius, r => tarPlayerScript.tweenRadius = r, worldInfo.playerInfo[tarPlayerIndex].tarRadiusTween.value, Mathf.Clamp(noteLengthSecs - toleranceSecsEndWait, 0f, noteLengthSecs == 0f ? 0f : float.MaxValue))
        .SetEase(worldInfo.playerInfo[tarPlayerIndex].tarRadiusTween.ease)); */

        // tarPlayerScript.SetRadiusTweener(worldInfo.playerInfo[tarPlayerIndex].tarRadiusTween.value, Mathf.Clamp(noteLengthSecs - toleranceSecsEndWait, 0f, noteLengthSecs == 0f ? 0f : float.MaxValue), worldInfo.playerInfo[tarPlayerIndex].tarRadiusTween.ease);
        myElapsedTime.Reset();
        myElapsedTime.Stop();
        needInput = true;
    }
    void SetNoteTransform()
    {
        if (playGM.GetNoteLengthSecs(tarPlayerIndex, myEachNoteIndex) != 0)
            curDeg = GetCurDeg(worldInfo.playerInfo[tarPlayerIndex].degTween.ease.Evaluate(Mathf.Clamp(holdElapsedSecs, 0f, float.MaxValue) / playGM.GetNoteLengthSecs(tarPlayerIndex, myEachNoteIndex)));
        else
            curDeg = stdDeg;
        transform.position = handy.GetCircularPos(needInput ? tarPlayerScript.curDeg : stdDeg, (float)curRadiusInfo.curValue);
        startNote.transform.localScale = tarPlayerScript.playerSide.transform.localScale;
        endNote.transform.localScale = tarPlayerScript.playerSide.transform.localScale;
        transform.rotation = Quaternion.Euler(0f, 0f, (float)totalRotationInfo.curValue);
    }
    void SetNotePartsTransform()
    {
        curPathIndex = (int)(100f * Mathf.Clamp(holdElapsedSecs, 0f, float.MaxValue));
        myLocalPathPoses.Clear();
        myProcessPathPoses.Clear();
        dottedLineLength = 0f;
        for (int i = curPathIndex; i < myPathPoses.Count; i++)
        {
            Vector3 curPathPos = myPathPoses[i] + transform.position - (Vector3)handy.GetCircularPos(needInput ? tarPlayerScript.curDeg : stdDeg, beforeWorldInfo.playerInfo[tarPlayerIndex].tarRadiusTween.tweenEndValue) - (Vector3)handy.GetCircularPos(GetCurDeg((float)i / (float)myPathPoses.Count), GetLongNoteCurRadius((float)curPathIndex / (float)myPathPoses.Count)) + (Vector3)centerPos;
            myLocalPathPoses.Add(curPathPos);
            myProcessPathPoses.Add(curPathPos);
            Sprite playerSprite = Resources.Load<Sprite>("Image/Play/Player/" + worldInfo.noteInfo.sideImageName);
            if (handy.CheckObjInOtherObj(myPathPoses[i], Vector2.zero, handy.GetCircularPos(stdDeg, stdRadius, centerPos), worldInfo.playerInfo[tarPlayerIndex].scaleTween.tweenEndValue, Vector2.zero, handy.GetSpritePixels(playerSprite)) || handy.CheckObjInOtherObj(myPathPoses[i], Vector2.zero, handy.GetCircularPos(tarDeg, worldInfo.playerInfo[tarPlayerIndex].tarRadiusTween.tweenEndValue, centerPos), worldInfo.playerInfo[tarPlayerIndex].scaleTween.tweenEndValue, Vector2.zero, handy.GetSpritePixels(playerSprite)))
            {
                myProcessPathPoses.RemoveAt(myProcessPathPoses.Count - 1);
            }
            else
                dottedLineLength += Vector2.Distance(myLocalPathPoses[handy.GetBeforeIndex(i - curPathIndex)], myLocalPathPoses[i - curPathIndex]);
        }
        if (myLocalPathPoses.Count > 0)
        {
            startNote.transform.position = myLocalPathPoses[0];
            endNote.transform.position = myLocalPathPoses[myLocalPathPoses.Count - 1];
        }
        if (myProcessPathPoses.Count > 0)
        {
            processNoteDottedLine.poses = myProcessPathPoses;
            processNoteDottedLine.SetRepeatCount(dottedLineLength * 2.444f);
        }
        startNote.transform.rotation = Quaternion.Euler(0f, 0f, (float)startRotationInfo.curValue);
        endNote.transform.rotation = Quaternion.Euler(0f, 0f, (float)endRotationInfo.curValue);
    }
    public void SetNoteRenderer()
    {
        startNoteRenderer.sprite = startNoteSprite;
        endNoteRenderer.sprite = endNoteSprite;
        startNoteRenderer.color = (Color)startColorInfo.curValue;
        processNoteRenderer.startColor = (Color)processStartColorInfo.curValue;
        processNoteRenderer.endColor = (Color)processEndColorInfo.curValue;
        endNoteRenderer.color = (Color)endColorInfo.curValue;
        ChangeNoteAlpha((float)appearInfo.curValue);
    }
    public void StopNote()
    {
        handy.TryKillSequence(appearInfo);
        handy.TryKillSequence(curRadiusInfo);
        // TryKillFadeTweener(true);
        // TryKillRadiusTweener(true);
        if (noteLengthSecs != 0f)
        {
            float accuracy01_temp = 1f;
            if (myEachNoteIndex != 0)
            {
                if (!isInputted)
                {
                    playGM.judgmentGenScript.SetJudgmentText(tarPlayerIndex, JudgmentType.Miss);
                    playGM.CountMissNote();
                    accuracy01_temp = 0f;
                }
                else
                    playGM.judgmentGenScript.SetJudgmentText(tarPlayerIndex, playGM.GetJudgment(tarPlayerIndex, 1f - holdElapsedSecs01, () => { accuracy01_temp = holdElapsedSecs01; playGM.CountMissNote(); }));
            }
            playGM.sumNoteAccuracy01 += accuracy01_temp;
            playGM.InputCount++;
            playGM.SetAccuracy01();
        }
        handy.TryKillSequence(tarPlayerScript.moveTweener);
        if (tarPlayerScript.sideScaleTweener != null)
            handy.StartCoroutine(tarPlayerScript.SetSideClickScaleTweenerDelayed(Vector2.one, 0.15f, 0.15f - tarPlayerScript.sideScaleTweener.Elapsed()));
        myElapsedTime.Stop();
        DisableMe();
        holdElapsedSecs = noteLengthSecs;
        isStop = true;
    }
    // void SetRadiusTweener(float curRadius, float tarRadius, float duration, AnimationCurve easeType)
    // {
    //     TryKillRadiusTweener(true);
    //     radiusTweener = DOTween.Sequence().Append(DOTween.To(() => curRadius, r => curRadius = r, tarRadius, duration).SetEase(easeType).OnUpdate(() => { this.curRadius = curRadius; }).OnComplete(() => { this.curRadius = tarRadius; }));
    // }
    /* void SetFadeTweener(float curValue, float tarValue, float duration, AnimationCurve easeType)
    {
        TryKillFadeTweener(true);
        fadeTweener = DOTween.Sequence().Append(DOTween.To(() => curValue, r => curValue = r, tarValue, duration).SetEase(easeType).OnUpdate(() => { colorAlpha = curValue; }).OnComplete(() => { colorAlpha = tarValue; }));
    } */
    /* public void TryKillRadiusTweener(bool isComplete = false)
    {
        if (radiusTweener != null)
        {
            radiusTweener.Kill(isComplete);
            radiusTweener = null;
        }
    } */
    /* public void TryKillFadeTweener(bool isComplete = false)
    {
        if (fadeTweener != null)
        {
            fadeTweener.Kill(isComplete);
            fadeTweener = null;
        }
    } */
    void ChangeNoteAlpha(float alpha)
    {
        handy.ChangeAlpha(startNoteRenderer, alpha);
        handy.ChangeAlpha(processNoteRenderer, noteLengthSecs == 0f ? 0f : alpha);
        handy.ChangeAlpha(endNoteRenderer, noteLengthSecs == 0f ? 0f : alpha);
    }
    void DisableMe()
    {
        playGM.noteGeneratorScript.closestNotes[tarPlayerIndex] = nextNote;
        playGM.noteGeneratorScript.closestNoteScripts[tarPlayerIndex] = nextNoteScript;
        gameObject.SetActive(false);
    }
    float GetCurDeg(float progress01)
    {
        return handy.GetCorrectDegMaxIs0(stdDeg + worldInfo.playerInfo[tarPlayerIndex].moveDir * handy.GetDistanceDeg(tarDeg, stdDeg, false, worldInfo.playerInfo[tarPlayerIndex].moveDir) * Mathf.Clamp01(progress01));
    }
    float GetLongNoteCurRadius(float progress01)
    {
        return worldInfo.noteInfo.holdRadiusTween.tweenEndValue * worldInfo.noteInfo.holdRadiusTween.ease.Evaluate(Mathf.Clamp01(progress01));
    }
}
