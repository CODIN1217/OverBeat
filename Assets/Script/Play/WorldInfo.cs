using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System;

[Serializable]
public class WorldInfo : MonoBehaviour
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
        public int moveDir;
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
            moveDir = 1;
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
    [Serializable]
    public class NoteInfo
    {
        public int eachNoteIndex;
        public int tarPlayerIndex;
        public float awakeSecs;
        public float speed;
        public string sideImageName;
        public TweenInfo<float> waitRadiusTween;
        public TweenInfo<float> holdRadiusTween;
        public TweenInfo<float> appearanceTween;
        public TweenInfo<float> totalRotationTween;
        public TweenInfo<float> startRotationTween;
        public TweenInfo<float> endRotationTween;
        public TweenInfo<Color> startColorTween;
        public TweenInfo<Color> processStartColorTween;
        public TweenInfo<Color> processEndColorTween;
        public TweenInfo<Color> endColorTween;
        public NoteInfo()
        {
            eachNoteIndex = 0;
            tarPlayerIndex = 0;
            awakeSecs = 0f;
            speed = 1f;
            sideImageName = "Basic";
            waitRadiusTween = new TweenInfo<float>(5f, 1.5f, AnimationCurve.Linear(0, 0, 1, 1));
            holdRadiusTween = new TweenInfo<float>(0f, 0f, AnimationCurve.Linear(0, 0, 1, 1));
            appearanceTween = new TweenInfo<float>(0f, 1f, AnimationCurve.Linear(0, 0, 1, 1));
            totalRotationTween = new TweenInfo<float>(0f, 0f, AnimationCurve.Linear(0, 0, 1, 1));
            startRotationTween = new TweenInfo<float>(0f, 0f, AnimationCurve.Linear(0, 0, 1, 1));
            endRotationTween = new TweenInfo<float>(0f, 0f, AnimationCurve.Linear(0, 0, 1, 1));
            startColorTween = new TweenInfo<Color>(new Color(100, 45, 250, 255) / 255f, new Color(100, 45, 250, 255) / 255f, AnimationCurve.Linear(0, 0, 1, 1));
            processStartColorTween = new TweenInfo<Color>(new Color(130, 80, 255, 255) / 255f, new Color(130, 80, 255, 255) / 255f, AnimationCurve.Linear(0, 0, 1, 1));
            processEndColorTween = new TweenInfo<Color>(new Color(130, 80, 255, 255) / 255f, new Color(130, 80, 255, 255) / 255f, AnimationCurve.Linear(0, 0, 1, 1));
            endColorTween = new TweenInfo<Color>(new Color(100, 45, 250, 255) / 255f, new Color(100, 45, 250, 255) / 255f, AnimationCurve.Linear(0, 0, 1, 1));
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
            coverColorTween = new TweenInfo<Color>();
            scaleTween = new TweenInfo<Vector2>(Vector2.one, Vector2.one, AnimationCurve.Linear(0, 0, 1, 1));
            posTween = new TweenInfo<Vector2>();
        }
    }
    [Serializable]
    public class JudgmentInfo
    {
        public float range;
        public Color[] judgmentColors;
        public JudgmentInfo()
        {
            range = 0.5f;
            judgmentColors = new Color[]{new Color(100, 255, 65) / 255f, new Color(255, 235, 0) / 255f, new Color(160, 0, 140) / 255f, new Color(215, 0, 15) / 255f};
        }
    }
    [Serializable]
    public class CreditInfo
    {
        public string worldName;
        public string worldEditor;
        public string songName;
        public string songWriter;
        public CreditInfo()
        {
            worldName = "Empty";
            worldEditor = "Empty";
            songName = "Empty";
            songWriter = "Empty";
        }
    }
    public CameraInfo cameraInfo;
    public CountDownInfo countDownInfo;
    public PlayerInfo[] playerInfo;
    public NoteInfo noteInfo;
    public CenterInfo centerInfo;
    public BoundaryInfo boundaryInfo;
    public JudgmentInfo judgmentInfo;
    public CreditInfo creditInfo;

    public WorldInfo()
    {
        cameraInfo = new CameraInfo();
        centerInfo = new CenterInfo();
        countDownInfo = new CountDownInfo();
        playerInfo = new PlayerInfo[2];
        noteInfo = new NoteInfo();
        boundaryInfo = new BoundaryInfo();
        judgmentInfo = new JudgmentInfo();
        creditInfo = new CreditInfo();
    }
}
