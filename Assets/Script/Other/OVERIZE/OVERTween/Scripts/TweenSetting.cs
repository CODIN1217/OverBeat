using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OVERIZE
{
    public class TweenSetting : Tween, ITween
    {
        bool isChained;
        internal bool IsChained { get => isChained; set => isChained = value; }

        // public override bool IsInfiniteLoopInEditMode { get => base.IsInfiniteLoopInEditMode && !IsChained; set => base.IsInfiniteLoopInEditMode = value && !IsChained; }
        public override bool IsInfiniteLoop { get => base.IsInfiniteLoop && !IsChained; set => base.IsInfiniteLoop = value && !IsChained; }

        TweenData tweenData;
        public TweenData TweenData { get => tweenData; internal set => tweenData = value; }
        float duration;
        public float Duration { get => duration; internal set => duration = value; }
        TweenAble value;
        public TweenAble Value { get => value; internal set => this.value = value; }

        public TweenSetting(TweenData tweenData, float duration)
        {
            this.tweenData = tweenData;
            this.duration = duration;

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
        public void InitLoop()
        {
            value = tweenData.startValue;
            Time = 0f;
        }
        public override void Rewind(bool isPlay = true)
        {
            base.Rewind(false);
            value = tweenData.endValue;
            Time = Duration;
            if (isPlay)
                Play();
        }
        public override void Complete()
        {
            base.Complete();
            value = tweenData.endValue;
            Time = Duration;
            curLoopCount = LoopCount;
            if (IsAutoKill)
                Kill(false);
        }
        public void CompleteLoop()
        {
            onCompleteLoop();
            if (!IsInfiniteLoop/*  && (Application.isEditor ? !IsInfiniteLoopInEditMode : true) */)
                if (curLoopCount >= LoopCount)
                    Complete();
            if (curLoopCount < LoopCount)
            {
                InitLoop();
                if (LoopType == LoopType.Yoyo)
                    Rewind();
                curLoopCount++;
            }
        }
        public void Kill(bool isComplete = true)
        {
            if (isComplete)
                Complete();
            IsPlaying = false;
            TweenCore.RemoveTweenSetting(TweenID);
        }
        public void ManualUpdate()
        {
            if (Time <= Duration)
                Value = Evaluate(Time);
            else
                CompleteLoop();
            onUpdate();
        }
        public TweenSetting SetName(string name)
        {
            TweenID.Name = name;
            return this;
        }
    }
}
