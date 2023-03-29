using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System.Diagnostics;
using System;
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
    public JudgmentType[] judgmentTypes;
    public Stopwatch[] partOfElapsedTime;
    public float[] partOfElapsedSeconds;
    public int worldInfoIndex;
    void Awake()
    {
        instance = this;
        isPause = true;
        canInputKeys = new List<List<KeyCode>>() { new List<KeyCode>() { KeyCode.W, KeyCode.D, KeyCode.S, KeyCode.A }, new List<KeyCode>() { KeyCode.I, KeyCode.L, KeyCode.K, KeyCode.J } };
        accuracy01 = 1f;
        partOfElapsedTime = new Stopwatch[handy.GetPlayerCount()];
        partOfElapsedSeconds = new float[handy.GetPlayerCount()];
        for (int i = 0; i < handy.GetPlayerCount(); i++)
        {
            partOfElapsedTime[i] = new Stopwatch();
            partOfElapsedTime[i].Reset();
            partOfElapsedTime[i].Stop();
        }
        judgmentTypes = new JudgmentType[handy.GetPlayerCount()];
        isKeyDowns = new List<List<bool>>();
        isKeyPresses = new List<List<bool>>();
        isKeyUps = new List<List<bool>>();
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
        progress01 = (float)lastHitedNoteIndex / (float)handy.GetNoteCount();
        MaxMissCount = (int)Mathf.Clamp(Mathf.Floor((float)handy.GetNoteCount() * 0.4f), 1f, (float)MaxHPCount);
        HP01 = 1f - (float)missCount / MaxMissCount;
        if (isPause)
        {
            foreach (var POET in partOfElapsedTime)
            {
                POET.Stop();
            }
            return;
        }
        foreach (var POET in partOfElapsedTime)
        {
            POET.Start();
        }
        for (int i = 0; i < handy.GetPlayerCount(); i++)
        {
            SetPartOfElapsedSeconds(i);
            float noteActivedSeconds = handy.GetNoteWaitTime(i) + (handy.GetNoteLengthTime(i) == 0f ? handy.judgmentRange : handy.GetNoteLengthTime(i));
            if (noteActivedSeconds <= partOfElapsedSeconds[i])
            {
                worldInfoIndex++;
                handy.GetNote(i).SetActive(true);
                handy.GetNoteScript(i).toleranceTimeWhenAwake = partOfElapsedSeconds[i] - noteActivedSeconds;
                partOfElapsedTime[i].Reset();
            }
        }
        if (isAutoPlay)
        {
            for (int i = 0; i < handy.GetPlayerCount(); i++)
            {
                if (handy.GetPlayer(i).activeSelf)
                {
                    /* if (handy.noteIndexes[i] != handy.noteGeneratorScript.closestNoteScripts[i].myNoteIndex)
                    {
                        isInputted = false;
                    } */
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
            }
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
                            handy.closestNoteScripts[i].isInputted = true; ;
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
                judgmentTypes[i] = JudgmentType.Perfect;
                float noteAccuracy01 = 1f;
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
            isEnd = true;
            isPause = true;
        }
    }
    void SetPartOfElapsedSeconds(int playerIndex)
    {
        playerIndex = handy.GetCorrectIndex(playerIndex, handy.GetMaxPlayerIndex());
        partOfElapsedSeconds[playerIndex] = partOfElapsedTime[playerIndex].ElapsedMilliseconds * 0.001f + handy.GetNoteScript(playerIndex).toleranceTimeWhenAwake/*  - lastNoteInitSeconds[playerIndex] */;
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
        for (int i = 0; i < isKeyPresses.Count; i++)
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
        for (int i = 0; i < isKeyUps.Count; i++)
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
            nextDegIndex = handy.GetWorldInfo(worldInfoIndex - 1).noteInfo[playerIndex].nextDegIndex;
        if (isKeyDowns[playerIndex][handy.GetCorrectIndex((int)nextDegIndex)])
        {
            return true;
        }
        return false;
    }
    public bool GetIsProperKeyPress(int playerIndex, int? nextDegIndex = null)
    {

        playerIndex = handy.GetCorrectIndex(playerIndex, handy.GetMaxPlayerIndex());
        if (nextDegIndex == null)
            nextDegIndex = handy.GetWorldInfo(worldInfoIndex - 1).noteInfo[playerIndex].nextDegIndex;
        if (isKeyPresses[playerIndex][handy.GetCorrectIndex((int)nextDegIndex)])
        {
            return true;
        }
        return false;
    }
    public bool GetIsProperKeyUp(int playerIndex, int? nextDegIndex = null)
    {

        playerIndex = handy.GetCorrectIndex(playerIndex, handy.GetMaxPlayerIndex());
        if (nextDegIndex == null)
            nextDegIndex = handy.GetWorldInfo(worldInfoIndex - 1).noteInfo[playerIndex].nextDegIndex;
        if (isKeyUps[playerIndex][handy.GetCorrectIndex((int)nextDegIndex)])
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
