using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace OVERIZE
{
    public enum UpdateMode { Manual, Fixed, Nomal, Late }
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

        internal UpdateMode updateMode = UpdateMode.Nomal;
        internal bool isExecuteInEditMode;
        internal bool isUnscaledTime;
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
            if (!isExecuteInEditMode || updateMode != UpdateMode.Fixed)
                return;
            UpdateTweens();
            if (isUpdated)
                UpdateTime(isUnscaledTime ? Time.fixedUnscaledDeltaTime : Time.fixedDeltaTime);
        }
        void Update()
        {
            if (!isExecuteInEditMode || updateMode != UpdateMode.Nomal)
                return;
            UpdateTweens();
            if (isUpdated)
                UpdateTime(isUnscaledTime ? Time.unscaledDeltaTime : Time.deltaTime);
        }
        void LateUpdate()
        {
            if (!isExecuteInEditMode || updateMode != UpdateMode.Late)
                return;
            UpdateTweens();
            if (isUpdated)
                UpdateTime(isUnscaledTime ? Time.unscaledDeltaTime : Time.deltaTime);
        }
        public void UpdateTweens()
        {
            if (!isUpdated)
            {
                foreach (var TC in TweenCore.TweenChains)
                {
                    foreach (var TS in TC.TweenSettings)
                    {
                        TS.Time = (time - TS.startTime) * TS.Speed;
                        TS.Update();
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
        void UpdateTime(float deltaTime) => time += deltaTime;
    }
}
