using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System.Diagnostics;
using System;
using System.Linq;
using System.Text;
using Debug = UnityEngine.Debug;

public class PlayGameManager : MonoBehaviour
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
    bool isAwake;
    bool isInputted;
    bool isInputted_temp;
    static PlayGameManager instance = null;
    public List<List<KeyCode>> canInputKeys;
    public float sumNoteAccuracy01;
    public float noteWaitSecs;
    public float progress01;
    public float accuracy01;
    public float HP01;
    public int MaxMissCount;
    public readonly int MaxHPCount;
    public float toleranceSecsWhenNeedInput;
    public Handy handy;
    public Vector3 curNotePos;
    public int[] closestNoteIndex;
    public int curWorldInfoIndex;
    public int totalNoteCount;
    Stopwatch totalElapsedTime;
    float totalElapsedSecs;
    float totalGamePlaySecs;
    void Awake()
    {
        instance = this;
        isPause = true;
        canInputKeys = new List<List<KeyCode>>() { new List<KeyCode>() { KeyCode.W, KeyCode.D, KeyCode.S, KeyCode.A }, new List<KeyCode>() { KeyCode.I, KeyCode.L, KeyCode.K, KeyCode.J } };
        accuracy01 = 1f;
        closestNoteIndex = new int[GetMaxPlayerCount()];
        isKeyDowns = new List<List<bool>>();
        isKeyPresses = new List<List<bool>>();
        isKeyUps = new List<List<bool>>();
        totalElapsedTime = new Stopwatch();
        totalElapsedTime.Reset();
        totalElapsedTime.Stop();
        handy.RepeatCode((i) => InfoViewer.Property.SetInfo(this.name, nameof(closestNoteIndex), () => closestNoteIndex[i], i), closestNoteIndex.Length);
        InfoViewer.Property.SetInfo(this.name, nameof(curWorldInfoIndex), () => curWorldInfoIndex);
        judgmentRange = new float[GetMaxPlayerCount()];
        bestJudgmentRange = new float[GetMaxPlayerCount()];
        isAwake = true;
    }
    void Update()
    {
        UpdateJudgmentRange();
        closestNotes = noteGeneratorScript.closestNotes;
        closestNoteScripts = noteGeneratorScript.closestNoteScripts;
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
        if (isAwake)
        {
            for (int i = 0; i < GetMaxPlayerCount(); i++)
            {
                totalNoteCount += GetNoteCountWithOutStartNote(i);
            }
            WorldInfo lastWorldInfo = GetWorldInfo(GetMaxWorldInfoIndex());
            NotePrefab lastNoteScript = GetNoteScript(lastWorldInfo.noteInfo.tarPlayerIndex, lastWorldInfo.noteInfo.eachNoteIndex);
            totalGamePlaySecs = lastWorldInfo.noteInfo.awakeSecs + lastNoteScript.noteWaitSecs + lastNoteScript.noteLengthSecs;
            isAwake = false;
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
        for (int i = 0; i < GetWorldInfoCount(); i++)
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
                        curNoteScript.toleranceSecsWhenAwake = totalElapsedSecs - GetWorldInfo(i).noteInfo.awakeSecs;
                    }
                }
            }
        }
        UpdateClosestNoteIndex();
        if (isAutoPlay)
        {
        }
        else
        {
            for (int i = 0; i < GetMaxPlayerCount(); i++)
            {
                if (GetPlayer(i).activeSelf)
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
        }
        for (int i = 0; i < GetMaxPlayerCount(); i++)
        {
            if (GetPlayer(i).activeSelf)
            {
                if (GetIsKeyDown(i))
                {
                    float noteAccuracy01 = 1f;
                    if (GetIsProperKeyDown(i))
                    {
                        judgmentGenScript.SetJudgmentText(i, GetJudgment(i, GetJudgmentValue(i), () => { noteAccuracy01 = Mathf.Clamp01(1f - GetJudgmentValue(i)); CountMissNote(); }));
                    }
                    else
                    {
                        noteAccuracy01 = 0f;
                        CountMissNote();
                        judgmentGenScript.SetJudgmentText(i, JudgmentType.Miss);
                    }
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
    public void SetCurWorldInfoIndex(float waitSec, int eachNoteIndex)
    {
        StartCoroutine(SetCurWorldInfoIndex_co(waitSec, eachNoteIndex));
    }
    IEnumerator SetCurWorldInfoIndex_co(float waitSec, int eachNoteIndex)
    {
        yield return new WaitForSeconds(waitSec);
        curWorldInfoIndex += (int)Mathf.Clamp01(eachNoteIndex);
    }
    void UpdateClosestNoteIndex()
    {
        for (int i = 0; i < GetMaxPlayerCount(); i++)
        {
            for (int j = GetMaxNoteIndex(i); j >= closestNoteIndex[i]; j--)
            {
                if (GetNote(i, j).activeSelf)
                {
                    closestNoteIndex[i] = j;
                }
            }
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
        if (judgmentValue > bestJudgmentRange[playerIndex] && judgmentValue <= bestJudgmentRange[playerIndex] * 2f)
        {
            judgmentType = JudgmentType.Great;
            codeOnJudgGood();
        }
        else if (judgmentValue > bestJudgmentRange[playerIndex] * 2f && judgmentValue <= bestJudgmentRange[playerIndex] * 3f)
        {
            judgmentType = JudgmentType.Good;
            codeOnJudgGood();
        }
        else if (judgmentValue > bestJudgmentRange[playerIndex] * 3f && judgmentValue <= bestJudgmentRange[playerIndex] * 4f)
        {
            judgmentType = JudgmentType.Bad;
            codeOnJudgBad();
        }
        else if (judgmentValue > bestJudgmentRange[playerIndex] * 4f)
        {
            judgmentType = JudgmentType.Miss;
            codeOnJudgBad();
        }
        return judgmentType;
    }
    public static PlayGameManager Property
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
        int KeyDownCount = 0;
        for (int i = 0; i < isKeyDowns[playerIndex].Count; i++)
        {
            if (isKeyDowns[handy.GetCorrectIndex(playerIndex, GetMaxPlayerIndex())][i])
            {
                KeyDownCount++;
            }
        }
        if (KeyDownCount > 0)
        {
            return true;
        }
        return false;
    }
    public bool GetIsKeyPress(int playerIndex)
    {
        int KeyPressCount = 0;
        for (int i = 0; i < isKeyPresses[playerIndex].Count; i++)
        {
            if (isKeyPresses[handy.GetCorrectIndex(playerIndex, GetMaxPlayerIndex())][i])
            {
                KeyPressCount++;
            }
        }
        if (KeyPressCount > 0)
        {
            return true;
        }
        return false;
    }
    public bool GetIsKeyUp(int playerIndex)
    {
        int KeyUpCount = 0;
        for (int i = 0; i < isKeyUps[playerIndex].Count; i++)
        {
            if (isKeyUps[handy.GetCorrectIndex(playerIndex, GetMaxPlayerIndex())][i])
            {
                KeyUpCount++;
            }
        }
        if (KeyUpCount > 0)
        {
            return true;
        }
        return false;
    }
    public bool GetIsProperKeyDown(int playerIndex, int? nextDegIndex = null)
    {
        playerIndex = handy.GetCorrectIndex(playerIndex, GetMaxPlayerIndex());
        if (nextDegIndex == null)
            nextDegIndex = GetWorldInfo(playerIndex, closestNoteIndex[playerIndex]).noteInfo.startDegIndex;
        if (isKeyDowns[playerIndex][GetCorrectNextDegIndex((int)nextDegIndex, playerIndex, closestNoteIndex[playerIndex])])
        {
            return true;
        }
        return false;
    }
    public bool GetIsProperKeyPress(int playerIndex, int? nextDegIndex = null)
    {

        playerIndex = handy.GetCorrectIndex(playerIndex, GetMaxPlayerIndex());
        if (nextDegIndex == null)
            nextDegIndex = GetWorldInfo(playerIndex, closestNoteIndex[playerIndex]).noteInfo.startDegIndex;
        if (isKeyPresses[playerIndex][GetCorrectNextDegIndex((int)nextDegIndex, playerIndex, closestNoteIndex[playerIndex])])
        {
            return true;
        }
        return false;
    }
    public bool GetIsProperKeyUp(int playerIndex, int? nextDegIndex = null)
    {

        playerIndex = handy.GetCorrectIndex(playerIndex, GetMaxPlayerIndex());
        if (nextDegIndex == null)
            nextDegIndex = GetWorldInfo(playerIndex, closestNoteIndex[playerIndex]).noteInfo.startDegIndex;
        if (isKeyUps[playerIndex][GetCorrectNextDegIndex((int)nextDegIndex, playerIndex, closestNoteIndex[playerIndex])])
        {
            return true;
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
                bestJudgmentRange[i] = judgmentRange[i] * 0.2f;
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
        for (int i = 0; i < GetWorldInfoCount(); i++)
        {
            WorldInfo curWorldInfo = GetWorldInfo(i);
            if (curWorldInfo.noteInfo.tarPlayerIndex == playerIndex && curWorldInfo.noteInfo.eachNoteIndex == eachNoteIndex)
                return curWorldInfo;
        }
        return null;
    }
    public float GetStartDeg(int playerIndex, int eachNoteIndex)
    {
        playerIndex = handy.GetCorrectIndex(playerIndex, GetMaxPlayerIndex());
        WorldInfo worldInfo = GetWorldInfo(playerIndex, eachNoteIndex);
        return worldInfo.playerInfo[playerIndex].stdDegs[worldInfo.noteInfo.startDegIndex];
    }
    public float GetEndDeg(int playerIndex, int eachNoteIndex)
    {
        playerIndex = handy.GetCorrectIndex(playerIndex, GetMaxPlayerIndex());
        WorldInfo worldInfo = GetWorldInfo(playerIndex, eachNoteIndex);
        return worldInfo.playerInfo[playerIndex].stdDegs[worldInfo.noteInfo.endDegIndex];
    }
    public float GetNoteWaitSecs(int playerIndex, int eachNoteIndex)
    {
        playerIndex = handy.GetCorrectIndex((int)playerIndex, GetMaxPlayerIndex());
        return (float)noteGeneratorScript.notesWaitSecs[playerIndex][handy.GetCorrectIndex((int)eachNoteIndex, GetMaxNoteIndex(playerIndex))];
    }
    public float GetNoteLengthSecs(int playerIndex, int eachNoteIndex)
    {
        playerIndex = handy.GetCorrectIndex((int)playerIndex, GetMaxPlayerIndex());
        return (float)noteGeneratorScript.notesLengthSecs[playerIndex][handy.GetCorrectIndex((int)eachNoteIndex, GetMaxNoteIndex(playerIndex))];
    }
    public GameObject GetNote(int playerIndex, int eachNoteIndex)
    {
        playerIndex = handy.GetCorrectIndex((int)playerIndex, GetMaxPlayerIndex());
        return noteGeneratorScript.notes[playerIndex][handy.GetCorrectIndex((int)eachNoteIndex, GetMaxNoteIndex(playerIndex))];
    }
    public NotePrefab GetNoteScript(int playerIndex, int eachNoteIndex)
    {
        playerIndex = handy.GetCorrectIndex((int)playerIndex, GetMaxPlayerIndex());
        return noteGeneratorScript.noteScripts[playerIndex][handy.GetCorrectIndex((int)eachNoteIndex, GetMaxNoteIndex(playerIndex))];
    }
    public float GetJudgmentValue(int playerIndex, float? elapsedSecsWhenNeedlessInput01 = null, float? elapsedSecsWhenNeedInput01 = null)
    {
        if (elapsedSecsWhenNeedlessInput01 == null)
            elapsedSecsWhenNeedlessInput01 = GetElapsedSecsWhenNeedlessInput01(playerIndex, closestNoteIndex[playerIndex]);
        if (elapsedSecsWhenNeedInput01 == null)
            elapsedSecsWhenNeedInput01 = GetElapsedSecsWhenNeedInput01(playerIndex, closestNoteIndex[playerIndex]);
        float judgmentValue = 1f;
        if (elapsedSecsWhenNeedInput01 > 0f)
            judgmentValue = (float)elapsedSecsWhenNeedInput01;
        else if (elapsedSecsWhenNeedlessInput01 >= 1f - judgmentRange[playerIndex])
            judgmentValue = 1f - (float)elapsedSecsWhenNeedlessInput01;
        return judgmentValue;
    }
    public int GetMaxStdDegCount(int playerIndex)
    {
        int maxStdDegCount = 0;
        for (int i = 0; i < GetWorldInfoCount(); i++)
        {
            int _playerIndex = handy.GetCorrectIndex(playerIndex, GetWorldInfo(i).playerInfo.Length - 1);
            if (maxStdDegCount < GetWorldInfo(i).playerInfo[_playerIndex].stdDegs.Length)
            {
                maxStdDegCount = GetWorldInfo(i).playerInfo[_playerIndex].stdDegs.Length;
            }
        }
        return maxStdDegCount;
    }
    public int GetCorrectNextDegIndex(int nextDegIndex, int playerIndex, int eachNoteIndex)
    {
        playerIndex = handy.GetCorrectIndex(playerIndex, GetMaxPlayerIndex());
        return nextDegIndex >= 0 ? nextDegIndex : nextDegIndex + GetWorldInfo(playerIndex, eachNoteIndex).playerInfo[playerIndex].stdDegs.Length;
    }
    public int GetMaxPlayerCount()
    {
        int maxPlayerCount = 0;
        for (int i = 0; i < GetWorldInfoCount(); i++)
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
    public int GetNoteCountWithOutStartNote(int playerIndex)
    {
        int noteCount = 0;
        for (int i = 0; i < GetWorldInfoCount(); i++)
        {
            if (GetWorldInfo(i).noteInfo.tarPlayerIndex == playerIndex && GetWorldInfo(i).noteInfo.eachNoteIndex != 0)
                noteCount++;
        }
        return noteCount;
    }
    public int GetMaxNoteIndexWithOutStartNote(int playerIndex)
    {
        playerIndex = handy.GetCorrectIndex((int)playerIndex, GetMaxPlayerIndex());
        return GetNoteCountWithOutStartNote(playerIndex) - 1;
    }
    public int GetNoteCount(int playerIndex)
    {
        int noteCount = 0;
        for (int i = 0; i < GetWorldInfoCount(); i++)
        {
            if (GetWorldInfo(i).noteInfo.tarPlayerIndex == playerIndex)
                noteCount++;
        }
        return noteCount;
    }
    public int GetMaxNoteIndex(int playerIndex)
    {
        playerIndex = handy.GetCorrectIndex((int)playerIndex, GetMaxPlayerIndex());
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
    public float GetElapsedSecsWhenNeedlessInput01(int playerIndex, int eachNoteIndex)
    {
        playerIndex = handy.GetCorrectIndex((int)playerIndex, GetMaxPlayerIndex());
        if (GetNoteScript(playerIndex, eachNoteIndex) != null)
            return GetNoteScript(playerIndex, eachNoteIndex).elapsedSecsWhenNeedlessInput01;
        return 0f;
    }
    public float GetElapsedSecsWhenNeedInput01(int playerIndex, int eachNoteIndex)
    {
        playerIndex = handy.GetCorrectIndex((int)playerIndex, GetMaxPlayerIndex());
        if (GetNoteScript(playerIndex, eachNoteIndex) != null)
            return GetNoteScript(playerIndex, eachNoteIndex).elapsedSecsWhenNeedInput01;
        return 0f;
    }
    public float[] GetCorrectStdDegs(float[] stdDegs)
    {
        float[] stdDegs_temp = new float[stdDegs.Length];
        for (int i = 0; i < stdDegs.Length; i++)
        {
            stdDegs_temp[i] = handy.GetCorrectDegMaxIs0(stdDegs[i]);
        }
        return stdDegs_temp;
    }
    PlayGameManager()
    {
        MaxHPCount = 15;
    }
}
