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

public class NotePrefab : MonoBehaviour, ITweenerInfo
{
    public bool isInputted;
    public int myWorldInfoIndex;
    public int myNoteIndex;
    public int tarPlayerIndex;
    public float holdElapsedSecs;
    public float waitElapsedSecs;
    public float waitElapsedSecs01;
    public float holdElapsedSecs01;
    public float noteWaitSecs;
    public float holdNoteSecs;
    public float toleranceSecsAwake;
    public float toleranceSecsEndWait;
    float stdDegDistance;
    float curDegDistance;
    float dottedLineLength;
    bool isAwake;
    public bool isDisable;
    public bool isStop;
    public bool isNeedInput;
    public bool isInput;
    public Sprite startNoteSprite;
    public Sprite endNoteSprite;
    GameObject startNote;
    GameObject processNote;
    GameObject endNote;
    GameObject tarPlayer;
    Player tarPlayerScript;
    GameObject nextNote;
    NotePrefab nextNoteScript;
    public List<Vector3> pathPoses;
    // List<Vector3> localPathPoses;
    List<Vector3> processPathPoses;

    public float waitRadius;
    public float holdRadius;
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
    public WorldInfo worldInfo;
    Handy handy;
    PlayManager PM;
    int posCount;
    void Awake()
    {
        PM = PlayManager.Property;
        myElapsedTime = new Stopwatch();
        myElapsedTime.Stop();
        isAwake = true;
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
        if (!isNeedInput && !isInput)
        {
            waitElapsedSecs = toleranceSecsAwake + myElapsedTime.ElapsedMilliseconds * 0.001f;
            if (waitElapsedSecs >= noteWaitSecs)
            {
                EndWaitingAsync();
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
                    EndWaitingAsync();
                }
                tarPlayerScript.StartCoroutine(CheckHoldingKey());
            }
        }
    }
    void LateUpdate()
    {
        if (myWorldInfoIndex == 0)
            return;
        UpdateNoteTransform();
        UpdateNotePartsTransform();
        UpdateNoteRenderer();
    }
    public void InitNote()
    {
        handy = Handy.Property;
        pathPoses = new List<Vector3>();
        // localPathPoses = new List<Vector3>();
        processPathPoses = new List<Vector3>();

        startNote = transform.GetChild(0).gameObject;
        processNote = transform.GetChild(1).gameObject;
        endNote = transform.GetChild(2).gameObject;
        startNoteRenderer = startNote.transform.GetChild(0).GetComponent<SpriteRenderer>();
        processNoteRenderer = processNote.GetComponent<LineRenderer>();
        processNoteDottedLine = processNote.GetComponent<DottedLine>();
        endNoteRenderer = endNote.transform.GetChild(0).GetComponent<SpriteRenderer>();

        worldInfo = PM.GetWorldInfo(myWorldInfoIndex);
        tarPlayer = PM.GetPlayer(tarPlayerIndex);
        tarPlayerScript = PM.GetPlayerScript(tarPlayerIndex);
        nextNote = PM.GetNote(tarPlayerIndex, myNoteIndex + 1);
        nextNoteScript = PM.GetNoteScript(tarPlayerIndex, myNoteIndex + 1);
        startNoteSprite = Resources.Load<Sprite>("Image/Play/Player/" + worldInfo.noteInfo.sideImageName);
        endNoteSprite = Resources.Load<Sprite>("Image/Play/Player/" + worldInfo.noteInfo.sideImageName);

        /* if (myWorldInfoIndex == 0)
        {
            InitTween();
            PlayWaitTween();
            PlayHoldTween();
        } */
        stdDegDistance = handy.GetDegDistance(worldInfo.playerInfo[tarPlayerIndex].degTween.endValue, worldInfo.playerInfo[tarPlayerIndex].degTween.startValue, false, worldInfo.playerInfo[tarPlayerIndex].degDir);
        posCount = 100;
        if (holdNoteSecs != 0f)
        {
            for (int i = 0; i <= posCount; i++)
            {
                float curProgress = (float)i / (float)posCount;

                float curDeg = TweenMethod.GetTweenValue(PM.CorrectDegTween(worldInfo.playerInfo[tarPlayerIndex].degTween, worldInfo.playerInfo[tarPlayerIndex].degDir), curProgress);
                float curRadius = TweenMethod.GetTweenValue(worldInfo.playerInfo[tarPlayerIndex].radiusTween, curProgress) + TweenMethod.GetTweenValue(worldInfo.noteInfo.holdRadiusTween, curProgress);
                pathPoses.Add(handy.GetCircularPos(curDeg, curRadius));
            }
        }
        else
        {
            pathPoses.Add(handy.GetCircularPos(worldInfo.playerInfo[tarPlayerIndex].degTween.startValue, worldInfo.playerInfo[tarPlayerIndex].radiusTween.startValue));
            pathPoses.Add(handy.GetCircularPos(worldInfo.playerInfo[tarPlayerIndex].degTween.endValue, worldInfo.playerInfo[tarPlayerIndex].radiusTween.endValue));
        }

        UpdateNoteTransform();
        UpdateNotePartsTransform();
        UpdateNoteRenderer();
        UpdateElapsedSecs01();
    }
    public void InitTween()
    {
        waitRadiusInfo = new TweeningInfo(worldInfo.noteInfo.waitRadiusTween, PM.GetNoteWaitSecs(myWorldInfoIndex));
        appearanceInfo = new TweeningInfo(worldInfo.noteInfo.appearanceTween, PM.GetNoteWaitSecs(myWorldInfoIndex) * 0.3f);

        holdRadiusInfo = new TweeningInfo(worldInfo.noteInfo.holdRadiusTween, PM.GetHoldNoteSecs(myWorldInfoIndex));
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
        waitRadius = ((TweenerInfo<float>)waitRadiusInfo).curValue;
        appearance = ((TweenerInfo<float>)appearanceInfo).curValue;

        holdRadius = ((TweenerInfo<float>)holdRadiusInfo).curValue;
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
            waitRadiusInfo,
            appearanceInfo);
    }
    public void PlayHoldTween()
    {
        handy.PlayTweens(
            holdRadiusInfo,
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
    public async void EndWaitingAsync()
    {
        PM.worldInfoIndex++;

        handy.TryKillTweens(
            appearanceInfo,
            waitRadiusInfo);

        PM.initTweenEvent();
        PM.playHoldTweenEvent();
        PlayHoldTween();

        isInput = true;

        if (noteWaitSecs > waitElapsedSecs)
            await Task.Delay((int)((noteWaitSecs - waitElapsedSecs) * 1000f));

        waitElapsedSecs = noteWaitSecs;
        toleranceSecsEndWait = waitElapsedSecs - noteWaitSecs;

        myElapsedTime.Reset();
        myElapsedTime.Stop();

        isNeedInput = true;
    }
    void UpdateNoteTransform()
    {
        transform.position = handy.GetCircularPos(tarPlayerScript.curDeg, waitRadius + (tarPlayerScript.curRadius - worldInfo.playerInfo[tarPlayerIndex].radiusTween.startValue), PM.centerScript.pos);
        startNote.transform.localScale = tarPlayerScript.playerSide.transform.localScale;
        endNote.transform.localScale = tarPlayerScript.playerSide.transform.localScale;
        transform.rotation = Quaternion.Euler(0f, 0f, totalRotation);
    }
    void UpdateNotePartsTransform()
    {
        // curDegDistance = handy.GetDegDistance(worldInfo.playerInfo[tarPlayerIndex].degTween.endValue, tarPlayerScript.curDeg, false, worldInfo.playerInfo[tarPlayerIndex].degDir);
        // int curPathIndex = (int)(holdElapsedSecs01 * (float)posCount);
        // localPathPoses.Clear();
        processPathPoses.Clear();
        dottedLineLength = 0f;
        // TweenInfo<float> correctedDegTween = PM.CorrectDegTween(worldInfo.playerInfo[tarPlayerIndex].degTween, worldInfo.playerInfo[tarPlayerIndex].degDir);
        for (int i = (int)(holdElapsedSecs01 * (float)posCount); i < posCount; i++)
        {
            // float degDir = worldInfo.playerInfo[tarPlayerIndex].degDir;

            // float curDegDistanceLoop = stdDegDistance - curDegDistance + (float)i;
            float curProgress = (float)i / (float)posCount;
            Vector2 pathPos = handy.GetCircularPos(TweenMethod.GetTweenValue(PM.CorrectDegTween(worldInfo.playerInfo[tarPlayerIndex].degTween, worldInfo.playerInfo[tarPlayerIndex].degDir), curProgress),
            TweenMethod.GetTweenValue(worldInfo.playerInfo[tarPlayerIndex].radiusTween, curProgress)
            + TweenMethod.GetTweenValue(worldInfo.noteInfo.holdRadiusTween, curProgress));

            processPathPoses.Add(GetNotePos(pathPos));
            dottedLineLength += Vector2.Distance(processPathPoses[handy.GetBeforeIndex(processPathPoses.Count - 1)], processPathPoses[processPathPoses.Count - 1]);
        }
        /* for (int i = curPathIndex; i < pathPoses.Count; i++)
        {
            processPathPoses.Add(GetNotePos(i));
            Sprite playerSprite = Resources.Load<Sprite>("Image/Play/Player/" + worldInfo.noteInfo.sideImageName);

            if (handy.CheckObjInOtherObj(pathPoses[i], Vector2.one * 0.2f, handy.GetCircularPos(tarPlayerScript.curDeg, tarPlayerScript.curRadius), tarPlayerScript.totalScale,
            Vector2.one * 100f, handy.GetSpritePixels(playerSprite))
            || handy.CheckObjInOtherObj(pathPoses[i], Vector2.one * 0.2f, handy.GetCircularPos(worldInfo.playerInfo[tarPlayerIndex].degTween.endValue, worldInfo.playerInfo[tarPlayerIndex].radiusTween.endValue), worldInfo.playerInfo[tarPlayerIndex].totalScaleTween.endValue,
            Vector2.one * 100f, handy.GetSpritePixels(playerSprite)))
            {
                processPathPoses.RemoveAt(processPathPoses.Count - 1);
            }
            else
            {
                dottedLineLength += Vector2.Distance(pathPoses[handy.GetBeforeIndex(i - curPathIndex)], pathPoses[i - curPathIndex]);
            }
        } */
        // startNote.transform.position = transform.position;
        processNoteDottedLine.poses = processPathPoses;
        processNoteDottedLine.SetRepeatCount(dottedLineLength * 2.444f);
        if (processPathPoses.Count > 0)
            endNote.transform.position = processPathPoses[processPathPoses.Count - 1];

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
    Vector3 GetNotePos(Vector3 pathPos)
    {
        return (Vector2)pathPos
        + handy.GetCircularPos(tarPlayerScript.curDeg, waitRadius - tarPlayerScript.curRadius)
        - handy.GetCircularPos(worldInfo.playerInfo[tarPlayerIndex].degTween.endValue, holdRadius)
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
            holdRadiusInfo,
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
