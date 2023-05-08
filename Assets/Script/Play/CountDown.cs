using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Diagnostics;
using Debug = UnityEngine.Debug;
using System;

public class CountDown : MonoBehaviour
{
    int numberOfCountDownTick;
    float intervalOfCountDownTick;
    public float totalCountDownSecs;
    public bool isCountDown;
    float countDownSecs;
    TextMeshProUGUI countDownTMP;
    WorldInfo worldInfo;
    Handy handy;
    PlayGameManager playGM;
    Stopwatch countDowner;
    void Awake()
    {
        playGM = PlayGameManager.Property;
        handy = Handy.Property;
        countDownTMP = GetComponent<TextMeshProUGUI>();
        countDowner = new Stopwatch();
    }
    void OnEnable()
    {
        PlayCountDown();
    }
    void Update()
    {
        if (playGM.isBreakUpdate() && !isCountDown)
            return;
        countDowner.Start();
        countDownSecs = countDowner.ElapsedMilliseconds * 0.001f;
        if (countDownSecs < totalCountDownSecs - intervalOfCountDownTick)
        {
            countDownTMP.text = ((float)numberOfCountDownTick - Mathf.Floor(countDownSecs / intervalOfCountDownTick) - 1).ToString();
        }
        else if (countDownSecs < totalCountDownSecs)
        {
            countDownTMP.text = "START";
        }
        else
        {
            countDownTMP.text = "";
            isCountDown = false;
            playGM.isPause = false;
            gameObject.SetActive(false);
        }
    }

    public void PlayCountDown(int? numberOfCountDownTick = null, float? intervalOfCountDownTick = null, int? worldInfoIndex = null)
    {
        if (worldInfoIndex == null)
            worldInfoIndex = playGM.worldInfoIndex;
        worldInfo = playGM.GetWorldInfo((int)worldInfoIndex);
        if (numberOfCountDownTick == null)
            this.numberOfCountDownTick = worldInfo.countDownInfo.numberOfTick;
        else
            this.numberOfCountDownTick = (int)numberOfCountDownTick;
        if (intervalOfCountDownTick == null)
            this.intervalOfCountDownTick = worldInfo.countDownInfo.intervalOfTick;
        else
            this.intervalOfCountDownTick = (float)intervalOfCountDownTick;
        totalCountDownSecs = this.numberOfCountDownTick * this.intervalOfCountDownTick;
        isCountDown = true;
        countDowner.Reset();
        countDowner.Stop();
    }
}
