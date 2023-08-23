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
using UnityEditor;

public class Note : MonoBehaviour, PlayManager.ITweenerInPlay, IGameObject
{
    public int myLevelInfoIndex;
    public int myEachNoteIndex;
    public int tarPlayerIndex;

    float myElapsedSecs;

    public float holdElapsedSecs;
    public float waitElapsedSecs;
    public float waitElapsedSecs01;
    public float holdElapsedSecs01;

    public float noteWaitSecs;
    public float holdNoteSecs;
    public float toleranceSecsAwake;

    float pathPosesLength;
    float processPathPosesLength;

    bool isAwake;
    public bool isHitted;
    public bool[] isJudgNotes;

    // public bool isDisableAble;
    public bool isDisable;
    public bool isStop;
    public bool isNeedInput;

    [SerializeField]
    GameObject noteUnit;
    public Sprite[] notesSideSprite;
    GameObject processNote;
    GameObject[] notes;
    LineRenderer processNoteRenderer;
    DottedLine processNoteDottedLine;
    SpriteRenderer[] notesSideRend;
    SpriteRenderer[] notesCenterRend;
    GameObject tarPlayer;
    Player tarPlayerScript;
    List<Vector2> pathPoses;
    List<Vector2> processPathPoses;

    public float waitDeltaRadius;
    public float holdDeltaRadius;
    public float[] notesRotation;
    public float fade;
    public Color processStartColor;
    public Color processEndColor;
    public Color[] notesColor;

    public TweeningInfo waitDeltaRadiusInfo;
    public TweeningInfo holdDeltaRadiusInfo;
    public TweeningInfo[] notesRotationInfos;
    public TweeningInfo fadeInfo;
    public TweeningInfo processStartColorInfo;
    public TweeningInfo processEndColorInfo;
    public TweeningInfo[] notesColorInfos;

    public LevelInfo levelInfo;
    PlayManager PM;
    void Awake()
    {
        PM = PlayManager.Member;
        pathPoses = new List<Vector2>();
        processPathPoses = new List<Vector2>();
        processNote = transform.GetChild(0).gameObject;
        processNoteRenderer = processNote.GetComponent<LineRenderer>();
        processNoteDottedLine = processNote.GetComponent<DottedLine>();
    }
    Stopwatch stopwatch = new Stopwatch();
    void Update()
    {
        stopwatch.Start();
        curve.AddKey(stopwatch.ElapsedMilliseconds * 0.001f, PM.GetJudgmentValue(this));


        if (PM.isStop || PM.isPause || myLevelInfoIndex == 0)
            return;
        if (isAwake)
        {
            PM.AddGO(this).AddTweenerInPlayGO(this);
            InitNoteTween();
            PM.PlayWaitTweenAll();
            isAwake = false;
        }
        myElapsedSecs += Time.deltaTime;
        if (!isNeedInput)
        {
            waitElapsedSecs = Mathf.Clamp(toleranceSecsAwake + myElapsedSecs, 0f, noteWaitSecs);
            if (waitElapsedSecs >= noteWaitSecs || (PM.GetJudgmentValue(this) <= PM.judgmentRange[tarPlayerIndex] && PM.keyControl.Down.GetIsInput(tarPlayerIndex)))
            {
                StopWaiting();
            }
        }
        if (isNeedInput)
        {
            holdElapsedSecs = Mathf.Clamp(myElapsedSecs, 0f, holdNoteSecs * (levelInfo.noteInfo.noteHitTiming01s[levelInfo.noteInfo.noteCount - 1] + PM.judgmentRange[tarPlayerIndex]));
            if (!isStop)
            {
                if (holdElapsedSecs >= holdNoteSecs)
                    isDisable = true;
                // if (holdElapsedSecs >= holdNoteSecs * (1f - PM.judgmentRange[tarPlayerIndex]))
                //     isDisableAble = true;
            }
            // if (holdNoteSecs != 0f)
            // {
            //     if (PM.keyControl.Up.GetIsInput(tarPlayerIndex) && !isDisableAble)
            //     {
            //         Handy.WriteLog(PM.keyControl.Up.GetIsInput(tarPlayerIndex));
            //         StopNote();
            //     }
            // }
        }
        UpdateElapsedSecs01();
        if (isDisable)
        {
            if (!isStop)
            {
                if (holdElapsedSecs01 > (holdNoteSecs != 0f ? levelInfo.noteInfo.noteHitTiming01s[levelInfo.noteInfo.noteCount - 1] + PM.judgmentRange[tarPlayerIndex] : PM.judgmentRange[tarPlayerIndex]))
                    StopNote();
            }
        }
        (int Down, int Press, int Up) keyCounts = (PM.keyControl.Down.Count, PM.keyControl.Press.Count, PM.keyControl.Up.Count);
        Handy.WriteLog(nameof(PM.keyControl.Up), PM.keyControl.Up.Count);
        for (int i = 0; i < levelInfo.noteInfo.noteCount; i++)
        {
            if (!isJudgNotes[i])
            {
                // float averageHitTiming01 = i > 0 ? (levelInfo.noteInfo.noteHitTiming01s[i] + levelInfo.noteInfo.noteHitTiming01s[i - 1]) * 0.5f : 0f;
                if (holdElapsedSecs01 >= levelInfo.noteInfo.noteHitTiming01s[i])
                {
                    if (PM.GetJudgmentValue(this) <= PM.judgmentRange[tarPlayerIndex])
                    {
                        // Handy.WriteLog(nameof(keyCounts.Up), keyCounts.Up);
                        // Handy.WriteLog("Can Hit Note", keyControlTemp.Down.Count, keyControlTemp.Press.Count, keyControlTemp.Up.Count);
                        // Handy.WriteLog("Can Hit Note", i, PM.GetJudgmentValue(this));
                        if (PM.GetIsHitNote(this, i, keyCounts, (KCs) => keyCounts = KCs))
                        {
                            float noteAccuracy01 = 1f;
                            PM.judgmentGenScript.SetJudgmentText(tarPlayerIndex, PM.GetJudgment(tarPlayerIndex, PM.GetJudgmentValue(this), () => { noteAccuracy01 = Mathf.Clamp01(1f - PM.GetJudgmentValue(this)); PM.CountMissNote(); }));
                            PM.sumNoteAccuracy01 += noteAccuracy01;
                            PM.SetAccuracy01();
                            isJudgNotes[i] = true;
                            Handy.WriteLog("Hit Note", i, PM.GetJudgmentValue(this));
                        }
                    }
                    else if (holdElapsedSecs01 > levelInfo.noteInfo.noteHitTiming01s[i])
                    {
                        // Handy.WriteLog("Miss Note   " + i + "   " + PM.GetJudgmentValue(this));
                        PM.judgmentGenScript.SetJudgmentText(tarPlayerIndex, JudgmentType.Miss);
                        PM.CountMissNote();
                        PM.InputCount++;
                        PM.SetAccuracy01();
                        isJudgNotes[i] = true;
                    }
                }
            }
        }
        UpdateNoteTweenValue();
    }
    public void InitNote()
    {
        levelInfo = PM.GetLevelInfo(myLevelInfoIndex);
        tarPlayer = PM.GetPlayer(tarPlayerIndex);
        tarPlayerScript = PM.GetPlayerScript(tarPlayerIndex);
        isJudgNotes = new bool[levelInfo.noteInfo.noteCount];
        InitNoteChild();
        Handy.RepeatCode((i) => { notesSideSprite[i] = Resources.Load<Sprite>("Image/Play/Player/" + levelInfo.noteInfo.sideImageName); }, levelInfo.noteInfo.noteCount);
    }
    public void InitNoteChild()
    {
        notes = new GameObject[levelInfo.noteInfo.noteCount];
        notesSideRend = new SpriteRenderer[levelInfo.noteInfo.noteCount];
        notesCenterRend = new SpriteRenderer[levelInfo.noteInfo.noteCount];
        notesSideSprite = new Sprite[levelInfo.noteInfo.noteCount];
        notesRotation = new float[levelInfo.noteInfo.noteCount];
        notesColor = new Color[levelInfo.noteInfo.noteCount];
        notesRotationInfos = new TweeningInfo[levelInfo.noteInfo.noteCount];
        notesColorInfos = new TweeningInfo[levelInfo.noteInfo.noteCount];
        processNoteRenderer.sortingOrder = PM.GetNoteScript(myLevelInfoIndex - 1).processNoteRenderer.sortingOrder + PM.GetLevelInfo(myLevelInfoIndex - 1).noteInfo.noteCount * 2 + (int)Mathf.Clamp01(myLevelInfoIndex - 1);

        for (int i = 0; i < levelInfo.noteInfo.noteCount; i++)
        {
            notes[i] = Instantiate(noteUnit, transform);
            notesSideRend[i] = notes[i].transform.GetChild(0).GetComponent<SpriteRenderer>();
            notesCenterRend[i] = notes[i].transform.GetChild(1).GetComponent<SpriteRenderer>();
            notesSideRend[i].sortingOrder = processNoteRenderer.sortingOrder + i * 2 + 2;
            notesCenterRend[i].sortingOrder = processNoteRenderer.sortingOrder + i * 2 + 1;
        }
    }
    public TweeningInfo[] GetNoteTweens()
    {
        List<TweeningInfo> tweeningInfos = new List<TweeningInfo>(Handy.GetArray(
            waitDeltaRadiusInfo,
            fadeInfo,
            holdDeltaRadiusInfo,
            processStartColorInfo,
            processEndColorInfo));
        foreach (var NRI in notesRotationInfos)
        {
            tweeningInfos.Add(NRI);
        }
        foreach (var NCI in notesColorInfos)
        {
            tweeningInfos.Add(NCI);
        }
        return tweeningInfos.ToArray();
    }
    public void InitNoteTween()
    {
        if (myLevelInfoIndex == 0)
            return;

        TryKillWaitTweens();
        TryKillHoldTweens();

        waitDeltaRadiusInfo = new TweeningInfo(levelInfo.noteInfo.waitDeltaRadiusTween, PM.GetNoteWaitSecs(myLevelInfoIndex));
        fadeInfo = new TweeningInfo(levelInfo.noteInfo.fadeTween, PM.GetNoteWaitSecs(myLevelInfoIndex) * 0.3f);

        holdDeltaRadiusInfo = new TweeningInfo(levelInfo.noteInfo.holdDeltaRadiusTween, PM.GetNoteHoldSecs(myLevelInfoIndex));
        Handy.RepeatCode((i) => { notesRotationInfos[i] = new TweeningInfo(levelInfo.noteInfo.rotationTweens[i], PM.GetNoteHoldSecs(myLevelInfoIndex)); }, levelInfo.noteInfo.noteCount);
        processStartColorInfo = new TweeningInfo(levelInfo.noteInfo.processStartColorTween, PM.GetNoteHoldSecs(myLevelInfoIndex));
        processEndColorInfo = new TweeningInfo(levelInfo.noteInfo.processEndColorTween, PM.GetNoteHoldSecs(myLevelInfoIndex));
        Handy.RepeatCode((i) => { notesColorInfos[i] = new TweeningInfo(levelInfo.noteInfo.colorTweens[i], PM.GetNoteHoldSecs(myLevelInfoIndex)); }, levelInfo.noteInfo.noteCount);
    }
    public void UpdateNoteTweenValue()
    {
        if (myLevelInfoIndex == 0)
            return;
        waitDeltaRadius = ((TweenerInfo<float>)waitDeltaRadiusInfo).curValue;
        fade = isStop ? 0f : ((TweenerInfo<float>)fadeInfo).curValue;

        holdDeltaRadius = ((TweenerInfo<float>)holdDeltaRadiusInfo).curValue;

        Handy.RepeatCode((i) =>
        {
            float curProgress = Mathf.Clamp(levelInfo.noteInfo.noteHitTiming01s[i], holdElapsedSecs01, float.MaxValue);
            notesRotation[i] = Handy.GetCorrectedDegMaxIs0(-(((TweenerInfo<float>)notesRotationInfos[i]).curValue + TweenMethod.GetTweenValue(PM.CorrectDegTween(levelInfo.playerInfo[tarPlayerIndex].degTween, levelInfo.playerInfo[tarPlayerIndex].degDir), curProgress)));
        }, levelInfo.noteInfo.noteCount);

        processStartColor = PM.GetColor01WithPlayerIndex(((TweenerInfo<Color>)processStartColorInfo).curValue, tarPlayerIndex);
        processEndColor = PM.GetColor01WithPlayerIndex(((TweenerInfo<Color>)processEndColorInfo).curValue, tarPlayerIndex);
        Handy.RepeatCode((i) => { notesColor[i] = PM.GetColor01WithPlayerIndex(((TweenerInfo<Color>)notesColorInfos[i]).curValue, tarPlayerIndex); }, levelInfo.noteInfo.noteCount);
    }
    public void PlayWaitTween()
    {
        TweenMethod.TryPlayTween(
            waitDeltaRadiusInfo,
            fadeInfo);
    }
    public void PlayHoldTween()
    {
        TweenMethod.TryPlayTween(
            holdDeltaRadiusInfo,
            processStartColorInfo,
            processEndColorInfo);
        Handy.RepeatCode((i) => TweenMethod.TryPlayTween(notesRotationInfos[i]), Handy.TryGetArrayLength(notesRotationInfos));
        Handy.RepeatCode((i) => TweenMethod.TryPlayTween(notesColorInfos[i]), Handy.TryGetArrayLength(notesColorInfos));
    }
    public void UpdateElapsedSecs01()
    {
        if (noteWaitSecs != 0f)
            waitElapsedSecs01 = waitElapsedSecs / noteWaitSecs;
        else
            waitElapsedSecs01 = 0f;

        if (holdNoteSecs != 0f)
            holdElapsedSecs01 = holdElapsedSecs / holdNoteSecs;
        else
            holdElapsedSecs01 = holdElapsedSecs / PM.GetNoteWaitSecs(tarPlayerIndex, myEachNoteIndex + 1);
    }
    public void StopWaiting()
    {
        PM.levelInfoIndex = myLevelInfoIndex;

        TryKillWaitTweens();
        PM.TryKillTweenAll();
        PM.InitTweenAll();
        PM.PlayHoldTweenAll();

        myElapsedSecs -= noteWaitSecs;
        waitElapsedSecs = noteWaitSecs;

        if (levelInfo.noteInfo.isCheckPoint)
        {
            PM.checkPointIndex = myLevelInfoIndex;
            PM.checkPointAccuracy01 = PM.accuracy01;
        }

        isNeedInput = true;
    }
    public void InitNoteScript()
    {
        if (myLevelInfoIndex > PM.levelInfoIndex)
        {

            waitElapsedSecs = 0f;
            holdElapsedSecs = 0f;
            waitElapsedSecs01 = 0f;
            holdElapsedSecs01 = 0f;

            toleranceSecsAwake = 0f;

            isAwake = true;
            isHitted = false;
            // isDisableAble = false;
            isDisable = false;
            isStop = false;
            isNeedInput = false;

            myElapsedSecs = 0f;

            fade = 0f;
        }
    }
    public void EndNoteScript()
    {

        waitElapsedSecs = PM.GetNoteWaitSecs(myLevelInfoIndex);
        holdElapsedSecs = PM.GetNoteHoldSecs(myLevelInfoIndex);
        waitElapsedSecs01 = 1f;
        holdElapsedSecs01 = 1f;

        toleranceSecsAwake = 0f;

        isAwake = false;
        isHitted = false;
        // isDisableAble = true;
        isDisable = true;
        isStop = true;
        isNeedInput = true;

        myElapsedSecs = holdNoteSecs;
    }
    public void UpdateTransform()
    {
        UpdatePathPoses();
        UpdateProcessPathPoses();
        Handy.SetDottedLine(processNoteDottedLine, processPathPoses, processPathPosesLength);
        Handy.RepeatCode((i) =>
        {
            float curProgress = Mathf.Clamp(levelInfo.noteInfo.noteHitTiming01s[i], holdElapsedSecs01, float.MaxValue);
            notes[i].transform.position = holdElapsedSecs01 == curProgress ? Handy.GetCircularPos(tarPlayerScript.curDeg, waitDeltaRadius + tarPlayerScript.curRadius, PM.centerScript.pos) : pathPoses[(int)((float)(pathPoses.Count - 1) * curProgress)];
        },
        levelInfo.noteInfo.noteCount);
        Handy.RepeatCode((i) => notes[i].transform.localScale = tarPlayerScript.playerSide.transform.localScale, levelInfo.noteInfo.noteCount);
        Handy.RepeatCode((i) => notes[i].transform.localRotation = Quaternion.Euler(0f, 0f, notesRotation[i]), levelInfo.noteInfo.noteCount);
    }
    public void UpdateRenderer()
    {
        Handy.RepeatCode((i) => notesSideRend[i].sprite = notesSideSprite[i], levelInfo.noteInfo.noteCount);

        processNoteRenderer.startColor = processStartColor;
        processNoteRenderer.endColor = processEndColor;
        Handy.RepeatCode((i) => { notesSideRend[i].color = notesColor[i]; notesCenterRend[i].color = notesColor[i]; }, levelInfo.noteInfo.noteCount);

        ChangeNoteAlpha(fade);
    }
    void UpdatePathPoses()
    {
        pathPoses.Clear();
        for (int i = 0; i < PM.notePathPosesCount; i++)
        {
            float curProgress = (float)i / (float)(PM.notePathPosesCount - 1);
            Vector2 pathPos = Handy.GetCircularPos(TweenMethod.GetTweenValue(PM.CorrectDegTween(levelInfo.playerInfo[tarPlayerIndex].degTween, levelInfo.playerInfo[tarPlayerIndex].degDir), curProgress),
            TweenMethod.GetTweenValue(levelInfo.playerInfo[tarPlayerIndex].radiusTween, curProgress)
            + TweenMethod.GetTweenValue(levelInfo.noteInfo.holdDeltaRadiusTween, curProgress) - holdDeltaRadius);

            pathPoses.Add(pathPos + Handy.GetCircularPos(tarPlayerScript.curDeg, waitDeltaRadius));
        }
        pathPosesLength = Handy.GetPosesDistance(pathPoses);
    }
    void UpdateProcessPathPoses()
    {
        float stdDegDistance = Handy.GetDegDistance(
        TweenMethod.GetTweenValue(PM.CorrectDegTween(levelInfo.playerInfo[tarPlayerIndex].degTween, levelInfo.playerInfo[tarPlayerIndex].degDir), 1f),
        TweenMethod.GetTweenValue(PM.CorrectDegTween(levelInfo.playerInfo[tarPlayerIndex].degTween, levelInfo.playerInfo[tarPlayerIndex].degDir), 0f),
        false,
        levelInfo.playerInfo[tarPlayerIndex].degDir);
        float stdRadiusDistance = Mathf.Abs(TweenMethod.GetTweenValue(levelInfo.noteInfo.holdDeltaRadiusTween, 1f) - TweenMethod.GetTweenValue(levelInfo.noteInfo.holdDeltaRadiusTween, 0f));

        processPathPoses.Clear();
        for (int i = (int)(holdElapsedSecs01 * (float)(PM.notePathPosesCount - 1)); i < PM.notePathPosesCount; i++)
        {
            float curProgress = (float)i / (float)(PM.notePathPosesCount - 1);
            Vector2 pathPos = Handy.GetCircularPos(TweenMethod.GetTweenValue(PM.CorrectDegTween(levelInfo.playerInfo[tarPlayerIndex].degTween, levelInfo.playerInfo[tarPlayerIndex].degDir), curProgress),
            TweenMethod.GetTweenValue(levelInfo.playerInfo[tarPlayerIndex].radiusTween, curProgress)
            + TweenMethod.GetTweenValue(levelInfo.noteInfo.holdDeltaRadiusTween, curProgress) - holdDeltaRadius);

            processPathPoses.Add(pathPos + Handy.GetCircularPos(tarPlayerScript.curDeg, waitDeltaRadius));
        }
        processPathPosesLength = Handy.GetPosesDistance(processPathPoses);
    }
    public void StopNote()
    {
        TryKillHoldTweens();
        tarPlayerScript.SetSideSubScaleTweener(true);
        holdElapsedSecs = holdNoteSecs;
        DisableMe();
        PM.stopedNoteIndex = myLevelInfoIndex;
        isStop = true;
    }
    public void TryKillWaitTweens()
    {
        TweenMethod.TryKillTween(
            fadeInfo,
            waitDeltaRadiusInfo);
    }
    public void TryKillHoldTweens()
    {
        TweenMethod.TryKillTween(
            holdDeltaRadiusInfo,
            processStartColorInfo,
            processEndColorInfo);
        Handy.RepeatCode((i) => TweenMethod.TryKillTween(notesRotationInfos[i]), Handy.TryGetArrayLength(notesRotationInfos));
        Handy.RepeatCode((i) => TweenMethod.TryKillTween(notesColorInfos[i]), Handy.TryGetArrayLength(notesColorInfos));
    }
    public void ChangeNoteAlpha(float alpha)
    {
        Handy.FadeColor(processNoteRenderer, alpha);
        Handy.RepeatCode((i) => { Handy.FadeColor(notesSideRend[i], alpha); Handy.FadeColor(notesCenterRend[i], alpha); }, levelInfo.noteInfo.noteCount);
    }
    void DisableMe()
    {
        PM.closestNoteIndex[tarPlayerIndex]++;
        gameObject.SetActive(false);
    }
    public void Active()
    {
        if (myLevelInfoIndex != 0)
        {
            gameObject.SetActive(true);
            toleranceSecsAwake = PM.totalElapsedSecs - levelInfo.noteInfo.awakeSecs;
        }
    }
    public T PlayByNoteIndex<T>(int noteIndex, Func<T> onStartNote, Func<T> onMiddleNote, Func<T> onEndNote)
    {
        /* if (!isJudgNotes[0])
        {
            // Handy.WriteLog(nameof(onStartNote));
            return onStartNote();
        }
        if (levelInfo.noteInfo.noteCount > 2)
        {
            for (int i = 1; i < levelInfo.noteInfo.noteCount - 1; i++)
            {
                if (!isJudgNotes[i])
                {
                    // Handy.WriteLog(nameof(onMiddleNote));
                    return onMiddleNote();
                }
            }
        }
        if (levelInfo.noteInfo.noteCount > 1)
        {
            if (!isJudgNotes[levelInfo.noteInfo.noteCount - 1])
            {
                // Handy.WriteLog(nameof(onEndNote));
                return onEndNote();
            }
        } */
        // if (!isJudgNotes[noteIndex])
        // {
        if (noteIndex <= 0)
            return onStartNote();
        if (noteIndex > 0 && noteIndex < levelInfo.noteInfo.noteCount - 1)
            return onMiddleNote();
        if (noteIndex >= levelInfo.noteInfo.noteCount - 1)
            return onEndNote();
        // }
        return default;
    }


    AnimationCurve curve = new AnimationCurve();
    void OnGUI()
    {
        EditorGUI.CurveField(new Rect(Vector2.zero, new Vector2(384f, 216f)), curve);
    }
}