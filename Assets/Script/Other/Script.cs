using System.Collections;
using System.Collections.Generic;
using TweenManager;
using UnityEngine;
using System;
using System.Linq;

namespace OVERIZE
{
    public abstract class Script : MonoBehaviour/* , IGameObject, IScript, ITweener */
    {
        PlayManager PM;
        bool isInit;
        LevelInfo levelInfo;
        List<string> tweenNames = new List<string>();
        // Dictionary<string, (TweenInfo<TweenAble> TweenInfo, float Duration, TweeningInfo TweeningInfo, TweenAble Value)> tweens
        //  = new Dictionary<string, (TweenInfo<TweenAble> TweenInfo, float Duration, TweeningInfo TweeningInfo, TweenAble Value)>();
        public Dictionary<string, float> tweenDurations = new Dictionary<string, float>();
        Dictionary<string, TweenInfo<TweenAble>> tweenInfos = new Dictionary<string, TweenInfo<TweenAble>>();
        Dictionary<string, TweenAble> tweenValues = new Dictionary<string, TweenAble>();
        Dictionary<string, TweeningInfo> tweeningInfos = new Dictionary<string, TweeningInfo>();

        public bool IsInit { get => isInit; private set => isInit = value; }
        public LevelInfo LevelInfo { get => levelInfo; private set => levelInfo = value; }
        public string[] TweenNames => tweenNames.ToArray();
        // public Dictionary<string, (TweenInfo<TweenAble> TweenInfo, float Duration, TweeningInfo TweeningInfo, TweenAble Value)> Tweens => tweens;
        public Dictionary<string, float> TweenDurations { get => tweenDurations; set => tweenDurations = value; }
        public Dictionary<string, TweenInfo<TweenAble>> TweenInfos { get => tweenInfos; set => tweenInfos = value; }
        public Dictionary<string, TweenAble> TweenValues => tweenValues;
        public Dictionary<string, TweeningInfo> TweeningInfos => tweeningInfos;
        public virtual void Awake()
        {
            PM = PlayManager.Member;
        }
        public TweeningInfo[] GetTweens()
        {
            return TweeningInfos.Values.ToArray();
        }
        /* public void InitTween()
        {
            IsInit = true;

            LevelInfo = PM.GetLevelInfo(PM.levelInfoIndex);
            foreach (var TN in TweenNames)
            {
                TweeningInfos[TN] = new TweeningInfo(TweenInfos[TN], TweenDurations[TN]);
            }

            orthoSizeInfo = new TweeningInfo(levelInfo.cameraInfo.sizeTween, PM.GetNoteHoldSecs(PM.levelInfoIndex));

            BGColorInfo = new TweeningInfo(levelInfo.cameraInfo.BGColorTween, PM.GetNoteHoldSecs(PM.levelInfoIndex));

            rotationInfo = new TweeningInfo(levelInfo.cameraInfo.rotationTween, PM.GetNoteHoldSecs(PM.levelInfoIndex));

            posInfo = new TweeningInfo(levelInfo.cameraInfo.posTween, PM.GetNoteHoldSecs(PM.levelInfoIndex));
        } */
    }
}
