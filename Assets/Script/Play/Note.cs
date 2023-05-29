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
    public float toleranceSecsEndWait;

    float pathPosesLength;
    float processPathPosesLength;

    bool isAwake;
    public bool isInputted;
    bool[] isJudgNotes;

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
    Coroutine checkHoldingKeyCo;
    void Awake()
    {
        PM = PlayManager.Member;
        pathPoses = new List<Vector2>();
        processPathPoses = new List<Vector2>();
        processNote = transform.GetChild(0).gameObject;
        processNoteRenderer = processNote.GetComponent<LineRenderer>();
        processNoteDottedLine = processNote.GetComponent<DottedLine>();
    }
    void Update()
    {
        if (PM.isStop || PM.isPause || myLevelInfoIndex == 0)
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
                StopWaiting();
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
                    StopWaiting();
                }
                StartCheckHoldingKeyCo();
            }
        }
        for (int i = 1; i < levelInfo.noteInfo.noteCount - 1; i++)
        {
            if (!isJudgNotes[i])
            {
                if (levelInfo.noteInfo.noteHitTiming01s[i] <= holdElapsedSecs01)
                {
                    if (isInputted && PM.GetIsKeyPress(tarPlayerIndex))
                    {
                        PM.judgmentGenScript.SetJudgmentText(tarPlayerIndex, JudgmentType.Perfect);
                        PM.sumNoteAccuracy01 += 1;
                        PM.InputCount++;
                        PM.SetAccuracy01();
                    }
                    isJudgNotes[i] = true;
                }
            }
        }
    }
    public void InitNote()
    {
        levelInfo = PM.GetLevelInfo(myLevelInfoIndex);
        tarPlayer = PM.GetPlayer(tarPlayerIndex);
        tarPlayerScript = PM.GetPlayerScript(tarPlayerIndex);
        isJudgNotes = new bool[levelInfo.noteInfo.noteCount];
        InitNoteChild();
        Handy.RepeatCode((i) => { notesSideSprite[i] = Resources.Load<Sprite>("Image/Play/Player/" + levelInfo.noteInfo.sideImageName); }, notesSideSprite.Length);
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
        Handy.RepeatCode((i) => { notesRotationInfos[i] = new TweeningInfo(levelInfo.noteInfo.rotationTweens[i], PM.GetNoteHoldSecs(myLevelInfoIndex)); }, notesRotationInfos.Length);
        processStartColorInfo = new TweeningInfo(levelInfo.noteInfo.processStartColorTween, PM.GetNoteHoldSecs(myLevelInfoIndex));
        processEndColorInfo = new TweeningInfo(levelInfo.noteInfo.processEndColorTween, PM.GetNoteHoldSecs(myLevelInfoIndex));
        Handy.RepeatCode((i) => { notesColorInfos[i] = new TweeningInfo(levelInfo.noteInfo.colorTweens[i], PM.GetNoteHoldSecs(myLevelInfoIndex)); }, notesColorInfos.Length);
    }
    public void UpdateNoteTweenValue()
    {
        if (myLevelInfoIndex == 0)
            return;
        waitDeltaRadius = ((TweenerInfo<float>)waitDeltaRadiusInfo).curValue;
        fade = isDisable ? 0f : ((TweenerInfo<float>)fadeInfo).curValue;

        holdDeltaRadius = ((TweenerInfo<float>)holdDeltaRadiusInfo).curValue;

        Handy.RepeatCode((i) =>
        {
            float curProgress = Mathf.Clamp((float)i / (float)(notesRotation.Length - 1), holdElapsedSecs01, 1f);
            notesRotation[i] = Handy.GetCorrectedDegMaxIs0(-(((TweenerInfo<float>)notesRotationInfos[i]).curValue + TweenMethod.GetTweenValue(PM.CorrectDegTween(levelInfo.playerInfo[tarPlayerIndex].degTween, levelInfo.playerInfo[tarPlayerIndex].degDir), curProgress)));
        }, notesRotation.Length);

        processStartColor = PM.GetColor01WithPlayerIndex(((TweenerInfo<Color>)processStartColorInfo).curValue, tarPlayerIndex);
        processEndColor = PM.GetColor01WithPlayerIndex(((TweenerInfo<Color>)processEndColorInfo).curValue, tarPlayerIndex);
        Handy.RepeatCode((i) => { notesColor[i] = PM.GetColor01WithPlayerIndex(((TweenerInfo<Color>)notesColorInfos[i]).curValue, tarPlayerIndex); }, notesColor.Length);
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
            waitElapsedSecs01 = Mathf.Clamp01(Mathf.Clamp(waitElapsedSecs, 0f, noteWaitSecs) / noteWaitSecs);
        else
            waitElapsedSecs01 = 0f;

        if (holdNoteSecs != 0f)
            holdElapsedSecs01 = Mathf.Clamp01(Mathf.Clamp(holdElapsedSecs, 0f, holdNoteSecs) / holdNoteSecs);
        else
            holdElapsedSecs01 = Mathf.Clamp01(Mathf.Clamp(holdElapsedSecs, 0f, PM.GetNoteWaitSecs(tarPlayerIndex, myEachNoteIndex + 1)) / PM.GetNoteWaitSecs(tarPlayerIndex, myEachNoteIndex + 1));
    }
    public void StopWaiting()
    {
        PM.levelInfoIndex = myLevelInfoIndex;

        TryKillWaitTweens();
        PM.TryKillTweenAll();
        PM.InitTweenAll();
        PM.PlayHoldTweenAll();

        myElapsedSecs -= noteWaitSecs;
        toleranceSecsEndWait = waitElapsedSecs - noteWaitSecs;
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
        waitElapsedSecs = PM.GetNoteWaitSecs(myLevelInfoIndex);
        holdElapsedSecs = PM.GetNoteHoldSecs(myLevelInfoIndex);
        waitElapsedSecs01 = 1f;
        holdElapsedSecs01 = 1f;

        toleranceSecsAwake = 0f;
        toleranceSecsEndWait = 0f;

        isAwake = false;
        isInputted = false;
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
            float curProgress = Mathf.Clamp(levelInfo.noteInfo.noteHitTiming01s[i], holdElapsedSecs01, 1f);
            notes[i].transform.position = holdElapsedSecs01 == curProgress ? Handy.GetCircularPos(tarPlayerScript.curDeg, waitDeltaRadius + tarPlayerScript.curRadius, PM.centerScript.pos) : pathPoses[(int)((float)(pathPoses.Count - 1) * curProgress)];
        },
        notes.Length);
        Handy.RepeatCode((i) => notes[i].transform.localScale = tarPlayerScript.playerSide.transform.localScale, notes.Length);
        Handy.RepeatCode((i) => notes[i].transform.localRotation = Quaternion.Euler(0f, 0f, notesRotation[i]), notes.Length);
    }
    public void UpdateRenderer()
    {
        Handy.RepeatCode((i) => notesSideRend[i].sprite = notesSideSprite[i], notesSideRend.Length);

        processNoteRenderer.startColor = processStartColor;
        processNoteRenderer.endColor = processEndColor;
        Handy.RepeatCode((i) => { notesSideRend[i].color = notesColor[i]; notesCenterRend[i].color = notesColor[i]; }, notesSideRend.Length);

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
        Handy.GetColor(processNoteRenderer, alpha);
        Handy.RepeatCode((i) => Handy.GetColor(notesSideRend[i], alpha), notesSideRend.Length);
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
        if (myLevelInfoIndex != 0)
        {
            gameObject.SetActive(true);
            toleranceSecsAwake = PM.totalElapsedSecs - levelInfo.noteInfo.awakeSecs;
        }
    }
}