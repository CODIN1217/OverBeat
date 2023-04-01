using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class CountDown : MonoBehaviour
{
    int numberOfCountDownTick;
    float intervalOfCountDownTick;
    float totalCountDownTime;
    public bool isCountDown;
    bool isAwake;
    float startTime;
    TextMeshProUGUI countDownTMP;
    WorldInfo worldInfo;
    Handy handy;
    GameManager GM;
    void Awake()
    {
        GM = GameManager.Property;
        handy = Handy.Property;
        countDownTMP = GetComponent<TextMeshProUGUI>();
    }
    void OnEnable() {
        PlayCountDown();
    }
    void Update()
    {
        if (isCountDown)
        {
            if (isAwake)
            {
                startTime = Time.unscaledTime;
                isAwake = false;
            }
            if (Time.unscaledTime - startTime < totalCountDownTime - intervalOfCountDownTick)
            {
                countDownTMP.text = ((float)numberOfCountDownTick - Mathf.Floor((Time.unscaledTime - startTime) / intervalOfCountDownTick) - 1).ToString();
            }
            else if(Time.unscaledTime - startTime < totalCountDownTime)
            {
                countDownTMP.text = "START";
            }
            else{
                countDownTMP.text = "";
                isCountDown = false;
                GameManager.Property.isPause = false;
            }
        }
    }

    public void PlayCountDown(){
        worldInfo = handy.GetWorldInfo(GM.curWorldInfoIndex);
        numberOfCountDownTick = worldInfo.countDownInfo.numberOfTick;
        intervalOfCountDownTick = worldInfo.countDownInfo.intervalOfTick;
        totalCountDownTime = (float)numberOfCountDownTick * intervalOfCountDownTick;
        isCountDown = true;
        isAwake = true;
    }
    public void PlayCountDown(int numberOfCountDownTick, float intervalOfCountDownTick){
        this.numberOfCountDownTick = numberOfCountDownTick;
        this.intervalOfCountDownTick = intervalOfCountDownTick;
        totalCountDownTime = (float)this.numberOfCountDownTick * this.intervalOfCountDownTick;
        isCountDown = true;
        isAwake = true;
    }
}
