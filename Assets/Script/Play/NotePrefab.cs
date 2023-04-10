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
    float rotation;
    public float elapsedTimeWhenNeedInput;
    public float elapsedTimeWhenNeedlessInput;
    public float elapsedTimeWhenNeedlessInput01;
    public float elapsedTimeWhenNeedInput01;
    public float noteWaitTime;
    public float noteLengthTime;
    public float toleranceTimeWhenNeedlessInput;
    float colorAlpha;
    float dottedLineLength;
    bool isBeforeAwake;
    bool isAfterAwake;
    public bool isDisable;
    public bool isStop;
    public bool needInput;
    public bool isPrepare;
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
    public float toleranceTimeWhenAwake;
    bool isDisable_temp;
    GameManager GM;
    void Awake()
    {
        GM = GameManager.Property;
        stopwatch = new Stopwatch();
        isBeforeAwake = true;
        isAfterAwake = true;
    }
    void Update()
    {
        if (GM.isPause)
            return;
        if (isBeforeAwake)
        {
            isBeforeAwake = false;
        }
        SetElapsedTime01();
        SetNoteTransform();
        SetNotePartsTransform();
        SetNoteRenderer();
        /* if (handy.GetNote(myNoteIndex - 1).GetComponent<NotePrefab>().isDisable)
        {
            isPrepare = false;
        }
        if (isPrepare)
        {
            return;
        } */
        if (isAfterAwake)
        {
            float fadeDuration = Mathf.Clamp(noteWaitTime * 0.3f - toleranceTimeWhenAwake, 0f, float.MaxValue);
            SetFadeTweener(colorAlpha, 1f, fadeDuration, worldInfo.noteInfo.appearEase);
            float radiusDuration = Mathf.Clamp(noteWaitTime - toleranceTimeWhenAwake, 0f, float.MaxValue);
            SetRadiusTweener(curRadius, tarRadius, radiusDuration, worldInfo.noteInfo.radiusEase);
            stopwatch.Reset();
            stopwatch.Stop();
            isAfterAwake = false;
        }
        stopwatch.Start();
        if (!needInput)
        {
            elapsedTimeWhenNeedlessInput = toleranceTimeWhenAwake + stopwatch.ElapsedMilliseconds * 0.001f;
            if (elapsedTimeWhenNeedlessInput >= noteWaitTime)
            {
                ActNeedInput();
            }
        }
        else if (needInput && !isStop)
        {
            elapsedTimeWhenNeedInput = toleranceTimeWhenNeedlessInput + stopwatch.ElapsedMilliseconds * 0.001f;
            if (elapsedTimeWhenNeedInput >= noteLengthTime)
            {
                isDisable = true;
            }
            // if (elapsedTimeWhenNeedInput <= noteLengthTime)
            //     GM.toleranceTimeWhenNeedInput = elapsedTimeWhenNeedInput - noteLengthTime;
        }
        if (isDisable)
        {
            colorAlpha = 0f;
            if (isDisable != isDisable_temp)
            {
                // ActiveNextNote();
            }
            if (!isStop)
            {
                if ((elapsedTimeWhenNeedInput01 > handy.judgmentRange[tarPlayerIndex] || myEachNoteIndex == 0))
                    StopNote();
            }
        }
        isDisable_temp = isDisable;
        /* Sprite boundarySprite = handy.boundaryScript.boundaryMaskImage.sprite;
        Sprite nextNoteSprite = nextNoteScript.startNoteSprite;
        float curNextNoteRadius = (afterWorldInfo.noteStartRadius - worldInfo.PlayerInfo.TarRadiuses) + (curRadius + GetLongNoteCurRadius(1f - elapsedTimeWhenNeedInput01));
        if (curNextNoteRadius <= handy.GetScaleAverage(handy.boundary.transform.localScale) * handy.GetScaleAverage(handy.GetSpritePixels(boundarySprite)) * 0.005f + handy.GetScaleAverage(nextNote.transform.localScale) * handy.GetScaleAverage(handy.GetSpritePixels(nextNoteSprite)) * 0.005f)
        {
            nextNote.SetActive(true);
            nextNoteScript.PrepareNote(() => curRadius + GetLongNoteCurRadius(1f - elapsedTimeWhenNeedInput01));
            nextNoteScript.isPrepare = true;
        } */

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

        beforeWorldInfo = handy.GetWorldInfo(tarPlayerIndex, myEachNoteIndex - 1);
        worldInfo = handy.GetWorldInfo(tarPlayerIndex, myEachNoteIndex);
        afterWorldInfo = handy.GetWorldInfo(tarPlayerIndex, myEachNoteIndex + 1);
        tarPlayer = handy.GetPlayer(tarPlayerIndex);
        tarPlayerScript = handy.GetPlayerScript(tarPlayerIndex);
        stdRadius = beforeWorldInfo.playerInfo[tarPlayerIndex].tarRadius;
        curRadius = worldInfo.noteInfo.startRadius;
        tarRadius = worldInfo.playerInfo[tarPlayerIndex].tarRadius;
        centerPos = worldInfo.centerInfo.pos;
        rotation = worldInfo.noteInfo.rotation;
        stdDeg = handy.GetStartDeg(tarPlayerIndex, myEachNoteIndex);
        tarDeg = handy.GetEndDeg(tarPlayerIndex, myEachNoteIndex);
        nextNote = handy.GetNote(tarPlayerIndex, myEachNoteIndex + 1);
        nextNoteScript = handy.GetNoteScript(tarPlayerIndex, myEachNoteIndex + 1);
        startNoteSprite = Resources.Load<Sprite>("Image/Play/Player/" + worldInfo.noteInfo.sideImageName);
        endNoteSprite = Resources.Load<Sprite>("Image/Play/Player/" + worldInfo.noteInfo.sideImageName);
        SetNoteTransform();
        if (noteLengthTime != 0f)
        {
            for (int i = 0; i <= (int)(100f * noteLengthTime); i++)
            {
                myPathPoses.Add(handy.GetCircularPos
                (handy.GetCorrectDegMaxIs0(stdDeg + worldInfo.playerInfo[tarPlayerIndex].moveDir * handy.GetDistanceDeg(tarDeg, stdDeg, false, worldInfo.playerInfo[tarPlayerIndex].moveDir) * worldInfo.playerInfo[tarPlayerIndex].degEase.Evaluate((float)i / Mathf.Floor(100f * noteLengthTime)))
                , beforeWorldInfo.playerInfo[tarPlayerIndex].tarRadius + (worldInfo.playerInfo[tarPlayerIndex].tarRadius - beforeWorldInfo.playerInfo[tarPlayerIndex].tarRadius) * worldInfo.playerInfo[tarPlayerIndex].tarRadiusEase.Evaluate((float)i / Mathf.Floor(100f * noteLengthTime))
                + GetLongNoteCurRadius((float)i / Mathf.Floor(100f * noteLengthTime))));
            }
        }
        else
        {
            myPathPoses.Add(handy.GetCircularPos(stdDeg, tarRadius, centerPos));
        }
        SetNotePartsTransform();
        SetNoteRenderer();
        SetElapsedTime01();
    }
    /* public void PrepareNote(Func<float> getRadius)
    {
        StartCoroutine(UpdateRadiusWhilePreparing(getRadius));
    }
    IEnumerator UpdateRadiusWhilePreparing(Func<float> getRadius)
    {
        while (!handy.GetNote(myNoteIndex - 1).GetComponent<NotePrefab>().isDisable)
        {
            yield return null;
            curRadius = getRadius() + (worldInfo.noteStartRadius - beforeWorldInfo.PlayerInfo.TarRadiuses);
        }
    } */
    public void ActiveNextNote()
    {
        nextNote.SetActive(true);
        nextNoteScript.toleranceTimeWhenAwake = elapsedTimeWhenNeedInput - noteLengthTime;
    }
    public void SetElapsedTime01()
    {
        if (noteWaitTime != 0f)
            elapsedTimeWhenNeedlessInput01 = Mathf.Clamp01(Mathf.Clamp(elapsedTimeWhenNeedlessInput, 0f, noteWaitTime) / noteWaitTime);
        else
            elapsedTimeWhenNeedlessInput01 = 0f;

        if (noteLengthTime != 0f)
            elapsedTimeWhenNeedInput01 = Mathf.Clamp01(Mathf.Clamp(elapsedTimeWhenNeedInput, 0f, noteLengthTime) / noteLengthTime);
        else
            elapsedTimeWhenNeedInput01 = Mathf.Clamp01(Mathf.Clamp(elapsedTimeWhenNeedInput, 0f, handy.GetNoteWaitTime(tarPlayerIndex, myEachNoteIndex + 1)) / handy.GetNoteWaitTime(tarPlayerIndex, myEachNoteIndex + 1));
    }
    public void ActNeedInput()
    {
        toleranceTimeWhenNeedlessInput = elapsedTimeWhenNeedlessInput - noteWaitTime;
        GM.SetCurWorldInfoIndex(Mathf.Clamp(-toleranceTimeWhenNeedlessInput, 0f, noteWaitTime), myEachNoteIndex);
        elapsedTimeWhenNeedlessInput = noteWaitTime;
        tarPlayerScript.SetRadiusTweener(worldInfo.playerInfo[tarPlayerIndex].tarRadius, Mathf.Clamp(noteLengthTime - toleranceTimeWhenNeedlessInput, 0f, noteLengthTime == 0f ? 0f : float.MaxValue), worldInfo.playerInfo[tarPlayerIndex].tarRadiusEase);
        stopwatch.Reset();
        stopwatch.Stop();
        needInput = true;
    }
    /* IEnumerator SetCurWorldInfoIndex_co()
    {
        yield return new WaitForSeconds(Mathf.Clamp(-toleranceTimeWhenNeedlessInput, 0f, noteWaitTime));
        GM.curWorldInfoIndex++;
    } */
    void SetNoteTransform()
    {
        if (handy.GetNoteLengthTime(tarPlayerIndex, myEachNoteIndex) != 0)
            curDeg = GetCurDeg(worldInfo.playerInfo[tarPlayerIndex].degEase.Evaluate(Mathf.Clamp(elapsedTimeWhenNeedInput, 0f, float.MaxValue) / handy.GetNoteLengthTime(tarPlayerIndex, myEachNoteIndex)));
        else
            curDeg = stdDeg;
        transform.position = handy.GetCircularPos(needInput ? tarPlayerScript.curDeg : stdDeg, curRadius);
        transform.rotation = Quaternion.Euler(0f, 0f, handy.GetCorrectDegMaxIs0(-(rotation + stdDeg)));
        startNote.transform.localScale = tarPlayerScript.playerSide.transform.localScale;
        endNote.transform.localScale = tarPlayerScript.playerSide.transform.localScale;
    }
    void SetNotePartsTransform()
    {
        curPathIndex = (int)(100f * Mathf.Clamp(elapsedTimeWhenNeedInput, 0f, float.MaxValue));
        myLocalPathPoses.Clear();
        myProcessPathPoses.Clear();
        dottedLineLength = 0f;
        for (int i = curPathIndex; i < myPathPoses.Count; i++)
        {
            Vector3 curPathPos = myPathPoses[i] + transform.position - (Vector3)handy.GetCircularPos(needInput ? tarPlayerScript.curDeg : stdDeg, beforeWorldInfo.playerInfo[tarPlayerIndex].tarRadius) - (Vector3)handy.GetCircularPos(GetCurDeg((float)i / (float)myPathPoses.Count), GetLongNoteCurRadius((float)curPathIndex / (float)myPathPoses.Count)/* worldInfo.noteLength * worldInfo.longNoteMoveEaseType.Evaluate((float)curPathIndex / (float)myPathPoses.Count) */) + (Vector3)centerPos;
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
        startNote.transform.rotation = Quaternion.Euler(0f, 0f, handy.GetCorrectDegMaxIs0(-(rotation + curDeg)));
        endNote.transform.rotation = Quaternion.Euler(0f, 0f, handy.GetCorrectDegMaxIs0(-(rotation + tarDeg)));
    }
    public void SetNoteRenderer()
    {
        startNoteRenderer.sprite = startNoteSprite;
        endNoteRenderer.sprite = endNoteSprite;
        startNoteRenderer.color = handy.GetColor01(worldInfo.noteInfo.startColor);
        processNoteRenderer.startColor = handy.GetColor01(worldInfo.noteInfo.processColor);
        processNoteRenderer.endColor = handy.GetColor01(worldInfo.noteInfo.processColor);
        endNoteRenderer.color = handy.GetColor01(worldInfo.noteInfo.endColor);
        ChangeNoteAlpha(colorAlpha);
    }
    public void StopNote()
    {
        TryKillFadeTweener(true);
        TryKillRadiusTweener(true);
        if (noteLengthTime != 0f)
        {
            float accuracy01_temp = 1f;
            if (myEachNoteIndex != 0 && !isInputted)
            {
                handy.judgmentGenScript.SetJudgmentText(tarPlayerIndex, JudgmentType.Miss);
                GM.CountMissNote();
                accuracy01_temp = 0f;
            }
            else if (myEachNoteIndex != 0)
            {
                handy.judgmentGenScript.SetJudgmentText(tarPlayerIndex, GM.GetJudgment(tarPlayerIndex, 1f - elapsedTimeWhenNeedInput01, () => { accuracy01_temp = elapsedTimeWhenNeedInput01; GM.CountMissNote(); }));
            }
            GM.sumNoteAccuracy01 += accuracy01_temp;
            GM.InputCount++;
            GM.SetAccuracy01();
        }
        /* if (!isInputted && myNoteIndex != 0)
        {
            GM.SetMissJudgment(playerIndex);
            Debug.Log(myNoteIndex);
        } */
        GM.lastHitedNoteIndex += (int)Mathf.Clamp01(myEachNoteIndex);
        // tarPlayerScript.stdDeg = tarDeg;
        // tarPlayerScript.curDeg = tarPlayerScript.stdDeg;
        // tarPlayerScript.tarDeg = handy.GetEndDeg(playerIndex, myNoteIndex + 1);
        tarPlayerScript.TryKillMoveTweener();
        tarPlayerScript.SideScaleToOrig();
        stopwatch.Stop();
        DisableMe();
        elapsedTimeWhenNeedInput = noteLengthTime;
        // GM.isNotesEnded[playerIndex, myNoteIndex] = true;
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
        handy.ChangeAlpha(processNoteRenderer, noteLengthTime == 0f ? 0f : alpha);
        handy.ChangeAlpha(endNoteRenderer, noteLengthTime == 0f ? 0f : alpha);
    }
    void DisableMe()
    {
        handy.noteGeneratorScript.closestNotes[tarPlayerIndex] = nextNote;
        handy.noteGeneratorScript.closestNoteScripts[tarPlayerIndex] = nextNoteScript;
        // nextNote.SetActive(true);
        gameObject.SetActive(false);
    }
    float GetCurDeg(float progress01)
    {
        return handy.GetCorrectDegMaxIs0(stdDeg + worldInfo.playerInfo[tarPlayerIndex].moveDir * handy.GetDistanceDeg(tarDeg, stdDeg, false, worldInfo.playerInfo[tarPlayerIndex].moveDir) * Mathf.Clamp01(progress01));
    }
    float GetLongNoteCurRadius(float progress01)
    {
        return worldInfo.noteInfo.length * worldInfo.noteInfo.holdRadiusEase.Evaluate(Mathf.Clamp01(progress01));
    }
}
