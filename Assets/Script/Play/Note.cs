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
    bool isInitNoteTween;
    bool[] isJudgNotes;

    public bool isDisable;
    public bool isStop;
    public bool isNeedInput;

    [SerializeField]
    GameObject noteUnit;
    public Sprite[] notesSprite;
    GameObject processNote;
    GameObject[] notes;
    LineRenderer processNoteRenderer;
    DottedLine processNoteDottedLine;
    SpriteRenderer[] notesRend;
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
    public TweeningInfo[] notesRotationInfo;
    public TweeningInfo fadeInfo;
    public TweeningInfo processStartColorInfo;
    public TweeningInfo processEndColorInfo;
    public TweeningInfo[] notesColorInfo;

    public WorldInfo worldInfo;
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
        for (int i = 1; i < worldInfo.noteInfo.noteCount - 1; i++)
        {
            if (!isJudgNotes[i])
            {
                if (worldInfo.noteInfo.noteHitTiming01s[i] <= holdElapsedSecs01)
                {
                    PM.judgmentGenScript.SetJudgmentText(tarPlayerIndex, JudgmentType.Perfect);
                    PM.sumNoteAccuracy01 += 1;
                    PM.InputCount++;
                    PM.SetAccuracy01();
                    isJudgNotes[i] = true;
                }
            }
        }
    }
    public void InitNote()
    {
        worldInfo = PM.GetWorldInfo(myWorldInfoIndex);
        tarPlayer = PM.GetPlayer(tarPlayerIndex);
        tarPlayerScript = PM.GetPlayerScript(tarPlayerIndex);
        isJudgNotes = new bool[worldInfo.noteInfo.noteCount];
        InitNoteChild();
        Handy.ProcessCode.RepeatCodeMethod.RepeatCode((i) => { notesSprite[i] = Resources.Load<Sprite>("Image/Play/Player/" + worldInfo.noteInfo.sideImageName); }, notesSprite.Length);
    }
    public void InitNoteChild()
    {
        notes = new GameObject[worldInfo.noteInfo.noteCount];
        notesRend = new SpriteRenderer[worldInfo.noteInfo.noteCount];
        notesSprite = new Sprite[worldInfo.noteInfo.noteCount];
        notesRotation = new float[worldInfo.noteInfo.noteCount];
        notesColor = new Color[worldInfo.noteInfo.noteCount];
        notesRotationInfo = new TweeningInfo[worldInfo.noteInfo.noteCount];
        notesColorInfo = new TweeningInfo[worldInfo.noteInfo.noteCount];
        processNoteRenderer.sortingOrder = PM.GetNoteScript(myWorldInfoIndex - 1).processNoteRenderer.sortingOrder + PM.GetWorldInfo(myWorldInfoIndex - 1).noteInfo.noteCount + (int)Mathf.Clamp01(myWorldInfoIndex - 1);

        for (int i = 0; i < worldInfo.noteInfo.noteCount; i++)
        {
            notes[i] = Instantiate(noteUnit, transform);
            notesRend[i] = notes[i].transform.GetChild(0).GetComponent<SpriteRenderer>();
            notesRend[i].sortingOrder = processNoteRenderer.sortingOrder + i + 1;
        }
    }
    public void InitNoteTween()
    {
        if (myWorldInfoIndex == 0 || isInitNoteTween)
            return;

        TryKillWaitTweens();
        TryKillHoldTweens();

        waitDeltaRadiusInfo = new TweeningInfo(worldInfo.noteInfo.waitDeltaRadiusTween, PM.GetNoteWaitSecs(myWorldInfoIndex));
        fadeInfo = new TweeningInfo(worldInfo.noteInfo.fadeTween, PM.GetNoteWaitSecs(myWorldInfoIndex) * 0.3f);

        holdDeltaRadiusInfo = new TweeningInfo(worldInfo.noteInfo.holdDeltaRadiusTween, PM.GetNoteHoldSecs(myWorldInfoIndex));
        Handy.ProcessCode.RepeatCodeMethod.RepeatCode((i) => { notesRotationInfo[i] = new TweeningInfo(worldInfo.noteInfo.rotationTweens[i], PM.GetNoteHoldSecs(myWorldInfoIndex)); }, notesRotationInfo.Length);
        processStartColorInfo = new TweeningInfo(worldInfo.noteInfo.processStartColorTween, PM.GetNoteHoldSecs(myWorldInfoIndex));
        processEndColorInfo = new TweeningInfo(worldInfo.noteInfo.processEndColorTween, PM.GetNoteHoldSecs(myWorldInfoIndex));
        Handy.ProcessCode.RepeatCodeMethod.RepeatCode((i) => { notesColorInfo[i] = new TweeningInfo(worldInfo.noteInfo.colorTweens[i], PM.GetNoteHoldSecs(myWorldInfoIndex)); }, notesColorInfo.Length);
        isInitNoteTween = true;
    }
    public void UpdateNoteTweenValue()
    {
        if (myWorldInfoIndex == 0)
            return;
        waitDeltaRadius = ((TweenerInfo<float>)waitDeltaRadiusInfo).curValue;
        fade = isDisable ? 0f : ((TweenerInfo<float>)fadeInfo).curValue;

        holdDeltaRadius = ((TweenerInfo<float>)holdDeltaRadiusInfo).curValue;

        Handy.ProcessCode.RepeatCodeMethod.RepeatCode((i) =>
        {
            float curProgress = Mathf.Clamp((float)i / (float)(notesRotation.Length - 1), holdElapsedSecs01, 1f);
            notesRotation[i] = Handy.Math.DegMethod.CorrectDegMaxIs0(-(((TweenerInfo<float>)notesRotationInfo[i]).curValue + TweenMethod.GetTweenValue(PM.CorrectDegTween(worldInfo.playerInfo[tarPlayerIndex].degTween, worldInfo.playerInfo[tarPlayerIndex].degDir), curProgress)));
        }, notesRotation.Length);

        processStartColor = PM.GetColor01WithPlayerIndex(((TweenerInfo<Color>)processStartColorInfo).curValue, tarPlayerIndex);
        processEndColor = PM.GetColor01WithPlayerIndex(((TweenerInfo<Color>)processEndColorInfo).curValue, tarPlayerIndex);
        Handy.ProcessCode.RepeatCodeMethod.RepeatCode((i) => { notesColor[i] = PM.GetColor01WithPlayerIndex(((TweenerInfo<Color>)notesColorInfo[i]).curValue, tarPlayerIndex); }, notesColor.Length);


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
            processStartColorInfo,
            processEndColorInfo);
        Handy.ProcessCode.RepeatCodeMethod.RepeatCode((i) => TweenMethod.PlayTween(notesRotationInfo[i]), Handy.ArrayMethod.TryGetLength(notesRotationInfo));
        Handy.ProcessCode.RepeatCodeMethod.RepeatCode((i) => TweenMethod.PlayTween(notesColorInfo[i]), Handy.ArrayMethod.TryGetLength(notesColorInfo));
        // foreach (var NRI in notesRotationInfo)
        //     TweenMethod.PlayTweens(NRI);
        // foreach (var NCI in notesColorInfo)
        //     TweenMethod.PlayTweens(NCI);
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
            isInitNoteTween = false;
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
        UpdatePathPoses();
        UpdateProcessPathPoses();
        Handy.LineRendMethod.SetDottedLine(processNoteDottedLine, processPathPoses, processPathPosesLength);
        Handy.ProcessCode.RepeatCodeMethod.RepeatCode((i) =>
        {
            float curProgress = Mathf.Clamp(worldInfo.noteInfo.noteHitTiming01s[i], holdElapsedSecs01, 1f);
            notes[i].transform.position = holdElapsedSecs01 == curProgress ? Handy.Transform.PosMethod.GetCircularPos(tarPlayerScript.curDeg, waitDeltaRadius + tarPlayerScript.curRadius, PM.centerScript.pos) : pathPoses[(int)((float)(pathPoses.Count - 1) * curProgress)];
        },
        notes.Length);
        Handy.ProcessCode.RepeatCodeMethod.RepeatCode((i) => notes[i].transform.localScale = tarPlayerScript.playerSide.transform.localScale, notes.Length);
        Handy.ProcessCode.RepeatCodeMethod.RepeatCode((i) => notes[i].transform.localRotation = Quaternion.Euler(0f, 0f, notesRotation[i]), notes.Length);

        // Handy.ProcessCode.RepeatCodeMethod.RepeatCode((i) => DevTool.Member.AddDottedLinePos(Handy.ReflectionMethod.GetPredicateName(Handy.ArrayMethod.GetParams<string>(this.name, nameof(notes) + i.ToString()), myWorldInfoIndex), notes[i].transform.position, new DevTool.StdColors()[i]), notes.Length);
    }
    public void UpdateRenderer()
    {
        Handy.ProcessCode.RepeatCodeMethod.RepeatCode((i) => notesRend[i].sprite = notesSprite[i], notesRend.Length);

        processNoteRenderer.startColor = processStartColor;
        processNoteRenderer.endColor = processEndColor;
        Handy.ProcessCode.RepeatCodeMethod.RepeatCode((i) => notesRend[i].color = notesColor[i], notesRend.Length);

        ChangeNoteAlpha(fade);
    }
    void UpdatePathPoses()
    {
        pathPoses.Clear();
        for (int i = 0; i < PM.notePathPosesCount; i++)
        {
            float curProgress = (float)i / (float)(PM.notePathPosesCount - 1);
            Vector2 pathPos = Handy.Transform.PosMethod.GetCircularPos(TweenMethod.GetTweenValue(PM.CorrectDegTween(worldInfo.playerInfo[tarPlayerIndex].degTween, worldInfo.playerInfo[tarPlayerIndex].degDir), curProgress),
            TweenMethod.GetTweenValue(worldInfo.playerInfo[tarPlayerIndex].radiusTween, curProgress)
            + TweenMethod.GetTweenValue(worldInfo.noteInfo.holdDeltaRadiusTween, curProgress) - holdDeltaRadius);

            pathPoses.Add(pathPos + Handy.Transform.PosMethod.GetCircularPos(tarPlayerScript.curDeg, waitDeltaRadius));
        }
        pathPosesLength = Handy.Math.VectorMethod.GetDistance(pathPoses);
    }
    void UpdateProcessPathPoses()
    {
        float stdDegDistance = Handy.Math.DegMethod.GetDegDistance(
        TweenMethod.GetTweenValue(PM.CorrectDegTween(worldInfo.playerInfo[tarPlayerIndex].degTween, worldInfo.playerInfo[tarPlayerIndex].degDir), 1f),
        TweenMethod.GetTweenValue(PM.CorrectDegTween(worldInfo.playerInfo[tarPlayerIndex].degTween, worldInfo.playerInfo[tarPlayerIndex].degDir), 0f),
        false,
        worldInfo.playerInfo[tarPlayerIndex].degDir);
        float stdRadiusDistance = Mathf.Abs(TweenMethod.GetTweenValue(worldInfo.noteInfo.holdDeltaRadiusTween, 1f) - TweenMethod.GetTweenValue(worldInfo.noteInfo.holdDeltaRadiusTween, 0f));

        processPathPoses.Clear();
        for (int i = (int)(holdElapsedSecs01 * (float)(PM.notePathPosesCount - 1)); i < PM.notePathPosesCount; i++)
        {
            float curProgress = (float)i / (float)(PM.notePathPosesCount - 1);
            Vector2 pathPos = Handy.Transform.PosMethod.GetCircularPos(TweenMethod.GetTweenValue(PM.CorrectDegTween(worldInfo.playerInfo[tarPlayerIndex].degTween, worldInfo.playerInfo[tarPlayerIndex].degDir), curProgress),
            TweenMethod.GetTweenValue(worldInfo.playerInfo[tarPlayerIndex].radiusTween, curProgress)
            + TweenMethod.GetTweenValue(worldInfo.noteInfo.holdDeltaRadiusTween, curProgress) - holdDeltaRadius);

            processPathPoses.Add(pathPos + Handy.Transform.PosMethod.GetCircularPos(tarPlayerScript.curDeg, waitDeltaRadius));
        }
        processPathPosesLength = Handy.Math.VectorMethod.GetDistance(processPathPoses);

        float removeLength = processPathPosesLength * 0.2f / (stdDegDistance / 90f) / stdRadiusDistance;

        float startRemoveLength = 0f;
        int startRemoveCount = 0;
        for (int i = 0; i < processPathPoses.Count; i++)
        {
            startRemoveLength += Vector2.Distance(processPathPoses[Handy.IndexMethod.GetBeforeIndex(i)], processPathPoses[i]);
            if (startRemoveLength >= removeLength)
            {
                startRemoveCount = i + 1;
                break;
            }
        }
        float endRemoveLength = 0f;
        int endRemoveCount = 0;
        for (int i = processPathPoses.Count - 1; i >= 0; i--)
        {
            endRemoveLength += Vector2.Distance(processPathPoses[Handy.IndexMethod.CorrectIndex(i + 1, processPathPoses.Count - 1)], processPathPoses[i]);
            if (endRemoveLength >= removeLength)
            {
                endRemoveCount = processPathPoses.Count - i;
                break;
            }
        }
        processPathPosesLength -= startRemoveLength + endRemoveLength;
        if (processPathPoses.Count >= startRemoveCount + endRemoveCount)
        {
            processPathPoses.RemoveRange(0, startRemoveCount);
            processPathPoses.RemoveRange(processPathPoses.Count - endRemoveCount, endRemoveCount);
        }
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
            processStartColorInfo,
            processEndColorInfo);
        Handy.ProcessCode.RepeatCodeMethod.RepeatCode((i) => TweenMethod.TryKillTween(notesRotationInfo[i]), Handy.ArrayMethod.TryGetLength(notesRotationInfo));
        Handy.ProcessCode.RepeatCodeMethod.RepeatCode((i) => TweenMethod.TryKillTween(notesColorInfo[i]), Handy.ArrayMethod.TryGetLength(notesColorInfo));
        // foreach (var NRI in notesRotationInfo)
        //     TweenMethod.TryKillTween(NRI);
        // foreach (var NCI in notesColorInfo)
        //     TweenMethod.TryKillTween(NCI);
    }
    public void ChangeNoteAlpha(float alpha)
    {
        Handy.Renderer.ColorMethod.FadeColor(processNoteRenderer, alpha);
        Handy.ProcessCode.RepeatCodeMethod.RepeatCode((i) => Handy.Renderer.ColorMethod.FadeColor(notesRend[i], alpha), notesRend.Length);
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