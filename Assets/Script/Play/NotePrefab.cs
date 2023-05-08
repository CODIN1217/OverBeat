using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Diagnostics;
using System;
using Debug = UnityEngine.Debug;
using DG.Tweening;
using TweenManager;

public class NotePrefab : MonoBehaviour
{
    public bool isInputted;
    public int myWorldInfoIndex;
    public int myNoteIndex;
    public int tarPlayerIndex;
    int curPathIndex;
    float stdDeg;
    float tarDeg;
    public float stdRadius;
    public float tarRadius;
    public float holdElapsedSecs;
    public float waitElapsedSecs;
    public float waitElapsedSecs01;
    public float holdElapsedSecs01;
    public float noteWaitSecs;
    public float holdNoteSecs;
    public float toleranceSecsWhenAwake;
    public float toleranceSecsEndWait;
    float dottedLineLength;
    bool isAwake;
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
    List<Vector3> pathPoses;
    List<Vector3> localPathPoses;
    List<Vector3> processPathPoses;

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
    public WorldInfo worldInfo;
    Handy handy;
    PlayGameManager playGM;
    void Awake()
    {
        playGM = PlayGameManager.Property;
        myElapsedTime = new Stopwatch();
        myElapsedTime.Stop();
        isAwake = true;
    }
    void Update()
    {
        if (playGM.isPause)
            return;
        if (tarPlayerIndex == -1 && myNoteIndex == -1)
            return;
        if (isAwake)
        {
            PlayWaitTween();
            myElapsedTime.Reset();
            myElapsedTime.Stop();
            isAwake = false;
        }
        UpdateTweenValue();
        UpdateElapsedSecs01();
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
        if (tarPlayerIndex == -1 && myNoteIndex == -1)
            return;
        UpdateNoteTransform();
        UpdateNotePartsTransform();
        UpdateNoteRenderer();
    }
    public void InitNote()
    {
        handy = Handy.Property;
        pathPoses = new List<Vector3>();
        localPathPoses = new List<Vector3>();
        processPathPoses = new List<Vector3>();

        InitChildInfo();

        worldInfo = playGM.GetWorldInfo(myWorldInfoIndex);
        tarPlayer = playGM.GetPlayer(tarPlayerIndex);
        tarPlayerScript = playGM.GetPlayerScript(tarPlayerIndex);
        stdRadius = worldInfo.playerInfo[tarPlayerIndex].radiusTween.startValue;
        tarRadius = worldInfo.playerInfo[tarPlayerIndex].radiusTween.endValue;
        stdDeg = worldInfo.playerInfo[tarPlayerIndex].degTween.startValue;
        tarDeg = worldInfo.playerInfo[tarPlayerIndex].degTween.endValue;
        nextNote = playGM.GetNote(tarPlayerIndex, myNoteIndex + 1);
        nextNoteScript = playGM.GetNoteScript(tarPlayerIndex, myNoteIndex + 1);
        startNoteSprite = Resources.Load<Sprite>("Image/Play/Player/" + worldInfo.noteInfo.sideImageName);
        endNoteSprite = Resources.Load<Sprite>("Image/Play/Player/" + worldInfo.noteInfo.sideImageName);

        InitTween();

        if (holdNoteSecs != 0f)
        {
            for (int i = 0; i <= (int)(100f * holdNoteSecs); i++)
            {
                float curDeg = TweenMethod.GetTweenValue(worldInfo.playerInfo[tarPlayerIndex].degTween, (float)i / Mathf.Floor(100f * holdNoteSecs));
                float curRadius = TweenMethod.GetTweenValue(worldInfo.playerInfo[tarPlayerIndex].radiusTween, (float)i / Mathf.Floor(100f * holdNoteSecs)) + TweenMethod.GetTweenValue(worldInfo.noteInfo.holdRadiusTween, (float)i / Mathf.Floor(100f * holdNoteSecs));
                pathPoses.Add(handy.GetCircularPos(curDeg, curRadius));
            }
        }
        else
        {
            pathPoses.Add(handy.GetCircularPos(stdDeg, stdRadius));
            pathPoses.Add(handy.GetCircularPos(tarDeg, tarRadius));
        }

        UpdateNoteTransform();
        UpdateNotePartsTransform();
        UpdateNoteRenderer();
        UpdateElapsedSecs01();

        playGM.playTweenEvent += PlayHoldTween;
    }
    public void InitTween()
    {
        waitRadiusInfo = new TweeningInfo(worldInfo.noteInfo.waitRadiusTween, playGM.GetNoteWaitSecs(myWorldInfoIndex));
        appearanceInfo = new TweeningInfo(worldInfo.noteInfo.appearanceTween, playGM.GetNoteWaitSecs(myWorldInfoIndex) * 0.3f);

        holdRadiusInfo = new TweeningInfo(worldInfo.noteInfo.holdRadiusTween, playGM.GetHoldNoteSecs(myWorldInfoIndex));
        totalRotationInfo = new TweeningInfo(worldInfo.noteInfo.totalRotationTween, playGM.GetHoldNoteSecs(myWorldInfoIndex));
        startRotationInfo = new TweeningInfo(worldInfo.noteInfo.startRotationTween, playGM.GetHoldNoteSecs(myWorldInfoIndex));
        endRotationInfo = new TweeningInfo(worldInfo.noteInfo.endRotationTween, playGM.GetHoldNoteSecs(myWorldInfoIndex));
        startColorInfo = new TweeningInfo(worldInfo.noteInfo.startColorTween, playGM.GetHoldNoteSecs(myWorldInfoIndex));
        processStartColorInfo = new TweeningInfo(worldInfo.noteInfo.processStartColorTween, playGM.GetHoldNoteSecs(myWorldInfoIndex));
        processEndColorInfo = new TweeningInfo(worldInfo.noteInfo.processEndColorTween, playGM.GetHoldNoteSecs(myWorldInfoIndex));
        endColorInfo = new TweeningInfo(worldInfo.noteInfo.endColorTween, playGM.GetHoldNoteSecs(myWorldInfoIndex));
    }
    public void UpdateTweenValue()
    {
        waitRadius = ((TweenerInfo<float>)waitRadiusInfo).curValue;
        appearance = ((TweenerInfo<float>)appearanceInfo).curValue;

        totalRotation = handy.GetCorrectDegMaxIs0(-(((TweenerInfo<float>)totalRotationInfo).curValue));
        startRotation = handy.GetCorrectDegMaxIs0(-(((TweenerInfo<float>)startRotationInfo).curValue + stdDeg));
        endRotation = handy.GetCorrectDegMaxIs0(-(((TweenerInfo<float>)endRotationInfo).curValue + tarDeg));
        startColor = playGM.GetColor01WithPlayerIndex(((TweenerInfo<Color>)startColorInfo).curValue, tarPlayerIndex);
        processStartColor = playGM.GetColor01WithPlayerIndex(((TweenerInfo<Color>)processStartColorInfo).curValue, tarPlayerIndex);
        processEndColor = playGM.GetColor01WithPlayerIndex(((TweenerInfo<Color>)processEndColorInfo).curValue, tarPlayerIndex);
        endColor = playGM.GetColor01WithPlayerIndex(((TweenerInfo<Color>)endColorInfo).curValue, tarPlayerIndex);
    }
    public void PlayWaitTween()
    {
        handy.PlayTweens(
            waitRadiusInfo,
            appearanceInfo);
    }
    public void PlayHoldTween()
    {
        handy.PlayTweens(
            totalRotationInfo,
            startRotationInfo,
            endRotationInfo,
            startColorInfo,
            processStartColorInfo,
            processEndColorInfo,
            endColorInfo);
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
            holdElapsedSecs01 = Mathf.Clamp01(Mathf.Clamp(holdElapsedSecs, 0f, playGM.GetNoteWaitSecs(tarPlayerIndex, myNoteIndex + 1)) / playGM.GetNoteWaitSecs(tarPlayerIndex, myNoteIndex + 1));
    }
    public void EndWaiting()
    {
        playGM.worldInfoIndex++;

        toleranceSecsEndWait = waitElapsedSecs - noteWaitSecs;
        waitElapsedSecs = noteWaitSecs;

        handy.TryKillTween(appearanceInfo);
        handy.TryKillTween(waitRadiusInfo);
        
        playGM.initTweenEvent();
        playGM.playTweenEvent();
        // playerScript.InitTween();

        // PlayHoldTween();
        // playerScript.PlayTween();
        myElapsedTime.Reset();
        myElapsedTime.Stop();
        isNeedInput = true;
    }
    void UpdateNoteTransform()
    {
        transform.position = handy.GetCircularPos(tarDeg, waitRadius, playGM.centerScript.pos);
        startNote.transform.localScale = tarPlayerScript.playerSide.transform.localScale;
        endNote.transform.localScale = tarPlayerScript.playerSide.transform.localScale;
        transform.rotation = Quaternion.Euler(0f, 0f, totalRotation);
    }
    void UpdateNotePartsTransform()
    {
        curPathIndex = (int)(100f * Mathf.Clamp(holdElapsedSecs, 0f, float.MaxValue));
        localPathPoses.Clear();
        processPathPoses.Clear();
        dottedLineLength = 0f;
        for (int i = curPathIndex; i < pathPoses.Count; i++)
        {
            processPathPoses.Add(pathPoses[i]);
            localPathPoses.Add(pathPoses[i] - (Vector3)handy.GetCircularPos(stdDeg, stdRadius));
            Sprite playerSprite = Resources.Load<Sprite>("Image/Play/Player/" + worldInfo.noteInfo.sideImageName);
            if (handy.CheckObjInOtherObj(pathPoses[i], Vector2.zero, handy.GetCircularPos(stdDeg, stdRadius), worldInfo.playerInfo[tarPlayerIndex].totalScaleTween.startValue, Vector2.zero, handy.GetSpritePixels(playerSprite)) || handy.CheckObjInOtherObj(pathPoses[i], Vector2.zero, handy.GetCircularPos(tarDeg, worldInfo.playerInfo[tarPlayerIndex].radiusTween.endValue), worldInfo.playerInfo[tarPlayerIndex].totalScaleTween.endValue, Vector2.zero, handy.GetSpritePixels(playerSprite)))
            {
                processPathPoses.RemoveAt(processPathPoses.Count - 1);
            }
            else
            {
                processPathPoses[processPathPoses.Count - 1] -= (Vector3)handy.GetCircularPos(stdDeg, stdRadius);
                dottedLineLength += Vector2.Distance(pathPoses[handy.GetBeforeIndex(i - curPathIndex)], pathPoses[i - curPathIndex]);
            }
        }
        if (localPathPoses.Count > 0)
        {
            startNote.transform.localPosition = localPathPoses[0];
            endNote.transform.localPosition = localPathPoses[localPathPoses.Count - 1];
        }
        if (processPathPoses.Count > 0)
        {
            processNoteDottedLine.poses = processPathPoses;
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
        playGM.closestNoteIndex[tarPlayerIndex]++;
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
