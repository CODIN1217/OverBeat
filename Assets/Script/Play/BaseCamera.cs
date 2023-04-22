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
    
    float tweenOrthoSize;
    Color tweenBGColor;
    float tweenRotation;
    Vector3 tweenPos;

    public float orthoSize;
    public Color BGColor;
    public float rotation;
    public Vector3 pos;

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
        beforeWorldInfo = playGM.GetWorldInfo(playGM.worldInfoIndex - 1);
        worldInfo = playGM.GetWorldInfo(playGM.worldInfoIndex);
        if (isAwake)
        {
            tweenOrthoSize = beforeWorldInfo.cameraInfo.sizeTween.value;
            tweenBGColor = beforeWorldInfo.cameraInfo.BGColorTween.value;
            tweenRotation = beforeWorldInfo.cameraInfo.rotationTween.value;
            tweenPos = beforeWorldInfo.cameraInfo.posTween.value;
            isAwake = false;
        }
        if (!handy.compareValue_int.CompareWithBeforeValue(this.name, nameof(Update), nameof(playGM.worldInfoIndex), playGM.worldInfoIndex))
        {
            handy.TryKillSequence(sizeTweener);
            sizeTweener = DOTween.Sequence()
            .Append(DOTween.To(() => tweenOrthoSize, (s) => tweenOrthoSize = s, worldInfo.cameraInfo.sizeTween.value, worldInfo.cameraInfo.sizeTween.duration))
            .SetEase(worldInfo.cameraInfo.sizeTween.ease);

            handy.TryKillSequence(colorTweener);
            colorTweener = DOTween.Sequence()
            .Append(DOTween.To(() => tweenBGColor, (c) => tweenBGColor = c, worldInfo.cameraInfo.BGColorTween.value, worldInfo.cameraInfo.BGColorTween.duration))
            .SetEase(worldInfo.cameraInfo.BGColorTween.ease);

            handy.TryKillSequence(rotationTweener);
            rotationTweener = DOTween.Sequence()
            .Append(DOTween.To(() => tweenRotation, (r) => tweenRotation = r, handy.GetCorrectDegMaxIs0(worldInfo.cameraInfo.rotationTween.value), worldInfo.cameraInfo.rotationTween.duration))
            .SetEase(worldInfo.cameraInfo.rotationTween.ease);

            handy.TryKillSequence(posTweener);
            posTweener = DOTween.Sequence()
            .Append(DOTween.To(() => tweenPos, (p) => tweenPos = p, (Vector3)worldInfo.cameraInfo.posTween.value + Vector3.back * 100f, worldInfo.cameraInfo.posTween.duration))
            .SetEase(worldInfo.cameraInfo.posTween.ease);

            handy.compareValue_int.SetValueForCompare(this.name, nameof(Update), nameof(playGM.worldInfoIndex), playGM.worldInfoIndex);
        }
        orthoSize = stdSize * tweenOrthoSize;
        BGColor = tweenBGColor;
        rotation = handy.GetCorrectDegMaxIs0(-tweenRotation);
        pos = tweenPos;

        baseCamera.orthographicSize = orthoSize;
        baseCamera.backgroundColor = BGColor;
        transform.rotation = Quaternion.Euler(0f, 0f, rotation);
        transform.position = pos;
    }
    BaseCamera()
    {
        stdSize = 5.4f;
    }
}
