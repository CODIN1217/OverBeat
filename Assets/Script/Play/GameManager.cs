using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System.Diagnostics;
using System;
using System.Linq;
using System.Text;
using Debug = UnityEngine.Debug;

public class GameManager : MonoBehaviour
{
    public int InputCount;
    public int missCount;
    public List<List<bool>> isKeyDowns;
    public List<List<bool>> isKeyPresses;
    public List<List<bool>> isKeyUps;
    public int lastHitedNoteIndex;
    public bool isPause;
    public bool isGameOver;
    public bool isEnd;
    public bool isAutoPlay;
    bool isInputted;
    bool isInputted_temp;
    static GameManager instance = null;
    public List<List<KeyCode>> canInputKeys;
    // public float elapsedTimeWhenNeedlessInput01;
    // public float elapsedTimeWhenNeedInput01;
    public float sumNoteAccuracy01;
    public float noteWaitTime;
    public float progress01;
    public float accuracy01;
    public float HP01;
    public int MaxMissCount;
    public readonly int MaxHPCount;
    public float toleranceTimeWhenNeedInput;
    public Handy handy;
    public Vector3 curNotePos;
    // WorldInfo worldInfo;
    // public List<JudgmentType> judgmentTypes;
    // public JudgmentType[] judgmentTypes;
    // public Stopwatch[] partOfElapsedTime;
    // public Stopwatch/* [] */ totalElapsedTime;
    // public float[] partOfElapsedSeconds;
    // public float/* [] */ totalElapsedSeconds;
    // public bool[,] isNotesActived;
    // public bool[,] isNotesEnded;
    // public float[,] notesActivedSeconds;
    // public float[,] notesActivedTotalSeconds;
    public int[] closestNoteIndex;
    public int curWorldInfoIndex;
    // public int lastActivatedNoteIndex;
    GameManager GM;
    public int totalNoteCount;
    void Awake()
    {
        GM = GameManager.Property;
        instance = this;
        isPause = true;
        canInputKeys = new List<List<KeyCode>>() { new List<KeyCode>() { KeyCode.W, KeyCode.D, KeyCode.S, KeyCode.A }, new List<KeyCode>() { KeyCode.I, KeyCode.L, KeyCode.K, KeyCode.J } };
        accuracy01 = 1f;
        // totalElapsedTime = new Stopwatch()/* [handy.GetPlayerCount()] */;
        // totalElapsedSeconds = new float[handy.GetPlayerCount()];
        // isNotesActived = new bool[handy.GetPlayerCount(), handy.GetWorldInfoCount()];
        // isNotesEnded = new bool[handy.GetPlayerCount(), handy.GetWorldInfoCount()];
        // notesActivedSeconds = new float[handy.GetPlayerCount(), handy.GetWorldInfoCount()];
        // notesActivedTotalSeconds = new float[handy.GetPlayerCount(), handy.GetWorldInfoCount()];
        closestNoteIndex = new int[handy.GetPlayerCount()];
        /* for (int i = 0; i < handy.GetPlayerCount(); i++)
        {
            // partOfElapsedTime[i] = new Stopwatch();
            // partOfElapsedTime[i].Reset();
            // partOfElapsedTime[i].Stop();
            totalElapsedTime[i] = new Stopwatch();
            totalElapsedTime[i].Reset();
            totalElapsedTime[i].Stop();
        } */
        // judgmentTypes = new JudgmentType[handy.GetPlayerCount()];
        isKeyDowns = new List<List<bool>>();
        isKeyPresses = new List<List<bool>>();
        isKeyUps = new List<List<bool>>();
        // isAutoPlay = true;
        handy.RepeatCode((i) => InfoViewer.Property.SetInfo(this.name, nameof(closestNoteIndex), () => closestNoteIndex[i], i), closestNoteIndex.Length);
        InfoViewer.Property.SetInfo(this.name, nameof(curWorldInfoIndex), () => curWorldInfoIndex);
    }
    void Update()
    {
        // worldInfo = handy.GetWorldInfo();
        Time.timeScale = isPause ? 0f : 1f;
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
        totalNoteCount = handy.GetPlayerCount() * handy.GetNoteCount();
        progress01 = (float)lastHitedNoteIndex / (float)totalNoteCount;
        MaxMissCount = (int)Mathf.Clamp(Mathf.Floor((float)totalNoteCount * 0.4f), 1f, (float)MaxHPCount);
        HP01 = 1f - (float)missCount / MaxMissCount;
        if (isPause)
        {
            /* for (int i = 0; i < handy.GetPlayerCount(); i++)
            {
                // partOfElapsedTime[i].Stop();
                totalElapsedTime[i].Stop();
            } */
            // totalElapsedTime.Stop();
            return;
        }

        // totalElapsedTime.Start();
        // totalElapsedSeconds = totalElapsedTime.ElapsedMilliseconds * 0.001f;
        /* for (int i = 0; i < handy.GetPlayerCount(); i++)
        {
            // totalElapsedTime[i].Start();
            // SetTotalElapsedSeconds(i);
            for (int j = lastActivatedNoteIndex; j < handy.GetWorldInfoCount(); j++)
            {
                // partOfElapsedTime[i].Start();

                // SetPartOfElapsedSeconds(i);
                // float noteActivedSeconds = handy.GetNoteWaitTime(i) + (handy.GetNoteLengthTime(i) == 0f ? handy.judgmentRange : handy.GetNoteLengthTime(i));
                int nextNoteIndex = handy.GetCorrectIndex(j + 1, handy.GetMaxWorldInfoIndex());
                if (notesActivedTotalSeconds[i, j] <= totalElapsedSeconds)
                {
                    // worldInfoIndex++;
                    handy.GetNote(i, nextNoteIndex).SetActive(true);
                    handy.GetNoteScript(i, nextNoteIndex).toleranceTimeWhenAwake = totalElapsedSeconds - notesActivedTotalSeconds[i, j];
                    lastActivatedNoteIndex = nextNoteIndex;
                    // partOfElapsedTime[i].Reset();
                    // isNotesActived[i, nextNoteIndex] = true;
                }
            }
        } */
        UpdateClosestNoteIndex();
        if (isAutoPlay)
        {
            /* for (int i = 0; i < handy.GetPlayerCount(); i++)
            {
                if (handy.GetPlayer(i).activeSelf)
                {
                    // if (handy.noteIndexes[i] != handy.noteGeneratorScript.closestNoteScripts[i].myNoteIndex)
                    // {
                    //     isInputted = false;
                    // }
                    if (handy.GetJudgmentValue(i) <= handy.bestJudgmentRange)
                    {
                        if (!isInputted)
                        {
                            isKeyDowns[i][handy.GetCorrectNextDegIndex(i, handy.GetWorldInfo().noteInfo[i].nextDegIndex - 1)] = true;
                            InputCount++;
                            handy.noteGeneratorScript.closestNoteScripts[i].isInputted = true;
                            isInputted = true;
                        }
                    }
                    if (isInputted && handy.closestNoteScripts[i].noteLengthTime != 0f)
                    {
                        isKeyPresses[i][handy.GetCorrectNextDegIndex(i, handy.GetWorldInfo().noteInfo[i].nextDegIndex - 1)] = true;
                    }
                    if (isInputted)
                        isKeyUps[i][handy.GetCorrectNextDegIndex(i, handy.GetWorldInfo().noteInfo[i].nextDegIndex - 1)] = true;
                }
            } */
        }
        else
        {
            for (int i = 0; i < handy.GetPlayerCount(); i++)
            {
                if (handy.GetPlayer(i).activeSelf)
                {
                    for (int j = 0; j < canInputKeys[i].Count; j++)
                    {
                        if (Input.GetKeyDown(canInputKeys[i][j]))
                        {
                            isKeyDowns[i][j] = true;
                            InputCount++;
                            handy.closestNoteScripts[i].isInputted = true;
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
        for (int i = 0; i < handy.GetPlayerCount(); i++)
        {
            if (GetIsKeyDown(i))
            {
                float noteAccuracy01 = 1f;
                if (GetIsProperKeyDown(i))
                {
                    handy.WriteLog(this.name + " / ProperKeyDown");
                    handy.judgmentGenScript.SetJudgmentText(i, GetJudgment(i, handy.GetJudgmentValue(i), () => { noteAccuracy01 = Mathf.Clamp01(1f - handy.GetJudgmentValue(i)); CountMissNote(); }));
                }
                else
                {
                    handy.WriteLog(this.name + " / WrongKeyDown", $"playerIndex : {i}", "noteIndex : " + closestNoteIndex[i].ToString());
                    noteAccuracy01 = 0f;
                    CountMissNote();
                    handy.judgmentGenScript.SetJudgmentText(i, JudgmentType.Miss);
                }
                /* judgmentTypes[i] = JudgmentType.Perfect;
                if (handy.GetJudgmentValue(i) > handy.bestJudgmentRange && handy.GetJudgmentValue(i) <= handy.bestJudgmentRange * 2f)
                {
                    judgmentTypes[i] = JudgmentType.Great;
                }
                else if (handy.GetJudgmentValue(i) > handy.bestJudgmentRange * 2f && handy.GetJudgmentValue(i) <= handy.bestJudgmentRange * 3f)
                {
                    judgmentTypes[i] = JudgmentType.Good;
                }
                else if (handy.GetJudgmentValue(i) > handy.bestJudgmentRange * 3f && handy.GetJudgmentValue(i) <= handy.bestJudgmentRange * 4f)
                {
                    judgmentTypes[i] = JudgmentType.Bad;
                    noteAccuracy01 = Mathf.Clamp01(1f - handy.GetJudgmentValue(i));
                    CountMissNote();
                }
                else if (handy.GetJudgmentValue(i) > handy.bestJudgmentRange * 4f)
                {
                    judgmentTypes[i] = JudgmentType.Miss;
                    noteAccuracy01 = Mathf.Clamp01(1f - handy.GetJudgmentValue(i));
                    CountMissNote();
                } */
                sumNoteAccuracy01 += noteAccuracy01;
                SetAccuracy01();
            }
        }
        if (HP01 <= 0f)
        {
            isGameOver = true;
            isPause = true;
        }
        if (progress01 >= 1f)
        {
            isEnd = true;
            isPause = true;
        }
    }
    void UpdateClosestNoteIndex()
    {
        for (int i = 0; i < handy.GetPlayerCount(); i++)
        {
            for (int j = handy.GetMaxWorldInfoIndex(); j >= closestNoteIndex[i]; j--)
            {
                if (handy.GetNote(i, j).activeSelf/*  && handy.GetNoteScript(i, j).myNoteIndex > curWorldInfoIndex */)
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
        playerIndex = handy.GetCorrectIndex(playerIndex, handy.GetMaxPlayerIndex());
        if (judgmentValue > handy.bestJudgmentRange[playerIndex] && judgmentValue <= handy.bestJudgmentRange[playerIndex] * 2f)
        {
            judgmentType = JudgmentType.Great;
            codeOnJudgGood();
        }
        else if (judgmentValue > handy.bestJudgmentRange[playerIndex] * 2f && judgmentValue <= handy.bestJudgmentRange[playerIndex] * 3f)
        {
            judgmentType = JudgmentType.Good;
            codeOnJudgGood();
        }
        else if (judgmentValue > handy.bestJudgmentRange[playerIndex] * 3f && judgmentValue <= handy.bestJudgmentRange[playerIndex] * 4f)
        {
            judgmentType = JudgmentType.Bad;
            codeOnJudgBad();
        }
        else if (judgmentValue > handy.bestJudgmentRange[playerIndex] * 4f)
        {
            judgmentType = JudgmentType.Miss;
            codeOnJudgBad();
        }
        return judgmentType;
    }
    /* void SetPartOfElapsedSeconds(int playerIndex)
    {
        playerIndex = handy.GetCorrectIndex(playerIndex, handy.GetMaxPlayerIndex());
        partOfElapsedSeconds[playerIndex] = partOfElapsedTime[playerIndex].ElapsedMilliseconds * 0.001f + handy.GetNoteScript(playerIndex).toleranceTimeWhenAwake;
    } */
    /* void SetTotalElapsedSeconds(int playerIndex)
    {
        playerIndex = handy.GetCorrectIndex(playerIndex, handy.GetMaxPlayerIndex());
        totalElapsedSeconds[playerIndex] = totalElapsedTime[playerIndex].ElapsedMilliseconds * 0.001f;
    } */
    public static GameManager Property
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
    /* public void SetMissJudgment(int playerIndex)
    {
        handy.judgmentGenScript.SetJudgmentText(playerIndex, JudgmentType.Miss);
        CountMissNote();
        SetAccuracy01();
    } */

    public void SetAccuracy01()
    {
        accuracy01 = sumNoteAccuracy01 / InputCount;
    }
    public bool GetIsKeyDown(int playerIndex)
    {
        int KeyDownCount = 0;
        for (int i = 0; i < isKeyDowns[playerIndex].Count; i++)
        {
            if (isKeyDowns[handy.GetCorrectIndex(playerIndex, handy.GetMaxPlayerIndex())][i])
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
            if (isKeyPresses[handy.GetCorrectIndex(playerIndex, handy.GetMaxPlayerIndex())][i])
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
            if (isKeyUps[handy.GetCorrectIndex(playerIndex, handy.GetMaxPlayerIndex())][i])
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
        playerIndex = handy.GetCorrectIndex(playerIndex, handy.GetMaxPlayerIndex());
        if (nextDegIndex == null)
            nextDegIndex = handy.GetWorldInfo(closestNoteIndex[playerIndex]).noteInfo[playerIndex].startDegIndex;
        if (isKeyDowns[playerIndex][handy.GetCorrectNextDegIndex(playerIndex, (int)nextDegIndex, closestNoteIndex[playerIndex])])
        {
            return true;
        }
        return false;
    }
    public bool GetIsProperKeyPress(int playerIndex, int? nextDegIndex = null)
    {

        playerIndex = handy.GetCorrectIndex(playerIndex, handy.GetMaxPlayerIndex());
        if (nextDegIndex == null)
            nextDegIndex = handy.GetWorldInfo(closestNoteIndex[playerIndex]).noteInfo[playerIndex].startDegIndex;
        if (isKeyPresses[playerIndex][handy.GetCorrectNextDegIndex(playerIndex, (int)nextDegIndex, closestNoteIndex[playerIndex])])
        {
            return true;
        }
        return false;
    }
    public bool GetIsProperKeyUp(int playerIndex, int? nextDegIndex = null)
    {

        playerIndex = handy.GetCorrectIndex(playerIndex, handy.GetMaxPlayerIndex());
        if (nextDegIndex == null)
            nextDegIndex = handy.GetWorldInfo(closestNoteIndex[playerIndex]).noteInfo[playerIndex].startDegIndex;
        if (isKeyUps[playerIndex][handy.GetCorrectNextDegIndex(playerIndex, (int)nextDegIndex, closestNoteIndex[playerIndex])])
        {
            return true;
        }
        return false;
    }
    GameManager()
    {
        MaxHPCount = 15;
    }
}
