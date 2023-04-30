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

    public TweeningInfo orthoSizeInfo;
    public TweeningInfo BGColorInfo;
    public TweeningInfo rotationInfo;
    public TweeningInfo posInfo;

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
        // InfoViewer.Property.SetInfo(this.name, nameof(playGM.GetHoldNoteSecs), () => playGM.GetHoldNoteSecs(playGM.worldInfoIndex));
        if (!handy.compareValue_int.CompareWithBeforeValue(this.name, nameof(Update), nameof(playGM.worldInfoIndex), playGM.worldInfoIndex))
        {
            handy.TryKillTween(orthoSizeInfo);
            orthoSizeInfo = new TweeningInfo(worldInfo.cameraInfo.sizeTween, playGM.GetHoldNoteSecs(playGM.worldInfoIndex));

            handy.TryKillTween(BGColorInfo);
            BGColorInfo = new TweeningInfo(worldInfo.cameraInfo.BGColorTween, playGM.GetHoldNoteSecs(playGM.worldInfoIndex));

            handy.TryKillTween(rotationInfo);
            rotationInfo = new TweeningInfo(worldInfo.cameraInfo.rotationTween, playGM.GetHoldNoteSecs(playGM.worldInfoIndex));

            handy.TryKillTween(posInfo);
            posInfo = new TweeningInfo(worldInfo.cameraInfo.posTween, playGM.GetHoldNoteSecs(playGM.worldInfoIndex));

            handy.PlayTweens(orthoSizeInfo, BGColorInfo, rotationInfo, posInfo);

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
        orthoSize = stdSize * (float)orthoSizeInfo.curValue;
        BGColor = (Color)BGColorInfo.curValue;
        rotation = handy.GetCorrectDegMaxIs0(-(float)rotationInfo.curValue);
        pos = (Vector2)posInfo.curValue;
    }
    BaseCamera()
    {
        stdSize = Screen.height * 0.005f;
    }
}
