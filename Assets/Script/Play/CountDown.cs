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
    PlayManager PM;
    void Awake()
    {
        PM = PlayManager.Member;
        countDownTMP = GetComponent<TextMeshProUGUI>();
    }
    void OnEnable()
    {
        PlayCountDown();
    }
    void Update()
    {
        if (PM.isPause || !isCountDown)
            return;
        countDownSecs += Time.deltaTime;
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
            PM.isStop = false;
            Handy.Renderer.ColorMethod.FadeColor(countDownTMP, 0f);
        }
    }

    public void PlayCountDown(int? numberOfCountDownTick = null, float? intervalOfCountDownTick = null, int? worldInfoIndex = null)
    {
        if (worldInfoIndex == null)
            worldInfoIndex = PM.worldInfoIndex;
        worldInfo = PM.GetWorldInfo((int)worldInfoIndex);
        if (numberOfCountDownTick == null)
            this.numberOfCountDownTick = worldInfo.countDownInfo.numberOfTick;
        else
            this.numberOfCountDownTick = (int)numberOfCountDownTick;
        if (intervalOfCountDownTick == null)
            this.intervalOfCountDownTick = worldInfo.countDownInfo.intervalOfTick;
        else
            this.intervalOfCountDownTick = (float)intervalOfCountDownTick;
        totalCountDownSecs = this.numberOfCountDownTick * this.intervalOfCountDownTick;
        countDownSecs = 0f;
        isCountDown = true;
        PM.isStop = true;
        Handy.Renderer.ColorMethod.FadeColor(countDownTMP, 1f);
    }
}
