using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class Center : MonoBehaviour
{
    Image centerImage;
    WorldInfo worldInfo;
    Handy handy;
    PlayGameManager playGM;

    public TweenValue.TweeningInfo scaleInfo;
    public TweenValue.TweeningInfo posInfo;
    public TweenValue.TweeningInfo colorInfo;

    WorldInfo beforeWorldInfo;
    void Awake()
    {
        playGM = PlayGameManager.Property;
        handy = Handy.Property;
        centerImage = GetComponent<Image>();
    }
    void Update()
    {
        if (playGM.isBreakUpdate())
            return;
        beforeWorldInfo = playGM.GetWorldInfo(playGM.worldInfoIndex - 1);
        worldInfo = playGM.GetWorldInfo(playGM.worldInfoIndex);
        centerImage.fillAmount = Mathf.Lerp(centerImage.fillAmount, playGM.HP01, Time.deltaTime * 4f);
        if (!handy.compareValue_int.CompareWithBeforeValue(this.name, nameof(Update), nameof(playGM.worldInfoIndex), playGM.worldInfoIndex))
        {
            handy.TryKillSequence(scaleInfo);
            scaleInfo = new TweenValue.TweeningInfo(worldInfo.centerInfo.scaleTween);

            handy.TryKillSequence(posInfo);
            posInfo = new TweenValue.TweeningInfo(worldInfo.centerInfo.posTween);

            handy.TryKillSequence(colorInfo);
            colorInfo = new TweenValue.TweeningInfo(worldInfo.centerInfo.colorTween);

            handy.compareValue_int.SetValueForCompare(this.name, nameof(Update), nameof(playGM.worldInfoIndex), playGM.worldInfoIndex);
        }

        transform.localScale = (Vector2)scaleInfo.curValue;
        transform.localPosition = (Vector2)posInfo.curValue;
        centerImage.color = (Color)colorInfo.curValue;
    }
}
