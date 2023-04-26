using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using TweenValue;

public class BaseCamera : MonoBehaviour
{
    Camera baseCamera;
    public readonly float stdSize;
    Handy handy;
    WorldInfo worldInfo;
    PlayGameManager playGM;

    public float orthoSize;
    public Color BGColor;
    public float rotation;
    public Vector2 pos;

    public TweeningInfo orthoSizeTweener;
    public TweeningInfo BGColorTweener;
    public TweeningInfo rotationTweener;
    public TweeningInfo posTweener;

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
            handy.TryKillTween(orthoSizeTweener);
            orthoSizeTweener = new TweeningInfo(worldInfo.cameraInfo.sizeTween);

            handy.TryKillTween(BGColorTweener);
            BGColorTweener = new TweeningInfo(worldInfo.cameraInfo.BGColorTween);

            handy.TryKillTween(rotationTweener);
            rotationTweener = new TweeningInfo(worldInfo.cameraInfo.rotationTween);

            handy.TryKillTween(posTweener);
            posTweener = new TweeningInfo(worldInfo.cameraInfo.posTween);

            handy.PlayTweens(orthoSizeTweener, BGColorTweener, rotationTweener, posTweener);

            handy.compareValue_int.SetValueForCompare(this.name, nameof(Update), nameof(playGM.worldInfoIndex), playGM.worldInfoIndex);
        }
        UpdateTweenValue();

        baseCamera.orthographicSize = orthoSize;
        baseCamera.backgroundColor = BGColor;
        transform.rotation = Quaternion.Euler(0f, 0f, rotation);
        transform.position = new Vector3(pos.x, pos.y, -1000f);
    }
    void UpdateTweenValue()
    {
        orthoSize = stdSize * (float)orthoSizeTweener.curValue;
        BGColor = (Color)BGColorTweener.curValue;
        rotation = handy.GetCorrectDegMaxIs0(-(float)rotationTweener.curValue);
        pos = (Vector2)posTweener.curValue;
    }
    BaseCamera()
    {
        stdSize = 5.4f;
    }
}
