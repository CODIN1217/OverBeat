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

    float tweenRadius;
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
    public Sequence rotationTweener;

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
        UpdateTween();
        if (playGM.isBreakUpdate() && !playGM.countDownScript.isCountDown)
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
            handy.TryKillSequence(rotationTweener);
            rotationTweener = DOTween.Sequence()
            .Append(DOTween.To(() => tweenTotalRotation, (r) => tweenTotalRotation = r, handy.GetCorrectDegMaxIs0(worldInfo.noteInfo.totalRotationTween.value), worldInfo.noteInfo.totalRotationTween.duration)
            .SetEase(worldInfo.noteInfo.totalRotationTween.ease))

            .Join(DOTween.To(() => tweenStartRotation, (r) => tweenStartRotation = r, handy.GetCorrectDegMaxIs0(worldInfo.noteInfo.startRotationTween.value), worldInfo.noteInfo.startRotationTween.duration)
            .SetEase(worldInfo.noteInfo.startRotationTween.ease))

            .Join(DOTween.To(() => tweenEndRotation, (r) => tweenEndRotation = r, handy.GetCorrectDegMaxIs0(worldInfo.noteInfo.endRotationTween.value), worldInfo.noteInfo.endRotationTween.duration)
            .SetEase(worldInfo.noteInfo.endRotationTween.ease));


            fadeTweener = DOTween.Sequence()
            .Append(DOTween.To(() => tweenFade, c => tweenFade = c, 1f, Mathf.Clamp(noteWaitSecs * 0.3f - toleranceSecsWhenAwake, 0f, float.MaxValue))
            .SetEase(worldInfo.noteInfo.appearTween.ease));
            // SetFadeTweener(colorAlpha, 1f, Mathf.Clamp(noteWaitSecs * 0.3f - toleranceSecsWhenAwake, 0f, float.MaxValue), worldInfo.noteInfo.appearTween.ease);

            handy.TryKillSequence(colorTweener);
            colorTweener = DOTween.Sequence()
            .Append(DOTween.To(() => tweenStartColor, (c) => tweenStartColor = c, worldInfo.noteInfo.startColorTween.value, worldInfo.noteInfo.startColorTween.duration)
            .SetEase(worldInfo.noteInfo.startColorTween.ease))

            .Join(DOTween.To(() => tweenProcessStartColor, (c) => tweenProcessStartColor = c,
            worldInfo.noteInfo.processStartColorTween.value,
            worldInfo.noteInfo.processStartColorTween.duration)
            .SetEase(worldInfo.noteInfo.processStartColorTween.ease))

            .Join(DOTween.To(() => tweenProcessEndColor, (c) => tweenProcessEndColor = c,
            worldInfo.noteInfo.processEndColorTween.value,
            worldInfo.noteInfo.processEndColorTween.duration)
            .SetEase(worldInfo.noteInfo.processEndColorTween.ease))

            .Join(DOTween.To(() => tweenEndColor, (c) => tweenEndColor = c, worldInfo.noteInfo.endColorTween.value, worldInfo.noteInfo.endColorTween.duration)
            .SetEase(worldInfo.noteInfo.endColorTween.ease));

            handy.TryKillSequence(radiusTweener);
            radiusTweener = DOTween.Sequence()
            .Append(DOTween.To(() => tweenRadius, r => tweenRadius = r, tarRadius, Mathf.Clamp(noteWaitSecs - toleranceSecsWhenAwake, 0f, float.MaxValue))
            .SetEase(worldInfo.noteInfo.waitRadiusTween.ease));
            // SetRadiusTweener(curRadius, tarRadius, Mathf.Clamp(noteWaitSecs - toleranceSecsWhenAwake, 0f, float.MaxValue), worldInfo.noteInfo.waitRadiusTween.ease);
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
            tweenFade = 0f;
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
        stdRadius = beforeWorldInfo.playerInfo[tarPlayerIndex].tarRadiusTween.value;
        tweenRadius = worldInfo.noteInfo.waitRadiusTween.value;
        tarRadius = worldInfo.playerInfo[tarPlayerIndex].tarRadiusTween.value;
        centerPos = worldInfo.centerInfo.posTween.value;
        stdDeg = playGM.GetStartDeg(tarPlayerIndex, myEachNoteIndex);
        tarDeg = playGM.GetEndDeg(tarPlayerIndex, myEachNoteIndex);
        nextNote = playGM.GetNote(tarPlayerIndex, myEachNoteIndex + 1);
        nextNoteScript = playGM.GetNoteScript(tarPlayerIndex, myEachNoteIndex + 1);
        startNoteSprite = Resources.Load<Sprite>("Image/Play/Player/" + worldInfo.noteInfo.sideImageName);
        endNoteSprite = Resources.Load<Sprite>("Image/Play/Player/" + worldInfo.noteInfo.sideImageName);
        SetNoteTransform();
        if (noteLengthSecs != 0f)
        {
            for (int i = 0; i <= (int)(100f * noteLengthSecs); i++)
            {
                myPathPoses.Add(handy.GetCircularPos
                (handy.GetCorrectDegMaxIs0(stdDeg + worldInfo.playerInfo[tarPlayerIndex].moveDir * handy.GetDistanceDeg(tarDeg, stdDeg, false, worldInfo.playerInfo[tarPlayerIndex].moveDir) * worldInfo.playerInfo[tarPlayerIndex].degTween.ease.Evaluate((float)i / Mathf.Floor(100f * noteLengthSecs)))
                , beforeWorldInfo.playerInfo[tarPlayerIndex].tarRadiusTween.value + (worldInfo.playerInfo[tarPlayerIndex].tarRadiusTween.value - beforeWorldInfo.playerInfo[tarPlayerIndex].tarRadiusTween.value) * worldInfo.playerInfo[tarPlayerIndex].tarRadiusTween.ease.Evaluate((float)i / Mathf.Floor(100f * noteLengthSecs))
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
    void UpdateTween()
    {
        curRadius = tweenRadius;
        totalRotation = tweenTotalRotation;
        startRotation = tweenStartRotation;
        endRotation = tweenEndRotation;
        fadeValue = tweenFade;
        startColor = tweenStartColor;
        processStartColor = tweenProcessStartColor;
        processEndColor = tweenProcessEndColor;
        endColor = tweenEndColor;
    }
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

        tarPlayerScript.radiusTweener = DOTween.Sequence()
        .Append(DOTween.To(() => tarPlayerScript.tweenRadius, r => tarPlayerScript.tweenRadius = r, worldInfo.playerInfo[tarPlayerIndex].tarRadiusTween.value, Mathf.Clamp(noteLengthSecs - toleranceSecsEndWait, 0f, noteLengthSecs == 0f ? 0f : float.MaxValue))
        .SetEase(worldInfo.playerInfo[tarPlayerIndex].tarRadiusTween.ease));

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
        transform.position = handy.GetCircularPos(needInput ? tarPlayerScript.tweenDeg : stdDeg, tweenRadius);
        startNote.transform.localScale = tarPlayerScript.playerSide.transform.localScale;
        endNote.transform.localScale = tarPlayerScript.playerSide.transform.localScale;
        transform.rotation = Quaternion.Euler(0f, 0f, handy.GetCorrectDegMaxIs0(-(tweenTotalRotation + stdDeg)));
    }
    void SetNotePartsTransform()
    {
        curPathIndex = (int)(100f * Mathf.Clamp(holdElapsedSecs, 0f, float.MaxValue));
        myLocalPathPoses.Clear();
        myProcessPathPoses.Clear();
        dottedLineLength = 0f;
        for (int i = curPathIndex; i < myPathPoses.Count; i++)
        {
            Vector3 curPathPos = myPathPoses[i] + transform.position - (Vector3)handy.GetCircularPos(needInput ? tarPlayerScript.tweenDeg : stdDeg, beforeWorldInfo.playerInfo[tarPlayerIndex].tarRadiusTween.value) - (Vector3)handy.GetCircularPos(GetCurDeg((float)i / (float)myPathPoses.Count), GetLongNoteCurRadius((float)curPathIndex / (float)myPathPoses.Count)) + (Vector3)centerPos;
            myLocalPathPoses.Add(curPathPos);
            myProcessPathPoses.Add(curPathPos);
            Sprite playerSprite = Resources.Load<Sprite>("Image/Play/Player/" + worldInfo.noteInfo.sideImageName);
            if (handy.CheckObjInOtherObj(myPathPoses[i], Vector2.zero, handy.GetCircularPos(stdDeg, stdRadius, centerPos), worldInfo.playerInfo[tarPlayerIndex].scaleTween.value, Vector2.zero, handy.GetSpritePixels(playerSprite)) || handy.CheckObjInOtherObj(myPathPoses[i], Vector2.zero, handy.GetCircularPos(tarDeg, worldInfo.playerInfo[tarPlayerIndex].tarRadiusTween.value, centerPos), worldInfo.playerInfo[tarPlayerIndex].scaleTween.value, Vector2.zero, handy.GetSpritePixels(playerSprite)))
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
        startNote.transform.rotation = Quaternion.Euler(0f, 0f, handy.GetCorrectDegMaxIs0(-(tweenStartRotation + curDeg)));
        endNote.transform.rotation = Quaternion.Euler(0f, 0f, handy.GetCorrectDegMaxIs0(-(tweenEndRotation + tarDeg)));
    }
    public void SetNoteRenderer()
    {
        startNoteRenderer.sprite = startNoteSprite;
        endNoteRenderer.sprite = endNoteSprite;
        startNoteRenderer.color = playGM.GetColor01WithPlayerIndex(tweenStartColor, tarPlayerIndex);
        processNoteRenderer.startColor = playGM.GetColor01WithPlayerIndex(tweenProcessStartColor, tarPlayerIndex);
        processNoteRenderer.endColor = playGM.GetColor01WithPlayerIndex(tweenProcessEndColor, tarPlayerIndex);
        endNoteRenderer.color = playGM.GetColor01WithPlayerIndex(tweenEndColor, tarPlayerIndex);
        ChangeNoteAlpha(tweenFade);
    }
    public void StopNote()
    {
        handy.TryKillSequence(fadeTweener);
        handy.TryKillSequence(radiusTweener);
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
        // tarPlayerScript.TryKillMoveTweener();
        StartCoroutine(tarPlayerScript.SetSideScaleTweenerDelayed(tarPlayerScript.worldInfo.playerInfo[tarPlayerIndex].scaleTween.value, 0.15f, 0.15f - tarPlayerScript.sideScaleTweener.Elapsed()));
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
        return worldInfo.noteInfo.holdRadiusTween.value * worldInfo.noteInfo.holdRadiusTween.ease.Evaluate(Mathf.Clamp01(progress01));
    }
}
