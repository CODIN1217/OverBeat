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
    public int myNoteIndex;
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
    float elapsedTimeWhenNeedInput01;
    public float noteWaitTime;
    public float noteLengthTime;
    public float toleranceTimeWhenNeedlessInput;
    float colorAlpha;
    float dottedLineLength;
    bool isBeforeAwake;
    bool isAfterAwake;
    public bool isDisable;
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
    // Vector2 noteScale;
    List<Vector3> myPathPoses;
    List<Vector3> myLocalPathPoses;
    List<Vector3> myProcessPathPoses;
    Sequence myRadiusTweener;
    Sequence myFadeTweener;
    // Sequence myNoteScaleTweener;
    SpriteRenderer startNoteRenderer;
    LineRenderer processNoteRenderer;
    DottedLine processNoteDottedLine;
    SpriteRenderer endNoteRenderer;
    Stopwatch stopwatch;
    NotePrefab nextNoteScript;
    WorldInfo beforeWorldInfo;
    WorldInfo afterWorldInfo;
    WorldInfo worldInfo;
    Handy handy;
    void Awake()
    {
        stopwatch = new Stopwatch();
        isBeforeAwake = true;
        isAfterAwake = true;
    }
    void Update()
    {
        startNote.transform.localScale = tarPlayerScript.playerSide.transform.localScale;
        endNote.transform.localScale = tarPlayerScript.playerSide.transform.localScale;
        if (GameManager.Property.isPause)
            return;
        if (isBeforeAwake)
        {
            float fadeDuration = Mathf.Clamp(noteWaitTime * 0.3f - GameManager.Property.toleranceTimeWhenNeedInput, 0f, float.MaxValue);
            SetFadeTweener(colorAlpha, 1f, fadeDuration, worldInfo.noteAppearEaseType);
            isBeforeAwake = false;
        }
        SetNoteTransform();
        SetNotePartsTransform();
        SetNoteRenderer();
        if (handy.GetNote(myNoteIndex - 1).GetComponent<NotePrefab>().isDisable)
        {
            isPrepare = false;
        }
        if (isPrepare)
        {
            return;
        }
        if (isAfterAwake)
        {
            // Debug.Log(GameManager.Property.toleranceTimeWhenNeedInput);
            float radiusDuration = Mathf.Clamp(noteWaitTime - GameManager.Property.toleranceTimeWhenNeedInput, 0f, float.MaxValue);
            SetRadiusTweener(curRadius, tarRadius, radiusDuration, worldInfo.noteMoveEaseType);
            stopwatch.Reset();
            stopwatch.Stop();
            isAfterAwake = false;
        }
        stopwatch.Start();
        if (!needInput)
        {
            elapsedTimeWhenNeedlessInput = GameManager.Property.toleranceTimeWhenNeedInput + stopwatch.ElapsedMilliseconds * 0.001f;
            if (elapsedTimeWhenNeedlessInput >= noteWaitTime)
            {
                ActToNeedInput();
                stopwatch.Start();
            }
        }
        else if (needInput)
        {
            elapsedTimeWhenNeedInput = toleranceTimeWhenNeedlessInput + stopwatch.ElapsedMilliseconds * 0.001f;
            if (elapsedTimeWhenNeedInput >= noteLengthTime)
            {
                isDisable = true;
            }
            if (elapsedTimeWhenNeedInput <= noteLengthTime)
                GameManager.Property.toleranceTimeWhenNeedInput = elapsedTimeWhenNeedInput - noteLengthTime;
        }
        if (noteLengthTime != 0f)
            elapsedTimeWhenNeedInput01 = Mathf.Clamp01(elapsedTimeWhenNeedInput / noteLengthTime);
        else
            elapsedTimeWhenNeedInput01 = Mathf.Clamp01(elapsedTimeWhenNeedInput / handy.GetNoteWaitTime(myNoteIndex + 1));
        if (isDisable)
        {
            colorAlpha = 0f;
            nextNote.SetActive(true);
            if (elapsedTimeWhenNeedInput01 > handy.judgmentRange || myNoteIndex == 0)
                StopNote();
        }
        /* Vector3 origNoteScale = noteScale;
        if (GameManager.Property.numberOfInputKeys > 0)
        {
            TryKillNoteScaleTweener(true);
            myNoteScaleTweener = DOTween.Sequence()
            .Append(startNote.transform.DOScale(origNoteScale * 0.8f, 0.15f))
            .Join(endNote.transform.DOScale(origNoteScale * 0.8f, 0.15f));
        }
        else if (GameManager.Property.numberOfInputtingKeys == 0)
        {
            TryKillNoteScaleTweener(true);
            myNoteScaleTweener = DOTween.Sequence()
            .Append(startNote.transform.DOScale(origNoteScale, 0.15f))
            .Join(endNote.transform.DOScale(origNoteScale, 0.15f));
        } */
        Sprite boundarySprite = handy.boundaryScript.boundaryMaskImage.sprite;
        Sprite nextNoteSprite = nextNoteScript.startNoteSprite;
        float curNextNoteRadius = (afterWorldInfo.noteStartRadius - worldInfo.playerRadius) + (curRadius + GetLongNoteCurRadius(1f - elapsedTimeWhenNeedInput01));
        if (curNextNoteRadius <= handy.GetScaleAverage(handy.boundary.transform.localScale) * handy.GetScaleAverage(new Vector2(boundarySprite.texture.width, boundarySprite.texture.height)) * 0.005f + handy.GetScaleAverage(nextNote.transform.localScale) * handy.GetScaleAverage(new Vector2(nextNoteSprite.texture.width, nextNoteSprite.texture.height)) * 0.005f)
        {
            nextNote.SetActive(true);
            nextNoteScript.PrepareNote(() => curRadius + GetLongNoteCurRadius(1f - elapsedTimeWhenNeedInput01));
            nextNoteScript.isPrepare = true;
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

        beforeWorldInfo = handy.GetWorldInfo(myNoteIndex - 1);
        worldInfo = handy.GetWorldInfo(myNoteIndex);
        afterWorldInfo = handy.GetWorldInfo(myNoteIndex + 1);
        tarPlayer = handy.player;
        tarPlayerScript = tarPlayer.GetComponent<Player>();
        stdRadius = beforeWorldInfo.playerRadius;
        curRadius = worldInfo.noteStartRadius;
        tarRadius = worldInfo.playerRadius;
        centerPos = worldInfo.centerPos;
        rotation = worldInfo.noteRotation;
        stdDeg = handy.GetBeforeDeg(myNoteIndex);
        tarDeg = handy.GetNextDeg(worldInfo);
        nextNote = handy.GetNote(myNoteIndex + 1);
        nextNoteScript = nextNote.GetComponent<NotePrefab>();
        startNoteSprite = Resources.Load<Sprite>("Image/Play/Player/" + handy.GetWorldInfo(myNoteIndex).SideImageName);
        endNoteSprite = Resources.Load<Sprite>("Image/Play/Player/" + handy.GetWorldInfo(myNoteIndex).SideImageName);
        SetNoteTransform();
        if (noteLengthTime != 0f)
        {
            for (int i = 0; i <= (int)(100f * noteLengthTime); i++)
            {
                myPathPoses.Add(handy.GetCircularPos
                (handy.GetCorrectDegMaxIs0(stdDeg + worldInfo.direction * handy.GetDistanceDeg(tarDeg, stdDeg, false) * worldInfo.playerMoveEaseType.Evaluate((float)i / Mathf.Floor(100f * noteLengthTime)))
                , beforeWorldInfo.playerRadius + (worldInfo.playerRadius - beforeWorldInfo.playerRadius) * worldInfo.playerRadiusEaseType.Evaluate((float)i / Mathf.Floor(100f * noteLengthTime))
                + GetLongNoteCurRadius((float)i / Mathf.Floor(100f * noteLengthTime))));
            }
        }
        else
        {
            myPathPoses.Add(handy.GetCircularPos(stdDeg, tarRadius, centerPos));
        }
        SetNotePartsTransform();
        SetNoteRenderer();
    }
    /* public void SetNoteScript()
    {
        worldInfo = handy.GetWorldInfo(myNoteIndex);
        tarPlayer = handy.player;
        tarPlayerScript = tarPlayer.GetComponent<Player>();
        curRadius = worldInfo.noteStartRadius;
        tarRadius = worldInfo.playerRadius;
        centerPos = worldInfo.centerPos;
        rotation = worldInfo.noteRotation;
        stdDeg = handy.GetBeforeDeg(myNoteIndex);
        tarDeg = worldInfo.nextDeg;
        nextNote = handy.GetNote(myNoteIndex + 1);
    } */
    public void PrepareNote(Func<float> getRadius)
    {
        StartCoroutine(UpdateRadiusWhilePreparing(getRadius));
    }
    IEnumerator UpdateRadiusWhilePreparing(Func<float> getRadius)
    {
        while (!handy.GetNote(myNoteIndex - 1).GetComponent<NotePrefab>().isDisable)
        {
            yield return null;
            curRadius = getRadius() + (worldInfo.noteStartRadius - beforeWorldInfo.playerRadius);
        }
    }
    public void ActToNeedInput()
    {
        toleranceTimeWhenNeedlessInput = elapsedTimeWhenNeedlessInput - noteWaitTime;
        // Debug.Log(toleranceTimeWhenNeedlessInput);
        tarPlayerScript.SetRadiusTweener(worldInfo.playerRadius, Mathf.Clamp(noteLengthTime - toleranceTimeWhenNeedlessInput, 0f, noteLengthTime == 0f ? 0f : float.MaxValue), worldInfo.playerRadiusEaseType);
        stopwatch.Reset();
        stopwatch.Stop();
        needInput = true;
    }
    void SetNoteTransform()
    {
        if (handy.GetNoteLengthTime(myNoteIndex) != 0)
            curDeg = GetCurDeg(worldInfo.playerMoveEaseType.Evaluate(Mathf.Clamp(elapsedTimeWhenNeedInput, 0f, float.MaxValue) / handy.GetNoteLengthTime(myNoteIndex)));
        // curDeg = stdDeg + handy.GetDistanceDeg(tarDeg, stdDeg, false) * worldInfo.playerMoveEaseType.Evaluate(Mathf.Clamp(elapsedTimeWhenNeedInput, 0f, float.MaxValue) / handy.GetNoteLengthTime(myNoteIndex));
        else
            curDeg = stdDeg;
        // deg = tarPlayerScript.curDeg;
        transform.position = handy.GetCircularPos(needInput ? tarPlayerScript.curDeg : stdDeg, curRadius);
        transform.rotation = Quaternion.Euler(0f, 0f, handy.GetCorrectDegMaxIs0(-(rotation + stdDeg)));
    }
    void SetNotePartsTransform()
    {
        curPathIndex = (int)(100f * Mathf.Clamp(elapsedTimeWhenNeedInput, 0f, float.MaxValue));
        myLocalPathPoses.Clear();
        myProcessPathPoses.Clear();
        dottedLineLength = 0f;
        for (int i = curPathIndex; i < myPathPoses.Count; i++)
        {
            Vector3 curPathPos = myPathPoses[i] + transform.position - (Vector3)handy.GetCircularPos(needInput ? tarPlayerScript.curDeg : stdDeg, beforeWorldInfo.playerRadius) - (Vector3)handy.GetCircularPos(GetCurDeg((float)i / (float)myPathPoses.Count), GetLongNoteCurRadius((float)curPathIndex / (float)myPathPoses.Count)/* worldInfo.noteLength * worldInfo.longNoteMoveEaseType.Evaluate((float)curPathIndex / (float)myPathPoses.Count) */) + (Vector3)centerPos;
            myLocalPathPoses.Add(curPathPos);
            myProcessPathPoses.Add(curPathPos);
            Sprite playerSprite = Resources.Load<Sprite>("Image/Play/Player/" + worldInfo.SideImageName);
            if (handy.CheckObjInOtherObj(myPathPoses[i], handy.GetCircularPos(stdDeg, stdRadius, centerPos), worldInfo.playerScale, new Vector2(playerSprite.texture.width, playerSprite.texture.height)) || handy.CheckObjInOtherObj(myPathPoses[i], handy.GetCircularPos(tarDeg, worldInfo.playerRadius, centerPos), worldInfo.playerScale, new Vector2(playerSprite.texture.width, playerSprite.texture.height)))
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
        startNoteRenderer.color = worldInfo.startNoteColor;
        processNoteRenderer.startColor = worldInfo.processNoteColor;
        processNoteRenderer.endColor = worldInfo.processNoteColor;
        endNoteRenderer.color = worldInfo.endNoteColor;
        ChangeNoteAlpha(colorAlpha);
    }
    public void StopNote()
    {
        TryKillFadeTweener(true);
        TryKillRadiusTweener(true);
        if (!isInputted && myNoteIndex != 0)
        {
            GameManager.Property.SetMissJudgment();
            // Debug.Log("ddd");
        }
        GameManager.Property.lastHitedNoteIndex += (int)Mathf.Clamp01(myNoteIndex);
        tarPlayerScript.stdDeg = tarPlayerScript.tarDeg;
        tarPlayerScript.curDeg = tarPlayerScript.stdDeg;
        tarPlayerScript.tarDeg = handy.GetNextDeg(myNoteIndex + 1);
        tarPlayerScript.TryKillMoveTweener();
        tarPlayerScript.SideScaleToOrig();
        stopwatch.Stop();
        DisableMe();
    }
    void SetRadiusTweener(float curRadius, float tarRadius, float duration, AnimationCurve easeType)
    {
        TryKillRadiusTweener(true);
        myRadiusTweener = DOTween.Sequence().Append(DOTween.To(() => curRadius, r => curRadius = r, tarRadius, duration).SetEase(easeType).OnUpdate(() => { this.curRadius = curRadius; }).OnComplete(() => { this.curRadius = tarRadius; })/* .OnComplete(() => isDisable = true) *//* .SetUpdate(true) */);
    }
    void SetFadeTweener(float curValue, float tarValue, float duration, AnimationCurve easeType)
    {
        TryKillFadeTweener(true);
        myFadeTweener = DOTween.Sequence().Append(DOTween.To(() => curValue, r => curValue = r, tarValue, duration).SetEase(easeType).OnUpdate(() => { colorAlpha = curValue; }).OnComplete(() => { colorAlpha = tarValue; })/* .SetUpdate(true) */);
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
    /* void TryKillNoteScaleTweener(bool isComplete = true)
    {
        if (myNoteScaleTweener != null)
        {
            myNoteScaleTweener.Kill(isComplete);
            myNoteScaleTweener = null;
        }
    } */
    void ChangeNoteAlpha(float alpha)
    {
        handy.ChangeAlpha(startNoteRenderer, alpha);
        handy.ChangeAlpha(processNoteRenderer, noteLengthTime == 0f ? 0f : alpha);
        handy.ChangeAlpha(endNoteRenderer, noteLengthTime == 0f ? 0f : alpha);
    }
    void DisableMe()
    {
        handy.noteGeneratorScript.closestNote = nextNote;
        handy.noteGeneratorScript.closestNoteScript = nextNote.GetComponent<NotePrefab>();
        nextNote.SetActive(true);
        gameObject.SetActive(false);
    }
    float GetCurDeg(float progress01)
    {
        return handy.GetCorrectDegMaxIs0(stdDeg + worldInfo.direction * handy.GetDistanceDeg(tarDeg, stdDeg, false) * Mathf.Clamp01(progress01));
    }
    float GetLongNoteCurRadius(float progress01)
    {
        return worldInfo.noteLength * worldInfo.longNoteMoveEaseType.Evaluate(Mathf.Clamp01(progress01));
    }
}
