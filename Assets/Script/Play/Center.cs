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

    Vector2 tweenScale;
    Vector2 tweenPos;
    Color tweenColor;

    public Vector2 scale;
    public Vector2 pos;
    public Color color;

    Sequence scaleTweener;
    Sequence posTweener;
    Sequence colorTweener;

    WorldInfo beforeWorldInfo;
    bool isAwake;
    void Awake()
    {
        playGM = PlayGameManager.Property;
        handy = Handy.Property;
        centerImage = GetComponent<Image>();
        isAwake = true;
    }
    void Update()
    {
        if (playGM.isBreakUpdate())
            return;
        beforeWorldInfo = playGM.GetWorldInfo(playGM.worldInfoIndex - 1);
        worldInfo = playGM.GetWorldInfo(playGM.worldInfoIndex);
        if (isAwake)
        {
            tweenScale = beforeWorldInfo.centerInfo.scaleTween.value;
            tweenPos = beforeWorldInfo.centerInfo.posTween.value;
            tweenColor = beforeWorldInfo.centerInfo.colorTween.value;
            isAwake = false;
        }
        centerImage.fillAmount = Mathf.Lerp(centerImage.fillAmount, playGM.HP01, Time.deltaTime * 4f);
        if (!handy.compareValue_int.CompareWithBeforeValue(this.name, nameof(Update), nameof(playGM.worldInfoIndex), playGM.worldInfoIndex))
        {
            handy.TryKillSequence(scaleTweener);
            scaleTweener = DOTween.Sequence()
            .Append(DOTween.To(() => tweenScale, (s) => tweenScale = s, worldInfo.centerInfo.scaleTween.value, worldInfo.centerInfo.scaleTween.duration))
            .SetEase(worldInfo.centerInfo.scaleTween.ease);

            handy.TryKillSequence(posTweener);
            posTweener = DOTween.Sequence()
            .Append(DOTween.To(() => tweenPos, (s) => tweenPos = s, worldInfo.centerInfo.posTween.value, worldInfo.centerInfo.posTween.duration))
            .SetEase(worldInfo.centerInfo.posTween.ease);

            handy.TryKillSequence(colorTweener);
            colorTweener = DOTween.Sequence()
            .Append(DOTween.To(() => tweenColor, (s) => tweenColor = s, worldInfo.centerInfo.colorTween.value, worldInfo.centerInfo.colorTween.duration))
            .SetEase(worldInfo.centerInfo.colorTween.ease);

            handy.compareValue_int.SetValueForCompare(this.name, nameof(Update), nameof(playGM.worldInfoIndex), playGM.worldInfoIndex);
        }
        scale = tweenScale;
        pos = tweenPos;
        color = tweenColor;

        transform.localScale = scale;
        transform.localPosition = pos;
        centerImage.color = color;
    }
}
