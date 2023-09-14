using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

namespace OVERIZE
{
    public class TweeningSetting
    {
        public TimeSetting Time;
        TweenSetting tweenSetting;
        public TweenSetting TweenSetting { get => tweenSetting; internal set { tweenSetting = value; Time.Duration = tweenSetting.Duration; } }
        public TweeningSetting(TweenSetting tweenSetting, float startTime)
        {
            Time = new(startTime, tweenSetting.Duration);
            TweenSetting = tweenSetting;
        }
    }
    public class DelaySetting
    {
        public TimeSetting Time;
        public DelaySetting(float delay, float startTime) => Time = new(startTime, delay);
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
        internal float TotalDuration => (from TS in tweenChain select TS.Time.EndTime).Max();

        public override bool IsUnscaledTime
        {
            get => base.IsUnscaledTime;
            set
            {
                base.IsUnscaledTime = value;
                foreach (var TS in TweenSettings)
                    TS.IsUnscaledTime = base.IsUnscaledTime;
            }
        }
        public TweenChain() => Init();
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
            OnAddTween((TS) => { TS.IsChained = true; TS.Init(); });
            tweenChain = new List<TweeningSetting>();
            if ((AutoPlay & AutoPlay.Chain) != 0)
                Play();
        }
        public void InitLoop()
        {
            foreach (var TS in TweenSettings)
                TS.InitLoop();
        }
        Timer eventTimer = null;
        public override void Play()
        {
            base.Play();
            if (UpdateMode == UpdateMode.Manual)
                return;
            eventTimer ??= new Timer(Updater.Member<TweenUpdater>(), (index) =>
            {
                tweenChain[index].TweenSetting.Play();
                if ((from TS in tweenChain select TS.TweenSetting.IsComplete).IsAll(true))
                    CompleteLoop();
            }, () => IsUnscaledTime, () => Toward, () => (from TS in tweenChain select TS.Time.StartTime).ToArray());
        }
        public override void Pause()
        {
            base.Pause();
            foreach (var TS in TweenSettings)
                TS.Pause();
        }
        public override void Rewind(bool isPlay = true)
        {
            base.Rewind(false);
            foreach (var TS in TweenSettings)
                TS.Rewind(false);
            if (isPlay)
                Play();
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
                if (!IsInfiniteLoop/*  && (Application.isEditor ? !IsInfiniteLoopInEditMode : true) */)
                    Complete();
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
                TS.Time.StartTime += prependTime;
            foreach (var D in delays)
                D.Time.StartTime += prependTime;
            return this;
        }

        public ITween Insert(TweenSetting tweenSetting, float startTime)
        {
            tweenChain.Add(new(tweenSetting, startTime));
            onAddTween(tweenSetting);
            return this;
        }
        public ITween Prepend(TweenSetting tweenSetting) => PrependStartTime(tweenSetting.Duration).Insert(tweenSetting, 0f);
        public ITween Join(TweenSetting tweenSetting) => Insert(tweenSetting, tweenChain[tweenChain.Count - 1].Time.StartTime);
        public ITween Append(TweenSetting tweenSetting) => Insert(tweenSetting, tweenChain[tweenChain.Count - 1].Time.EndTime);

        public ITween InsertDelay(float delay, float startTime)
        {
            delays.Add(new(delay, startTime));
            onAddDelay(delay);
            return this;
        }
        public ITween PrependDelay(float delay) => PrependStartTime(delay).InsertDelay(delay, 0f);
        public ITween JoinDelay(float delay) => InsertDelay(delay, tweenChain[tweenChain.Count - 1].Time.StartTime);
        public ITween AppendDelay(float delay) => InsertDelay(delay, tweenChain[tweenChain.Count - 1].Time.EndTime);
    }
}