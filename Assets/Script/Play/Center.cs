using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TweenValue;

public class Center : MonoBehaviour
{
    Image centerImage;
    WorldInfo worldInfo;
    Handy handy;
    PlayGameManager playGM;
    
    public Vector2 scale;
    public Vector2 pos;
    public Color color;

    public TweeningInfo scaleTweener;
    public TweeningInfo posTweener;
    public TweeningInfo colorTweener;

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
            handy.TryKillTween(scaleTweener);
            scaleTweener = new TweeningInfo(worldInfo.centerInfo.scaleTween);

            handy.TryKillTween(posTweener);
            posTweener = new TweeningInfo(worldInfo.centerInfo.posTween);

            handy.TryKillTween(colorTweener);
            colorTweener = new TweeningInfo(worldInfo.centerInfo.colorTween);

            handy.PlayTweens(scaleTweener, posTweener, colorTweener);

            handy.compareValue_int.SetValueForCompare(this.name, nameof(Update), nameof(playGM.worldInfoIndex), playGM.worldInfoIndex);
        }
        UpdateTweenValue();

        transform.localScale = scale;
        transform.localPosition = pos;
        centerImage.color = color;
    }
    void UpdateTweenValue()
    {
        scale = (Vector2)scaleTweener.curValue;
        pos = (Vector2)posTweener.curValue;
        color = (Color)colorTweener.curValue;
    }
}
