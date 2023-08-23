using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System;
using TweenManager;

[Serializable]
public class LevelInfo
{
    [Serializable]
    public class CameraInfo
    {
        public TweenInfo<float> rotationTween;
        public TweenInfo<float> sizeTween;
        public TweenInfo<Vector2> posTween;
        public TweenInfo<Color> BGColorTween;
        public CameraInfo()
        {
            rotationTween = new TweenInfo<float>(0f, 0f, AnimationCurve.Linear(0, 0, 1, 1));
            sizeTween = new TweenInfo<float>(1f, 1f, AnimationCurve.Linear(0, 0, 1, 1));
            posTween = new TweenInfo<Vector2>(Vector2.zero, Vector2.zero, AnimationCurve.Linear(0, 0, 1, 1));
            BGColorTween = new TweenInfo<Color>(new Color(39, 29, 35, 255) / 255f, new Color(39, 29, 35, 255) / 255f, AnimationCurve.Linear(0, 0, 1, 1));
        }
    }
    [Serializable]
    public class CountDownInfo
    {
        public int numberOfTick;
        public float intervalOfTick;
        public CountDownInfo()
        {
            numberOfTick = 4;
            intervalOfTick = 1f;
        }
    }
    [Serializable]
    public class PlayerInfo
    {
        public int degDir;
        public TweenInfo<float> degTween;
        public TweenInfo<float> radiusTween;
        public TweenInfo<float> rotationTween;
        public TweenInfo<Color> sideColorTween;
        public TweenInfo<Color> centerColorTween;
        public TweenInfo<Vector2> totalScaleTween;
        public TweenInfo<Vector2> sideScaleTween;
        public TweenInfo<Vector2> centerScaleTween;
        public PlayerInfo()
        {
            degDir = 1;
            degTween = new TweenInfo<float>(0f, 0f, AnimationCurve.Linear(0, 0, 1, 1));
            radiusTween = new TweenInfo<float>(1.5f, 1.5f, AnimationCurve.Linear(0, 0, 1, 1));
            rotationTween = new TweenInfo<float>(0f, 0f, AnimationCurve.Linear(0, 0, 1, 1));
            sideColorTween = new TweenInfo<Color>(new Color(100, 45, 250, 255) / 255f, new Color(100, 45, 250, 255) / 255f, AnimationCurve.Linear(0, 0, 1, 1));
            centerColorTween = new TweenInfo<Color>(new Color(65, 20, 185, 255) / 255f, new Color(65, 20, 185, 255) / 255f, AnimationCurve.Linear(0, 0, 1, 1));
            totalScaleTween = new TweenInfo<Vector2>(Vector2.one, Vector2.one, AnimationCurve.Linear(0, 0, 1, 1));
            sideScaleTween = new TweenInfo<Vector2>(Vector2.one, Vector2.one, AnimationCurve.Linear(0, 0, 1, 1));
            centerScaleTween = new TweenInfo<Vector2>(Vector2.one, Vector2.one, AnimationCurve.Linear(0, 0, 1, 1));
        }
    }
    public class NotePreInfo
    {
        public int noteCount;
        public NotePreInfo()
        {
            noteCount = 2;
        }
    }
    [Serializable]
    public enum InsideNoteType { Tap, Keep }
    [Serializable]
    public class NoteInfo : NotePreInfo
    {
        public InsideNoteType insideNoteType;
        public int eachNoteIndex;
        public int tarPlayerIndex;
        public float awakeSecs;
        public float speed;
        public float[] noteHitTiming01s;
        public string sideImageName;
        public bool isCheckPoint;
        public TweenInfo<float> waitDeltaRadiusTween;
        public TweenInfo<float> holdDeltaRadiusTween;
        public TweenInfo<float> fadeTween;
        public TweenInfo<float>[] rotationTweens;
        public TweenInfo<Color>[] colorTweens;
        public TweenInfo<Color> processStartColorTween;
        public TweenInfo<Color> processEndColorTween;
        public NoteInfo() : base()
        {
            insideNoteType = InsideNoteType.Tap;

            eachNoteIndex = 0;
            tarPlayerIndex = 0;

            awakeSecs = 0f;
            speed = 1f;
            noteHitTiming01s = new float[noteCount];
            sideImageName = "PlayerBasic";
            isCheckPoint = false;
            waitDeltaRadiusTween = new TweenInfo<float>(3.5f, 0f, AnimationCurve.Linear(0, 0, 1, 1));
            holdDeltaRadiusTween = new TweenInfo<float>(0f, 0f, AnimationCurve.Linear(0, 0, 1, 1));
            fadeTween = new TweenInfo<float>(0f, 1f, AnimationCurve.Linear(0, 0, 1, 1));

            rotationTweens = new TweenInfo<float>[noteCount];
            Handy.RepeatCode((i) => { rotationTweens[i] = new TweenInfo<float>(0f, 0f, AnimationCurve.Linear(0, 0, 1, 1)); }, noteCount);

            colorTweens = new TweenInfo<Color>[noteCount];
            Handy.RepeatCode((i) => { colorTweens[i] = new TweenInfo<Color>(new Color(100, 45, 250, 255) / 255f, new Color(100, 45, 250, 255) / 255f, AnimationCurve.Linear(0, 0, 1, 1)); }, noteCount);
            processStartColorTween = new TweenInfo<Color>(new Color(130, 80, 255, 255) / 255f, new Color(130, 80, 255, 255) / 255f, AnimationCurve.Linear(0, 0, 1, 1));
            processEndColorTween = new TweenInfo<Color>(new Color(130, 80, 255, 255) / 255f, new Color(130, 80, 255, 255) / 255f, AnimationCurve.Linear(0, 0, 1, 1));
        }
    }
    [Serializable]
    public class CenterInfo
    {
        public TweenInfo<Color> colorTween;
        public TweenInfo<Vector2> posTween;
        public TweenInfo<Vector2> scaleTween;
        public CenterInfo()
        {
            colorTween = new TweenInfo<Color>(new Color(0, 255, 160, 255) / 255f, new Color(0, 255, 160, 255) / 255f, AnimationCurve.Linear(0, 0, 1, 1));
            posTween = new TweenInfo<Vector2>(Vector2.zero, Vector2.zero, AnimationCurve.Linear(0, 0, 1, 1));
            scaleTween = new TweenInfo<Vector2>(Vector2.one, Vector2.one, AnimationCurve.Linear(0, 0, 1, 1));
        }
    }
    [Serializable]
    public class BoundaryInfo
    {
        public TweenInfo<Color> lineColorTween;
        public TweenInfo<Color> coverColorTween;
        public TweenInfo<Vector2> scaleTween;
        public TweenInfo<Vector2> posTween;
        public BoundaryInfo()
        {
            lineColorTween = new TweenInfo<Color>(new Color(0, 255, 160, 255) / 255f, new Color(0, 255, 160, 255) / 255f, AnimationCurve.Linear(0, 0, 1, 1));
            coverColorTween = new TweenInfo<Color>(new Color(39, 29, 35, 255) / 255f, new Color(39, 29, 35, 255) / 255f, AnimationCurve.Linear(0, 0, 1, 1));
            scaleTween = new TweenInfo<Vector2>(Vector2.one, Vector2.one, AnimationCurve.Linear(0, 0, 1, 1));
            posTween = new TweenInfo<Vector2>(Vector2.zero, Vector2.zero, AnimationCurve.Linear(0, 0, 1, 1));
        }
    }
    [Serializable]
    public class JudgmentInfo
    {
        public float range;
        public Color[] judgmentColors;
        public JudgmentInfo()
        {
            range = 0.4f;
            judgmentColors = new Color[] { new Color(100, 255, 65) / 255f, new Color(255, 235, 0) / 255f, new Color(160, 0, 140) / 255f, new Color(215, 0, 15) / 255f };
        }
    }
    public CameraInfo cameraInfo;
    public CountDownInfo countDownInfo;
    public PlayerInfo[] playerInfo;
    public NoteInfo noteInfo;
    public CenterInfo centerInfo;
    public BoundaryInfo boundaryInfo;
    public JudgmentInfo judgmentInfo;


    public LevelInfo()
    {
        cameraInfo = new CameraInfo();
        centerInfo = new CenterInfo();
        countDownInfo = new CountDownInfo();
        playerInfo = new PlayerInfo[1];
        noteInfo = new NoteInfo();
        boundaryInfo = new BoundaryInfo();
        judgmentInfo = new JudgmentInfo();
    }
}
