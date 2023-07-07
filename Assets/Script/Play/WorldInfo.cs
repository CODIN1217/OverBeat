using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using TweenManager;

[Serializable]
public class WorldInfo
{
    [Serializable]
    public class CreditInfo
    {
        public string levelName;
        public string levelEditor;
        public string songName;
        public string songWriter;
        public CreditInfo()
        {
            levelName = "Empty";
            levelEditor = "Empty";
            songName = "Empty";
            songWriter = "Empty";
        }
    }
    public LevelInfo[] levelInfos;

    public CreditInfo creditInfo;
    public WorldInfo()
    {
        levelInfos = new LevelInfo[9];
        WorldInfoMethod.InitLevelInfo(ref levelInfos, 2, 4, 3.5f, 4f, 90f, LevelInfo.InsideNoteType.Keep);
        creditInfo = new CreditInfo();
    }
}

public static class WorldInfoMethod
{
    public static void InitLevelInfo(ref LevelInfo[] levelInfos, int playerInfoCount, int noteCount, float noteWaitRadius, float noteHoldRadius, float degDistance, LevelInfo.InsideNoteType insideNoteType)
    {
        float awakeSecs = 0f;
        for (int i = 0; i < levelInfos.Length; i++)
        {
            levelInfos[i] = new LevelInfo();

            levelInfos[i].playerInfo = new LevelInfo.PlayerInfo[playerInfoCount];
            if (i == 0)
            {
                levelInfos[i].noteInfo.tarPlayerIndex = -1;
                levelInfos[i].noteInfo.eachNoteIndex = -1;
                for (int j = 0; j < playerInfoCount; j++)
                {
                    levelInfos[i].playerInfo[j] = new LevelInfo.PlayerInfo();
                    levelInfos[i].playerInfo[j].degTween = new TweenInfo<float>(Handy.GetCorrectedDegMaxIs0((float)j * 90f), Handy.GetCorrectedDegMaxIs0((float)j * 90f), AnimationCurve.Linear(0f, 0f, 1f, 1f));
                }
            }
            else
            {
                levelInfos[i].noteInfo.tarPlayerIndex = (i + 1) % playerInfoCount;
                levelInfos[i].noteInfo.eachNoteIndex = (int)Handy.CeilValue((i - 1) / playerInfoCount);
                for (int j = 0; j < playerInfoCount; j++)
                {
                    levelInfos[i].playerInfo[j] = new LevelInfo.PlayerInfo();
                    levelInfos[i].playerInfo[j].degTween = new TweenInfo<float>(levelInfos[Handy.GetBeforeIndex(i)].playerInfo[j].degTween.endValue, levelInfos[Handy.GetBeforeIndex(i)].playerInfo[j].degTween.endValue + (levelInfos[i].noteInfo.tarPlayerIndex == j ? 90f : 0f), AnimationCurve.Linear(0f, 0f, 1f, 1f));
                }
            }

            levelInfos[i].noteInfo.noteCount = noteCount;
            levelInfos[i].noteInfo.noteHitTiming01s = new float[noteCount];
            levelInfos[i].noteInfo.rotationTweens = new TweenInfo<float>[noteCount];
            levelInfos[i].noteInfo.colorTweens = new TweenInfo<Color>[noteCount];
            for (int j = 0; j < noteCount; j++)
            {
                levelInfos[i].noteInfo.noteHitTiming01s[j] = (float)j / (float)(noteCount - 1);
                levelInfos[i].noteInfo.rotationTweens[j] = new TweenInfo<float>(0f, 0f, AnimationCurve.Linear(0f, 0f, 1f, 1f));
                levelInfos[i].noteInfo.colorTweens[j] = new TweenInfo<Color>(new Color(100, 45, 250, 255) / 255f, new Color(100, 45, 250, 255) / 255f, AnimationCurve.Linear(0, 0, 1, 1));
            }
            levelInfos[i].noteInfo.waitDeltaRadiusTween = new TweenInfo<float>(noteWaitRadius, 0f, AnimationCurve.Linear(0f, 0f, 1f, 1f));
            levelInfos[i].noteInfo.holdDeltaRadiusTween = new TweenInfo<float>(0f, noteHoldRadius, AnimationCurve.Linear(0f, 0f, 1f, 1f));

            float noteWaitSecs = Mathf.Abs(noteWaitRadius / 3.5f / levelInfos[i].noteInfo.speed);
            noteWaitSecs *= Mathf.Clamp01(i);
            float holdNoteSecs = Mathf.Abs(noteHoldRadius / levelInfos[i].noteInfo.speed);
            holdNoteSecs *= Mathf.Clamp01(i);

            levelInfos[i].noteInfo.awakeSecs = awakeSecs;
            awakeSecs += noteWaitSecs + holdNoteSecs;

            levelInfos[i].noteInfo.insideNoteType = insideNoteType;
        }
    }
}
