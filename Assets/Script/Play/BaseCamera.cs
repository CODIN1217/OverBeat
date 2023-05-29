using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using TweenManager;

public class BaseCamera : MonoBehaviour, PlayManager.ITweenerInPlay, IGameObject, IScript, ITweener
{
    public readonly float stdSize;
    public bool isInit;
    Camera baseCamera;
    LevelInfo levelInfo;
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
        baseCamera = Camera.main;
        InitScript();
    }
    public TweeningInfo[] GetTweens()
    {
        return Handy.GetArray(orthoSizeInfo, BGColorInfo, rotationInfo, posInfo);
    }
    public void InitTween()
    {
        isInit = true;

        levelInfo = PM.GetLevelInfo(PM.levelInfoIndex);

        orthoSizeInfo = new TweeningInfo(levelInfo.cameraInfo.sizeTween, PM.GetNoteHoldSecs(PM.levelInfoIndex));

        BGColorInfo = new TweeningInfo(levelInfo.cameraInfo.BGColorTween, PM.GetNoteHoldSecs(PM.levelInfoIndex));

        rotationInfo = new TweeningInfo(levelInfo.cameraInfo.rotationTween, PM.GetNoteHoldSecs(PM.levelInfoIndex));

        posInfo = new TweeningInfo(levelInfo.cameraInfo.posTween, PM.GetNoteHoldSecs(PM.levelInfoIndex));
    }
    public void UpdateTweenValue()
    {
        orthoSize = stdSize * ((TweenerInfo<float>)orthoSizeInfo).curValue;
        BGColor = ((TweenerInfo<Color>)BGColorInfo).curValue;
        rotation = Handy.GetCorrectedDegMaxIs0(-((TweenerInfo<float>)rotationInfo).curValue);
        pos = ((TweenerInfo<Vector2>)posInfo).curValue;
    }
    public void PlayWaitTween() { }
    public void PlayHoldTween()
    {
        TweenMethod.TryPlayTween(GetTweens());
    }
    public void TryKillTween()
    {
        TweenMethod.TryKillTween(GetTweens());

        isInit = false;
    }
    public void GotoTween(float toSecs)
    {
        if (isInit)
        {
            foreach (var T in GetTweens())
            {
                T.Goto(toSecs);
            }
        }
    }
    public void InitScript()
    {
        PM.AddGO(this).AddTweenerInPlayGO(this).AddScript(this).AddTweener(this);
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
