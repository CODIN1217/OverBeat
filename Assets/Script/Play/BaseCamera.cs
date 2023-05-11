using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using TweenManager;

public class BaseCamera : MonoBehaviour, ITweenerInfo
{
    Camera baseCamera;
    public readonly float stdSize;
    Handy handy;
    WorldInfo worldInfo;
    PlayManager PM;

    public float orthoSize;
    public Color BGColor;
    public float rotation;
    public Vector2 pos;

    public TweeningInfo orthoSizeInfo;
    public TweeningInfo BGColorInfo;
    public TweeningInfo rotationInfo;
    public TweeningInfo posInfo;

    // WorldInfo beforeWorldInfo;
    void Awake()
    {
        PM = PlayManager.Property;
        handy = Handy.Property;
        baseCamera = Camera.main;
        PM.initTweenEvent += InitTween;
        PM.playHoldTweenEvent += PlayHoldTween;
    }
    void Update()
    {
        if (PM.isBreakUpdate())
            return;
        // beforeWorldInfo = playGM.GetWorldInfo(playGM.worldInfoIndex - 1);
        /* if (!handy.compareValue_int.CompareWithBeforeValue(this.name, nameof(Update), nameof(playGM.worldInfoIndex), playGM.worldInfoIndex))
        {
            InitTween();

            PlayTween();

            handy.compareValue_int.SetValueForCompare(this.name, nameof(Update), nameof(playGM.worldInfoIndex), playGM.worldInfoIndex);
        } */
        UpdateTweenValue();

        baseCamera.orthographicSize = orthoSize;
        baseCamera.backgroundColor = BGColor;
        transform.rotation = Quaternion.Euler(0f, 0f, rotation);
        transform.position = new Vector3(pos.x, pos.y, -1000f);
    }
    public void InitTween()
    {
        worldInfo = PM.GetWorldInfo(PM.worldInfoIndex);
        
        handy.TryKillTween(orthoSizeInfo);
        orthoSizeInfo = new TweeningInfo(worldInfo.cameraInfo.sizeTween, PM.GetHoldNoteSecs(PM.worldInfoIndex));

        handy.TryKillTween(BGColorInfo);
        BGColorInfo = new TweeningInfo(worldInfo.cameraInfo.BGColorTween, PM.GetHoldNoteSecs(PM.worldInfoIndex));

        handy.TryKillTween(rotationInfo);
        rotationInfo = new TweeningInfo(worldInfo.cameraInfo.rotationTween, PM.GetHoldNoteSecs(PM.worldInfoIndex));

        handy.TryKillTween(posInfo);
        posInfo = new TweeningInfo(worldInfo.cameraInfo.posTween, PM.GetHoldNoteSecs(PM.worldInfoIndex));
    }
    public void UpdateTweenValue()
    {
        orthoSize = stdSize * ((TweenerInfo<float>)orthoSizeInfo).curValue;
        BGColor = ((TweenerInfo<Color>)BGColorInfo).curValue;
        rotation = handy.GetCorrectDegMaxIs0(-((TweenerInfo<float>)rotationInfo).curValue);
        pos = ((TweenerInfo<Vector2>)posInfo).curValue;
    }
    public void PlayWaitTween(){}
    public void PlayHoldTween()
    {
        handy.PlayTweens(orthoSizeInfo, BGColorInfo, rotationInfo, posInfo);
    }
    BaseCamera()
    {
        stdSize = Screen.height * 0.005f;
    }
}
