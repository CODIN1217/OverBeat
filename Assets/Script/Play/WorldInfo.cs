using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System;

[Serializable] [ExecuteInEditMode]
public class WorldInfo : MonoBehaviour
{
    [Serializable] [ExecuteInEditMode]
    public class CameraInfo_Class
    {
        public /* readonly */ float Rotation;
        public /* readonly */ float Size;
        public /* readonly */ Color BGColor;
        public CameraInfo_Class()
        {
            Rotation = 0f;
            Size = 1f;
            BGColor = new Color(39, 29, 35, 255);
        }
    }
    [Serializable] [ExecuteInEditMode]
    public class CountDownInfo_Class
    {
        public /* readonly */ int NumberOfTick;
        public /* readonly */ float IntervalOfTick;
        public CountDownInfo_Class()
        {
            NumberOfTick = 4;
            IntervalOfTick = 1f;
        }
    }
    [Serializable] [ExecuteInEditMode]
    public class SeveralModeInfo_Class{
        public int Count;
        public SeveralModeInfo_Class(){
            Count = 1;
        }
    }
    [Serializable] [ExecuteInEditMode]
    public class PlayerInfo_Class
    {
        // public /* readonly */ int Index;
        public /* readonly */ int MoveDir;
        public /* readonly */ float TarRadius;
        public /* readonly */ float Rotation;
        public /* readonly */ List<float> StdDegs;
        public /* readonly */ Color PosesGuideColor;
        public /* readonly */ Color SideColor;
        public /* readonly */ Color CenterColor;
        public /* readonly */ Vector2 Scale;
        public /* readonly */ AnimationCurve DegEase;
        public /* readonly */ AnimationCurve TarRadiusEase;
        public PlayerInfo_Class()
        {
            // Index = 0;
            MoveDir = 1;
            TarRadius = 1.5f;
            Rotation = 0f;
            StdDegs = new List<float>() { 0f, 90f, 180f, 270f };
            PosesGuideColor = new Color(115, 85, 200, 255);
            SideColor = new Color(100, 45, 250, 255);
            CenterColor = new Color(65, 20, 185, 255);
            Scale = Vector2.one;
            DegEase = AnimationCurve.Linear(0f, 0f, 1f, 1f);
            TarRadiusEase = AnimationCurve.Linear(0f, 0f, 1f, 1f);
        }
    }
    [Serializable] [ExecuteInEditMode]
    public class NoteInfo_Class
    {
        public /* readonly */ int NextDegIndex;
        // public /* readonly */ float AwakeTime;
        public /* readonly */ float Speed;
        public /* readonly */ float StartRadius;
        public /* readonly */ float Length;
        public /* readonly */ float Rotation;
        public /* readonly */ string SideImageName;
        public /* readonly */ Color StartColor;
        public /* readonly */ Color ProcessColor;
        public /* readonly */ Color EndColor;
        public /* readonly */ AnimationCurve RadiusEase;
        public /* readonly */ AnimationCurve HoldRadiusEase;
        public /* readonly */ AnimationCurve AppearEase;
        public NoteInfo_Class()
        {
            NextDegIndex = 0;
            // AwakeTime = 0f;
            Speed = 1f;
            StartRadius = 5f;
            Length = 0f;
            Rotation = 0f;
            SideImageName = "Basic";
            StartColor = new Color(100, 45, 250, 255);
            ProcessColor = new Color(130, 80, 255, 255);
            EndColor = new Color(100, 45, 250, 255);
            RadiusEase = AnimationCurve.Linear(0f, 0f, 1f, 1f);
            HoldRadiusEase = AnimationCurve.Linear(0f, 0f, 1f, 1f);
            AppearEase = AnimationCurve.Linear(0f, 0f, 1f, 1f);
        }
    }
    [Serializable] [ExecuteInEditMode]
    public class CenterInfo_Class
    {
        public /* readonly */ Color Color;
        public /* readonly */ Vector2 Pos;
        public /* readonly */ Vector2 Scale;
        public CenterInfo_Class()
        {
            Color = new Color(0, 255, 160, 255);
            Pos = Vector2.zero;
            Scale = Vector2.one;
        }
    }
    [Serializable] [ExecuteInEditMode]
    public class BoundaryInfo_Class
    {
        public /* readonly */ Color LineColor;
        public /* readonly */ Color? CoverColor;
        public /* readonly */ Vector2 Scale;
        public BoundaryInfo_Class()
        {
            LineColor = new Color(0, 255, 160, 255);
            CoverColor = null/* new Color(39, 29, 35, 255) */;
            Scale = Vector2.one;
        }
    }
    [Serializable] [ExecuteInEditMode]
    public class JudgmentInfo_Class
    {
        public /* readonly */ float Range;
        public JudgmentInfo_Class()
        {
            Range = 0.5f;
        }
    }
    [Serializable] [ExecuteInEditMode]
    public class CreditInfo_Class
    {
        public /* readonly */ string WorldName;
        public /* readonly */ string WorldEditor;
        public /* readonly */ string SongName;
        public /* readonly */ string SongWriter;
        public CreditInfo_Class()
        {
            WorldName = "Empty";
            WorldEditor = "Empty";
            SongName = "Empty";
            SongWriter = "Empty";
        }
    }
    public readonly float IntervalTimeToWait;
    public readonly CameraInfo_Class CameraInfo;
    public readonly CountDownInfo_Class CountDownInfo;
    public readonly SeveralModeInfo_Class SeveralModeInfo;
    public readonly List<PlayerInfo_Class> PlayerInfo;
    public readonly List<NoteInfo_Class> NoteInfo;
    public readonly CenterInfo_Class CenterInfo;
    public readonly BoundaryInfo_Class BoundaryInfo;
    public readonly JudgmentInfo_Class JudgmentInfo;
    public readonly CreditInfo_Class CreditInfo;

    public WorldInfo()
    {
        IntervalTimeToWait = 1f;
        CameraInfo = new CameraInfo_Class();
        CountDownInfo = new CountDownInfo_Class();
        SeveralModeInfo = new SeveralModeInfo_Class();
        PlayerInfo = new List<PlayerInfo_Class>(SeveralModeInfo.Count);
        NoteInfo = new List<NoteInfo_Class>(SeveralModeInfo.Count);
        CenterInfo = new CenterInfo_Class();
        BoundaryInfo = new BoundaryInfo_Class();
        JudgmentInfo = new JudgmentInfo_Class();
        CreditInfo = new CreditInfo_Class();
    }
}
