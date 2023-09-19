using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

namespace OVERIZE
{
    public class DelaySetting : TimeSetting, ITimeSetting
    {
        public DelaySetting(float delay, float startTime) : base(startTime, startTime + delay) { }
    }
    public class TweenChain : Tween, ITween
    {
        List<ITween> tweenSettings = new();
        public List<ITween> TweenSettings { get => tweenSettings; set => tweenSettings = value; }
        List<DelaySetting> delaySettings = new();
        public List<DelaySetting> DelaySettings { get => delaySettings; set => delaySettings = value; }
        public CallBack onUpdateChain;
        CallBack onChainedUpdate;

        public TweenChain OnAddTween(Action<ITween> callBack)
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
        Action<ITween> onAddTween = (TS) => { };
        public override float Duration => (from TS in TweenSettings select TS.Duration).Sum() + (from D in DelaySettings select D.Duration).Sum();

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
            LoopedCount = 0;
            TweenSettings = new();
            DelaySettings = new();
            onAddTween = (TS) =>
            {
                TweenSettings.Add(TS);
                TS.TweenChain = this;
                TS.Init();
            };
            onAddDelay = (delay) => { };
            onChainedUpdate =
            () =>
            {
                if (IsPlaying)
                {
                    foreach (var TS in TweenSettings)
                    {
                        TS.Time = Time - TS.StartTime;
                        if (TS.Time >= 0f)
                            TS.Play();
                    }
                }
            };
            if ((AutoPlay & AutoPlay.Chain) != 0)
                Play();
        }
        public override ITween InitLoop()
        {
            base.InitLoop();
            foreach (var TS in TweenSettings)
                TS.InitLoop();
            return this;
        }
        public override ITween Play()
        {
            base.Play();
            if (IsTweenAble)
            {
                if (IsPlaying)
                {
                    if (UpdateMode != UpdateMode.Manual)
                        TweenCore.TweenUpdater.OnUpdate += onChainedUpdate;
                }
            }
            return this;
        }
        public override ITween Pause()
        {
            base.Pause();
            foreach (var TS in TweenSettings)
                TS.Pause();
            TweenCore.TweenUpdater.OnUpdate -= onChainedUpdate;
            return this;
        }
        public override ITween Rewind()
        {
            base.Rewind();
            foreach (var TS in TweenSettings)
                TS.Rewind();
            return this;
        }
        public override ITween ManualUpdate()
        {
            base.ManualUpdate();
            foreach (var TS in TweenSettings)
                TS.ManualUpdate();
            return this;
        }
        public override ITween Complete()
        {
            base.Complete();
            LoopedCount = LoopCount;
            if (IsAutoKill)
                Kill();
            TweenCore.TweenUpdater.OnUpdate -= onChainedUpdate;
            return this;
        }
        public override ITween CompleteLoop()
        {
            OVERTween.CompleteLoop(this);
            return this;
        }
        public override ITween Kill()
        {
            base.Kill();
            foreach (var TS in TweenSettings)
                TS.Kill();
            TweenCore.TweenUpdater.OnUpdate -= onChainedUpdate;
            return this;
        }
        public override void Defer(float time)
        {
            base.Defer(time);
            foreach (var TS in TweenSettings)
                TS.Defer(time);
            foreach (var D in DelaySettings)
                D.Defer(time);
        }

        public ITween Insert(ITween tweenSetting, float startTime)
        {
            tweenSetting.Defer(startTime);
            TweenSettings.Add(tweenSetting);
            onAddTween(tweenSetting);
            return this;
        }
        public ITween Prepend(ITween tweenSetting)
        {
            Defer(tweenSetting.Duration);
            return Insert(tweenSetting, 0f);
        }
        public ITween Join(ITween tweenSetting) => Insert(tweenSetting, TweenSettings[TweenSettings.Count - 1].StartTime);
        public ITween Append(ITween tweenSetting) => Insert(tweenSetting, TweenSettings[TweenSettings.Count - 1].EndTime);

        public ITween InsertDelay(float delay, float startTime)
        {
            DelaySettings.Add(new(delay, startTime));
            onAddDelay(delay);
            return this;
        }
        public ITween PrependDelay(float delay)
        {
            Defer(delay);
            return InsertDelay(delay, 0f);
        }
        public ITween JoinDelay(float delay) => InsertDelay(delay, TweenSettings[TweenSettings.Count - 1].StartTime);
        public ITween AppendDelay(float delay) => InsertDelay(delay, TweenSettings[TweenSettings.Count - 1].EndTime);
    }
}