using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class BaseCamera : MonoBehaviour
{
    Camera baseCamera;
    public readonly float stdSize;
    Handy handy;
    WorldInfo worldInfo;
    PlayGameManager playGM;
    float tweenSize;
    Color tweenColor;
    float tweenRotation;
    Vector3 tweenPos;
    Sequence sizeTweener;
    Sequence colorTweener;
    Sequence rotationTweener;
    Sequence posTweener;
    WorldInfo beforeWorldInfo;
    bool isAwake;
    void Awake()
    {
        playGM = PlayGameManager.Property;
        handy = Handy.Property;
        baseCamera = Camera.main;
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
            tweenSize = beforeWorldInfo.cameraInfo.size;
            tweenColor = beforeWorldInfo.cameraInfo.BGColor;
            tweenRotation = beforeWorldInfo.cameraInfo.rotation;
            tweenPos = beforeWorldInfo.cameraInfo.pos;
            isAwake = false;
        }
        if (!handy.compareValue_int.CompareWithBeforeValue(this.name, nameof(Update), nameof(playGM.curWorldInfoIndex), playGM.curWorldInfoIndex))
        {
            handy.TryKillSequence(sizeTweener);
            sizeTweener = DOTween.Sequence()
            .Append(DOTween.To(() => tweenSize, (s) => tweenSize = s, worldInfo.cameraInfo.size, worldInfo.cameraInfo.sizeTween.duration))
            .SetEase(worldInfo.cameraInfo.sizeTween.ease);

            handy.TryKillSequence(colorTweener);
            colorTweener = DOTween.Sequence()
            .Append(DOTween.To(() => tweenColor, (c) => tweenColor = c, worldInfo.cameraInfo.BGColor, worldInfo.cameraInfo.BGColorTween.duration))
            .SetEase(worldInfo.cameraInfo.BGColorTween.ease);

            handy.TryKillSequence(rotationTweener);
            rotationTweener = DOTween.Sequence()
            .Append(DOTween.To(() => tweenRotation, (r) => tweenRotation = r, handy.GetCorrectDegMaxIs0(-worldInfo.cameraInfo.rotation), worldInfo.cameraInfo.rotationTween.duration))
            .SetEase(worldInfo.cameraInfo.rotationTween.ease);

            handy.TryKillSequence(posTweener);
            posTweener = DOTween.Sequence()
            .Append(DOTween.To(() => tweenPos, (p) => tweenPos = p, (Vector3)worldInfo.cameraInfo.pos + Vector3.back * 100f, worldInfo.cameraInfo.posTween.duration))
            .SetEase(worldInfo.cameraInfo.posTween.ease);

            handy.compareValue_int.SetValueForCompare(this.name, nameof(Update), nameof(playGM.curWorldInfoIndex), playGM.curWorldInfoIndex);
        }
        baseCamera.orthographicSize = stdSize * tweenSize;
        baseCamera.backgroundColor = tweenColor;
        transform.rotation = Quaternion.Euler(0f, 0f, handy.GetCorrectDegMaxIs0(tweenRotation));
        transform.position = tweenPos;
    }
    BaseCamera()
    {
        stdSize = 5.4f;
    }
}
