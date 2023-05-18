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

public class Note : MonoBehaviour, PlayManager.ITweenerInPlay, IGameObject
{
    public int myWorldInfoIndex;
    public int myNoteIndex;
    public int tarPlayerIndex;

    float myElapsedSecs;

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
    Note nextNoteScript;
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
    PlayManager PM;
    Coroutine checkHoldingKeyCo;
    void Awake()
    {
        PM = PlayManager.Property;
        pathPoses = new List<Vector3>();

        startNote = transform.GetChild(0).gameObject;
        processNote = transform.GetChild(1).gameObject;
        endNote = transform.GetChild(2).gameObject;
        startNoteRenderer = startNote.transform.GetChild(0).GetComponent<SpriteRenderer>();
        processNoteRenderer = processNote.GetComponent<LineRenderer>();
        processNoteDottedLine = processNote.GetComponent<DottedLine>();
        endNoteRenderer = endNote.transform.GetChild(0).GetComponent<SpriteRenderer>();
    }
    void OnEnable()
    {
        InitGameObjectScript();
    }
    void Update()
    {
        if (PM.isStop || PM.isPause || myWorldInfoIndex == 0)
            return;
        if (isAwake)
        {
            PM.AddGO(this).AddTweenerInPlayGO(this);
            InitNoteTween();
            PM.PlayWaitTweenAll();
            isAwake = false;
        }
        UpdateNoteTweenValue();
        UpdateElapsedSecs01();
        myElapsedSecs += Time.deltaTime;
        if (!isNeedInput)
        {
            waitElapsedSecs = toleranceSecsAwake + myElapsedSecs;
            if (waitElapsedSecs >= noteWaitSecs)
            {
                EndWaiting();
            }
        }
        if (isNeedInput && !isStop)
        {
            holdElapsedSecs = toleranceSecsEndWait + myElapsedSecs;
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
                StartCheckHoldingKeyCo();
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
    }
    public void InitNoteTween()
    {
        if (myWorldInfoIndex == 0)
            return;
        TryKillWaitTweens();
        TryKillHoldTweens();
        waitDeltaRadiusInfo = new TweeningInfo(worldInfo.noteInfo.waitDeltaRadiusTween, PM.GetNoteWaitSecs(myWorldInfoIndex));
        fadeInfo = new TweeningInfo(worldInfo.noteInfo.fadeTween, PM.GetNoteWaitSecs(myWorldInfoIndex) * 0.3f);

        holdDeltaRadiusInfo = new TweeningInfo(worldInfo.noteInfo.holdDeltaRadiusTween, PM.GetNoteHoldSecs(myWorldInfoIndex));
        totalRotationInfo = new TweeningInfo(worldInfo.noteInfo.totalRotationTween, PM.GetNoteHoldSecs(myWorldInfoIndex));
        startRotationInfo = new TweeningInfo(worldInfo.noteInfo.startRotationTween, PM.GetNoteHoldSecs(myWorldInfoIndex));
        endRotationInfo = new TweeningInfo(worldInfo.noteInfo.endRotationTween, PM.GetNoteHoldSecs(myWorldInfoIndex));
        startColorInfo = new TweeningInfo(worldInfo.noteInfo.startColorTween, PM.GetNoteHoldSecs(myWorldInfoIndex));
        processStartColorInfo = new TweeningInfo(worldInfo.noteInfo.processStartColorTween, PM.GetNoteHoldSecs(myWorldInfoIndex));
        processEndColorInfo = new TweeningInfo(worldInfo.noteInfo.processEndColorTween, PM.GetNoteHoldSecs(myWorldInfoIndex));
        endColorInfo = new TweeningInfo(worldInfo.noteInfo.endColorTween, PM.GetNoteHoldSecs(myWorldInfoIndex));
    }
    public void UpdateNoteTweenValue()
    {
        if (myWorldInfoIndex == 0)
            return;
        waitDeltaRadius = ((TweenerInfo<float>)waitDeltaRadiusInfo).curValue;
        fade = isDisable ? 0f : ((TweenerInfo<float>)fadeInfo).curValue;

        holdDeltaRadius = ((TweenerInfo<float>)holdDeltaRadiusInfo).curValue;
        totalRotation = Handy.Math.DegMethod.CorrectDegMaxIs0(-(((TweenerInfo<float>)totalRotationInfo).curValue/*  + tarPlayerScript.curDeg */));
        startRotation = Handy.Math.DegMethod.CorrectDegMaxIs0(-(((TweenerInfo<float>)startRotationInfo).curValue + tarPlayerScript.curDeg));
        endRotation = Handy.Math.DegMethod.CorrectDegMaxIs0(-(((TweenerInfo<float>)endRotationInfo).curValue + worldInfo.playerInfo[tarPlayerIndex].degTween.endValue));
        startColor = PM.GetColor01WithPlayerIndex(((TweenerInfo<Color>)startColorInfo).curValue, tarPlayerIndex);
        processStartColor = PM.GetColor01WithPlayerIndex(((TweenerInfo<Color>)processStartColorInfo).curValue, tarPlayerIndex);
        processEndColor = PM.GetColor01WithPlayerIndex(((TweenerInfo<Color>)processEndColorInfo).curValue, tarPlayerIndex);
        endColor = PM.GetColor01WithPlayerIndex(((TweenerInfo<Color>)endColorInfo).curValue, tarPlayerIndex);
    }
    public void PlayWaitTween()
    {
        TweenMethod.PlayTweens(
            waitDeltaRadiusInfo,
            fadeInfo);
    }
    public void PlayHoldTween()
    {
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

        TryKillWaitTweens();
        PM.TryKillTweenAll();
        PM.InitTweenAll();
        PM.PlayHoldTweenAll();

        waitElapsedSecs = noteWaitSecs;
        toleranceSecsEndWait = waitElapsedSecs - noteWaitSecs;
        myElapsedSecs = 0f;

        if (worldInfo.noteInfo.isCheckPoint)
        {
            PM.checkPointIndex = myWorldInfoIndex;
            PM.checkPointAccuracy01 = PM.accuracy01;
        }

        isNeedInput = true;
    }
    public void InitGameObjectScript()
    {
        if (myWorldInfoIndex > PM.worldInfoIndex)
        {
            waitElapsedSecs = 0f;
            holdElapsedSecs = 0f;
            waitElapsedSecs01 = 0f;
            holdElapsedSecs01 = 0f;

            toleranceSecsAwake = 0f;
            toleranceSecsEndWait = 0f;

            isAwake = true;
            isInputted = false;
            isDisable = false;
            isStop = false;
            isNeedInput = false;

            myElapsedSecs = 0f;
        }
    }
    public void EndNoteScript()
    {
        waitElapsedSecs = PM.GetNoteWaitSecs(myWorldInfoIndex);
        holdElapsedSecs = PM.GetNoteHoldSecs(myWorldInfoIndex);
        waitElapsedSecs01 = 1f;
        holdElapsedSecs01 = 1f;

        toleranceSecsAwake = 0f;
        toleranceSecsEndWait = 0f;

        isAwake = false;
        isInputted = false;
        isDisable = true;
        isStop = true;
        isNeedInput = true;

        myElapsedSecs = holdElapsedSecs;

        PM.AddGO(this).AddTweenerInPlayGO(this);
    }
    public void UpdateTransform()
    {
        transform.position = Handy.Transform.PosMethod.GetCircularPos(tarPlayerScript.curDeg, waitDeltaRadius + tarPlayerScript.curRadius, PM.centerScript.pos);
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
        for (int i = (int)(holdElapsedSecs01 * (float)PM.notePathPosesCount); i < PM.notePathPosesCount; i++)
        {
            float curProgress = (float)i / (float)PM.notePathPosesCount;
            Vector2 pathPos = Handy.Transform.PosMethod.GetCircularPos(TweenMethod.GetTweenValue(PM.CorrectDegTween(worldInfo.playerInfo[tarPlayerIndex].degTween, worldInfo.playerInfo[tarPlayerIndex].degDir), curProgress),
            TweenMethod.GetTweenValue(worldInfo.playerInfo[tarPlayerIndex].radiusTween, curProgress)
            + TweenMethod.GetTweenValue(worldInfo.noteInfo.holdDeltaRadiusTween, curProgress));

            pathPoses.Add(GetNotePos(pathPos));
            pathPosesLength += Vector2.Distance(pathPoses[Handy.IndexMethod.GetBeforeIndex(pathPoses.Count - 1)], pathPoses[pathPoses.Count - 1]);
        }
    }
    void UpdateProcessPathPoses()
    {
        float stdDegDistance = Handy.Math.DegMethod.GetDegDistance(
        TweenMethod.GetTweenValue(worldInfo.playerInfo[tarPlayerIndex].degTween, 1f),
        TweenMethod.GetTweenValue(worldInfo.playerInfo[tarPlayerIndex].degTween, 0f),
        false,
        worldInfo.playerInfo[tarPlayerIndex].degDir);
        float stdRadiusDistance = Mathf.Abs(TweenMethod.GetTweenValue(worldInfo.noteInfo.holdDeltaRadiusTween, 1f) - TweenMethod.GetTweenValue(worldInfo.noteInfo.holdDeltaRadiusTween, 0f));
        float removeLength = pathPosesLength * 0.2f / (stdDegDistance / 90f) / stdRadiusDistance;

        processPathPoses = new List<Vector3>(pathPoses);
        processPathPosesLength = pathPosesLength;
        float startRemoveLength = 0f;
        int startRemoveCount = 0;
        for (int i = 0; i < pathPoses.Count; i++)
        {
            startRemoveLength += Vector2.Distance(pathPoses[Handy.IndexMethod.GetBeforeIndex(i)], pathPoses[i]);
            if (startRemoveLength >= removeLength)
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
            endRemoveLength += Vector2.Distance(pathPoses[Handy.IndexMethod.CorrectIndex(i + 1, pathPoses.Count - 1)], pathPoses[i]);
            if (endRemoveLength >= removeLength)
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
        + Handy.Transform.PosMethod.GetCircularPos(tarPlayerScript.curDeg, waitDeltaRadius - holdDeltaRadius)
        // - Handy.Transform.PosMethod.GetCircularPos(worldInfo.playerInfo[tarPlayerIndex].degTween.endValue, holdDeltaRadius)
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
        TryKillHoldTweens();
        tarPlayerScript.SetSideClickScaleTweener(true);
        holdElapsedSecs = holdNoteSecs;
        DisableMe();
        PM.stopedNoteIndex = myWorldInfoIndex;
        isStop = true;
    }
    public void TryKillWaitTweens()
    {
        TweenMethod.TryKillTweens(
            fadeInfo,
            waitDeltaRadiusInfo);
    }
    public void TryKillHoldTweens()
    {
        TweenMethod.TryKillTweens(
            holdDeltaRadiusInfo,
            totalRotationInfo,
            startRotationInfo,
            endRotationInfo,
            startColorInfo,
            processStartColorInfo,
            processEndColorInfo,
            endColorInfo);
    }
    public void ChangeNoteAlpha(float alpha)
    {
        Handy.Renderer.ColorMethod.FadeColor(startNoteRenderer, alpha);
        Handy.Renderer.ColorMethod.FadeColor(processNoteRenderer, holdNoteSecs == 0f ? 0f : alpha);
        Handy.Renderer.ColorMethod.FadeColor(endNoteRenderer, holdNoteSecs == 0f ? 0f : alpha);
    }
    void DisableMe()
    {
        PM.closestNoteIndex[tarPlayerIndex]++;
        gameObject.SetActive(false);
    }
    void StartCheckHoldingKeyCo()
    {
        checkHoldingKeyCo = PM.StartCoroutine(CheckHoldingKeyCo());
    }
    public void TryStopCheckHoldingKeyCo()
    {
        if (checkHoldingKeyCo != null)
            PM.StopCoroutine(checkHoldingKeyCo);
    }
    IEnumerator CheckHoldingKeyCo()
    {
        if (holdNoteSecs != 0f)
        {
            while (PM.GetIsKeyPress(tarPlayerIndex) && !isDisable)
            {
                yield return null;
                if (PM.isPause)
                    yield return new WaitUntil(() => !PM.isPause);
            }
            if (!isDisable)
            {
                StopNote();
            }
        }
    }
    public void Active()
    {
        if (myWorldInfoIndex != 0)
        {
            gameObject.SetActive(true);
            toleranceSecsAwake = PM.totalElapsedSecs - worldInfo.noteInfo.awakeSecs;
        }
    }
}