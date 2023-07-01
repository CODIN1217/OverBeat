using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

namespace OVERIZE
{
    public class TweenChain : Tween, ITween
    {
        List<List<TweenSetting>> tweenSettings = new List<List<TweenSetting>>();
        List<float> delays = new List<float>() { 0f };

        Action<TweenSetting> onAddTween = (TS) => { };
        public TweenChain OnAddTween(Action<TweenSetting> callBack)
        {
            onAddTween += callBack;
            return this;
        }
        /* Action<float> onAddDelay = (delay) => { };
        public TweenChain OnAddDelay(Action<float> callBack)
        {
            onAddDelay += callBack;
            return this;
        } */

        List<TweenSetting> tweenSettingsTemp = new List<TweenSetting>();
        internal List<TweenSetting> TweenSettings { get => tweenSettingsTemp; }

        public override bool IsUnscaledTime { get => UpdateIsUnscaledTime(base.IsUnscaledTime); set => base.IsUnscaledTime = UpdateIsUnscaledTime(value); }
        bool UpdateIsUnscaledTime(bool isUnscaledTime)
        {
            foreach (var TS in TweenSettings)
            {
                TS.IsUnscaledTime = isUnscaledTime;
            }
            return isUnscaledTime;
        }
        public TweenChain()
        {
            OnAddTween((TS) => tweenSettingsTemp.Add(TS));
            OnAddTween((TS) => TS.IsChained = true);
            // OnAddTween((TS) => totalDuration += TS.Duration);

            // OnAddDelay((delay) => totalDuration += delay);
        }
        public override void Init()
        {
            base.Init();
            InitLoop();
            // totalDuration = 0f;
            curLoopCount = 0;
            tweenSettings = new List<List<TweenSetting>>();
            delays = new List<float>() { 0f };
            onAddTween = (TS) => { };
            OnAddTween((TS) => tweenSettingsTemp.Add(TS));
            OnAddTween((TS) => TS.IsChained = true);
            tweenSettingsTemp = new List<TweenSetting>();
        }
        public void InitLoop()
        {
            foreach (var TS in TweenSettings)
                TS.InitLoop();
        }
        public override void Play()
        {
            if (UpdateMode == UpdateMode.Manual)
                return;
            base.Play();
            if (curLoopCount == 0)
            {
                curLoopCount++;
                onStart();
            }
            TweenUpdater.Member.StartCoroutine(PlayCo());
        }
        public void ManualUpdate()
        {
            foreach (var TS in TweenSettings)
                TS.ManualUpdate();
            onUpdate();
        }
        public override void Complete()
        {
            base.Complete();
            curLoopCount = LoopCount;
            if (IsAutoKill)
                foreach (var TS in TweenSettings)
                    TS.Kill(false);
        }
        public void Kill(bool isComplete = true)
        {
            if (isComplete)
                Complete();
            IsPlaying = false;
            foreach (var TS in TweenSettings)
                TweenCore.RemoveTweenSetting(TS.TweenID);
        }
        int tweenPlayIndex = 0;
        IEnumerator PlayCo()
        {
            float delay = delays[0];
            yield return IsUnscaledTime ? new WaitForSecondsRealtime(delay) : new WaitForSeconds(delay);
            for (; tweenPlayIndex < tweenSettings.Count; tweenPlayIndex++)
            {
                foreach (var TS in tweenSettings[tweenPlayIndex])
                    TS.Play();
                yield return new WaitUntil(() => tweenSettings[tweenPlayIndex][0].IsComplete);
                delay = delays[tweenPlayIndex + 1];
                yield return IsUnscaledTime ? new WaitForSecondsRealtime(delay) : new WaitForSeconds(delay);
            }
            bool isComplete = true;
            do
            {
                foreach (var TS in TweenSettings)
                    isComplete = TS.IsComplete;
                if (!isComplete)
                    yield return new WaitForEndOfFrame();
            }
            while (!isComplete);
            TweenUpdater.Member.StartCoroutine(CompleteCo());
        }
        IEnumerator CompleteCo()
        {
            yield return new WaitForEndOfFrame();
            onCompleteLoop();
            if (!IsInfiniteLoop && (Application.isEditor ? !IsInfiniteLoopInEditMode : true))
            {
                if (curLoopCount >= LoopCount)
                {
                    Complete();
                }
            }
            if (curLoopCount < LoopCount)
            {
                InitLoop();
                curLoopCount++;
            }
        }
        public TweenChain Prepend(TweenSetting tweenSetting)
        {
            tweenSettings.Insert(0, new List<TweenSetting> { tweenSetting });
            delays.Insert(0, 0f);
            onAddTween(tweenSetting);
            return this;
        }
        public TweenChain Join(TweenSetting tweenSetting)
        {
            if (tweenSettings.Count > 0)
                tweenSettings[tweenSettings.Count - 1].Add(tweenSetting);
            onAddTween(tweenSetting);
            return this;
        }
        public TweenChain Append(TweenSetting tweenSetting)
        {
            tweenSettings.Add(new List<TweenSetting> { tweenSetting });
            delays.Add(0f);
            onAddTween(tweenSetting);
            return this;
        }
        public TweenChain PrependDelay(float delay)
        {
            delays[0] += delay;
            // onAddDelay(delay);
            return this;
        }
        public TweenChain AppendDelay(float delay)
        {
            delays[delays.Count - 1] += delay;
            // onAddDelay(delay);
            return this;
        }
    }
}