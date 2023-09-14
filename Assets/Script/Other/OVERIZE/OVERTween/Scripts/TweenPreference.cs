using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OVERIZE
{
    public enum LoopType { Restart = 1 << 0, Yoyo = 1 << 1, Continue = 1 << 2 }
    [Flags] public enum AutoPlay { None = 0, Tween = 1 << 0, Chain = 1 << 1, All = Tween | Chain }
    public class TweenPreference : TweenCallback
    {
        // bool isInfiniteLoopInEditMode = true;
        // public virtual bool IsInfiniteLoopInEditMode { get => isInfiniteLoopInEditMode; set => isInfiniteLoopInEditMode = value; }
        float speed = 1f;
        public float Speed { get => speed; set => speed = value; }
        // float timeScale = 1f;
        // public float TimeScale { get => timeScale; set => timeScale = value; }
        bool isUnscaledTime = false;
        public virtual bool IsUnscaledTime { get => isUnscaledTime; set => isUnscaledTime = value; }
        bool isAutoKill = true;
        public bool IsAutoKill { get => isAutoKill; set => isAutoKill = value; }
        AutoPlay autoPlay = AutoPlay.None;
        public AutoPlay AutoPlay { get => autoPlay; set => autoPlay = value; }
        LoopType loopType = LoopType.Restart;
        public LoopType LoopType { get => loopType; set => loopType = value; }
        UpdateMode updateMode = UpdateMode.Nomal;
        public UpdateMode UpdateMode { get => updateMode; set => updateMode = value; }
        ExecuteMode executeMode = ExecuteMode.RunTime;
        public ExecuteMode ExecuteMode { get => executeMode; set => executeMode = value; }
        public override void Init()
        {
            base.Init();
            // IsInfiniteLoopInEditMode = TweenCore.TweenPreference.IsInfiniteLoopInEditMode;
            // TimeScale = TweenCore.TweenPreference.TimeScale;
            Speed = TweenCore.TweenPreference.Speed;
            AutoPlay = TweenCore.TweenPreference.AutoPlay;
            LoopType = TweenCore.TweenPreference.LoopType;
            IsUnscaledTime = TweenCore.TweenPreference.IsUnscaledTime;
            UpdateMode = TweenCore.TweenPreference.UpdateMode;
            ExecuteMode = TweenCore.TweenPreference.ExecuteMode;
        }
    }
}
