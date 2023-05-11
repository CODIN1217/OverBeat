using System.Collections;
using System.Collections.Generic;
using UnityEngine;
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
    public GameObject[] closestNotes;
    public NotePrefab[] closestNoteScripts;
    public float[] judgmentRange;
    public float[] bestJudgmentRange;
    public int InputCount;
    public int missCount;
    public List<List<bool>> isKeyDowns;
    public List<List<bool>> isKeyPresses;
    public List<List<bool>> isKeyUps;
    public bool isPause;
    public bool isGameOver;
    public bool isClearWorld;
    public bool isAutoPlay;
    bool isBeforeAwake;
    bool isInputted;
    bool isInputted_temp;
    static PlayManager instance = null;
    public List<List<KeyCode>> canInputKeys;
    public float sumNoteAccuracy01;
    public float progress01;
    public float accuracy01;
    public float HP01;
    public int MaxMissCount;
    public readonly int MaxHPCount;
    public Handy handy;
    public int[] closestNoteIndex;
    public int worldInfoIndex;
    public int totalNoteCount;
    Stopwatch totalElapsedTime;
    public float totalElapsedSecs;
    float totalGamePlaySecs;
    public delegate void TweenEvent();
    public TweenEvent initTweenEvent;
    public TweenEvent playHoldTweenEvent;
    void Awake()
    {
        instance = this;
        isPause = true;
        canInputKeys = new List<List<KeyCode>>() { new List<KeyCode>() { KeyCode.A, KeyCode.S, KeyCode.D, KeyCode.F }, new List<KeyCode>() { KeyCode.H, KeyCode.J, KeyCode.K, KeyCode.L } };
        accuracy01 = 1f;
        closestNoteIndex = new int[GetMaxPlayerCount()];
        isKeyDowns = new List<List<bool>>();
        isKeyPresses = new List<List<bool>>();
        isKeyUps = new List<List<bool>>();
        totalElapsedTime = new Stopwatch();
        totalElapsedTime.Reset();
        totalElapsedTime.Stop();
        handy.RepeatCode((i) => InfoViewer.Property.SetInfo(this.name, nameof(closestNoteIndex), () => closestNoteIndex[i], i), closestNoteIndex.Length);
        InfoViewer.Property.SetInfo(this.name, nameof(worldInfoIndex), () => worldInfoIndex);
        judgmentRange = new float[GetMaxPlayerCount()];
        bestJudgmentRange = new float[GetMaxPlayerCount()];
        closestNotes = new GameObject[GetMaxPlayerCount()];
        closestNoteScripts = new NotePrefab[GetMaxPlayerCount()];
        isBeforeAwake = true;
        initTweenEvent = () => { };
        playHoldTweenEvent = () => { };
    }
    void Update()
    {
        UpdateJudgmentRange();
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
        if (isBeforeAwake)
        {
            initTweenEvent();
            playHoldTweenEvent();
            for (int i = 0; i < GetMaxPlayerCount(); i++)
            {
                totalNoteCount += GetNoteCount(i);
            }
            WorldInfo lastWorldInfo = GetWorldInfo(GetMaxWorldInfoIndex());
            NotePrefab lastNoteScript = GetNoteScript(lastWorldInfo.noteInfo.tarPlayerIndex, lastWorldInfo.noteInfo.eachNoteIndex);
            totalGamePlaySecs = lastWorldInfo.noteInfo.awakeSecs + lastNoteScript.noteWaitSecs + lastNoteScript.holdNoteSecs;
            isBeforeAwake = false;
        }
        progress01 = Mathf.Clamp01(totalElapsedSecs / totalGamePlaySecs);
        MaxMissCount = (int)Mathf.Clamp(Mathf.Floor((float)totalNoteCount * 0.4f), 1f, (float)MaxHPCount);
        HP01 = 1f - (float)missCount / MaxMissCount;
        totalElapsedTime.Stop();
        if (isPause)
        {
            return;
        }

        totalElapsedTime.Start();
        totalElapsedSecs = totalElapsedTime.ElapsedMilliseconds * 0.001f;
        for (int i = 1; i < GetWorldInfoCount(); i++)
        {
            GameObject curNote = GetNote(GetWorldInfo(i).noteInfo.tarPlayerIndex, GetWorldInfo(i).noteInfo.eachNoteIndex);
            NotePrefab curNoteScript = GetNoteScript(GetWorldInfo(i).noteInfo.tarPlayerIndex, GetWorldInfo(i).noteInfo.eachNoteIndex);
            if (!curNoteScript.isStop)
            {
                if (!curNote.activeSelf)
                {
                    if (GetWorldInfo(i).noteInfo.awakeSecs <= totalElapsedSecs)
                    {
                        curNote.SetActive(true);
                        curNoteScript.toleranceSecsAwake = totalElapsedSecs - GetWorldInfo(i).noteInfo.awakeSecs;
                    }
                }
            }
        }
        for (int i = 0; i < GetMaxPlayerCount(); i++)
        {
            closestNotes[i] = GetNote(i, closestNoteIndex[i]);
            closestNoteScripts[i] = GetNoteScript(i, closestNoteIndex[i]);
        }
        if (isAutoPlay)
        {
        }
        else
        {
            for (int i = 0; i < canInputKeys.Count; i++)
            {
                for (int j = 0; j < canInputKeys[i].Count; j++)
                {
                    if (Input.GetKeyDown(canInputKeys[i][j]))
                    {
                        isKeyDowns[i][j] = true;
                        InputCount++;
                        closestNoteScripts[i].isInputted = true;
                    }
                    if (Input.GetKey(canInputKeys[i][j]))
                    {
                        isKeyPresses[i][j] = true;
                    }
                    if (Input.GetKeyUp(canInputKeys[i][j]))
                    {
                        isKeyUps[i][j] = true;
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
        if (HP01 <= 0f)
        {
            isGameOver = true;
            isPause = true;
        }
        if (progress01 >= 1f)
        {
            isClearWorld = true;
            isPause = true;
        }
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
        playerIndex = handy.GetCorrectIndex(playerIndex, GetMaxPlayerIndex());
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
    public static PlayManager Property
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
        accuracy01 = sumNoteAccuracy01 / InputCount;
    }
    public bool GetIsKeyDown(int playerIndex)
    {
        playerIndex = handy.GetCorrectIndex(playerIndex, GetMaxPlayerIndex());
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
        playerIndex = handy.GetCorrectIndex(playerIndex, GetMaxPlayerIndex());
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
        playerIndex = handy.GetCorrectIndex(playerIndex, GetMaxPlayerIndex());
        for (int i = 0; i < isKeyUps[playerIndex].Count; i++)
        {
            if (isKeyUps[playerIndex][i])
            {
                return true;
            }
        }
        return false;
    }
    public bool isBreakUpdate()
    {
        if (isPause)
        {
            if (!isClearWorld && !isGameOver)
                return true;
        }
        return false;
    }
    public void UpdateJudgmentRange()
    {
        for (int i = 0; i < GetMaxPlayerCount(); i++)
        {
            if (GetPlayer(i).activeSelf)
            {
                judgmentRange[i] = GetWorldInfo(i, closestNoteIndex[i]).judgmentInfo.range;
                bestJudgmentRange[i] = judgmentRange[i] * 0.25f;
            }
        }
    }
    public WorldInfo GetWorldInfo(int worldInfoIndex)
    {
        return worldReaderScript.worldInfos[handy.GetCorrectIndex(worldInfoIndex, GetMaxWorldInfoIndex())];
    }
    public WorldInfo GetWorldInfo(int playerIndex, int eachNoteIndex)
    {
        playerIndex = handy.GetCorrectIndex(playerIndex, GetMaxPlayerIndex());
        eachNoteIndex = handy.GetCorrectIndex(eachNoteIndex, GetMaxNoteIndex(playerIndex));
        for (int i = 1; i < GetWorldInfoCount(); i++)
        {
            WorldInfo curWorldInfo = GetWorldInfo(i);
            if (curWorldInfo.noteInfo.tarPlayerIndex == playerIndex && curWorldInfo.noteInfo.eachNoteIndex == eachNoteIndex)
                return curWorldInfo;
        }
        return null;
    }
    public float GetNoteWaitSecs(int playerIndex, int eachNoteIndex)
    {
        playerIndex = handy.GetCorrectIndex(playerIndex, GetMaxPlayerIndex(), -1);
        eachNoteIndex = handy.GetCorrectIndex(eachNoteIndex, GetMaxNoteIndex(playerIndex), -1);
        return playerIndex == -1 && eachNoteIndex == -1 ? 0f : noteGeneratorScript.notesWaitSecs[playerIndex][eachNoteIndex];
    }
    public float GetHoldNoteSecs(int playerIndex, int eachNoteIndex)
    {
        playerIndex = handy.GetCorrectIndex(playerIndex, GetMaxPlayerIndex(), -1);
        eachNoteIndex = handy.GetCorrectIndex(eachNoteIndex, GetMaxNoteIndex(playerIndex), -1);
        return playerIndex == -1 && eachNoteIndex == -1 ? countDownScript.totalCountDownSecs : noteGeneratorScript.notesLengthSecs[playerIndex][eachNoteIndex];
    }
    public float GetNoteWaitSecs(int worldInfoIndex)
    {
        return worldInfoIndex == 0 ? 0f : noteGeneratorScript.notesWaitSecs[GetWorldInfo(worldInfoIndex).noteInfo.tarPlayerIndex][GetWorldInfo(worldInfoIndex).noteInfo.eachNoteIndex];
    }
    public float GetHoldNoteSecs(int worldInfoIndex)
    {
        return worldInfoIndex == 0 ? countDownScript.totalCountDownSecs : noteGeneratorScript.notesLengthSecs[GetWorldInfo(worldInfoIndex).noteInfo.tarPlayerIndex][GetWorldInfo(worldInfoIndex).noteInfo.eachNoteIndex];
    }
    public float GetNoteWaitSecs(WorldInfo worldInfo)
    {
        return worldInfo.noteInfo.tarPlayerIndex == -1 && worldInfo.noteInfo.eachNoteIndex == -1 ? 0f : noteGeneratorScript.notesWaitSecs[worldInfo.noteInfo.tarPlayerIndex][worldInfo.noteInfo.eachNoteIndex];
    }
    public float GetHoldNoteSecs(WorldInfo worldInfo)
    {
        return worldInfo.noteInfo.tarPlayerIndex == -1 && worldInfo.noteInfo.eachNoteIndex == -1 ? countDownScript.totalCountDownSecs : noteGeneratorScript.notesLengthSecs[worldInfo.noteInfo.tarPlayerIndex][worldInfo.noteInfo.eachNoteIndex];
    }
    public GameObject GetNote(int playerIndex, int eachNoteIndex)
    {
        playerIndex = handy.GetCorrectIndex(playerIndex, GetMaxPlayerIndex(), -1);
        eachNoteIndex = handy.GetCorrectIndex(eachNoteIndex, GetMaxNoteIndex(playerIndex), -1);
        return playerIndex == -1 && eachNoteIndex == -1 ? noteGeneratorScript.startNote : noteGeneratorScript.notes[playerIndex][eachNoteIndex];
    }
    public NotePrefab GetNoteScript(int playerIndex, int eachNoteIndex)
    {
        playerIndex = handy.GetCorrectIndex(playerIndex, GetMaxPlayerIndex(), -1);
        eachNoteIndex = handy.GetCorrectIndex(eachNoteIndex, GetMaxNoteIndex(playerIndex), -1);
        return playerIndex == -1 && eachNoteIndex == -1 ? noteGeneratorScript.startNoteScript : noteGeneratorScript.noteScripts[playerIndex][eachNoteIndex];
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
        for (int i = 1; i < GetWorldInfoCount(); i++)
        {
            if (maxPlayerCount < GetWorldInfo(i).playerInfo.Length)
            {
                maxPlayerCount = GetWorldInfo(i).playerInfo.Length;
            }
        }
        return maxPlayerCount;
    }
    public int GetMaxPlayerIndex()
    {
        return GetMaxPlayerCount() - 1;
    }
    public int GetWorldInfoCount()
    {
        return worldReaderScript.worldInfos.Count;
    }
    public int GetMaxWorldInfoIndex()
    {
        return GetWorldInfoCount() - 1;
    }
    public int GetNoteCount(int playerIndex)
    {
        int noteCount = 0;
        for (int i = 1; i < GetWorldInfoCount(); i++)
        {
            if (GetWorldInfo(i).noteInfo.tarPlayerIndex == playerIndex)
                noteCount++;
        }
        return noteCount;
    }
    public int GetMaxNoteIndex(int playerIndex)
    {
        playerIndex = handy.GetCorrectIndex(playerIndex, GetMaxPlayerIndex());
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
        return playerControllerScript.players[handy.GetCorrectIndex(playerIndex, GetMaxPlayerIndex())];
    }
    public Player GetPlayerScript(int playerIndex)
    {
        return playerControllerScript.playerScripts[handy.GetCorrectIndex(playerIndex, GetMaxPlayerIndex())];
    }
    public GameObject GetPlayerSide(int playerIndex)
    {
        return playerControllerScript.playerSides[handy.GetCorrectIndex(playerIndex, GetMaxPlayerIndex())];
    }
    public SpriteRenderer GetPlayerSideRend(int playerIndex)
    {
        return playerControllerScript.playerSideRends[handy.GetCorrectIndex(playerIndex, GetMaxPlayerIndex())];
    }
    public GameObject GetPlayerCenter(int playerIndex)
    {
        return playerControllerScript.playerCenters[handy.GetCorrectIndex(playerIndex, GetMaxPlayerIndex())];
    }
    public SpriteRenderer GetPlayerCenterRend(int playerIndex)
    {
        return playerControllerScript.playerCenterRends[handy.GetCorrectIndex(playerIndex, GetMaxPlayerIndex())];
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
        playerIndex = handy.GetCorrectIndex(playerIndex, GetMaxPlayerIndex());
        if (GetNoteScript(playerIndex, eachNoteIndex) != null)
            return GetNoteScript(playerIndex, eachNoteIndex).waitElapsedSecs01;
        return 0f;
    }
    public float GetHoldElapsedSecs01(int playerIndex, int eachNoteIndex)
    {
        playerIndex = handy.GetCorrectIndex(playerIndex, GetMaxPlayerIndex());
        if (GetNoteScript(playerIndex, eachNoteIndex) != null)
            return GetNoteScript(playerIndex, eachNoteIndex).holdElapsedSecs01;
        return 0f;
    }
    public TweenInfo<float> CorrectDegTween(TweenInfo<float> degTween, int dir)
    {
        TweenInfo<float> degTweenTemp = degTween.Clone();
        dir = (int)handy.GetSign0Is0(dir);
        degTweenTemp.endValue += dir * degTweenTemp.startValue > dir * degTweenTemp.endValue ? dir * 360f : 0f;
        return degTweenTemp;
    }
    PlayManager()
    {
        MaxHPCount = 15;
    }
}
