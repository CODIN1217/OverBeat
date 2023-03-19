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
    public List<bool> isKeyDowns;
    public List<bool> isKeyPresses;
    public List<bool> isKeyUps;
    public int lastHitedNoteIndex;
    public bool isPause;
    public bool isGameOver;
    public bool isAutoPlay;
    bool isInputted;
    bool isInputted_temp;
    static GameManager instance = null;
    public List<KeyCode> canInputKeys;
    public float elapsedTimeWhenNeedlessInput01;
    public float elapsedTimeWhenNeedInput01;
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
    WorldInfo worldInfo;
    public JudgmentType judgmentType;
    public Stopwatch totalElapsedTime;
    public float totalElapsedSeconds;
    public float toleranceTimeWhenAwake;
    void Awake()
    {
        instance = this;
        isPause = true;
        canInputKeys = new List<KeyCode>() { KeyCode.W, KeyCode.D, KeyCode.S, KeyCode.A };
        accuracy01 = 1f;
        totalElapsedTime.Reset();
        // isAutoPlay = true;
    }
    void Update()
    {
        worldInfo = handy.GetWorldInfo(handy.noteIndex);
        Time.timeScale = isPause ? 0f : 1f;
        isKeyDowns.Clear();
        for (int i = 0; i < canInputKeys.Count; i++)
        {
            isKeyDowns.Add(false);
        }
        isKeyPresses.Clear();
        for (int i = 0; i < canInputKeys.Count; i++)
        {
            isKeyPresses.Add(false);
        }
        isKeyUps.Clear();
        for (int i = 0; i < canInputKeys.Count; i++)
        {
            isKeyUps.Add(false);
        }
        notesCount = handy.worldReaderScript.notesCount;
        progress01 = (float)lastHitedNoteIndex / (float)notesCount;
        MaxMissCount = (int)Mathf.Clamp(Mathf.Floor(notesCount * 0.4f), 1f, (float)MaxHPCount);
        HP01 = 1f - (float)missCount / MaxMissCount;
        if (isPause || handy.noteIndex == 0)
        {
            totalElapsedTime.Stop();
            return;
        }
        totalElapsedTime.Start();
        totalElapsedSeconds = totalElapsedTime.ElapsedMilliseconds * 0.001f;
        for(int i = 0; i <= notesCount; i++){
            if(handy.GetWorldInfo(i).awakeTime <= totalElapsedSeconds){
                handy.noteGenerator.transform.GetChild(i).gameObject.SetActive(true);
                toleranceTimeWhenAwake = totalElapsedSeconds - handy.GetWorldInfo(i).awakeTime;
            }
        }
        SetElapsedTime01s();
        if (isAutoPlay)
        {
            if (handy.noteIndex != handy.noteGeneratorScript.closestNoteScript.myNoteIndex)
            {
                isInputted = false;
            }
            if (handy.GetJudgmentValue() <= handy.bestJudgmentRange)
            {
                if (!isInputted)
                {
                    isKeyDowns[handy.GetCorrectNextDegIndex(worldInfo.nextDegIndex[worldInfo.playerIndex] - 1)] = true;
                    InputCount++;
                    handy.noteGeneratorScript.closestNoteScript.isInputted = true;
                    isInputted = true;
                }
            }
            if (isInputted && handy.closestNoteScript.noteLengthTime != 0f)
            {
                isKeyPresses[handy.GetCorrectNextDegIndex(worldInfo.nextDegIndex[worldInfo.playerIndex] - 1)] = true;
            }
            if (isInputted)
                isKeyUps[handy.GetCorrectNextDegIndex(worldInfo.nextDegIndex[worldInfo.playerIndex] - 1)] = true;
        }
        else
        {
            for (int i = 0; i < canInputKeys.Count; i++)
            {
                if (Input.GetKeyDown(canInputKeys[i]))
                {
                    isKeyDowns[i] = true;
                    InputCount++;
                    handy.noteGeneratorScript.closestNoteScript.isInputted = true; ;
                }
                if (Input.GetKey(canInputKeys[i]))
                {
                    isKeyPresses[i] = true;
                }
                if (Input.GetKeyUp(canInputKeys[i]))
                {
                    isKeyUps[i] = true;
                }
            }
        }
        if (GetIsKeyDown())
        {
            judgmentType = JudgmentType.Perfect;
            float noteAccuracy01 = 1f;
            if (handy.GetJudgmentValue() > handy.bestJudgmentRange && handy.GetJudgmentValue() <= handy.bestJudgmentRange * 2f)
            {
                judgmentType = JudgmentType.Great;
            }
            else if (handy.GetJudgmentValue() > handy.bestJudgmentRange * 2f && handy.GetJudgmentValue() <= handy.bestJudgmentRange * 3f)
            {
                judgmentType = JudgmentType.Good;
            }
            else if (handy.GetJudgmentValue() > handy.bestJudgmentRange * 3f && handy.GetJudgmentValue() <= handy.bestJudgmentRange * 4f)
            {
                judgmentType = JudgmentType.Bad;
                noteAccuracy01 = Mathf.Clamp01(1f - handy.GetJudgmentValue());
                CountMissNote();
            }
            else if (handy.GetJudgmentValue() > handy.bestJudgmentRange * 4f)
            {
                judgmentType = JudgmentType.Miss;
                noteAccuracy01 = Mathf.Clamp01(1f - handy.GetJudgmentValue());
                CountMissNote();
            }
            sumNoteAccuracy01 += noteAccuracy01;
            SetAccuracy01();
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
    public void SetMissJudgment()
    {
        handy.judgmentGenScript.SetJudgmentText(JudgmentType.Miss);
        CountMissNote();
        InputCount++;
        SetAccuracy01();
    }
    public void SetElapsedTime01s()
    {
        elapsedTimeWhenNeedlessInput01 = Mathf.Clamp01(handy.closestNoteScript.elapsedTimeWhenNeedlessInput / handy.closestNoteScript.noteWaitTime);
        if (handy.closestNoteScript.noteLengthTime != 0f)
            elapsedTimeWhenNeedInput01 = Mathf.Clamp01(handy.closestNoteScript.elapsedTimeWhenNeedInput / handy.closestNoteScript.noteLengthTime);
        else
            elapsedTimeWhenNeedInput01 = Mathf.Clamp01(handy.closestNoteScript.elapsedTimeWhenNeedInput / handy.GetNoteWaitTime(handy.noteIndex + 1));
    }
    public void SetAccuracy01()
    {
        accuracy01 = sumNoteAccuracy01 / InputCount;
    }
    public bool GetIsKeyDown()
    {
        int KeyDownCount = 0;
        for (int i = 0; i < isKeyDowns.Count; i++)
        {
            if (isKeyDowns[i])
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
    public bool GetIsKeyPress()
    {
        int KeyPressCount = 0;
        for (int i = 0; i < isKeyPresses.Count; i++)
        {
            if (isKeyPresses[i])
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
    public bool GetIsKeyUp()
    {
        int KeyUpCount = 0;
        for (int i = 0; i < isKeyUps.Count; i++)
        {
            if (isKeyUps[i])
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
    public bool GetIsProperKeyDown(int? playerIndex = null, int? nextDegIndex = null)
    {
        if (playerIndex == null)
            playerIndex = handy.GetWorldInfo().playerIndex;
        if (nextDegIndex == null)
            nextDegIndex = handy.GetWorldInfo(handy.noteIndex - 1).nextDegIndex[(int)playerIndex];
        if (isKeyDowns[(int)Mathf.Clamp((float)nextDegIndex, 0f, float.MaxValue)])
        {
            return true;
        }
        return false;
    }
    public bool GetIsProperKeyPress(int? playerIndex = null, int? nextDegIndex = null)
    {
        if (playerIndex == null)
            playerIndex = handy.GetWorldInfo().playerIndex;
        if (nextDegIndex == null)
            nextDegIndex = handy.GetWorldInfo(handy.noteIndex - 1).nextDegIndex[(int)playerIndex];
        if (isKeyPresses[(int)Mathf.Clamp((float)nextDegIndex, 0f, float.MaxValue)])
        {
            return true;
        }
        return false;
    }
    public bool GetIsProperKeyUp(int? playerIndex = null, int? nextDegIndex = null)
    {
        if (playerIndex == null)
            playerIndex = handy.GetWorldInfo().playerIndex;
        if (nextDegIndex == null)
            nextDegIndex = handy.GetWorldInfo(handy.noteIndex - 1).nextDegIndex[(int)playerIndex];
        if (isKeyUps[(int)Mathf.Clamp((float)nextDegIndex, 0f, float.MaxValue)])
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
