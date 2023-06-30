using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OVERIZE
{
    public class TweenPreference : TweenCallback
    {
        bool isInfiniteLoopInEditMode = true;
        public bool IsInfiniteLoopInEditMode { get => isInfiniteLoopInEditMode; set => isInfiniteLoopInEditMode = value; }
        bool isUnscaledTime = false;
        public bool IsUnscaledTime { get => isUnscaledTime; set => isUnscaledTime = value; }
        bool isInfiniteLoop = false;
        public bool IsInfiniteLoop { get => isInfiniteLoop; set => isInfiniteLoop = value; }
        int loopCount = 1;
        public int LoopCount { get => loopCount; set => loopCount = OVERMath.ClampMin(value, 0); }
        Direction.Horizontal toward = Direction.Horizontal.Right;
        public Direction.Horizontal Toward { get => toward; set => toward = value; }
        float speed = 1f;
        public float Speed { get => speed; set => speed = value; }
        UpdateMode updateMode = UpdateMode.Nomal;
        public UpdateMode UpdateMode { get => updateMode; set => updateMode = value; }
        ExecuteMode executeMode = ExecuteMode.RunTime;
        public ExecuteMode ExecuteMode { get => executeMode; set => executeMode = value; }
        public override void Init()
        {
            base.Init();
            IsInfiniteLoopInEditMode = TweenCore.TweenPreference.IsInfiniteLoopInEditMode;
            IsUnscaledTime = TweenCore.TweenPreference.IsUnscaledTime;
            IsInfiniteLoop = TweenCore.TweenPreference.IsInfiniteLoop;
            LoopCount = TweenCore.TweenPreference.LoopCount;
            Toward = TweenCore.TweenPreference.Toward;
            Speed = TweenCore.TweenPreference.Speed;
            UpdateMode = TweenCore.TweenPreference.UpdateMode;
            ExecuteMode = TweenCore.TweenPreference.ExecuteMode;
        }
    }
}
