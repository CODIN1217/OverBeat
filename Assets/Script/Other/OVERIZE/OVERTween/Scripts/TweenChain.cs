using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

namespace OVERIZE
{
    public class TweeningSetting
    {
        float startTime;
        public float StartTime { get => startTime; internal set => startTime = value; }
        public float EndTime => StartTime + TweenSetting.Duration;
        TweenSetting tweenSetting;
        public TweenSetting TweenSetting { get => tweenSetting; internal set => tweenSetting = value; }
        public TweeningSetting(TweenSetting tweenSetting, float startTime)
        {
            StartTime = startTime;
            TweenSetting = tweenSetting;
        }
    }
    public class DelaySetting
    {
        float startTime;
        public float StartTime { get => startTime; internal set => startTime = value; }
        public float EndTime => StartTime + Delay;
        float delay;
        public float Delay { get => delay; internal set => delay = value; }
        public DelaySetting(float delay, float startTime)
        {
            StartTime = startTime;
            Delay = delay;
        }
    }
    public class TweenChain : Tween, ITween
    {
        List<TweeningSetting> tweenChain = new();
        List<DelaySetting> delays = new();

        public TweenChain OnAddTween(Action<TweenSetting> callBack)
        {
            onAddTween += callBack;
            return this;
        }
        public TweenChain OnAddDelay(Action<float> callBack)
        {
            onAddDelay += callBack;
            return this;
        }

        Action<float> onAddDelay = (delay) => { };
        Action<TweenSetting> onAddTween = (TS) => { };
        internal List<TweenSetting> TweenSettings => (from TS in tweenChain select TS.TweenSetting).ToList();

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
            OnAddTween((TS) => TweenSettings.Add(TS));
            OnAddTween((TS) => TS.IsChained = true);
        }
        public override void Init()
        {
            base.Init();
            InitLoop();
            curLoopCount = 0;
            tweenChain = new();
            delays = new();
            onAddTween = (TS) => { };
            onAddDelay = (delay) => { };
            OnAddTween((TS) => TweenSettings.Add(TS));
            OnAddTween((TS) => TS.IsChained = true);
            tweenChain = new List<TweeningSetting>();
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
            new Timer(Updater.Member<TweenUpdater>(), (index) =>
            {
                tweenChain[index].TweenSetting.Play();
                if ((from TS in tweenChain select TS.TweenSetting.IsComplete).IsAll(true))
                    CompleteLoop();
            }, IsUnscaledTime, (from TS in tweenChain select TS.StartTime).ToArray());
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
        public void CompleteLoop()
        {
            onCompleteLoop();
            if (curLoopCount >= LoopCount)
            {
                if (!IsInfiniteLoop && (Application.isEditor ? !IsInfiniteLoopInEditMode : true))
                {
                    Complete();
                }
            }
            else
            {
                InitLoop();
                curLoopCount++;
            }
        }
        public void Kill(bool isComplete = true)
        {
            if (isComplete)
                Complete();
            IsPlaying = false;
            foreach (var TS in TweenSettings)
                TweenCore.RemoveTweenSetting(TS.TweenID);
        }

        TweenChain PrependStartTime(float prependTime)
        {
            foreach (var TS in tweenChain)
                TS.StartTime += prependTime;
            foreach (var D in delays)
                D.StartTime += prependTime;
            return this;
        }

        public ITween Insert(TweenSetting tweenSetting, float startTime)
        {
            tweenChain.Add(new(tweenSetting, startTime));
            onAddTween(tweenSetting);
            return this;
        }
        public ITween Prepend(TweenSetting tweenSetting) => PrependStartTime(tweenSetting.Duration).Insert(tweenSetting, 0f);
        public ITween Join(TweenSetting tweenSetting) => Insert(tweenSetting, tweenChain[tweenChain.Count - 1].StartTime);
        public ITween Append(TweenSetting tweenSetting) => Insert(tweenSetting, tweenChain[tweenChain.Count - 1].EndTime);

        public ITween InsertDelay(float delay, float startTime)
        {
            delays.Add(new(delay, startTime));
            onAddDelay(delay);
            return this;
        }
        public ITween PrependDelay(float delay) => PrependStartTime(delay).InsertDelay(delay, 0f);
        public ITween JoinDelay(float delay) => InsertDelay(delay, tweenChain[tweenChain.Count - 1].StartTime);
        public ITween AppendDelay(float delay) => InsertDelay(delay, tweenChain[tweenChain.Count - 1].EndTime);
    }
}