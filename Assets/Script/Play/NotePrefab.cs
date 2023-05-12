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

public class NotePrefab : MonoBehaviour, ITweenerInfo, IGameObject
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

    float dottedLineLength;

    bool isAwake;
    public bool isInputted;

    public bool isDisable;
    public bool isStop;
    public bool isNeedInput;
    // public bool isInput;

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

    public float waitDeltaRadius;
    public float holdDeltaRadius;
    public float totalRotation;
    public float startRotation;
    public float endRotation;
    public float appearance;
    public Color startColor;
    public Color processStartColor;
    public Color processEndColor;
    public Color endColor;

    public TweeningInfo waitDeltaRadiusInfo;
    public TweeningInfo holdDeltaRadiusInfo;
    public TweeningInfo totalRotationInfo;
    public TweeningInfo startRotationInfo;
    public TweeningInfo endRotationInfo;
    public TweeningInfo appearanceInfo;
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
            InitTween();
            PlayWaitTween();
            myElapsedTime.Reset();
            myElapsedTime.Stop();
            isAwake = false;
        }
        UpdateTweenValue();
        UpdateElapsedSecs01();
        myElapsedTime.Start();
        if (!isNeedInput/*  && !isInput */)
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
            appearance = 0f;
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
    void LateUpdate()
    {
        if (myWorldInfoIndex == 0)
            return;
        UpdateTransform();
        UpdateRenderer();
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
    public void InitTween()
    {
        waitDeltaRadiusInfo = new TweeningInfo(worldInfo.noteInfo.waitDeltaRadiusTween, PM.GetNoteWaitSecs(myWorldInfoIndex));
        appearanceInfo = new TweeningInfo(worldInfo.noteInfo.appearanceTween, PM.GetNoteWaitSecs(myWorldInfoIndex) * 0.3f);

        holdDeltaRadiusInfo = new TweeningInfo(worldInfo.noteInfo.holdDeltaRadiusTween, PM.GetHoldNoteSecs(myWorldInfoIndex));
        totalRotationInfo = new TweeningInfo(worldInfo.noteInfo.totalRotationTween, PM.GetHoldNoteSecs(myWorldInfoIndex));
        startRotationInfo = new TweeningInfo(worldInfo.noteInfo.startRotationTween, PM.GetHoldNoteSecs(myWorldInfoIndex));
        endRotationInfo = new TweeningInfo(worldInfo.noteInfo.endRotationTween, PM.GetHoldNoteSecs(myWorldInfoIndex));
        startColorInfo = new TweeningInfo(worldInfo.noteInfo.startColorTween, PM.GetHoldNoteSecs(myWorldInfoIndex));
        processStartColorInfo = new TweeningInfo(worldInfo.noteInfo.processStartColorTween, PM.GetHoldNoteSecs(myWorldInfoIndex));
        processEndColorInfo = new TweeningInfo(worldInfo.noteInfo.processEndColorTween, PM.GetHoldNoteSecs(myWorldInfoIndex));
        endColorInfo = new TweeningInfo(worldInfo.noteInfo.endColorTween, PM.GetHoldNoteSecs(myWorldInfoIndex));
    }
    public void UpdateTweenValue()
    {
        waitDeltaRadius = ((TweenerInfo<float>)waitDeltaRadiusInfo).curValue;
        appearance = ((TweenerInfo<float>)appearanceInfo).curValue;

        holdDeltaRadius = ((TweenerInfo<float>)holdDeltaRadiusInfo).curValue;
        totalRotation = handy.GetCorrectDegMaxIs0(-(((TweenerInfo<float>)totalRotationInfo).curValue));
        startRotation = handy.GetCorrectDegMaxIs0(-(((TweenerInfo<float>)startRotationInfo).curValue + tarPlayerScript.curDeg));
        endRotation = handy.GetCorrectDegMaxIs0(-(((TweenerInfo<float>)endRotationInfo).curValue + worldInfo.playerInfo[tarPlayerIndex].degTween.endValue));
        startColor = PM.GetColor01WithPlayerIndex(((TweenerInfo<Color>)startColorInfo).curValue, tarPlayerIndex);
        processStartColor = PM.GetColor01WithPlayerIndex(((TweenerInfo<Color>)processStartColorInfo).curValue, tarPlayerIndex);
        processEndColor = PM.GetColor01WithPlayerIndex(((TweenerInfo<Color>)processEndColorInfo).curValue, tarPlayerIndex);
        endColor = PM.GetColor01WithPlayerIndex(((TweenerInfo<Color>)endColorInfo).curValue, tarPlayerIndex);
    }
    public void PlayWaitTween()
    {
        handy.PlayTweens(
            waitDeltaRadiusInfo,
            appearanceInfo);
    }
    public void PlayHoldTween()
    {
        handy.PlayTweens(
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
    public /* async */ void EndWaiting()
    {
        PM.worldInfoIndex++;

        handy.TryKillTweens(
            appearanceInfo,
            waitDeltaRadiusInfo);

        PM.initTweenEvent();
        PM.playHoldTweenEvent();
        PlayHoldTween();

        // isInput = true;

        /* if (noteWaitSecs > waitElapsedSecs)
            await Task.Delay((int)((noteWaitSecs - waitElapsedSecs) * 1000f)); */

        waitElapsedSecs = noteWaitSecs;
        toleranceSecsEndWait = waitElapsedSecs - noteWaitSecs;

        myElapsedTime.Reset();
        myElapsedTime.Stop();

        isNeedInput = true;
    }
    /* void UpdateNoteTransform()
    {
        transform.position = handy.GetCircularPos(tarPlayerScript.curDeg, waitRadius + (tarPlayerScript.curRadius - worldInfo.playerInfo[tarPlayerIndex].radiusTween.startValue), PM.centerScript.pos);
        startNote.transform.localScale = tarPlayerScript.playerSide.transform.localScale;
        endNote.transform.localScale = tarPlayerScript.playerSide.transform.localScale;
        transform.rotation = Quaternion.Euler(0f, 0f, totalRotation);
    }
    void UpdateNotePartsTransform()
    {
        pathPoses.Clear();
        dottedLineLength = 0f;
        for (int i = (int)(holdElapsedSecs01 * (float)posCount); i < posCount; i++)
        {
            float curProgress = (float)i / (float)posCount;
            Vector2 pathPos = handy.GetCircularPos(TweenMethod.GetTweenValue(PM.CorrectDegTween(worldInfo.playerInfo[tarPlayerIndex].degTween, worldInfo.playerInfo[tarPlayerIndex].degDir), curProgress),
            TweenMethod.GetTweenValue(worldInfo.playerInfo[tarPlayerIndex].radiusTween, curProgress)
            + TweenMethod.GetTweenValue(worldInfo.noteInfo.holdRadiusTween, curProgress));

            pathPoses.Add(GetNotePos(pathPos));
            dottedLineLength += Vector2.Distance(pathPoses[handy.GetBeforeIndex(pathPoses.Count - 1)], pathPoses[pathPoses.Count - 1]);
        }
        processNoteDottedLine.poses = pathPoses;
        processNoteDottedLine.SetRepeatCount(dottedLineLength * 2.444f);
        if (pathPoses.Count > 0)
            endNote.transform.position = pathPoses[pathPoses.Count - 1];

        startNote.transform.rotation = Quaternion.Euler(0f, 0f, startRotation);
        endNote.transform.rotation = Quaternion.Euler(0f, 0f, endRotation);
    } */
    public void UpdateTransform()
    {
        transform.position = handy.GetCircularPos(tarPlayerScript.curDeg, waitDeltaRadius + tarPlayerScript.curRadius, PM.centerScript.pos);
        startNote.transform.localScale = tarPlayerScript.playerSide.transform.localScale;
        endNote.transform.localScale = tarPlayerScript.playerSide.transform.localScale;
        transform.rotation = Quaternion.Euler(0f, 0f, totalRotation);


        UpdatePathPoses();

        processNoteDottedLine.poses = pathPoses;
        processNoteDottedLine.SetRepeatCount(dottedLineLength * 2.444f);

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
        ChangeNoteAlpha(appearance);
    }
    void UpdatePathPoses()
    {
        pathPoses.Clear();
        dottedLineLength = 0f;
        for (int i = (int)(holdElapsedSecs01 * (float)posCount); i < posCount; i++)
        {
            float curProgress = (float)i / (float)posCount;
            Vector2 pathPos = handy.GetCircularPos(TweenMethod.GetTweenValue(PM.CorrectDegTween(worldInfo.playerInfo[tarPlayerIndex].degTween, worldInfo.playerInfo[tarPlayerIndex].degDir), curProgress),
            TweenMethod.GetTweenValue(worldInfo.playerInfo[tarPlayerIndex].radiusTween, curProgress)
            + TweenMethod.GetTweenValue(worldInfo.noteInfo.holdDeltaRadiusTween, curProgress));

            pathPoses.Add(GetNotePos(pathPos));
            dottedLineLength += Vector2.Distance(pathPoses[handy.GetBeforeIndex(pathPoses.Count - 1)], pathPoses[pathPoses.Count - 1]);
            /* if (!handy.CheckColliding(pathPos, Vector2.one * 0.2f, Vector2.one * 100f, transform.position, transform.localScale, handy.GetSpritePixels(startNoteSprite))
            && !handy.CheckColliding(pathPos, Vector2.one * 0.2f, Vector2.one * 100f, handy.GetCircularPos(worldInfo.playerInfo[tarPlayerIndex].degTween.endValue, worldInfo.playerInfo[tarPlayerIndex].radiusTween.endValue), worldInfo.playerInfo[tarPlayerIndex].sideScaleTween.endValue, handy.GetSpritePixels(endNoteSprite)))
            {
            } */
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
        handy.TryKillTweens(
            holdDeltaRadiusInfo,
            totalRotationInfo,
            startRotationInfo,
            endRotationInfo,
            startColorInfo,
            processStartColorInfo,
            processEndColorInfo,
            endColorInfo);
        handy.StartCoroutine(tarPlayerScript.SetSideClickScaleTweener(true));
        myElapsedTime.Stop();
        holdElapsedSecs = holdNoteSecs;
        DisableMe();
        isStop = true;
    }
    public void ChangeNoteAlpha(float alpha)
    {
        handy.ChangeAlpha(startNoteRenderer, alpha);
        handy.ChangeAlpha(processNoteRenderer, holdNoteSecs == 0f ? 0f : alpha);
        handy.ChangeAlpha(endNoteRenderer, holdNoteSecs == 0f ? 0f : alpha);
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
