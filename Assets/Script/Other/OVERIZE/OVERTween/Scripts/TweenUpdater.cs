using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Unity.VisualScripting;

namespace OVERIZE
{
    public enum UpdateMode { Manual = 0, Fixed = 1 << 0, Nomal = 1 << 1, Late = 1 << 2 }
    [Flags] public enum ExecuteMode { Editor = 1 << 0, RunTime = 1 << 1 }
    [ExecuteInEditMode]
    public class TweenUpdater : Updater
    {
        // static GameObject instance = null;
        // static TweenUpdater tweenUpdater = null;
        // internal static TweenUpdater Member
        // {
        //     get
        //     {
        //         if (instance == null)
        //         {
        //             instance = new GameObject("OVERTween", typeof(TweenUpdater), typeof(DontDestroyOnLoad));
        //             tweenUpdater = instance.GetComponent<TweenUpdater>();
        //         }
        //         return tweenUpdater;
        //     }
        // }
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
            // UpdateMode = updateMode;
            ExecuteCodeWithTweens((TS) =>
            {
                if (TS.UpdateMode == updateMode)
                {
                    TS.ManualUpdate();
                    TS.DeltaTime = GetDeltaTime(TS) * TS.Speed;
                }
            });
        }
        internal void ManualUpdate(UpdateMode updateMode, float deltaTime)
        {
            // UpdateMode = updateMode;
            ExecuteCodeWithTweens((TS) =>
            {
                if (TS.UpdateMode == updateMode)
                {
                    TS.ManualUpdate();
                    TS.DeltaTime = deltaTime * TS.Speed;
                }
            });
            // ManualUpdate(UpdateMode.Manual);
        }
        internal void ManualUpdate(float deltaTime)
        {
            ExecuteCodeWithTweens((TS) =>
            {
                TS.ManualUpdate();
                TS.DeltaTime = deltaTime * TS.Speed;
            });
            // ManualUpdate(UpdateMode.Manual);
        }
        /* internal void UpdateValue(Setter<TweenAble> setter, TweenSetting tweenSetting) => StartCoroutine(UpdateValueCo(setter, tweenSetting));
        IEnumerator UpdateValueCo(Setter<TweenAble> setter, TweenSetting tweenSetting)
        {
            while (!tweenSetting.IsComplete)
            {
                setter(tweenSetting.Value);
                yield return new WaitForEndOfFrame();
            }
        } */
        float GetDeltaTime(ITween tweenSetting)
        {
            if (tweenSetting.UpdateMode == UpdateMode.Fixed)
                return tweenSetting.IsUnscaledTime ? Time.fixedUnscaledDeltaTime : Time.fixedDeltaTime;
            else if ((tweenSetting.UpdateMode & (UpdateMode.Nomal | UpdateMode.Late)) != 0)
                return tweenSetting.IsUnscaledTime ? Time.unscaledDeltaTime : Time.deltaTime;
            return 0f;
        }
    }
}
