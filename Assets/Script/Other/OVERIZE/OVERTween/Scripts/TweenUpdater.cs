using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace OVERIZE
{
    public enum UpdateMode { Manual, Fixed, Nomal, Late }
    [Flags] public enum ExecuteMode { Editor = 1 << 0, RunTime = 1 << 1 }
    [ExecuteInEditMode]
    class TweenUpdater : MonoBehaviour
    {
        static TweenUpdater instance = null;
        internal static TweenUpdater Member
        {
            get
            {
                return instance;
            }
        }
        internal static float time;
        bool isUpdated;

        // internal bool isExecuteInEditMode;
        void Awake()
        {
            if (instance == null)
            {
                instance = this;
                try { DontDestroyOnLoad(gameObject); } catch { }
            }
            else
            {
                Destroy(gameObject);
            }
        }
        void FixedUpdate()
        {
            // if (!isExecuteInEditMode)
            //     return;
            UpdateTweens(UpdateMode.Fixed);
        }
        void Update()
        {
            // if (!isExecuteInEditMode)
            //     return;
            UpdateTweens(UpdateMode.Nomal);
        }
        void LateUpdate()
        {
            // if (!isExecuteInEditMode)
            //     return;
            UpdateTweens(UpdateMode.Late);
        }
        void UpdateTweens(UpdateMode? updateMode = null, float? deltaTime = null)
        {
            if (!isUpdated)
            {
                foreach (var TC in TweenCore.TweenChains)
                {
                    foreach (var TS in TC.TweenSettings)
                    {
                        bool isExecute = true;
                        if ((TS.ExecuteMode.HasFlag(ExecuteMode.Editor) && !Application.isEditor) && (TS.ExecuteMode.HasFlag(ExecuteMode.RunTime) && !Application.isPlaying))
                            isExecute = false;
                        if (isExecute)
                        {
                            if (updateMode == null || TS.UpdateMode == updateMode)
                            {
                                TS.Update();

                                if ((updateMode == UpdateMode.Manual || updateMode == null) && deltaTime == null)
                                    return;
                                switch ((int)TS.UpdateMode)
                                {
                                    case 1:
                                        deltaTime = TS.IsUnscaledTime ? Time.fixedUnscaledDeltaTime : Time.fixedDeltaTime;
                                        break;
                                    case 2:
                                    case 3:
                                        deltaTime = TS.IsUnscaledTime ? Time.unscaledDeltaTime : Time.deltaTime;
                                        break;
                                }
                                TS.Time += (float)deltaTime * TS.Speed;
                            }
                        }
                    }
                }
                isUpdated = true;
                StartCoroutine(SetIsUpdated());
            }
        }
        IEnumerator SetIsUpdated()
        {
            yield return new WaitForEndOfFrame();
            isUpdated = false;
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
    }
}
