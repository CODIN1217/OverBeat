using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OVERIZE
{
    public class TweenSetting : Tween, ITween
    {
        public override bool IsInfiniteLoop { get => base.IsInfiniteLoop && !IsChained; set => base.IsInfiniteLoop = value && !IsChained; }

        TweenData tweenData;
        public TweenData TweenData { get => tweenData; internal set => tweenData = value; }
        TweenAble value;
        public TweenAble Value { get => value; internal set => this.value = value; }

        public TweenSetting(TweenData tweenData, float duration)
        {
            this.tweenData = tweenData;
            StartTime = 0f;
            EndTime = duration;

            Init();
            ManualUpdate();
        }

        public TweenAble Evaluate(float time) => tweenData.startValue + (tweenData.endValue - tweenData.startValue) * tweenData.ease[time / Duration];
        public override void Init()
        {
            base.Init();
            InitLoop();
            if (((AutoPlay & AutoPlay.Tween) != 0) && !IsChained)
                Play();
            else
                Pause();

        }
        public override ITween InitLoop()
        {
            base.InitLoop();
            value = tweenData.startValue;
            return this;
        }
        public override ITween Rewind()
        {
            base.Rewind();
            value = tweenData.endValue;
            Time = Duration;
            return this;
        }
        public override ITween Complete()
        {
            base.Complete();
            Value = tweenData.endValue;
            Time = Duration;
            LoopedCount = LoopCount;
            if (IsAutoKill)
                Kill();
            return this;
        }
        public override ITween CompleteLoop()
        {
            OVERTween.CompleteLoop(this);
            return this;
        }
        public override ITween ManualUpdate()
        {
            base.ManualUpdate();
            Value = Evaluate(Time);
            if (!IsTweenAble)
                CompleteLoop();
            return this;
        }
        public TweenSetting SetName(string name)
        {
            TweenID.Name = name;
            return this;
        }
    }
}
