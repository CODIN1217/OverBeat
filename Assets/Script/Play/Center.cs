using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TweenManager;

public class Center : MonoBehaviour, ITweenerInfo
{
    Image centerImage;
    WorldInfo worldInfo;
    Handy handy;
    PlayGameManager playGM;

    public Vector2 scale;
    public Vector2 pos;
    public Color color;

    public TweeningInfo scaleInfo;
    public TweeningInfo posInfo;
    public TweeningInfo colorInfo;

    // WorldInfo beforeWorldInfo;
    void Awake()
    {
        playGM = PlayGameManager.Property;
        handy = Handy.Property;
        centerImage = GetComponent<Image>();
        playGM.initTweenEvent += InitTween;
        playGM.playTweenEvent += PlayTween;
    }
    void Update()
    {
        if (playGM.isBreakUpdate())
            return;
        // beforeWorldInfo = playGM.GetWorldInfo(playGM.worldInfoIndex - 1);
        centerImage.fillAmount = Mathf.Lerp(centerImage.fillAmount, playGM.HP01, Time.deltaTime * 4f);
        /* if (!handy.compareValue_int.CompareWithBeforeValue(this.name, nameof(Update), nameof(playGM.worldInfoIndex), playGM.worldInfoIndex))
        {
            InitTween();

            PlayTween();

            handy.compareValue_int.SetValueForCompare(this.name, nameof(Update), nameof(playGM.worldInfoIndex), playGM.worldInfoIndex);
        } */
        UpdateTweenValue();

        transform.localScale = scale;
        transform.localPosition = pos;
        centerImage.color = color;
    }
    public void InitTween()
    {
        worldInfo = playGM.GetWorldInfo(playGM.worldInfoIndex);
        
        handy.TryKillTween(scaleInfo);
        scaleInfo = new TweeningInfo(worldInfo.centerInfo.scaleTween, playGM.GetHoldNoteSecs(playGM.worldInfoIndex));

        handy.TryKillTween(posInfo);
        posInfo = new TweeningInfo(worldInfo.centerInfo.posTween, playGM.GetHoldNoteSecs(playGM.worldInfoIndex));

        handy.TryKillTween(colorInfo);
        colorInfo = new TweeningInfo(worldInfo.centerInfo.colorTween, playGM.GetHoldNoteSecs(playGM.worldInfoIndex));
    }
    public void UpdateTweenValue()
    {
        scale = ((TweenerInfo<Vector2>)scaleInfo).curValue;
        pos = ((TweenerInfo<Vector2>)posInfo).curValue;
        color = ((TweenerInfo<Color>)colorInfo).curValue;
    }
    public void PlayTween()
    {
        handy.PlayTweens(scaleInfo, posInfo, colorInfo);
    }
}
