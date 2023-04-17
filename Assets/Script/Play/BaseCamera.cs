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
    void Awake()
    {
        playGM = PlayGameManager.Property;
        handy = Handy.Property;
        baseCamera = Camera.main;
    }
    void Update()
    {
        if (playGM.isBreakUpdate())
            return;
        if (!handy.CompareWithBeforeValue(this.name, nameof(Update), nameof(playGM.curWorldInfoIndex), playGM.curWorldInfoIndex))
        {
            worldInfo = playGM.GetWorldInfo(playGM.curWorldInfoIndex);
            baseCamera.DOOrthoSize(stdSize * worldInfo.cameraInfo.size, worldInfo.cameraInfo.sizeTween.duration)
            .SetEase(worldInfo.cameraInfo.sizeTween.ease);
            baseCamera.DOColor(handy.GetColor01(worldInfo.cameraInfo.BGColor), worldInfo.cameraInfo.BGColorTween.duration)
            .SetEase(worldInfo.cameraInfo.BGColorTween.ease);
            transform.DORotate(Vector3.forward * handy.GetCorrectDegMaxIs0(-worldInfo.cameraInfo.rotation), worldInfo.cameraInfo.rotationTween.duration)
            .SetEase(worldInfo.cameraInfo.rotationTween.ease);
            transform.DOMove((Vector3)worldInfo.cameraInfo.pos + Vector3.back * 100f, worldInfo.cameraInfo.posTween.duration)
            .SetEase(worldInfo.cameraInfo.posTween.ease);
            handy.SetValueForCompare(this.name, nameof(Update), nameof(playGM.curWorldInfoIndex), playGM.curWorldInfoIndex);
        }
        /* baseCamera.orthographicSize = stdSize * worldInfo.cameraInfo.size;
        baseCamera.backgroundColor = handy.GetColor01(worldInfo.cameraInfo.BGColor);
        transform.rotation = Quaternion.Euler(0f, 0f, handy.GetCorrectDegMaxIs0(worldInfo.cameraInfo.rotation));
        transform.position = worldInfo.cameraInfo.pos; */
    }
    BaseCamera()
    {
        stdSize = 5.4f;
    }
}
