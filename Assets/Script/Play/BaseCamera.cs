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

    public TweenValue.TweeningInfo sizeInfo;
    public TweenValue.TweeningInfo colorInfo;
    public TweenValue.TweeningInfo rotationInfo;
    public TweenValue.TweeningInfo posInfo;

    WorldInfo beforeWorldInfo;
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
        beforeWorldInfo = playGM.GetWorldInfo(playGM.worldInfoIndex - 1);
        worldInfo = playGM.GetWorldInfo(playGM.worldInfoIndex);
        if (!handy.compareValue_int.CompareWithBeforeValue(this.name, nameof(Update), nameof(playGM.worldInfoIndex), playGM.worldInfoIndex))
        {
            handy.TryKillSequence(sizeInfo);
            sizeInfo = new TweenValue.TweeningInfo(worldInfo.cameraInfo.sizeTween, (s) => stdSize * s);

            handy.TryKillSequence(colorInfo);
            colorInfo = new TweenValue.TweeningInfo(worldInfo.cameraInfo.BGColorTween);

            handy.TryKillSequence(rotationInfo);
            rotationInfo = new TweenValue.TweeningInfo(worldInfo.cameraInfo.rotationTween, (r) => handy.GetCorrectDegMaxIs0(-r));

            handy.TryKillSequence(posInfo);
            posInfo = new TweenValue.TweeningInfo(worldInfo.cameraInfo.posTween);

            handy.compareValue_int.SetValueForCompare(this.name, nameof(Update), nameof(playGM.worldInfoIndex), playGM.worldInfoIndex);
        }

        baseCamera.orthographicSize = (float)sizeInfo.curValue;
        baseCamera.backgroundColor = (Color)colorInfo.curValue;
        transform.rotation = Quaternion.Euler(0f, 0f, (float)rotationInfo.curValue);
        transform.position = (Vector2)posInfo.curValue;
    }
    BaseCamera()
    {
        stdSize = 5.4f;
    }
}
