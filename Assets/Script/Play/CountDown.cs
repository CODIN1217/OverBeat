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
    float totalCountDownSecs;
    public bool isCountDown;
    bool isEnable;
    float countDownSecs;
    TextMeshProUGUI countDownTMP;
    WorldInfo worldInfo;
    Handy handy;
    GameManager GM;
    Stopwatch countDowner;
    void Awake()
    {
        GM = GameManager.Property;
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
        if (GM.isBreakUpdate() && !isCountDown)
            return;
        if (isEnable)
        {
            isEnable = false;
        }
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
            GM.isPause = false;
            gameObject.SetActive(false);
        }
        /* if (isCountDown)
        {
        } */
    }

    public void PlayCountDown(int? numberOfCountDownTick = null, float? intervalOfCountDownTick = null, int? worldInfoIndex = null)
    {
        if (worldInfoIndex == null)
            worldInfoIndex = GM.curWorldInfoIndex;
        worldInfo = handy.GetWorldInfo((int)worldInfoIndex);
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
        isEnable = true;
        countDowner.Reset();
        countDowner.Stop();
    }
}
