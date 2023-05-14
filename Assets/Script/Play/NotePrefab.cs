using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Diagnostics;
using System;
using System.Linq;
using Debug = UnityEngine.Debug;
using DG.Tweening;
using TweenManager;
using System.Threading;
using System.Threading.Tasks;

public class NotePrefab : MonoBehaviour, PlayManager.ITweenerInPlay, IGameObject
{
    public int myWorldInfoIndex;
    public int myNoteIndex;
    public int tarPlayerIndex;

    int posCount;

    public float holdElapsedSecs;
    public float waitElapsedSecs;
    public float waitElapsedSecs01;
    public float holdElapsedSecs01;

    public float noteWaitSecs;
    public float holdNoteSecs;
    public float toleranceSecsAwake;
    public float toleranceSecsEndWait;

    float pathPosesLength;
    float processPathPosesLength;

    bool isAwake;
    public bool isInputted;

    public bool isDisable;
    public bool isStop;
    public bool isNeedInput;

    public Sprite startNoteSprite;
    public Sprite endNoteSprite;
    GameObject startNote;
    GameObject processNote;
    GameObject endNote;
    SpriteRenderer startNoteRenderer;
    LineRenderer processNoteRenderer;
    DottedLine processNoteDottedLine;
    SpriteRenderer endNoteRenderer;
    GameObject tarPlayer;
    Player tarPlayerScript;
    GameObject nextNote;
    NotePrefab nextNoteScript;
    List<Vector3> pathPoses;
    List<Vector3> processPathPoses;

    public float waitDeltaRadius;
    public float holdDeltaRadius;
    public float totalRotation;
    public float startRotation;
    public float endRotation;
    public float fade;
    public Color startColor;
    public Color processStartColor;
    public Color processEndColor;
    public Color endColor;

    public TweeningInfo waitDeltaRadiusInfo;
    public TweeningInfo holdDeltaRadiusInfo;
    public TweeningInfo totalRotationInfo;
    public TweeningInfo startRotationInfo;
    public TweeningInfo endRotationInfo;
    public TweeningInfo fadeInfo;
    public TweeningInfo startColorInfo;
    public TweeningInfo processStartColorInfo;
    public TweeningInfo processEndColorInfo;
    public TweeningInfo endColorInfo;

    public WorldInfo worldInfo;
    Stopwatch myElapsedTime;
    Handy handy;
    PlayManager PM;
    void Awake()
    {
        handy = Handy.Property;
        PM = PlayManager.Property;
        myElapsedTime = new Stopwatch();
        myElapsedTime.Stop();
        pathPoses = new List<Vector3>();
        isAwake = true;

        startNote = transform.GetChild(0).gameObject;
        processNote = transform.GetChild(1).gameObject;
        endNote = transform.GetChild(2).gameObject;
        startNoteRenderer = startNote.transform.GetChild(0).GetComponent<SpriteRenderer>();
        processNoteRenderer = processNote.GetComponent<LineRenderer>();
        processNoteDottedLine = processNote.GetComponent<DottedLine>();
        endNoteRenderer = endNote.transform.GetChild(0).GetComponent<SpriteRenderer>();
    }
    void Update()
    {
        if (PM.isPause)
            return;
        if (myWorldInfoIndex == 0)
            return;
        if (isAwake)
        {
            PM.AddGO(this).AddTweenerInPlayGO(this);
            InitNoteTween();
            PM.PlayWaitTweenAll();
            myElapsedTime.Reset();
            myElapsedTime.Stop();
            isAwake = false;
        }
        UpdateNoteTweenValue();
        UpdateElapsedSecs01();
        myElapsedTime.Start();
        if (!isNeedInput)
        {
            waitElapsedSecs = toleranceSecsAwake + myElapsedTime.ElapsedMilliseconds * 0.001f;
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
            if (!isStop)
            {
                if (holdElapsedSecs01 > PM.judgmentRange[tarPlayerIndex])
                    StopNote();
            }
        }
        if (PM.GetJudgmentValue(tarPlayerIndex) <= PM.judgmentRange[tarPlayerIndex])
        {
            if (PM.GetIsKeyDown(tarPlayerIndex))
            {
                if (!isNeedInput)
                {
                    EndWaiting();
                }
                tarPlayerScript.StartCoroutine(CheckHoldingKey());
            }
        }
    }
    public void InitNote()
    {
        worldInfo = PM.GetWorldInfo(myWorldInfoIndex);
        tarPlayer = PM.GetPlayer(tarPlayerIndex);
        tarPlayerScript = PM.GetPlayerScript(tarPlayerIndex);
        nextNote = PM.GetNote(tarPlayerIndex, myNoteIndex + 1);
        nextNoteScript = PM.GetNoteScript(tarPlayerIndex, myNoteIndex + 1);
        startNoteSprite = Resources.Load<Sprite>("Image/Play/Player/" + worldInfo.noteInfo.sideImageName);
        endNoteSprite = Resources.Load<Sprite>("Image/Play/Player/" + worldInfo.noteInfo.sideImageName);

        posCount = 100;
    }
    public void InitNoteTween()
    {
        if (myWorldInfoIndex == 0)
            return;
        waitDeltaRadiusInfo = new TweeningInfo(worldInfo.noteInfo.waitDeltaRadiusTween, PM.GetNoteWaitSecs(myWorldInfoIndex));
        fadeInfo = new TweeningInfo(worldInfo.noteInfo.fadeTween, PM.GetNoteWaitSecs(myWorldInfoIndex) * 0.3f);

        holdDeltaRadiusInfo = new TweeningInfo(worldInfo.noteInfo.holdDeltaRadiusTween, PM.GetHoldNoteSecs(myWorldInfoIndex));
        totalRotationInfo = new TweeningInfo(worldInfo.noteInfo.totalRotationTween, PM.GetHoldNoteSecs(myWorldInfoIndex));
        startRotationInfo = new TweeningInfo(worldInfo.noteInfo.startRotationTween, PM.GetHoldNoteSecs(myWorldInfoIndex));
        endRotationInfo = new TweeningInfo(worldInfo.noteInfo.endRotationTween, PM.GetHoldNoteSecs(myWorldInfoIndex));
        startColorInfo = new TweeningInfo(worldInfo.noteInfo.startColorTween, PM.GetHoldNoteSecs(myWorldInfoIndex));
        processStartColorInfo = new TweeningInfo(worldInfo.noteInfo.processStartColorTween, PM.GetHoldNoteSecs(myWorldInfoIndex));
        processEndColorInfo = new TweeningInfo(worldInfo.noteInfo.processEndColorTween, PM.GetHoldNoteSecs(myWorldInfoIndex));
        endColorInfo = new TweeningInfo(worldInfo.noteInfo.endColorTween, PM.GetHoldNoteSecs(myWorldInfoIndex));
    }
    public void UpdateNoteTweenValue()
    {
        if (myWorldInfoIndex == 0)
            return;
        waitDeltaRadius = ((TweenerInfo<float>)waitDeltaRadiusInfo).curValue;
        fade = isDisable ? 0f : ((TweenerInfo<float>)fadeInfo).curValue;

        holdDeltaRadius = ((TweenerInfo<float>)holdDeltaRadiusInfo).curValue;
        totalRotation = handy.CorrectDegMaxIs0(-(((TweenerInfo<float>)totalRotationInfo).curValue));
        startRotation = handy.CorrectDegMaxIs0(-(((TweenerInfo<float>)startRotationInfo).curValue + tarPlayerScript.curDeg));
        endRotation = handy.CorrectDegMaxIs0(-(((TweenerInfo<float>)endRotationInfo).curValue + worldInfo.playerInfo[tarPlayerIndex].degTween.endValue));
        startColor = PM.GetColor01WithPlayerIndex(((TweenerInfo<Color>)startColorInfo).curValue, tarPlayerIndex);
        processStartColor = PM.GetColor01WithPlayerIndex(((TweenerInfo<Color>)processStartColorInfo).curValue, tarPlayerIndex);
        processEndColor = PM.GetColor01WithPlayerIndex(((TweenerInfo<Color>)processEndColorInfo).curValue, tarPlayerIndex);
        endColor = PM.GetColor01WithPlayerIndex(((TweenerInfo<Color>)endColorInfo).curValue, tarPlayerIndex);
    }
    public void PlayWaitTween()
    {
        if (myWorldInfoIndex == 0)
            return;
        TweenMethod.PlayTweens(
            waitDeltaRadiusInfo,
            fadeInfo);
    }
    public void PlayHoldTween()
    {
        if (myWorldInfoIndex == 0)
            return;
        TweenMethod.PlayTweens(
            holdDeltaRadiusInfo,
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
            holdElapsedSecs01 = Mathf.Clamp01(Mathf.Clamp(holdElapsedSecs, 0f, PM.GetNoteWaitSecs(tarPlayerIndex, myNoteIndex + 1)) / PM.GetNoteWaitSecs(tarPlayerIndex, myNoteIndex + 1));
    }
    public void EndWaiting()
    {
        PM.worldInfoIndex = myWorldInfoIndex;

        TweenMethod.TryKillTweens(
            fadeInfo,
            waitDeltaRadiusInfo);
        PM.InitTweenAll();
        PM.PlayHoldTweenAll();
        PlayHoldTween();

        waitElapsedSecs = noteWaitSecs;
        toleranceSecsEndWait = waitElapsedSecs - noteWaitSecs;

        myElapsedTime.Reset();
        myElapsedTime.Stop();

        isNeedInput = true;
    }
    public void UpdateTransform()
    {
        transform.position = handy.GetCircularPos(tarPlayerScript.curDeg, waitDeltaRadius + tarPlayerScript.curRadius, PM.centerScript.pos);
        startNote.transform.localScale = tarPlayerScript.playerSide.transform.localScale;
        endNote.transform.localScale = tarPlayerScript.playerSide.transform.localScale;
        transform.rotation = Quaternion.Euler(0f, 0f, totalRotation);


        UpdatePathPoses();
        UpdateProcessPathPoses();

        processNoteDottedLine.poses = processPathPoses;
        processNoteDottedLine.SetRepeatCount(processPathPosesLength * 2.45f);

        if (pathPoses.Count > 0)
            endNote.transform.position = pathPoses[pathPoses.Count - 1];

        startNote.transform.rotation = Quaternion.Euler(0f, 0f, startRotation);
        endNote.transform.rotation = Quaternion.Euler(0f, 0f, endRotation);
    }
    public void UpdateRenderer()
    {
        startNoteRenderer.sprite = startNoteSprite;
        endNoteRenderer.sprite = endNoteSprite;
        startNoteRenderer.color = startColor;
        processNoteRenderer.startColor = processStartColor;
        processNoteRenderer.endColor = processEndColor;
        endNoteRenderer.color = endColor;
        ChangeNoteAlpha(fade);
    }
    void UpdatePathPoses()
    {
        pathPoses.Clear();
        pathPosesLength = 0f;
        for (int i = (int)(holdElapsedSecs01 * (float)posCount); i < posCount; i++)
        {
            float curProgress = (float)i / (float)posCount;
            Vector2 pathPos = handy.GetCircularPos(TweenMethod.GetTweenValue(PM.CorrectDegTween(worldInfo.playerInfo[tarPlayerIndex].degTween, worldInfo.playerInfo[tarPlayerIndex].degDir), curProgress),
            TweenMethod.GetTweenValue(worldInfo.playerInfo[tarPlayerIndex].radiusTween, curProgress)
            + TweenMethod.GetTweenValue(worldInfo.noteInfo.holdDeltaRadiusTween, curProgress));

            pathPoses.Add(GetNotePos(pathPos));
            pathPosesLength += Vector2.Distance(pathPoses[handy.GetBeforeIndex(pathPoses.Count - 1)], pathPoses[pathPoses.Count - 1]);
        }
    }
    void UpdateProcessPathPoses()
    {
        processPathPoses = new List<Vector3>(pathPoses);
        processPathPosesLength = pathPosesLength;
        float startRemoveLength = 0f;
        int startRemoveCount = 0;
        for (int i = 0; i < pathPoses.Count; i++)
        {
            startRemoveLength += Vector2.Distance(pathPoses[handy.GetBeforeIndex(i)], pathPoses[i]);
            if (startRemoveLength >= pathPosesLength * 0.2f)
            {
                startRemoveCount = i + 1;
                processPathPosesLength -= startRemoveLength;
                break;
            }
        }
        float endRemoveLength = 0f;
        int endRemoveCount = 0;
        for (int i = pathPoses.Count - 1; i >= 0; i--)
        {
            endRemoveLength += Vector2.Distance(pathPoses[handy.CorrectIndex(i + 1, pathPoses.Count - 1)], pathPoses[i]);
            if (endRemoveLength >= pathPosesLength * 0.2f)
            {
                endRemoveCount = pathPoses.Count - i;
                processPathPosesLength -= endRemoveLength;
                break;
            }
        }
        if (processPathPoses.Count >= startRemoveCount + endRemoveCount)
        {
            processPathPoses.RemoveRange(0, startRemoveCount);
            processPathPoses.RemoveRange(processPathPoses.Count - endRemoveCount, endRemoveCount);
        }
    }
    Vector2 GetNotePos(Vector2 pathPos)
    {
        return pathPos
        + handy.GetCircularPos(tarPlayerScript.curDeg, waitDeltaRadius)
        - handy.GetCircularPos(worldInfo.playerInfo[tarPlayerIndex].degTween.endValue, holdDeltaRadius)
        + PM.centerScript.pos;
    }
    public void StopNote()
    {
        if (holdNoteSecs != 0f)
        {
            float accuracy01_temp = 1f;
            if (!isInputted)
            {
                PM.judgmentGenScript.SetJudgmentText(tarPlayerIndex, JudgmentType.Miss);
                PM.CountMissNote();
                accuracy01_temp = 0f;
            }
            else
                PM.judgmentGenScript.SetJudgmentText(tarPlayerIndex, PM.GetJudgment(tarPlayerIndex, 1f - holdElapsedSecs01, () => { accuracy01_temp = holdElapsedSecs01; PM.CountMissNote(); }));

            PM.sumNoteAccuracy01 += accuracy01_temp;
            PM.InputCount++;
            PM.SetAccuracy01();
        }
        TweenMethod.TryKillTweens(
            holdDeltaRadiusInfo,
            totalRotationInfo,
            startRotationInfo,
            endRotationInfo,
            startColorInfo,
            processStartColorInfo,
            processEndColorInfo,
            endColorInfo);
        tarPlayerScript.SetSideClickScaleTweener(true);
        myElapsedTime.Stop();
        holdElapsedSecs = holdNoteSecs;
        DisableMe();
        PM.stopedNoteIndex = myWorldInfoIndex;
        isStop = true;
    }
    public void ChangeNoteAlpha(float alpha)
    {
        handy.FadeColor(startNoteRenderer, alpha);
        handy.FadeColor(processNoteRenderer, holdNoteSecs == 0f ? 0f : alpha);
        handy.FadeColor(endNoteRenderer, holdNoteSecs == 0f ? 0f : alpha);
    }
    void DisableMe()
    {
        PM.closestNoteIndex[tarPlayerIndex]++;
        gameObject.SetActive(false);
    }
    IEnumerator CheckHoldingKey()
    {
        if (holdNoteSecs != 0f)
        {
            while (PM.GetIsKeyPress(tarPlayerIndex) && !isDisable)
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