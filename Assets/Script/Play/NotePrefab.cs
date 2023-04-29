using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Diagnostics;
using System;
using Debug = UnityEngine.Debug;
using DG.Tweening;
using TweenValue;

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
    public float holdNoteSecs;
    public float toleranceSecsEndWait;
    float dottedLineLength;
    bool isAfterAwake;
    public bool isDisable;
    public bool isStop;
    public bool isNeedInput;
    public Sprite startNoteSprite;
    public Sprite endNoteSprite;
    GameObject startNote;
    GameObject processNote;
    GameObject endNote;
    GameObject tarPlayer;
    Player tarPlayerScript;
    GameObject nextNote;
    List<Vector3> myPathPoses;
    List<Vector3> myLocalPathPoses;
    List<Vector3> myProcessPathPoses;

    public float waitRadius;
    public float totalRotation;
    public float startRotation;
    public float endRotation;
    public float appearance;
    public Color startColor;
    public Color processStartColor;
    public Color processEndColor;
    public Color endColor;

    public TweeningInfo waitRadiusInfo;
    public TweeningInfo holdRadiusInfo;
    public TweeningInfo totalRotationInfo;
    public TweeningInfo startRotationInfo;
    public TweeningInfo endRotationInfo;
    public TweeningInfo appearanceInfo;
    public TweeningInfo startColorInfo;
    public TweeningInfo processStartColorInfo;
    public TweeningInfo processEndColorInfo;
    public TweeningInfo endColorInfo;

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
        myElapsedTime.Stop();
        isAfterAwake = true;
    }
    void Update()
    {
        if (playGM.isBreakUpdate() && playGM.countDownScript.isCountDown)
            return;
        if (playGM.countDownScript.isCountDown)
            return;
        UpdateTweenValue();
        UpdateElapsedSecs01();
        if (isAfterAwake)
        {
            handy.PlayTweens(
                waitRadiusInfo,
                appearanceInfo);
            myElapsedTime.Reset();
            myElapsedTime.Stop();
            isAfterAwake = false;
        }
        myElapsedTime.Start();
        if (!isNeedInput)
        {
            waitElapsedSecs = toleranceSecsWhenAwake + myElapsedTime.ElapsedMilliseconds * 0.001f;
            if (waitElapsedSecs >= noteWaitSecs)
            {
                EndWaiting();
            }
        }
        if (isNeedInput && !isStop)
        {
            holdElapsedSecs = toleranceSecsEndWait + myElapsedTime.ElapsedMilliseconds * 0.001f;
            if (holdElapsedSecs >= holdNoteSecs)
            {
                isDisable = true;
            }
        }
        if (isDisable)
        {
            appearance = 0f;
            if (!isStop)
            {
                if (holdElapsedSecs01 > playGM.judgmentRange[tarPlayerIndex])
                    StopNote();
            }
        }
        if (playGM.GetJudgmentValue(tarPlayerIndex) <= playGM.judgmentRange[tarPlayerIndex])
        {
            if (playGM.GetIsKeyDown(tarPlayerIndex))
            {
                if (!isNeedInput)
                {
                    EndWaiting();
                }
                tarPlayerScript.StartCoroutine(CheckHoldingKey());
            }
        }
    }
    void LateUpdate()
    {
        if (tarPlayerIndex == -1 && myEachNoteIndex == -1)
            return;
        UpdateNoteTransform();
        UpdateNotePartsTransform();
        UpdateNoteRenderer();
    }
    public void InitNote()
    {
        handy = Handy.Property;
        myPathPoses = new List<Vector3>();
        myLocalPathPoses = new List<Vector3>();
        myProcessPathPoses = new List<Vector3>();

        InitChildInfo();

        beforeWorldInfo = playGM.GetWorldInfo(tarPlayerIndex, myEachNoteIndex - 1);
        worldInfo = playGM.GetWorldInfo(tarPlayerIndex, myEachNoteIndex);
        afterWorldInfo = playGM.GetWorldInfo(tarPlayerIndex, myEachNoteIndex + 1);
        tarPlayer = playGM.GetPlayer(tarPlayerIndex);
        tarPlayerScript = playGM.GetPlayerScript(tarPlayerIndex);
        stdRadius = worldInfo.playerInfo[tarPlayerIndex].radiusTween.startValue;
        tarRadius = worldInfo.playerInfo[tarPlayerIndex].radiusTween.endValue;
        stdDeg = worldInfo.playerInfo[tarPlayerIndex].degTween.startValue;
        tarDeg = worldInfo.playerInfo[tarPlayerIndex].degTween.endValue;
        nextNote = playGM.GetNote(tarPlayerIndex, myEachNoteIndex + 1);
        nextNoteScript = playGM.GetNoteScript(tarPlayerIndex, myEachNoteIndex + 1);
        startNoteSprite = Resources.Load<Sprite>("Image/Play/Player/" + worldInfo.noteInfo.sideImageName);
        endNoteSprite = Resources.Load<Sprite>("Image/Play/Player/" + worldInfo.noteInfo.sideImageName);

        waitRadiusInfo = new TweeningInfo(worldInfo.noteInfo.waitRadiusTween, playGM.GetNoteWaitSecs(tarPlayerIndex, myEachNoteIndex));
        appearanceInfo = new TweeningInfo(worldInfo.noteInfo.appearanceTween, playGM.GetNoteWaitSecs(tarPlayerIndex, myEachNoteIndex) * 0.3f);

        holdRadiusInfo = new TweeningInfo(worldInfo.noteInfo.holdRadiusTween, playGM.GetHoldNoteSecs(tarPlayerIndex, myEachNoteIndex));
        totalRotationInfo = new TweeningInfo(worldInfo.noteInfo.totalRotationTween, playGM.GetHoldNoteSecs(tarPlayerIndex, myEachNoteIndex));
        startRotationInfo = new TweeningInfo(worldInfo.noteInfo.startRotationTween, playGM.GetHoldNoteSecs(tarPlayerIndex, myEachNoteIndex));
        endRotationInfo = new TweeningInfo(worldInfo.noteInfo.endRotationTween, playGM.GetHoldNoteSecs(tarPlayerIndex, myEachNoteIndex));
        startColorInfo = new TweeningInfo(worldInfo.noteInfo.startColorTween, playGM.GetHoldNoteSecs(tarPlayerIndex, myEachNoteIndex));
        processStartColorInfo = new TweeningInfo(worldInfo.noteInfo.processStartColorTween, playGM.GetHoldNoteSecs(tarPlayerIndex, myEachNoteIndex));
        processEndColorInfo = new TweeningInfo(worldInfo.noteInfo.processEndColorTween, playGM.GetHoldNoteSecs(tarPlayerIndex, myEachNoteIndex));
        endColorInfo = new TweeningInfo(worldInfo.noteInfo.endColorTween, playGM.GetHoldNoteSecs(tarPlayerIndex, myEachNoteIndex));
        TweeningInfo playerDegInfo = new TweeningInfo(worldInfo.playerInfo[tarPlayerIndex].degTween, holdNoteSecs);
        TweeningInfo playerRadiusInfo = new TweeningInfo(worldInfo.playerInfo[tarPlayerIndex].radiusTween, holdNoteSecs);
        if (holdNoteSecs != 0f)
        {
            for (int i = 0; i <= (int)(100f * holdNoteSecs); i++)
            {
                float curDeg = (float)handy.GetTweenValue(playerDegInfo, (float)i / Mathf.Floor(100f * holdNoteSecs));
                float curRadius = (float)handy.GetTweenValue(playerRadiusInfo, (float)i / Mathf.Floor(100f * holdNoteSecs)) + (float)handy.GetTweenValue(holdRadiusInfo, (float)i / Mathf.Floor(100f * holdNoteSecs));
                myPathPoses.Add(handy.GetCircularPos(curDeg, curRadius));
            }
        }
        else
        {
            myPathPoses.Add(handy.GetCircularPos(stdDeg, stdRadius));
            myPathPoses.Add(handy.GetCircularPos(tarDeg, tarRadius));
        }

        UpdateNoteTransform();
        UpdateNotePartsTransform();
        UpdateNoteRenderer();
        UpdateElapsedSecs01();

        playerDegInfo.tweener.Kill();
        playerRadiusInfo.tweener.Kill();
    }
    void UpdateTweenValue()
    {
        waitRadius = (float)waitRadiusInfo.curValue;
        appearance = (float)appearanceInfo.curValue;

        totalRotation = handy.GetCorrectDegMaxIs0(-((float)totalRotationInfo.curValue));
        startRotation = handy.GetCorrectDegMaxIs0(-((float)startRotationInfo.curValue + curDeg));
        endRotation = handy.GetCorrectDegMaxIs0(-((float)endRotationInfo.curValue + tarDeg));
        startColor = playGM.GetColor01WithPlayerIndex((Color)startColorInfo.curValue, tarPlayerIndex);
        processStartColor = playGM.GetColor01WithPlayerIndex((Color)processStartColorInfo.curValue, tarPlayerIndex);
        processEndColor = playGM.GetColor01WithPlayerIndex((Color)processEndColorInfo.curValue, tarPlayerIndex);
        endColor = playGM.GetColor01WithPlayerIndex((Color)endColorInfo.curValue, tarPlayerIndex);
    }
    public void UpdateElapsedSecs01()
    {
        if (noteWaitSecs != 0f)
            waitElapsedSecs01 = Mathf.Clamp01(Mathf.Clamp(waitElapsedSecs, 0f, noteWaitSecs) / noteWaitSecs);
        else
            waitElapsedSecs01 = 0f;

        if (holdNoteSecs != 0f)
            holdElapsedSecs01 = Mathf.Clamp01(Mathf.Clamp(holdElapsedSecs, 0f, holdNoteSecs) / holdNoteSecs);
        else
            holdElapsedSecs01 = Mathf.Clamp01(Mathf.Clamp(holdElapsedSecs, 0f, playGM.GetNoteWaitSecs(tarPlayerIndex, myEachNoteIndex + 1)) / playGM.GetNoteWaitSecs(tarPlayerIndex, myEachNoteIndex + 1));
    }
    public void EndWaiting()
    {
        toleranceSecsEndWait = waitElapsedSecs - noteWaitSecs;
        waitElapsedSecs = noteWaitSecs;

        handy.TryKillTween(appearanceInfo);
        handy.TryKillTween(waitRadiusInfo);

        handy.PlayTweens(
            totalRotationInfo,
            startRotationInfo,
            endRotationInfo,
            startColorInfo,
            processStartColorInfo,
            processEndColorInfo,
            endColorInfo,

            tarPlayerScript.radiusTweener,
            tarPlayerScript.degInfo,
            tarPlayerScript.rotationInfo,
            tarPlayerScript.totalScaleInfo,
            tarPlayerScript.sideScaleInfo,
            tarPlayerScript.centerScaleInfo,
            tarPlayerScript.sideColorInfo,
            tarPlayerScript.centerColorInfo);
        myElapsedTime.Reset();
        myElapsedTime.Stop();
        playGM.worldInfoIndex += 1;
        isNeedInput = true;
    }
    void UpdateNoteTransform()
    {
        curDeg = tarPlayerScript.curDeg;
        transform.position = handy.GetCircularPos(curDeg, waitRadius, playGM.centerScript.pos);
        startNote.transform.localScale = tarPlayerScript.playerSide.transform.localScale;
        endNote.transform.localScale = tarPlayerScript.playerSide.transform.localScale;
        transform.rotation = Quaternion.Euler(0f, 0f, totalRotation);
    }
    void UpdateNotePartsTransform()
    {
        curPathIndex = (int)(100f * Mathf.Clamp(holdElapsedSecs, 0f, float.MaxValue));
        myLocalPathPoses.Clear();
        myProcessPathPoses.Clear();
        dottedLineLength = 0f;
        for (int i = curPathIndex; i < myPathPoses.Count; i++)
        {
            myProcessPathPoses.Add(myPathPoses[i]);
            myLocalPathPoses.Add(myPathPoses[i] - (Vector3)handy.GetCircularPos(stdDeg, stdRadius));
            Sprite playerSprite = Resources.Load<Sprite>("Image/Play/Player/" + worldInfo.noteInfo.sideImageName);
            if (handy.CheckObjInOtherObj(myPathPoses[i], Vector2.zero, handy.GetCircularPos(stdDeg, stdRadius), worldInfo.playerInfo[tarPlayerIndex].totalScaleTween.startValue, Vector2.zero, handy.GetSpritePixels(playerSprite)) || handy.CheckObjInOtherObj(myPathPoses[i], Vector2.zero, handy.GetCircularPos(tarDeg, worldInfo.playerInfo[tarPlayerIndex].radiusTween.endValue), worldInfo.playerInfo[tarPlayerIndex].totalScaleTween.endValue, Vector2.zero, handy.GetSpritePixels(playerSprite)))
            {
                myProcessPathPoses.RemoveAt(myProcessPathPoses.Count - 1);
            }
            else
            {
                myProcessPathPoses[myProcessPathPoses.Count - 1] -= (Vector3)handy.GetCircularPos(stdDeg, stdRadius);
                dottedLineLength += Vector2.Distance(myPathPoses[handy.GetBeforeIndex(i - curPathIndex)], myPathPoses[i - curPathIndex]);
            }
        }
        if (myLocalPathPoses.Count > 0)
        {
            startNote.transform.localPosition = myLocalPathPoses[0];
            endNote.transform.localPosition = myLocalPathPoses[myLocalPathPoses.Count - 1];
        }
        if (myProcessPathPoses.Count > 0)
        {
            processNoteDottedLine.poses = myProcessPathPoses;
            processNoteDottedLine.SetRepeatCount(dottedLineLength * 2.444f);
        }
        startNote.transform.rotation = Quaternion.Euler(0f, 0f, startRotation);
        endNote.transform.rotation = Quaternion.Euler(0f, 0f, endRotation);
    }
    public void UpdateNoteRenderer()
    {
        startNoteRenderer.sprite = startNoteSprite;
        endNoteRenderer.sprite = endNoteSprite;
        startNoteRenderer.color = startColor;
        processNoteRenderer.startColor = processStartColor;
        processNoteRenderer.endColor = processEndColor;
        endNoteRenderer.color = endColor;
        ChangeNoteAlpha(appearance);
    }
    public void StopNote()
    {
        if (holdNoteSecs != 0f)
        {
            float accuracy01_temp = 1f;
            if (!isInputted)
            {
                playGM.judgmentGenScript.SetJudgmentText(tarPlayerIndex, JudgmentType.Miss);
                playGM.CountMissNote();
                accuracy01_temp = 0f;
            }
            else
                playGM.judgmentGenScript.SetJudgmentText(tarPlayerIndex, playGM.GetJudgment(tarPlayerIndex, 1f - holdElapsedSecs01, () => { accuracy01_temp = holdElapsedSecs01; playGM.CountMissNote(); }));

            playGM.sumNoteAccuracy01 += accuracy01_temp;
            playGM.InputCount++;
            playGM.SetAccuracy01();
        }
        handy.StartCoroutine(tarPlayerScript.SetSideClickScaleTweener(true));
        myElapsedTime.Stop();
        holdElapsedSecs = holdNoteSecs;
        DisableMe();
        isStop = true;
    }
    public void InitChildInfo()
    {
        startNote = transform.GetChild(0).gameObject;
        processNote = transform.GetChild(1).gameObject;
        endNote = transform.GetChild(2).gameObject;
        startNoteRenderer = startNote.transform.GetChild(0).GetComponent<SpriteRenderer>();
        processNoteRenderer = processNote.GetComponent<LineRenderer>();
        processNoteDottedLine = processNote.GetComponent<DottedLine>();
        endNoteRenderer = endNote.transform.GetChild(0).GetComponent<SpriteRenderer>();
    }
    public void ChangeNoteAlpha(float alpha)
    {
        handy.ChangeAlpha(startNoteRenderer, alpha);
        handy.ChangeAlpha(processNoteRenderer, holdNoteSecs == 0f ? 0f : alpha);
        handy.ChangeAlpha(endNoteRenderer, holdNoteSecs == 0f ? 0f : alpha);
    }
    void DisableMe()
    {
        gameObject.SetActive(false);
    }
    IEnumerator CheckHoldingKey()
    {
        if (holdNoteSecs != 0f)
        {
            while (playGM.GetIsKeyPress(tarPlayerIndex) && !isDisable)
            {
                yield return null;
            }
            if (!isDisable)
            {
                StopNote();
            }
        }
    }
}
