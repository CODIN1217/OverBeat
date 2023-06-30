using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace OVERIZE
{
    public enum UpdateMode { Manual = 1 << 0, Fixed = 1 << 1, Nomal = 1 << 2, Late = 1 << 3 }
    [Flags] public enum ExecuteMode { Editor = 1 << 0, RunTime = 1 << 1 }
    [ExecuteInEditMode]
    class TweenUpdater : MonoBehaviour
    {
        static GameObject instance = null;
        static TweenUpdater tweenUpdater = null;
        internal static TweenUpdater Member
        {
            get
            {
                if (instance == null)
                {
                    instance = new GameObject("OVERTween", typeof(TweenUpdater), typeof(DontDestroyOnLoad));
                }
                return tweenUpdater;
            }
        }
        internal static float time;
        void FixedUpdate()
        {
            UpdateTweens(UpdateMode.Fixed);
        }
        void Update()
        {
            UpdateTweens(UpdateMode.Nomal);
        }
        void LateUpdate()
        {
            UpdateTweens(UpdateMode.Late);
        }
        void ExecuteCodeWithTweens(Action<TweenSetting> executeCode)
        {
            foreach (var TS in TweenCore.TweenSettings)
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
        void UpdateTweens(UpdateMode updateMode)
        {
            ExecuteCodeWithTweens((TS) =>
            {
                if (TS.UpdateMode == updateMode)
                {
                    TS.ManualUpdate();
                    TS.Time += GetDeltaTime(TS) * TS.Speed;
                }
            });
        }
        internal void ManualUpdate(UpdateMode updateMode, float deltaTime)
        {
            ExecuteCodeWithTweens((TS) =>
            {
                if (TS.UpdateMode == updateMode)
                {
                    TS.ManualUpdate();
                    TS.Time += deltaTime * TS.Speed;
                }
            });
        }
        internal void ManualUpdate(float deltaTime)
        {
            ExecuteCodeWithTweens((TS) =>
            {
                TS.ManualUpdate();
                TS.Time += deltaTime * TS.Speed;
            });
        }
        internal void UpdateValue(Setter setter, TweenSetting tweenSetting) => StartCoroutine(UpdateValueCo(setter, tweenSetting));
        IEnumerator UpdateValueCo(Setter setter, TweenSetting tweenSetting)
        {
            while (!tweenSetting.IsComplete)
            {
                setter(tweenSetting.Value);
                yield return new WaitForEndOfFrame();
            }
        }
        float GetDeltaTime(TweenSetting tweenSetting)
        {
            if (tweenSetting.UpdateMode == UpdateMode.Fixed)
                return tweenSetting.IsUnscaledTime ? Time.fixedUnscaledDeltaTime : Time.fixedDeltaTime;
            else if ((tweenSetting.UpdateMode & (UpdateMode.Nomal | UpdateMode.Late)) != 0)
                return tweenSetting.IsUnscaledTime ? Time.unscaledDeltaTime : Time.deltaTime;
            return 0f;
        }
    }
}
