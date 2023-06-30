using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OVERIZE
{
    public class TweenSetting : Tween
    {
        TweenID tweenID;
        public TweenID TweenID { get => tweenID; internal set => tweenID = value; }
        float time;
        public float Time { get => time; internal set => time = value; }

        int curLoopCount;

        TweenData tweenData;
        public TweenData TweenData { get => tweenData; }
        float duration;
        public float Duration { get => duration; }
        TweenAble value;
        public TweenAble Value { get => value; }

        public TweenSetting(TweenData tweenData, float duration)
        {
            this.tweenData = tweenData;
            this.duration = duration;

            Init();
            ManualUpdate();
        }

        public TweenAble Evaluate(float time)
        {
            return tweenData.startValue + (tweenData.endValue - tweenData.startValue) * tweenData.ease[Toward == Direction.Horizontal.Left ? 1f - time / Duration : time / Duration];
        }
        public override void Init()
        {
            base.Init();
            value = tweenData.startValue;
            Time = 0f;
            curLoopCount = 0;
        }
        public override void Play()
        {
            base.Play();
            if (curLoopCount == 0)
            {
                curLoopCount++;
                onStart();
            }
        }
        public override void Complete()
        {
            base.Complete();
            value = tweenData.endValue;
            Time = Duration;
        }
        public void Kill(bool isComplete = true)
        {
            if (isComplete)
                Complete();
            TweenCore.RemoveTweenSetting(tweenID);
        }
        public void ManualUpdate()
        {
            if (Time < duration)
            {
                value = Evaluate(Time);
            }
            else
            {
                onCompleteLoop();
                if (!IsInfiniteLoop && (Application.isEditor ? !IsInfiniteLoopInEditMode : true))
                {
                    if (curLoopCount >= LoopCount)
                    {
                        Complete();
                    }
                }
                if (curLoopCount < LoopCount)
                    curLoopCount++;
            }

            onUpdate();
        }
        public TweenSetting SetName(string name)
        {
            TweenID.Name = name;
            return this;
        }
        public void SetTimeAsUnscaled() => IsUnscaledTime = true;
        public void SetTimeAsScaled() => IsUnscaledTime = false;
    }
}
