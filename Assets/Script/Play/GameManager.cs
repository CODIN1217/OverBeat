using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System.Diagnostics;
using System;
using Debug = UnityEngine.Debug;

public class GameManager : MonoBehaviour
{
    int notesCount;
    public int InputCount;
    public int missCount;
    public List<List<bool>> isKeyDowns;
    public List<List<bool>> isKeyPresses;
    public List<List<bool>> isKeyUps;
    public int lastHitedNoteIndex;
    public bool isPause;
    public bool isGameOver;
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
    public List<JudgmentType> judgmentTypes;
    public Stopwatch totalElapsedTime;
    public float totalElapsedSeconds;
    public float toleranceTimeWhenAwake;
    void Awake()
    {
        instance = this;
        isPause = true;
        canInputKeys = new List<List<KeyCode>>() { new List<KeyCode>() { KeyCode.W, KeyCode.D, KeyCode.S, KeyCode.A }, new List<KeyCode>() { KeyCode.I, KeyCode.L, KeyCode.K, KeyCode.J } };
        accuracy01 = 1f;
        totalElapsedTime = new Stopwatch();
        totalElapsedTime.Reset();
        judgmentTypes = new List<JudgmentType>();
        // isAutoPlay = true;
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
        notesCount = handy.worldReaderScript.notesCount;
        progress01 = (float)lastHitedNoteIndex / (float)notesCount;
        MaxMissCount = (int)Mathf.Clamp(Mathf.Floor(notesCount * 0.4f), 1f, (float)MaxHPCount);
        HP01 = 1f - (float)missCount / MaxMissCount;
        if (isPause)
        {
            totalElapsedTime.Stop();
            return;
        }
        totalElapsedTime.Start();
        totalElapsedSeconds = totalElapsedTime.ElapsedMilliseconds * 0.001f;
        for (int i = 0; i < handy.GetTotalMaxPlayerIndex(); i++)
        {
            for (int j = 0; j <= notesCount; j++)
            {
                if (handy.GetWorldInfo(i, j).NoteInfo.AwakeTime <= totalElapsedSeconds)
                {
                    handy.noteGenerator.transform.GetChild(j).gameObject.SetActive(true);
                    toleranceTimeWhenAwake = totalElapsedSeconds - handy.GetWorldInfo(i, j).NoteInfo.AwakeTime;
                }
            }
        }
        if (isAutoPlay)
        {
            for (int i = 0; i < handy.GetTotalMaxPlayerIndex(); i++)
            {
                if (handy.GetPlayer(i).activeSelf)
                {
                    if (handy.noteIndexes[i] != handy.noteGeneratorScript.closestNoteScripts[i].myNoteIndex)
                    {
                        isInputted = false;
                    }
                    if (handy.GetJudgmentValue(i) <= handy.bestJudgmentRanges[i])
                    {
                        if (!isInputted)
                        {
                            isKeyDowns[i][handy.GetCorrectNextDegIndex(i, handy.GetWorldInfo(i).NoteInfo.NextDegIndex - 1)] = true;
                            InputCount++;
                            handy.noteGeneratorScript.closestNoteScripts[i].isInputted = true;
                            isInputted = true;
                        }
                    }
                    if (isInputted && handy.closestNoteScripts[i].noteLengthTime != 0f)
                    {
                        isKeyPresses[i][handy.GetCorrectNextDegIndex(i, handy.GetWorldInfo(i).NoteInfo.NextDegIndex - 1)] = true;
                    }
                    if (isInputted)
                        isKeyUps[i][handy.GetCorrectNextDegIndex(i, handy.GetWorldInfo(i).NoteInfo.NextDegIndex - 1)] = true;
                }
            }
        }
        else
        {
            for (int i = 0; i < handy.GetTotalMaxPlayerIndex(); i++)
            {
                if (handy.GetPlayer(i).activeSelf)
                {
                    for (int j = 0; j < canInputKeys[i].Count; j++)
                    {
                        if (Input.GetKeyDown(canInputKeys[i][j]))
                        {
                            isKeyDowns[i][j] = true;
                            InputCount++;
                            handy.noteGeneratorScript.closestNoteScripts[i].isInputted = true; ;
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
        for (int i = 0; i <= handy.GetTotalMaxPlayerIndex(); i++)
        {
            if (GetIsKeyDown(i))
            {
                judgmentTypes[i] = JudgmentType.Perfect;
                float noteAccuracy01 = 1f;
                if (handy.GetJudgmentValue(i) > handy.bestJudgmentRanges[i] && handy.GetJudgmentValue(i) <= handy.bestJudgmentRanges[i] * 2f)
                {
                    judgmentTypes[i] = JudgmentType.Great;
                }
                else if (handy.GetJudgmentValue(i) > handy.bestJudgmentRanges[i] * 2f && handy.GetJudgmentValue(i) <= handy.bestJudgmentRanges[i] * 3f)
                {
                    judgmentTypes[i] = JudgmentType.Good;
                }
                else if (handy.GetJudgmentValue(i) > handy.bestJudgmentRanges[i] * 3f && handy.GetJudgmentValue(i) <= handy.bestJudgmentRanges[i] * 4f)
                {
                    judgmentTypes[i] = JudgmentType.Bad;
                    noteAccuracy01 = Mathf.Clamp01(1f - handy.GetJudgmentValue(i));
                    CountMissNote();
                }
                else if (handy.GetJudgmentValue(i) > handy.bestJudgmentRanges[i] * 4f)
                {
                    judgmentTypes[i] = JudgmentType.Miss;
                    noteAccuracy01 = Mathf.Clamp01(1f - handy.GetJudgmentValue(i));
                    CountMissNote();
                }
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
            isPause = true;
        }
    }
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
    public void SetMissJudgment(int playerIndex)
    {
        handy.judgmentGenScript.SetJudgmentText(playerIndex, JudgmentType.Miss);
        CountMissNote();
        InputCount++;
        SetAccuracy01();
    }

    public void SetAccuracy01()
    {
        accuracy01 = sumNoteAccuracy01 / InputCount;
    }
    public bool GetIsKeyDown(int playerIndex)
    {
        int KeyDownCount = 0;
        for (int i = 0; i < isKeyDowns.Count; i++)
        {
            if (isKeyDowns[handy.GetCorrectIndex(playerIndex, handy.GetTotalMaxPlayerIndex())][i])
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
        for (int i = 0; i < isKeyPresses.Count; i++)
        {
            if (isKeyPresses[handy.GetCorrectIndex(playerIndex, handy.GetTotalMaxPlayerIndex())][i])
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
        for (int i = 0; i < isKeyUps.Count; i++)
        {
            if (isKeyUps[handy.GetCorrectIndex(playerIndex, handy.GetTotalMaxPlayerIndex())][i])
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
        playerIndex = handy.GetCorrectIndex(playerIndex, handy.GetTotalMaxPlayerIndex());
        if (nextDegIndex == null)
            nextDegIndex = handy.GetWorldInfo(playerIndex, handy.noteIndexes[playerIndex] - 1).NoteInfo.NextDegIndex;
        if (isKeyDowns[playerIndex][(int)Mathf.Clamp((float)nextDegIndex, 0f, float.MaxValue)])
        {
            return true;
        }
        return false;
    }
    public bool GetIsProperKeyPress(int playerIndex, int? nextDegIndex = null)
    {

        playerIndex = handy.GetCorrectIndex(playerIndex, handy.GetTotalMaxPlayerIndex());
        if (nextDegIndex == null)
            nextDegIndex = handy.GetWorldInfo(playerIndex, handy.noteIndexes[playerIndex] - 1).NoteInfo.NextDegIndex;
        if (isKeyPresses[playerIndex][(int)Mathf.Clamp((float)nextDegIndex, 0f, float.MaxValue)])
        {
            return true;
        }
        return false;
    }
    public bool GetIsProperKeyUp(int playerIndex, int? nextDegIndex = null)
    {

        playerIndex = handy.GetCorrectIndex(playerIndex, handy.GetTotalMaxPlayerIndex());
        if (nextDegIndex == null)
            nextDegIndex = handy.GetWorldInfo(playerIndex, handy.noteIndexes[playerIndex] - 1).NoteInfo.NextDegIndex;
        if (isKeyUps[playerIndex][(int)Mathf.Clamp((float)nextDegIndex, 0f, float.MaxValue)])
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
