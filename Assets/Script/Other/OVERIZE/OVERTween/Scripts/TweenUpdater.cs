using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Unity.VisualScripting;

namespace OVERIZE
{
    public enum UpdateMode { Manual = 0, Fixed = 1 << 0, Nomal = 1 << 1, Late = 1 << 2 }
    [Flags] public enum ExecuteMode { Editor = 1 << 0, RunTime = 1 << 1, All = Editor | RunTime }
    [ExecuteInEditMode]
    public class TweenUpdater : Updater
    {
        internal static float time;
        void Awake()
        {
            gameObject.name = "OVERTween";
        }
        protected override void FixedUpdate() => base.FixedUpdate();
        protected override void Update() => base.Update();
        protected override void LateUpdate() => base.LateUpdate();
        void ExecuteCodeWithTweens(Action<ITween> executeCode)
        {
            foreach (var TS in TweenCore.Tweens)
            {
                bool isExecute = false;
                if ((TS.ExecuteMode.HasFlag(ExecuteMode.Editor) && Application.isEditor) || (TS.ExecuteMode.HasFlag(ExecuteMode.RunTime) && Application.isPlaying))
                    isExecute = true;
                if (isExecute)
                {
                    if (TS.IsPlaying)
                        executeCode(TS);
                }
            }
        }
        protected override void Update(UpdateMode updateMode)
        {
            ExecuteCodeWithTweens((TS) =>
            {
                if (TS.UpdateMode == updateMode)
                {
                    TS.DeltaTime = DeltaTime(TS.UpdateMode, TS.IsUnscaledTime);
                    TS.ManualUpdate();
                }
            });
        }
        internal void ManualUpdate(UpdateMode updateMode, float deltaTime)
        {
            ExecuteCodeWithTweens((TS) =>
            {
                if (TS.UpdateMode == updateMode)
                {
                    TS.DeltaTime = deltaTime;
                    TS.ManualUpdate();
                }
            });
        }
        internal void ManualUpdate(float deltaTime)
        {
            ExecuteCodeWithTweens((TS) =>
            {
                TS.DeltaTime = deltaTime;
                TS.ManualUpdate();
            });
        }
        internal float DeltaTime(UpdateMode updateMode, bool isUnscaledTime)
        {
            if (updateMode == UpdateMode.Fixed)
                return isUnscaledTime ? Time.fixedUnscaledDeltaTime : Time.fixedDeltaTime;
            else if ((updateMode & (UpdateMode.Nomal | UpdateMode.Late)) != 0)
                return isUnscaledTime ? Time.unscaledDeltaTime : Time.deltaTime;
            return 0f;
        }
    }
}
