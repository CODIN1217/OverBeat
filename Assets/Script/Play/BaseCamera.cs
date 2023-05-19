using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using TweenManager;

public class BaseCamera : MonoBehaviour, ITweener, PlayManager.ITweenerInPlay, IGameObject
{
    public readonly float stdSize;
    public bool isInit;
    Camera baseCamera;
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
    void Awake()
    {
        PM = PlayManager.Member;
        InitGameObjectScript();
    }
    public void InitTween()
    {
        isInit = true;

        worldInfo = PM.GetWorldInfo(PM.worldInfoIndex);

        // TweenMethod.TryKillTween(orthoSizeInfo);
        orthoSizeInfo = new TweeningInfo(worldInfo.cameraInfo.sizeTween, PM.GetNoteHoldSecs(PM.worldInfoIndex));

        // TweenMethod.TryKillTween(BGColorInfo);
        BGColorInfo = new TweeningInfo(worldInfo.cameraInfo.BGColorTween, PM.GetNoteHoldSecs(PM.worldInfoIndex));

        // TweenMethod.TryKillTween(rotationInfo);
        rotationInfo = new TweeningInfo(worldInfo.cameraInfo.rotationTween, PM.GetNoteHoldSecs(PM.worldInfoIndex));

        // TweenMethod.TryKillTween(posInfo);
        posInfo = new TweeningInfo(worldInfo.cameraInfo.posTween, PM.GetNoteHoldSecs(PM.worldInfoIndex));
    }
    public void UpdateTweenValue()
    {
        orthoSize = stdSize * ((TweenerInfo<float>)orthoSizeInfo).curValue;
        BGColor = ((TweenerInfo<Color>)BGColorInfo).curValue;
        rotation = Handy.Math.DegMethod.CorrectDegMaxIs0(-((TweenerInfo<float>)rotationInfo).curValue);
        pos = ((TweenerInfo<Vector2>)posInfo).curValue;
    }
    public void PlayWaitTween() { }
    public void PlayHoldTween()
    {
        TweenMethod.PlayTweens(orthoSizeInfo, BGColorInfo, rotationInfo, posInfo);
    }
    public void TryKillTween()
    {
        TweenMethod.TryKillTweens(orthoSizeInfo, BGColorInfo, rotationInfo, posInfo);
        
        isInit = false;
    }
    public void GotoTween(float toSecs)
    {
        if (isInit)
        {
            orthoSizeInfo.Goto(toSecs);
            BGColorInfo.Goto(toSecs);
            rotationInfo.Goto(toSecs);
            posInfo.Goto(toSecs);
        }
    }
    public void InitGameObjectScript()
    {
        baseCamera = Camera.main;
        PM.AddGO(this).AddTweenerGO(this).AddTweenerInPlayGO(this);
    }
    public void UpdateTransform()
    {
        transform.rotation = Quaternion.Euler(0f, 0f, rotation);
        transform.position = new Vector3(pos.x, pos.y, -1000f);
    }
    public void UpdateRenderer()
    {
        baseCamera.orthographicSize = orthoSize;
        baseCamera.backgroundColor = BGColor;
    }
    BaseCamera()
    {
        stdSize = Screen.height * 0.005f;
    }
}
