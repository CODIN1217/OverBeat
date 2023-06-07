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
    public GameObject Accuracy;
    public Accuracy AccuracyScript;

    [SerializeField]
    GameObject PauseController;
    [SerializeField]
    Toggle Toggle_Auto;
    [SerializeField]
    Toggle Toggle_ShowAccuracy;

    public GameObject[] closestNotes;
    public Note[] closestNoteScripts;
    public float[] judgmentRange;
    public float[] bestJudgmentRange;
    public int InputCount;
    public int missCount;
    public List<List<bool>> isKeyDowns;
    public List<List<bool>> isKeyPresses;
    public List<List<bool>> isKeyUps;
    public bool isStop;
    public bool isPause;
    public bool isGameOver;
    public bool isClearWorld;
    public bool isAutoPlay;
    public bool isShowAccuracy;
    bool isBeforeAwake;
    bool isBeforeEnable;
    static PlayManager instance = null;
    public List<List<KeyCode>> canInputKeys;
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
        instance = this;
        canInputKeys = new List<List<KeyCode>>() { new List<KeyCode>() { KeyCode.A, KeyCode.S, KeyCode.D, KeyCode.F }, new List<KeyCode>() { KeyCode.H, KeyCode.J, KeyCode.K, KeyCode.L } };
        isKeyDowns = new List<List<bool>>();
        isKeyPresses = new List<List<bool>>();
        isKeyUps = new List<List<bool>>();
        judgmentRange = new float[GetMaxPlayerCount()];
        bestJudgmentRange = new float[GetMaxPlayerCount()];
        closestNotes = new GameObject[GetMaxPlayerCount()];
        closestNoteScripts = new Note[GetMaxPlayerCount()];
        checkPointIndex = 0;
        checkPointAccuracy01 = 1f;
        isBeforeAwake = true;
        tryCount = 1;

        InitPlayManagerScript(0);
        // PauseGroupAlphaInfo = new TweeningInfo(new TweenInfo<float>(0f, 1f, AnimationCurve.Linear(0f, 0f, 1f, 1f)), 0.3f);

        DevTool.Member.infoViewer.SetInfo("FPS", () => 1f / Time.unscaledDeltaTime, 0.3f);
        Handy.RepeatCode((i) => DevTool.Member.infoViewer.SetInfo(Handy.GetPredicateName(Handy.GetArray(this.name, nameof(closestNoteIndex)), i), () => closestNoteIndex[i]), closestNoteIndex.Length);
        DevTool.Member.infoViewer.SetInfo(Handy.GetPredicateName(Handy.GetArray(this.name, nameof(levelInfoIndex))), () => levelInfoIndex);
        // for(int i = 0; i < Pause.transform.GetChild(1).childCount; i++){
        //     DevTool.Member.infoViewer.SetInfo(Handy.GetPredicateName(Handy.GetArray(this.name, nameof(Pause), nameof(Pause.transform), nameof(Pause.transform.position)), i), () => Pause.transform.GetChild(1).GetChild(i).position);
        // }
    }
    void Update()
    {
        notePathPosesCount = 360;
        isAutoPlay = Toggle_Auto.isOn;
        isShowAccuracy = Toggle_ShowAccuracy.isOn;
        Accuracy.SetActive(isShowAccuracy);
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPause)
                Play();
            else
                Pause();
        }
        // PauseGroup.alpha = ((TweenerInfo<float>)PauseGroupAlphaInfo).curValue;
        /* if (isPause && Input.GetKeyDown(KeyCode.A))
            isAutoPlay = !isAutoPlay;
        if (Input.GetKeyDown(KeyCode.R))
        {
            Restart(0);
        }
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            Restart(Handy.GetBeforeIndex(levelInfoIndex, 0));
        }
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            Restart(Handy.GetNextIndex(levelInfoIndex + 1, GetMaxLevelInfoIndex()));
        } */
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

        ClearIsKeyInput();

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
                                if (!closestNoteScripts[i].isInputted)
                                {
                                    KeyDown(i, 0);
                                }
                                if (GetNoteHoldSecs(curLevelInfo) != 0f)
                                {
                                    KeyPress(i, 0);
                                }
                            }
                        }
                    }
                }
            }
        }
        else
        {
            for (int i = 0; i < canInputKeys.Count; i++)
            {
                for (int j = 0; j < canInputKeys[i].Count; j++)
                {
                    if (Input.GetKeyDown(canInputKeys[i][j]))
                    {
                        KeyDown(i, j);
                    }
                    if (Input.GetKey(canInputKeys[i][j]))
                    {
                        KeyPress(i, j);
                    }
                    if (Input.GetKeyUp(canInputKeys[i][j]))
                    {
                        KeyUp(i, j);
                    }
                }
            }
        }

        for (int i = 0; i < GetMaxPlayerCount(); i++)
        {
            if (GetPlayer(i).activeSelf)
            {
                if (GetIsKeyDown(i))
                {
                    float noteAccuracy01 = 1f;
                    judgmentGenScript.SetJudgmentText(i, GetJudgment(i, GetJudgmentValue(i), () => { noteAccuracy01 = Mathf.Clamp01(1f - GetJudgmentValue(i)); CountMissNote(); }));
                    sumNoteAccuracy01 += noteAccuracy01;
                    SetAccuracy01();
                }
            }
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
    public void Pause()
    {
        isPause = true;
        PauseController.SetActive(true);
    }
    public void Play()
    {
        isPause = false;
        // if (isPause)
        //     TweenMethod.TryPlayTween(PauseGroupAlphaInfo);
        // else
        //     PauseGroupAlphaInfo.Goto(0f);
    }
    public void InitPlayManagerScript(int startLevelInfoIndex)
    {
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
        //Exit
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
    void ClearIsKeyInput()
    {
        isKeyDowns.Clear();
        for (int i = 0; i < canInputKeys.Count; i++)
        {
            isKeyDowns.Add(new List<bool>());
            for (int j = 0; j < canInputKeys[i].Count; j++)
            {
                isKeyDowns[i].Add(false);
            }
        }
        isKeyPresses.Clear();
        for (int i = 0; i < canInputKeys.Count; i++)
        {
            isKeyPresses.Add(new List<bool>());
            for (int j = 0; j < canInputKeys[i].Count; j++)
            {
                isKeyPresses[i].Add(false);
            }
        }
        isKeyUps.Clear();
        for (int i = 0; i < canInputKeys.Count; i++)
        {
            isKeyUps.Add(new List<bool>());
            for (int j = 0; j < canInputKeys[i].Count; j++)
            {
                isKeyUps[i].Add(false);
            }
        }
    }
    void KeyDown(int playerIndex, int keyIndex)
    {
        isKeyDowns[playerIndex][keyIndex] = true;
        InputCount++;
        closestNoteScripts[playerIndex].isInputted = true;
    }
    void KeyPress(int playerIndex, int keyIndex)
    {
        isKeyPresses[playerIndex][keyIndex] = true;
    }
    void KeyUp(int playerIndex, int keyIndex)
    {
        isKeyUps[playerIndex][keyIndex] = true;
    }
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
    public bool GetIsKeyDown(int playerIndex)
    {
        playerIndex = Handy.GetCorrectedIndex(playerIndex, GetMaxPlayerIndex());
        for (int i = 0; i < isKeyDowns[playerIndex].Count; i++)
        {
            if (isKeyDowns[playerIndex][i])
            {
                return true;
            }
        }
        return false;
    }
    public bool GetIsKeyPress(int playerIndex)
    {
        playerIndex = Handy.GetCorrectedIndex(playerIndex, GetMaxPlayerIndex());
        for (int i = 0; i < isKeyPresses[playerIndex].Count; i++)
        {
            if (isKeyPresses[playerIndex][i])
            {
                return true;
            }
        }
        return false;
    }
    public bool GetIsKeyUp(int playerIndex)
    {
        playerIndex = Handy.GetCorrectedIndex(playerIndex, GetMaxPlayerIndex());
        for (int i = 0; i < isKeyUps[playerIndex].Count; i++)
        {
            if (isKeyUps[playerIndex][i])
            {
                return true;
            }
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
    public float GetJudgmentValue(int playerIndex, float? waitElapsedSecs01 = null, float? holdElapsedSecs01 = null)
    {
        if (waitElapsedSecs01 == null)
            waitElapsedSecs01 = GetWaitElapsedSecs01(playerIndex, closestNoteIndex[playerIndex]);
        if (holdElapsedSecs01 == null)
            holdElapsedSecs01 = GetHoldElapsedSecs01(playerIndex, closestNoteIndex[playerIndex]);
        float judgmentValue = 1f;
        if (holdElapsedSecs01 > 0f)
            judgmentValue = (float)holdElapsedSecs01;
        else if (waitElapsedSecs01 >= 1f - judgmentRange[playerIndex])
            judgmentValue = 1f - (float)waitElapsedSecs01;
        return judgmentValue;
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
