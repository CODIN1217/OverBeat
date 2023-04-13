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
    public float curRadius;
    public float tarRadius;
    float longNoteCurRadius;
    public float elapsedSecsWhenNeedInput;
    public float elapsedSecsWhenNeedlessInput;
    public float elapsedSecsWhenNeedlessInput01;
    public float elapsedSecsWhenNeedInput01;
    public float noteWaitSecs;
    public float noteLengthSecs;
    public float toleranceSecsWhenNeedlessInput;
    float colorAlpha;
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
    Sequence myRadiusTweener;
    Sequence myFadeTweener;
    SpriteRenderer startNoteRenderer;
    LineRenderer processNoteRenderer;
    DottedLine processNoteDottedLine;
    SpriteRenderer endNoteRenderer;
    Stopwatch stopwatch;
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
        stopwatch = new Stopwatch();
        isBeforeAwake = true;
        isAfterAwake = true;
    }
    void Update()
    {
        if (playGM.isBreakUpdate())
            return;
        if (isBeforeAwake)
        {
            isBeforeAwake = false;
        }
        SetElapsedSecs01();
        SetNoteTransform();
        SetNotePartsTransform();
        SetNoteRenderer();
        if (isAfterAwake)
        {
            float fadeDuration = Mathf.Clamp(noteWaitSecs * 0.3f - toleranceSecsWhenAwake, 0f, float.MaxValue);
            SetFadeTweener(colorAlpha, 1f, fadeDuration, worldInfo.noteInfo.appearTween.ease);
            float radiusDuration = Mathf.Clamp(noteWaitSecs - toleranceSecsWhenAwake, 0f, float.MaxValue);
            SetRadiusTweener(curRadius, tarRadius, radiusDuration, worldInfo.noteInfo.radiusTween.ease);
            stopwatch.Reset();
            stopwatch.Stop();
            isAfterAwake = false;
        }
        stopwatch.Start();
        if (!needInput)
        {
            elapsedSecsWhenNeedlessInput = toleranceSecsWhenAwake + stopwatch.ElapsedMilliseconds * 0.001f;
            if (elapsedSecsWhenNeedlessInput >= noteWaitSecs)
            {
                ActNeedInput();
            }
        }
        else if (needInput && !isStop)
        {
            elapsedSecsWhenNeedInput = toleranceSecsWhenNeedlessInput + stopwatch.ElapsedMilliseconds * 0.001f;
            if (elapsedSecsWhenNeedInput >= noteLengthSecs)
            {
                isDisable = true;
            }
        }
        if (isDisable)
        {
            colorAlpha = 0f;
            if (!isStop)
            {
                if ((elapsedSecsWhenNeedInput01 > playGM.judgmentRange[tarPlayerIndex] || myEachNoteIndex == 0))
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
        stdRadius = beforeWorldInfo.playerInfo[tarPlayerIndex].tarRadius;
        curRadius = worldInfo.noteInfo.startRadius;
        tarRadius = worldInfo.playerInfo[tarPlayerIndex].tarRadius;
        centerPos = worldInfo.centerInfo.pos;
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
                , beforeWorldInfo.playerInfo[tarPlayerIndex].tarRadius + (worldInfo.playerInfo[tarPlayerIndex].tarRadius - beforeWorldInfo.playerInfo[tarPlayerIndex].tarRadius) * worldInfo.playerInfo[tarPlayerIndex].tarRadiusTween.ease.Evaluate((float)i / Mathf.Floor(100f * noteLengthSecs))
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
    public void SetElapsedSecs01()
    {
        if (noteWaitSecs != 0f)
            elapsedSecsWhenNeedlessInput01 = Mathf.Clamp01(Mathf.Clamp(elapsedSecsWhenNeedlessInput, 0f, noteWaitSecs) / noteWaitSecs);
        else
            elapsedSecsWhenNeedlessInput01 = 0f;

        if (noteLengthSecs != 0f)
            elapsedSecsWhenNeedInput01 = Mathf.Clamp01(Mathf.Clamp(elapsedSecsWhenNeedInput, 0f, noteLengthSecs) / noteLengthSecs);
        else
            elapsedSecsWhenNeedInput01 = Mathf.Clamp01(Mathf.Clamp(elapsedSecsWhenNeedInput, 0f, playGM.GetNoteWaitSecs(tarPlayerIndex, myEachNoteIndex + 1)) / playGM.GetNoteWaitSecs(tarPlayerIndex, myEachNoteIndex + 1));
    }
    public void ActNeedInput()
    {
        toleranceSecsWhenNeedlessInput = elapsedSecsWhenNeedlessInput - noteWaitSecs;
        handy.PlayCodeWaitForSecs(Mathf.Clamp(-toleranceSecsWhenNeedlessInput, 0f, noteWaitSecs)
        , () =>
        {
            playGM.curWorldInfoIndex += (int)Mathf.Clamp01(myEachNoteIndex);
            transform.DORotate(Vector3.forward * handy.GetCorrectDegMaxIs0(-(worldInfo.noteInfo.totalRotation + stdDeg)), worldInfo.noteInfo.totalRotationTween.duration)
            .SetEase(worldInfo.noteInfo.totalRotationTween.ease);
        });
        // playGM.SetCurWorldInfoIndex(Mathf.Clamp(-toleranceSecsWhenNeedlessInput, 0f, noteWaitSecs), myEachNoteIndex);
        elapsedSecsWhenNeedlessInput = noteWaitSecs;
        tarPlayerScript.SetRadiusTweener(worldInfo.playerInfo[tarPlayerIndex].tarRadius, Mathf.Clamp(noteLengthSecs - toleranceSecsWhenNeedlessInput, 0f, noteLengthSecs == 0f ? 0f : float.MaxValue), worldInfo.playerInfo[tarPlayerIndex].tarRadiusTween.ease);
        stopwatch.Reset();
        stopwatch.Stop();
        needInput = true;
    }
    void SetNoteTransform()
    {
        if (playGM.GetNoteLengthSecs(tarPlayerIndex, myEachNoteIndex) != 0)
            curDeg = GetCurDeg(worldInfo.playerInfo[tarPlayerIndex].degTween.ease.Evaluate(Mathf.Clamp(elapsedSecsWhenNeedInput, 0f, float.MaxValue) / playGM.GetNoteLengthSecs(tarPlayerIndex, myEachNoteIndex)));
        else
            curDeg = stdDeg;
        transform.position = handy.GetCircularPos(needInput ? tarPlayerScript.curDeg : stdDeg, curRadius);
        startNote.transform.localScale = tarPlayerScript.playerSide.transform.localScale;
        endNote.transform.localScale = tarPlayerScript.playerSide.transform.localScale;
        // transform.rotation = Quaternion.Euler(0f, 0f, handy.GetCorrectDegMaxIs0(-(worldInfo.noteInfo.totalRotation + stdDeg)));
    }
    void SetNotePartsTransform()
    {
        curPathIndex = (int)(100f * Mathf.Clamp(elapsedSecsWhenNeedInput, 0f, float.MaxValue));
        myLocalPathPoses.Clear();
        myProcessPathPoses.Clear();
        dottedLineLength = 0f;
        for (int i = curPathIndex; i < myPathPoses.Count; i++)
        {
            Vector3 curPathPos = myPathPoses[i] + transform.position - (Vector3)handy.GetCircularPos(needInput ? tarPlayerScript.curDeg : stdDeg, beforeWorldInfo.playerInfo[tarPlayerIndex].tarRadius) - (Vector3)handy.GetCircularPos(GetCurDeg((float)i / (float)myPathPoses.Count), GetLongNoteCurRadius((float)curPathIndex / (float)myPathPoses.Count)) + (Vector3)centerPos;
            myLocalPathPoses.Add(curPathPos);
            myProcessPathPoses.Add(curPathPos);
            Sprite playerSprite = Resources.Load<Sprite>("Image/Play/Player/" + worldInfo.noteInfo.sideImageName);
            if (handy.CheckObjInOtherObj(myPathPoses[i], Vector2.zero, handy.GetCircularPos(stdDeg, stdRadius, centerPos), worldInfo.playerInfo[tarPlayerIndex].scale, Vector2.zero, handy.GetSpritePixels(playerSprite)) || handy.CheckObjInOtherObj(myPathPoses[i], Vector2.zero, handy.GetCircularPos(tarDeg, worldInfo.playerInfo[tarPlayerIndex].tarRadius, centerPos), worldInfo.playerInfo[tarPlayerIndex].scale, Vector2.zero, handy.GetSpritePixels(playerSprite)))
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
        startNote.transform.rotation = Quaternion.Euler(0f, 0f, handy.GetCorrectDegMaxIs0(-(worldInfo.noteInfo.startRotation + curDeg)));
        endNote.transform.rotation = Quaternion.Euler(0f, 0f, handy.GetCorrectDegMaxIs0(-(worldInfo.noteInfo.endRotation + tarDeg)));
    }
    public void SetNoteRenderer()
    {
        startNoteRenderer.sprite = startNoteSprite;
        endNoteRenderer.sprite = endNoteSprite;
        startNoteRenderer.color = playGM.GetColor01WithPlayerIndex(handy.GetColor01(worldInfo.noteInfo.startColor), tarPlayerIndex);
        processNoteRenderer.startColor = playGM.GetColor01WithPlayerIndex(handy.GetColor01(worldInfo.noteInfo.processColor), tarPlayerIndex);
        processNoteRenderer.endColor = playGM.GetColor01WithPlayerIndex(handy.GetColor01(worldInfo.noteInfo.processColor), tarPlayerIndex);
        endNoteRenderer.color = playGM.GetColor01WithPlayerIndex(handy.GetColor01(worldInfo.noteInfo.endColor), tarPlayerIndex);
        ChangeNoteAlpha(colorAlpha);
    }
    public void StopNote()
    {
        TryKillFadeTweener(true);
        TryKillRadiusTweener(true);
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
                    playGM.judgmentGenScript.SetJudgmentText(tarPlayerIndex, playGM.GetJudgment(tarPlayerIndex, 1f - elapsedSecsWhenNeedInput01, () => { accuracy01_temp = elapsedSecsWhenNeedInput01; playGM.CountMissNote(); }));
            }
            playGM.sumNoteAccuracy01 += accuracy01_temp;
            playGM.InputCount++;
            playGM.SetAccuracy01();
        }
        tarPlayerScript.TryKillMoveTweener();
        tarPlayerScript.SideScaleToOrig();
        stopwatch.Stop();
        DisableMe();
        elapsedSecsWhenNeedInput = noteLengthSecs;
        isStop = true;
    }
    void SetRadiusTweener(float curRadius, float tarRadius, float duration, AnimationCurve easeType)
    {
        TryKillRadiusTweener(true);
        myRadiusTweener = DOTween.Sequence().Append(DOTween.To(() => curRadius, r => curRadius = r, tarRadius, duration).SetEase(easeType).OnUpdate(() => { this.curRadius = curRadius; }).OnComplete(() => { this.curRadius = tarRadius; }));
    }
    void SetFadeTweener(float curValue, float tarValue, float duration, AnimationCurve easeType)
    {
        TryKillFadeTweener(true);
        myFadeTweener = DOTween.Sequence().Append(DOTween.To(() => curValue, r => curValue = r, tarValue, duration).SetEase(easeType).OnUpdate(() => { colorAlpha = curValue; }).OnComplete(() => { colorAlpha = tarValue; }));
    }
    public void TryKillRadiusTweener(bool isComplete = false)
    {
        if (myRadiusTweener != null)
        {
            myRadiusTweener.Kill(isComplete);
            myRadiusTweener = null;
        }
    }
    public void TryKillFadeTweener(bool isComplete = false)
    {
        if (myFadeTweener != null)
        {
            myFadeTweener.Kill(isComplete);
            myFadeTweener = null;
        }
    }
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
        return worldInfo.noteInfo.length * worldInfo.noteInfo.holdRadiusTween.ease.Evaluate(Mathf.Clamp01(progress01));
    }
}
