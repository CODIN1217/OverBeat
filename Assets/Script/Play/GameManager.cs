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
    // public int lastHitedNoteIndex;
    public bool isPause;
    public bool isGameOver;
    public bool isClearWorld;
    public bool isAutoPlay;
    bool isAwake;
    bool isInputted;
    bool isInputted_temp;
    static GameManager instance = null;
    public List<List<KeyCode>> canInputKeys;
    // public float elapsedTimeWhenNeedlessInput01;
    // public float elapsedTimeWhenNeedInput01;
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
        closestNoteIndex = new int[handy.GetMaxPlayerCount()];
        isKeyDowns = new List<List<bool>>();
        isKeyPresses = new List<List<bool>>();
        isKeyUps = new List<List<bool>>();
        totalElapsedTime = new Stopwatch();
        totalElapsedTime.Reset();
        totalElapsedTime.Stop();
        handy.RepeatCode((i) => InfoViewer.Property.SetInfo(this.name, nameof(closestNoteIndex), () => closestNoteIndex[i], i), closestNoteIndex.Length);
        InfoViewer.Property.SetInfo(this.name, nameof(curWorldInfoIndex), () => curWorldInfoIndex);
        isAwake = true;
    }
    void Update()
    {

        // Time.timeScale = isPause ? 0f : 1f;
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
            for (int i = 0; i < handy.GetMaxPlayerCount(); i++)
            {
                totalNoteCount += handy.GetNoteCountWithOutStartNote(i);
            }
            WorldInfo lastWorldInfo = handy.GetWorldInfo(handy.GetMaxWorldInfoIndex());
            NotePrefab lastNoteScript = handy.GetNoteScript(lastWorldInfo.noteInfo.tarPlayerIndex, lastWorldInfo.noteInfo.eachNoteIndex);
            totalGamePlaySecs = lastWorldInfo.noteInfo.awakeSecs + lastNoteScript.noteWaitSecs + lastNoteScript.noteLengthSecs;
            isAwake = false;
        }
        progress01 = Mathf.Clamp01(totalElapsedSecs / totalGamePlaySecs);
        // InfoViewer.Property.SetInfo(this.name, nameof(totalNoteCount), () => totalNoteCount);
        MaxMissCount = (int)Mathf.Clamp(Mathf.Floor((float)totalNoteCount * 0.4f), 1f, (float)MaxHPCount);
        HP01 = 1f - (float)missCount / MaxMissCount;
        totalElapsedTime.Stop();
        if (isPause)
        {
            return;
        }
        totalElapsedTime.Start();
        totalElapsedSecs = totalElapsedTime.ElapsedMilliseconds * 0.001f;
        for (int i = 0; i < handy.GetWorldInfoCount(); i++)
        {
            GameObject curNote = handy.GetNote(handy.GetWorldInfo(i).noteInfo.tarPlayerIndex, handy.GetWorldInfo(i).noteInfo.eachNoteIndex);
            NotePrefab curNoteScript = handy.GetNoteScript(handy.GetWorldInfo(i).noteInfo.tarPlayerIndex, handy.GetWorldInfo(i).noteInfo.eachNoteIndex);
            if (!curNoteScript.isStop)
            {
                if (!curNote.activeSelf)
                {
                    if (handy.GetWorldInfo(i).noteInfo.awakeSecs <= totalElapsedSecs)
                    {
                        curNote.SetActive(true);
                        curNoteScript.toleranceSecsWhenAwake = totalElapsedSecs - handy.GetWorldInfo(i).noteInfo.awakeSecs;
                    }
                }
            }
        }
        UpdateClosestNoteIndex();
        if (isAutoPlay)
        {
            //Empty
        }
        else
        {
            for (int i = 0; i < handy.GetMaxPlayerCount(); i++)
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
        for (int i = 0; i < handy.GetMaxPlayerCount(); i++)
        {
            if (handy.GetPlayer(i).activeSelf)
            {
                if (GetIsKeyDown(i))
                {
                    float noteAccuracy01 = 1f;
                    if (GetIsProperKeyDown(i))
                    {
                        handy.judgmentGenScript.SetJudgmentText(i, GetJudgment(i, handy.GetJudgmentValue(i), () => { noteAccuracy01 = Mathf.Clamp01(1f - handy.GetJudgmentValue(i)); CountMissNote(); }));
                    }
                    else
                    {
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
        for (int i = 0; i < handy.GetMaxPlayerCount(); i++)
        {
            for (int j = handy.GetMaxNoteIndex(i); j >= closestNoteIndex[i]; j--)
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
            nextDegIndex = handy.GetWorldInfo(playerIndex, closestNoteIndex[playerIndex]).noteInfo.startDegIndex;
        if (isKeyDowns[playerIndex][handy.GetCorrectNextDegIndex((int)nextDegIndex, playerIndex, closestNoteIndex[playerIndex])])
        {
            return true;
        }
        return false;
    }
    public bool GetIsProperKeyPress(int playerIndex, int? nextDegIndex = null)
    {

        playerIndex = handy.GetCorrectIndex(playerIndex, handy.GetMaxPlayerIndex());
        if (nextDegIndex == null)
            nextDegIndex = handy.GetWorldInfo(playerIndex, closestNoteIndex[playerIndex]).noteInfo.startDegIndex;
        if (isKeyPresses[playerIndex][handy.GetCorrectNextDegIndex((int)nextDegIndex, playerIndex, closestNoteIndex[playerIndex])])
        {
            return true;
        }
        return false;
    }
    public bool GetIsProperKeyUp(int playerIndex, int? nextDegIndex = null)
    {

        playerIndex = handy.GetCorrectIndex(playerIndex, handy.GetMaxPlayerIndex());
        if (nextDegIndex == null)
            nextDegIndex = handy.GetWorldInfo(playerIndex, closestNoteIndex[playerIndex]).noteInfo.startDegIndex;
        if (isKeyUps[playerIndex][handy.GetCorrectNextDegIndex((int)nextDegIndex, playerIndex, closestNoteIndex[playerIndex])])
        {
            return true;
        }
        return false;
    }
    // public void SetUpdateSequence(Sequence tweener)
    // {
    //     if (tweener != null)
    //     {
    //         if (isPause)
    //         {
    //             tweener.SetUpdate(false);
    //             if (isClearWorld || isGameOver)
    //             {
    //                 tweener.SetUpdate(true);
    //             }
    //         }
    //     }
    // }
    public bool isBreakUpdate()
    {
        if (isPause)
        {
            if (!isClearWorld && !isGameOver)
                return true;
        }
        return false;
    }
    GameManager()
    {
        MaxHPCount = 15;
    }
}
