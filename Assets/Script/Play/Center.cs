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
        beforeWorldInfo = playGM.GetWorldInfo(playGM.curWorldInfoIndex - 1);
        worldInfo = playGM.GetWorldInfo(playGM.curWorldInfoIndex);
        if (isAwake)
        {
            tweenScale = beforeWorldInfo.centerInfo.scale;
            tweenPos = beforeWorldInfo.centerInfo.pos;
            tweenColor = beforeWorldInfo.centerInfo.color;
            isAwake = false;
        }
        centerImage.fillAmount = Mathf.Lerp(centerImage.fillAmount, playGM.HP01, Time.deltaTime * 4f);
        if (!handy.compareValue_int.CompareWithBeforeValue(this.name, nameof(Update), nameof(playGM.curWorldInfoIndex), playGM.curWorldInfoIndex))
        {
            handy.TryKillSequence(scaleTweener);
            scaleTweener = DOTween.Sequence()
            .Append(DOTween.To(() => tweenScale, (s) => tweenScale = s, worldInfo.centerInfo.scale, worldInfo.centerInfo.scaleTween.duration))
            .SetEase(worldInfo.centerInfo.scaleTween.ease);

            handy.TryKillSequence(posTweener);
            posTweener = DOTween.Sequence()
            .Append(DOTween.To(() => tweenPos, (s) => tweenPos = s, worldInfo.centerInfo.pos, worldInfo.centerInfo.posTween.duration))
            .SetEase(worldInfo.centerInfo.posTween.ease);

            handy.TryKillSequence(colorTweener);
            colorTweener = DOTween.Sequence()
            .Append(DOTween.To(() => tweenColor, (s) => tweenColor = s, worldInfo.centerInfo.color, worldInfo.centerInfo.colorTween.duration))
            .SetEase(worldInfo.centerInfo.colorTween.ease);

            handy.compareValue_int.SetValueForCompare(this.name, nameof(Update), nameof(playGM.curWorldInfoIndex), playGM.curWorldInfoIndex);
        }
        transform.localScale = tweenScale;
        transform.localPosition = tweenPos;
        centerImage.color = tweenColor;
    }
}
