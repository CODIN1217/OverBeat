using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System.Diagnostics;
using System;
using System.Linq;
using System.Text;
using Debug = UnityEngine.Debug;
using TweenManager;

public class PlayManager : MonoBehaviour
{
    public GameObject countDown;
    public CountDown countDownScript;
    public GameObject noteGenerator;
    public NoteGenerator noteGeneratorScript;
    public GameObject worldReader;
    public WorldReader worldReaderScript;
    public GameObject playerController;
    public PlayerController playerControllerScript;
    public GameObject judgmentGen;
    public JudgmentGen judgmentGenScript;
    public GameObject boundary;
    public Boundary boundaryScript;
    public GameObject baseCamera;
    public BaseCamera baseCameraScript;
    public GameObject center;
    public Center centerScript;
    public GameObject endMessage;
    public EndMessage endMessageScript;
    public GameObject accuracy;
    public Accuracy accuracyScript;

    [SerializeField]
    GameObject PauseController;

    public GameObject[] closestNotes;
    public Note[] closestNoteScripts;
    public float[] judgmentRange;
    public float[] bestJudgmentRange;
    public int InputCount;
    public int missCount;
    public KeyControl keyControl;
    // public int keyDownCount;
    // public int keyPressCount;
    // public int keyUpCount;
    // List<List<bool>> isKeyDowns;
    // List<List<bool>> isKeyPresses;
    // List<List<bool>> isKeyUps;
    public bool isStop;
    public bool isPause;
    public bool isGameOver;
    public bool isClearWorld;
    public bool isAutoPlay;
    public bool isShowAccuracy;
    bool isBeforeAwake;
    bool isBeforeEnable;
    static PlayManager instance = null;
    // public List<List<KeyCode>> canInputKeys;
    public float sumNoteAccuracy01;
    public float progress01;
    public float accuracy01;
    public float checkPointAccuracy01;
    public float HP01;
    public int MaxMissCount;
    public readonly int MaxHPCount;
    public int notePathPosesCount;
    public int[] closestNoteIndex;
    public int stopedNoteIndex;
    public int levelInfoIndex;
    public int totalNoteCount;
    public int checkPointIndex;
    public int tryCount;
    public float totalElapsedSecs;
    List<ITweener> tweeners;
    List<ITweenerInPlay> tweenerInPlayGOs;
    List<IGameObject> GOs;
    List<IScript> scripts;
    public PlayManager AddTweener(ITweener tweener) { tweeners.Add(tweener); return this; }
    public PlayManager AddTweenerInPlayGO(ITweenerInPlay tweenerInPlayGO) { tweenerInPlayGOs.Add(tweenerInPlayGO); return this; }
    public PlayManager AddGO(IGameObject GO) { GOs.Add(GO); return this; }
    public PlayManager AddScript(IScript script) { scripts.Add(script); return this; }
    public interface ITweenerInPlay
    {
        void PlayWaitTween();
        void PlayHoldTween();
    }
    void Awake()
    {
        countDownScript = countDown.GetComponent<CountDown>();
        noteGeneratorScript = noteGenerator.GetComponent<NoteGenerator>();
        worldReaderScript = worldReader.GetComponent<WorldReader>();
        playerControllerScript = playerController.GetComponent<PlayerController>();
        judgmentGenScript = judgmentGen.GetComponent<JudgmentGen>();
        boundaryScript = boundary.GetComponent<Boundary>();
        baseCameraScript = baseCamera.GetComponent<BaseCamera>();
        centerScript = center.GetComponent<Center>();
        endMessageScript = endMessage.GetComponent<EndMessage>();
        accuracyScript = accuracy.GetComponent<Accuracy>();

        instance = this;
        // canInputKeys = new List<List<KeyCode>>() { new List<KeyCode>() { KeyCode.A, KeyCode.S, KeyCode.D, KeyCode.F }, new List<KeyCode>() { KeyCode.H, KeyCode.J, KeyCode.K, KeyCode.L } };
        // isKeyDowns = new List<List<bool>>();
        // isKeyPresses = new List<List<bool>>();
        // isKeyUps = new List<List<bool>>();
        judgmentRange = new float[GetMaxPlayerCount()];
        bestJudgmentRange = new float[GetMaxPlayerCount()];
        closestNotes = new GameObject[GetMaxPlayerCount()];
        closestNoteScripts = new Note[GetMaxPlayerCount()];
        checkPointIndex = 0;
        checkPointAccuracy01 = 1f;
        isBeforeAwake = true;
        tryCount = 1;

        InitPlayManagerScript(0);

        DevTool.Member.infoViewer.SetInfo("FPS", () => 1f / Time.unscaledDeltaTime, 0.3f);
        Handy.RepeatCode((i) => DevTool.Member.infoViewer.SetInfo(Handy.GetPredicateName(i, this.name, nameof(closestNoteIndex)), () => closestNoteIndex[i]), closestNoteIndex.Length);
        DevTool.Member.infoViewer.SetInfo(Handy.GetPredicateName(null, this.name, nameof(levelInfoIndex)), () => levelInfoIndex);
    }
    void Update()
    {
        notePathPosesCount = 360;
        accuracy.SetActive(isShowAccuracy);
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPause)
                Play();
            else
                Pause();
        }
        PauseTweensOnPause();
        if (isPause)
            return;

        if (isBeforeEnable)
        {
            InitTweenAll();
            GotoTweenAll(Mathf.Clamp(GetNoteHoldSecs(levelInfoIndex) - countDownScript.totalCountDownSecs, 0f, GetNoteHoldSecs(levelInfoIndex)));
            PlayHoldTweenAll();
            isBeforeEnable = false;
        }
        if (isBeforeAwake)
        {
            for (int i = 0; i < GetMaxPlayerCount(); i++)
            {
                totalNoteCount += GetNoteCount(i);
            }
            isBeforeAwake = false;
        }

        progress01 = Mathf.Clamp01((float)stopedNoteIndex / (float)GetMaxLevelInfoIndex());
        MaxMissCount = (int)Mathf.Clamp(Mathf.Floor((float)totalNoteCount * 0.4f), 1f, (float)MaxHPCount);
        HP01 = 1f - (float)missCount / MaxMissCount;

        UpdateTweenValueAll();

        keyControl.SetDefault();
        // ClearIsKeyInput();

        UpdateJudgmentRange();

        if (isStop)
        {
            return;
        }

        ActiveNote();

        totalElapsedSecs += Time.deltaTime;
        UpdateClosestNote();

        if (isAutoPlay)
        {
            for (int i = 0; i < GetMaxPlayerCount(); i++)
            {
                if (GetPlayer(i).activeSelf)
                {
                    for (int j = 0; j < GetNoteCount(i); j++)
                    {
                        if (GetNote(i, j).activeSelf)
                        {
                            LevelInfo curLevelInfo = GetLevelInfo(i, j);
                            if (curLevelInfo.noteInfo.awakeSecs + GetNoteWaitSecs(curLevelInfo) <= totalElapsedSecs)
                            {
                                if (!closestNoteScripts[i].isHitted)
                                {
                                    keyControl.Down.OnInput(i, 0);
                                    // KeyDown(i, 0);
                                }
                                if (GetNoteHoldSecs(curLevelInfo) != 0f)
                                {
                                    keyControl.Press.OnInput(i, 0);
                                    // KeyPress(i, 0);
                                }
                            }
                        }
                    }
                }
            }
        }
        else
        {
            keyControl.Update();
            // for (int i = 0; i < canInputKeys.Count; i++)
            // {
            //     for (int j = 0; j < canInputKeys[i].Count; j++)
            //     {
            //         if (Input.GetKeyDown(canInputKeys[i][j]))
            //         {
            //             KeyDown(i, j);
            //         }
            //         if (Input.GetKey(canInputKeys[i][j]))
            //         {
            //             KeyPress(i, j);
            //         }
            //         if (Input.GetKeyUp(canInputKeys[i][j]))
            //         {
            //             KeyUp(i, j);
            //         }
            //     }
            // }
        }
        if (progress01 >= 1f)
        {
            isClearWorld = true;
            isStop = true;
        }
        else if (HP01 <= 0f)
        {
            isGameOver = true;
            isStop = true;
        }
        if (isClearWorld || isGameOver)
            endMessageScript.SetEndMessage();
    }
    void LateUpdate()
    {
        if (isPause)
            return;
        UpdateTransformAll();
        UpdateRendererAll();
    }
    public void SetIsAutoPlay(Toggle toggle)
    {
        isAutoPlay = toggle.isOn;
    }
    public void SetIsShowAccuracy(Toggle toggle)
    {
        isShowAccuracy = toggle.isOn;
    }
    public void Pause()
    {
        isPause = true;
        PauseController.SetActive(true);
    }
    public void Play()
    {
        isPause = false;
    }
    public void InitPlayManagerScript(int startLevelInfoIndex)
    {
        keyControl = new();
        keyControl.Down.AddOnInput((i, j) =>
        {
            InputCount++;
            closestNoteScripts[i].isHitted = true;
        });
        if (startLevelInfoIndex == 0)
            checkPointAccuracy01 = 1f;
        levelInfoIndex = startLevelInfoIndex;
        stopedNoteIndex = levelInfoIndex;
        InputCount = levelInfoIndex;
        missCount = 0;

        accuracy01 = checkPointAccuracy01;
        sumNoteAccuracy01 = (float)accuracy01 * (float)InputCount;
        progress01 = Mathf.Clamp01((float)stopedNoteIndex / (float)GetMaxLevelInfoIndex());
        HP01 = 1f;
        totalElapsedSecs = GetLevelInfo(levelInfoIndex).noteInfo.awakeSecs;

        isStop = true;
        isPause = false;
        isGameOver = false;
        isClearWorld = false;
        isBeforeEnable = true;

        closestNoteIndex = new int[GetMaxPlayerCount()];
        if (levelInfoIndex != 0)
        {
            List<int> playerIndexList = new List<int>();
            for (int i = levelInfoIndex; i < GetLevelInfoCount(); i++)
            {
                if (!playerIndexList.Contains(GetPlayerIndex(i)))
                {
                    closestNoteIndex[GetPlayerIndex(i)] = GetEachNoteIndex(i);
                    playerIndexList.Add(GetPlayerIndex(i));
                }
            }
        }

        tweeners = new List<ITweener>();
        tweenerInPlayGOs = new List<ITweenerInPlay>();
        GOs = new List<IGameObject>();
        scripts = new List<IScript>();
    }
    public void Restart(int startLevelInfoIndex)
    {
        List<IScript> scriptsTemp = new List<IScript>(scripts);
        InitPlayManagerScript(startLevelInfoIndex);
        InitScriptAll(scriptsTemp);
        TryKillTweenAll();
        for (int i = 0; i < GetMaxPlayerCount(); i++)
        {
            GetPlayerScript(i).InitSideSubScaleInfo();
        }
        for (int i = 0; i < GetLevelInfoCount(); i++)
        {
            Note curNoteScript = GetNoteScript(i);
            curNoteScript.TryKillWaitTweens();
            curNoteScript.TryKillHoldTweens();
            curNoteScript.gameObject.SetActive(false);
            curNoteScript.InitNoteScript();
            if (i < levelInfoIndex)
                curNoteScript.EndNoteScript();
        }
        tryCount++;
        if (levelInfoIndex > 0)
            levelInfoIndex--;
        countDownScript.PlayCountDown();
    }
    void PauseTweensOnPause()
    {
        if (isPause)
            StartCoroutine(PauseTweensOnPauseCo());
    }
    IEnumerator PauseTweensOnPauseCo()
    {
        List<TweeningInfo> playingTweens = new List<TweeningInfo>();
        Handy.RepeatCode((i) =>
        {
            foreach (var NT in GetNoteScript(i).GetNoteTweens())
            {
                if (!TweenMethod.IsInfoNull(NT))
                    if (NT.IsPlaying())
                        playingTweens.Add(NT);
            }
        }, GetLevelInfoCount(), 1);
        foreach (var T in tweeners)
        {
            foreach (var TI in T.GetTweens())
            {
                if (!TweenMethod.IsInfoNull(TI))
                    if (TI.IsPlaying())
                        playingTweens.Add(TI);
            }
        }
        foreach (var PT in playingTweens)
        {
            TweenMethod.TryPauseTween(PT);
        }
        yield return new WaitUntil(() => !isPause);
        foreach (var PT in playingTweens)
        {
            TweenMethod.TryPlayTween(PT);
        }
    }
    public void Exit()
    {
    }
    void UpdateClosestNote()
    {
        for (int i = 0; i < GetMaxPlayerCount(); i++)
        {
            closestNotes[i] = GetNote(i, closestNoteIndex[i]);
            closestNoteScripts[i] = GetNoteScript(i, closestNoteIndex[i]);
        }
    }
    void ActiveNote()
    {
        for (int i = levelInfoIndex + 1; i < GetLevelInfoCount(); i++)
        {
            Note curNoteScript = GetNoteScript(i);
            if (!curNoteScript.isStop)
            {
                if (!curNoteScript.gameObject.activeSelf)
                {
                    if (GetLevelInfo(i).noteInfo.awakeSecs <= totalElapsedSecs)
                    {
                        curNoteScript.Active();
                    }
                }
            }
        }
    }
    // void ClearIsKeyInput()
    // {
    //     keyDownCount = 0;
    //     keyPressCount = 0;
    //     keyUpCount = 0;
    //     isKeyDowns.Clear();
    //     for (int i = 0; i < canInputKeys.Count; i++)
    //     {
    //         isKeyDowns.Add(new List<bool>());
    //         for (int j = 0; j < canInputKeys[i].Count; j++)
    //         {
    //             isKeyDowns[i].Add(false);
    //         }
    //     }
    //     isKeyPresses.Clear();
    //     for (int i = 0; i < canInputKeys.Count; i++)
    //     {
    //         isKeyPresses.Add(new List<bool>());
    //         for (int j = 0; j < canInputKeys[i].Count; j++)
    //         {
    //             isKeyPresses[i].Add(false);
    //         }
    //     }
    //     isKeyUps.Clear();
    //     for (int i = 0; i < canInputKeys.Count; i++)
    //     {
    //         isKeyUps.Add(new List<bool>());
    //         for (int j = 0; j < canInputKeys[i].Count; j++)
    //         {
    //             isKeyUps[i].Add(false);
    //         }
    //     }
    // }
    // void KeyDown(int playerIndex, int keyIndex)
    // {
    //     isKeyDowns[playerIndex][keyIndex] = true;
    //     keyDownCount++;
    //     InputCount++;
    //     closestNoteScripts[playerIndex].isHitted = true;
    // }
    // void KeyPress(int playerIndex, int keyIndex)
    // {
    //     isKeyPresses[playerIndex][keyIndex] = true;
    //     keyPressCount++;
    // }
    // void KeyUp(int playerIndex, int keyIndex)
    // {
    //     isKeyUps[playerIndex][keyIndex] = true;
    //     keyUpCount++;
    // }
    public JudgmentType GetJudgment(int playerIndex, float judgmentValue, Action codeOnJudgBad = null, Action codeOnJudgGood = null, Action codeOnStart = null)
    {
        if (codeOnStart == null)
            codeOnStart = () => { };
        if (codeOnJudgBad == null)
            codeOnJudgBad = () => { };
        if (codeOnJudgGood == null)
            codeOnJudgGood = () => { };
        codeOnStart();
        JudgmentType judgmentType = JudgmentType.Perfect;
        playerIndex = Handy.GetCorrectedIndex(playerIndex, GetMaxPlayerIndex());
        if (judgmentValue > bestJudgmentRange[playerIndex] * 1.25f && judgmentValue <= bestJudgmentRange[playerIndex] * 2.75f)
        {
            judgmentType = JudgmentType.Good;
            codeOnJudgGood();
        }
        else if (judgmentValue > bestJudgmentRange[playerIndex] * 2.75f && judgmentValue <= bestJudgmentRange[playerIndex] * 3.5f)
        {
            judgmentType = JudgmentType.Bad;
            codeOnJudgBad();
        }
        else if (judgmentValue > bestJudgmentRange[playerIndex] * 3.5f)
        {
            judgmentType = JudgmentType.Miss;
            codeOnJudgBad();
        }
        return judgmentType;
    }
    public static PlayManager Member
    {
        get
        {
            return instance;
        }
    }
    public void CountMissNote()
    {
        missCount++;
    }

    public void SetAccuracy01()
    {
        accuracy01 = sumNoteAccuracy01 / (float)InputCount;
    }
    // public int GetKeyDownCount() => keyDownCount;
    // public int GetKeyPressCount() => keyPressCount;
    // public int GetKeyUpCount() => keyUpCount;
    // public bool GetIsKeyDown(int playerIndex)
    // {
    //     playerIndex = Handy.GetCorrectedIndex(playerIndex, GetMaxPlayerIndex());
    //     for (int i = 0; i < isKeyDowns[playerIndex].Count; i++)
    //     {
    //         if (isKeyDowns[playerIndex][i])
    //         {
    //             return true;
    //         }
    //     }
    //     return false;
    // }
    // public bool GetIsKeyPress(int playerIndex)
    // {
    //     playerIndex = Handy.GetCorrectedIndex(playerIndex, GetMaxPlayerIndex());
    //     for (int i = 0; i < isKeyPresses[playerIndex].Count; i++)
    //     {
    //         if (isKeyPresses[playerIndex][i])
    //         {
    //             return true;
    //         }
    //     }
    //     return false;
    // }
    // public bool GetIsKeyUp(int playerIndex)
    // {
    //     playerIndex = Handy.GetCorrectedIndex(playerIndex, GetMaxPlayerIndex());
    //     for (int i = 0; i < isKeyUps[playerIndex].Count; i++)
    //     {
    //         if (isKeyUps[playerIndex][i])
    //         {
    //             return true;
    //         }
    //     }
    //     return false;
    // }
    public bool GetIsHitNote((int index, int count) note, Note.Type noteType, (int Down, int Press, int Up) keyCounts, Action<(int Down, int Press, int Up)> keyCountsSetter)
    {
        if (noteType == Note.Type.Tap)
        {
            /* if (note.levelInfo.noteInfo.insideNoteType == LevelInfo.InsideNoteType.Tap)
            {
                return note.PlayByNoteIndex(
                () => keyControl.Down.GetIsInput(),
                () => keyControl.Down.GetIsInput(),
                () => keyControl.Up.GetIsInput());
            }
            else  */
            return Note.PlayByNoteIndex(
            note,
            () =>
            {
                keyCounts.Down--;
                keyCountsSetter(keyCounts);
                if (keyCounts.Down >= 0)
                    return true;
                return false;
            },
            () =>
            {
                keyCounts.Press--;
                keyCountsSetter(keyCounts);
                if (keyCounts.Press >= 0)
                    return true;
                return false;
            },
            () =>
            {
                // Handy.WriteLog(nameof(keyControl.Up), keyControl.Up.Count);
                // Handy.WriteLog(nameof(keyCounts.Up), keyCounts.Up);
                keyCounts.Up--;
                keyCountsSetter(keyCounts);
                if (keyCounts.Up >= 0)
                    return true;
                return false;
            });
            // if (note.levelInfo.noteInfo.insideNoteType == LevelInfo.InsideNoteType.Keep)
            // {
            // }
        }
        else if (noteType == Note.Type.Hold)
        {
            return Note.PlayByNoteIndex(
            note,
            () =>
            {
                keyCounts.Down--;
                keyCountsSetter(keyCounts);
                if (keyCounts.Down >= 0)
                    return true;
                return false;
            },
            () =>
            {
                keyCounts.Down--;
                keyCountsSetter(keyCounts);
                if (keyCounts.Down >= 0)
                    return true;
                return false;
            },
            () =>
            {
                keyCounts.Down--;
                keyCountsSetter(keyCounts);
                if (keyCounts.Down >= 0)
                    return true;
                return false;
            });
        }
        return false;
    }
    public void UpdateJudgmentRange()
    {
        for (int i = 0; i < GetMaxPlayerCount(); i++)
        {
            if (GetPlayer(i).activeSelf)
            {
                judgmentRange[i] = GetLevelInfo(i, closestNoteIndex[i]).judgmentInfo.range;
                bestJudgmentRange[i] = judgmentRange[i] * 0.25f;
            }
        }
    }
    public WorldInfo worldInfo { get { return worldReaderScript.worldInfo; } }
    public LevelInfo GetLevelInfo(int levelInfoIndex) => worldReaderScript.worldInfo.levelInfos[Handy.GetCorrectedIndex(levelInfoIndex, worldReaderScript.worldInfo.levelInfos.Length - 1)];
    public LevelInfo GetLevelInfo(int playerIndex, int eachNoteIndex)
    {
        playerIndex = Handy.GetCorrectedIndex(playerIndex, GetMaxPlayerIndex());
        eachNoteIndex = Handy.GetCorrectedIndex(eachNoteIndex, GetMaxNoteIndex(playerIndex));
        for (int i = 1; i < GetLevelInfoCount(); i++)
        {
            LevelInfo curLevelInfo = GetLevelInfo(i);
            if (curLevelInfo.noteInfo.tarPlayerIndex == playerIndex && curLevelInfo.noteInfo.eachNoteIndex == eachNoteIndex)
                return curLevelInfo;
        }
        return null;
    }
    public int GetPlayerIndex(int levelInfoIndex)
    {
        return GetLevelInfo(levelInfoIndex).noteInfo.tarPlayerIndex;
    }
    public int GetEachNoteIndex(int levelInfoIndex)
    {
        return GetLevelInfo(levelInfoIndex).noteInfo.eachNoteIndex;
    }
    public float GetNoteWaitSecs(int playerIndex, int eachNoteIndex)
    {
        playerIndex = Handy.GetCorrectedIndex(playerIndex, GetMaxPlayerIndex(), -1);
        eachNoteIndex = Handy.GetCorrectedIndex(eachNoteIndex, GetMaxNoteIndex(playerIndex), -1);
        return playerIndex == -1 && eachNoteIndex == -1 ? 0f : noteGeneratorScript.notesWaitSecs[playerIndex][eachNoteIndex];
    }
    public float GetNoteHoldSecs(int playerIndex, int eachNoteIndex)
    {
        playerIndex = Handy.GetCorrectedIndex(playerIndex, GetMaxPlayerIndex(), -1);
        eachNoteIndex = Handy.GetCorrectedIndex(eachNoteIndex, GetMaxNoteIndex(playerIndex), -1);
        return playerIndex == -1 && eachNoteIndex == -1 ? countDownScript.totalCountDownSecs : noteGeneratorScript.notesLengthSecs[playerIndex][eachNoteIndex];
    }
    public float GetNoteWaitSecs(int levelInfoIndex)
    {
        return levelInfoIndex == 0 ? 0f : noteGeneratorScript.notesWaitSecs[GetLevelInfo(levelInfoIndex).noteInfo.tarPlayerIndex][GetLevelInfo(levelInfoIndex).noteInfo.eachNoteIndex];
    }
    public float GetNoteHoldSecs(int levelInfoIndex)
    {
        return levelInfoIndex == 0 ? countDownScript.totalCountDownSecs : noteGeneratorScript.notesLengthSecs[GetLevelInfo(levelInfoIndex).noteInfo.tarPlayerIndex][GetLevelInfo(levelInfoIndex).noteInfo.eachNoteIndex];
    }
    public float GetNoteWaitSecs(LevelInfo levelInfo)
    {
        return levelInfo.noteInfo.tarPlayerIndex == -1 && levelInfo.noteInfo.eachNoteIndex == -1 ? 0f : noteGeneratorScript.notesWaitSecs[levelInfo.noteInfo.tarPlayerIndex][levelInfo.noteInfo.eachNoteIndex];
    }
    public float GetNoteHoldSecs(LevelInfo levelInfo)
    {
        return levelInfo.noteInfo.tarPlayerIndex == -1 && levelInfo.noteInfo.eachNoteIndex == -1 ? countDownScript.totalCountDownSecs : noteGeneratorScript.notesLengthSecs[levelInfo.noteInfo.tarPlayerIndex][levelInfo.noteInfo.eachNoteIndex];
    }
    public GameObject GetNote(int playerIndex, int eachNoteIndex)
    {
        playerIndex = Handy.GetCorrectedIndex(playerIndex, GetMaxPlayerIndex(), -1);
        eachNoteIndex = Handy.GetCorrectedIndex(eachNoteIndex, GetMaxNoteIndex(playerIndex), -1);
        return playerIndex == -1 && eachNoteIndex == -1 ? noteGeneratorScript.startNote : noteGeneratorScript.notes[playerIndex][eachNoteIndex];
    }
    public Note GetNoteScript(int playerIndex, int eachNoteIndex)
    {
        playerIndex = Handy.GetCorrectedIndex(playerIndex, GetMaxPlayerIndex(), -1);
        eachNoteIndex = Handy.GetCorrectedIndex(eachNoteIndex, GetMaxNoteIndex(playerIndex), -1);
        return playerIndex == -1 && eachNoteIndex == -1 ? noteGeneratorScript.startNoteScript : noteGeneratorScript.noteScripts[playerIndex][eachNoteIndex];
    }
    public GameObject GetNote(int levelInfoIndex)
    {
        int playerIndex = Handy.GetCorrectedIndex(GetPlayerIndex(levelInfoIndex), GetMaxPlayerIndex(), -1);
        int eachNoteIndex = Handy.GetCorrectedIndex(GetEachNoteIndex(levelInfoIndex), GetMaxNoteIndex(playerIndex), -1);
        return GetNote(playerIndex, eachNoteIndex);
    }
    public Note GetNoteScript(int levelInfoIndex)
    {
        int playerIndex = Handy.GetCorrectedIndex(GetPlayerIndex(levelInfoIndex), GetMaxPlayerIndex(), -1);
        int eachNoteIndex = Handy.GetCorrectedIndex(GetEachNoteIndex(levelInfoIndex), GetMaxNoteIndex(playerIndex), -1);
        return GetNoteScript(playerIndex, eachNoteIndex);
    }
    // public float GetJudgmentValue(Note note)
    // {
    //     float judgmentValue = 1f;
    //     if (note.waitElapsedSecs01 >= 1f - judgmentRange[note.tarPlayerIndex])
    //     {
    //         if (note.holdElapsedSecs01 == 0f)
    //             judgmentValue = (1f - note.waitElapsedSecs01) / judgmentRange[note.tarPlayerIndex];
    //         else
    //         {
    //             for (int i = note.levelInfo.noteInfo.noteCount - 1; i >= 0; i--)
    //             {
    //                 if (note.holdElapsedSecs01 >= note.levelInfo.noteInfo.noteHitTiming01s[i])
    //                 {
    //                     float range =
    //                     i < note.levelInfo.noteInfo.noteCount - 1
    //                     ? note.levelInfo.noteInfo.noteHitTiming01s[i + 1] - note.levelInfo.noteInfo.noteHitTiming01s[i]
    //                     : judgmentRange[note.tarPlayerIndex] * 2f;
    //                     judgmentValue = 1f - Mathf.Abs(((note.holdElapsedSecs01 - note.levelInfo.noteInfo.noteHitTiming01s[i]) / range) - 0.5f) * 2f;
    //                     break;
    //                 }
    //             }
    //         }
    //     }
    //     return Mathf.Clamp01(judgmentValue);
    // }
    public float GetJudgmentValue(int noteIndex, (int count, float waitSec01s, float holdSec01s, float[] hitTiming01s, float judgmentRange) note)
    {
        float judgmentValue = 1f;
        if (note.waitSec01s >= 1f + note.hitTiming01s[0] - note.judgmentRange)
        {
            float averageHitTiming01 =
            noteIndex > 0
            ? (note.hitTiming01s[noteIndex] + note.hitTiming01s[noteIndex - 1]) * 0.5f
            : note.hitTiming01s[0];

            if (note.holdSec01s <= note.hitTiming01s[0])
                judgmentValue = (1f + note.hitTiming01s[0] - note.waitSec01s) / note.judgmentRange;

            else if (note.holdSec01s >= averageHitTiming01)
            {
                float range =
                noteIndex < note.count - 1
                ? note.hitTiming01s[noteIndex + 1] - note.hitTiming01s[noteIndex]
                : note.judgmentRange * 2f;
                judgmentValue = 1f - Mathf.Abs(((note.holdSec01s - note.hitTiming01s[noteIndex]) / range) - 0.5f) * 2f;
            }
        }
        return Mathf.Clamp01(judgmentValue);
    }
    public float GetJudgmentValue((int count, float waitSec01s, float holdSec01s, float[] hitTiming01s, float judgmentRange) note)
    {
        float judgmentValue = 1f;
        if (note.waitSec01s >= 1f + note.hitTiming01s[0] - note.judgmentRange)
        {
            float averageHitTiming01 =
            Note.index > 0
            ? (note.hitTiming01s[Note.index] + note.hitTiming01s[Note.index - 1]) * 0.5f
            : note.hitTiming01s[0];

            if (note.holdSec01s <= note.hitTiming01s[0])
                judgmentValue = (1f + note.hitTiming01s[0] - note.waitSec01s) / note.judgmentRange;

            else if (note.holdSec01s >= averageHitTiming01)
            {
                float range =
                Note.index < note.count - 1
                ? note.hitTiming01s[Note.index + 1] - note.hitTiming01s[Note.index]
                : note.judgmentRange * 2f;
                judgmentValue = 1f - Mathf.Abs(((note.holdSec01s - note.hitTiming01s[Note.index]) / range) - 0.5f) * 2f;
            }
        }
        return Mathf.Clamp01(judgmentValue);
    }
    public int GetMaxPlayerCount()
    {
        int maxPlayerCount = 0;
        for (int i = 1; i < GetLevelInfoCount(); i++)
        {
            if (maxPlayerCount < GetLevelInfo(i).playerInfo.Length)
            {
                maxPlayerCount = GetLevelInfo(i).playerInfo.Length;
            }
        }
        return maxPlayerCount;
    }
    public int GetMaxPlayerIndex()
    {
        return GetMaxPlayerCount() - 1;
    }
    public int GetLevelInfoCount()
    {
        return worldReaderScript.worldInfo.levelInfos.Length;
    }
    public int GetMaxLevelInfoIndex()
    {
        return GetLevelInfoCount() - 1;
    }
    public int GetNoteCount(int playerIndex)
    {
        int noteCount = 0;
        for (int i = 1; i < GetLevelInfoCount(); i++)
        {
            if (GetLevelInfo(i).noteInfo.tarPlayerIndex == playerIndex)
                noteCount++;
        }
        return noteCount;
    }
    public int GetMaxNoteIndex(int playerIndex)
    {
        playerIndex = Handy.GetCorrectedIndex(playerIndex, GetMaxPlayerIndex());
        return GetNoteCount(playerIndex) - 1;
    }
    public int GetActivePlayerCount()
    {
        int activeMaxPlayerCount = 0;
        for (int i = 0; i < GetMaxPlayerCount(); i++)
        {
            if (GetPlayer(i).activeSelf)
                activeMaxPlayerCount++;
        }
        return activeMaxPlayerCount;
    }
    public GameObject GetPlayer(int playerIndex)
    {
        return playerControllerScript.players[Handy.GetCorrectedIndex(playerIndex, GetMaxPlayerIndex())];
    }
    public Player GetPlayerScript(int playerIndex)
    {
        return playerControllerScript.playerScripts[Handy.GetCorrectedIndex(playerIndex, GetMaxPlayerIndex())];
    }
    public GameObject GetPlayerSide(int playerIndex)
    {
        return playerControllerScript.playerSides[Handy.GetCorrectedIndex(playerIndex, GetMaxPlayerIndex())];
    }
    public SpriteRenderer GetPlayerSideRend(int playerIndex)
    {
        return playerControllerScript.playerSideRends[Handy.GetCorrectedIndex(playerIndex, GetMaxPlayerIndex())];
    }
    public GameObject GetPlayerCenter(int playerIndex)
    {
        return playerControllerScript.playerCenters[Handy.GetCorrectedIndex(playerIndex, GetMaxPlayerIndex())];
    }
    public SpriteRenderer GetPlayerCenterRend(int playerIndex)
    {
        return playerControllerScript.playerCenterRends[Handy.GetCorrectedIndex(playerIndex, GetMaxPlayerIndex())];
    }
    public Color GetColor01WithPlayerIndex(Color color01, int playerIndex)
    {
        Color color01_temp = color01;
        for (int i = 0; i < playerIndex; i++)
        {
            color01_temp = Color.white - color01_temp;
        }
        color01_temp /= Mathf.Floor(playerIndex * 0.5f) + 1;
        color01_temp.a = color01.a;
        return color01_temp;
    }
    public float GetWaitElapsedSecs01(int playerIndex, int eachNoteIndex)
    {
        playerIndex = Handy.GetCorrectedIndex(playerIndex, GetMaxPlayerIndex());
        if (GetNoteScript(playerIndex, eachNoteIndex) != null)
            return GetNoteScript(playerIndex, eachNoteIndex).waitElapsedSecs01;
        return 0f;
    }
    public float GetHoldElapsedSecs01(int playerIndex, int eachNoteIndex)
    {
        playerIndex = Handy.GetCorrectedIndex(playerIndex, GetMaxPlayerIndex());
        if (GetNoteScript(playerIndex, eachNoteIndex) != null)
            return GetNoteScript(playerIndex, eachNoteIndex).holdElapsedSecs01;
        return 0f;
    }
    public TweenInfo<float> CorrectDegTween(TweenInfo<float> degTween, int dir)
    {
        TweenInfo<float> degTweenTemp = degTween.Clone();
        dir = (int)Handy.GetSign0IsZero(dir);
        degTweenTemp.endValue += dir * degTweenTemp.startValue > dir * degTweenTemp.endValue ? dir * 360f : 0f;
        return degTweenTemp;
    }
    public void InitTween(ITweener tweenerGO)
    {
        tweenerGO.InitTween();
    }
    public void UpdateTweenValue(ITweener tweenerGO)
    {
        tweenerGO.UpdateTweenValue();
    }
    public void TryKillTween(ITweener tweenerGO)
    {
        tweenerGO.TryKillTween();
    }
    public void GotoTween(float toSecs, ITweener tweenerGO)
    {
        tweenerGO.GotoTween(toSecs);
    }
    public void PlayWaitTween(ITweenerInPlay tweenerInPlayGO)
    {
        tweenerInPlayGO.PlayWaitTween();
    }
    public void PlayHoldTween(ITweenerInPlay tweenerInPlayGO)
    {
        tweenerInPlayGO.PlayHoldTween();
    }
    public void InitScript(IScript script)
    {
        script.InitScript();
    }
    public void UpdateTransform(IGameObject GO)
    {
        GO.UpdateTransform();
    }
    public void UpdateRenderer(IGameObject GO)
    {
        GO.UpdateRenderer();
    }
    public void InitTweenAll(List<ITweener> tweenerGOs = null)
    {
        if (tweenerGOs == null)
            tweenerGOs = this.tweeners;
        foreach (var TGO in tweenerGOs)
        {
            TGO.InitTween();
        }
    }
    public void UpdateTweenValueAll(List<ITweener> tweenerGOs = null)
    {
        if (tweenerGOs == null)
            tweenerGOs = this.tweeners;
        foreach (var TGO in tweenerGOs)
        {
            TGO.UpdateTweenValue();
        }
    }
    public void TryKillTweenAll(List<ITweener> tweenerGOs = null)
    {
        if (tweenerGOs == null)
            tweenerGOs = this.tweeners;
        foreach (var TGO in tweenerGOs)
        {
            TGO.TryKillTween(); ;
        }
    }
    public void GotoTweenAll(float toSecs, List<ITweener> tweenerGOs = null)
    {
        if (tweenerGOs == null)
            tweenerGOs = this.tweeners;
        foreach (var TGO in tweenerGOs)
        {
            TGO.GotoTween(toSecs);
        }
    }
    public void PlayWaitTweenAll(List<ITweenerInPlay> tweenerInPlayGOs = null)
    {
        if (tweenerInPlayGOs == null)
            tweenerInPlayGOs = this.tweenerInPlayGOs;
        foreach (var TPGO in tweenerInPlayGOs)
        {
            TPGO.PlayWaitTween();
        }
    }
    public void PlayHoldTweenAll(List<ITweenerInPlay> tweenerInPlayGOs = null)
    {
        if (tweenerInPlayGOs == null)
            tweenerInPlayGOs = this.tweenerInPlayGOs;
        foreach (var TPGO in tweenerInPlayGOs)
        {
            TPGO.PlayHoldTween();
        }
    }
    public void UpdateTransformAll(List<ITweenerInPlay> tweenerInPlayGOs = null)
    {
        if (tweenerInPlayGOs == null)
            tweenerInPlayGOs = this.tweenerInPlayGOs;
        foreach (var GO in GOs)
        {
            GO.UpdateTransform();
        }
    }
    public void UpdateRendererAll(List<IGameObject> GOs = null)
    {
        if (GOs == null)
            GOs = this.GOs;
        foreach (var GO in GOs)
        {
            GO.UpdateRenderer();
        }
    }
    public void InitScriptAll(List<IScript> scripts = null)
    {
        if (scripts == null)
            scripts = this.scripts;
        foreach (var script in scripts)
        {
            script.InitScript();
        }
    }
    PlayManager()
    {
        MaxHPCount = 15;
    }
}
