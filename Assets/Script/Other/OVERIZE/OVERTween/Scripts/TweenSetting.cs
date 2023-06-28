using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using OVERIZE;

namespace OVERIZE
{
    public class TweenSetting : Tween
    {

        float time;
        public float Time { get => time; internal set => time = value; }
        float speed;
        public float Speed { get => speed; set => speed = value; }

        int curLoopCount;
        internal float startTime;

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
            Update();
        }

        public TweenAble Evaluate(float time)
        {
            return tweenData.startValue + (tweenData.endValue - tweenData.startValue) * tweenData.ease[time];
        }
        public override void Init()
        {
            base.Init();
            startTime = TweenUpdater.time;
            value = tweenData.startValue;
            Time = 0f;
            curLoopCount = 0;
            speed = 1f;
        }
        public override void Play()
        {
            base.Play();
            if (curLoopCount == 0)
            {
                startTime = TweenUpdater.time;
                curLoopCount++;
            }
        }
        public override void Complete()
        {
            base.Complete();
            value = tweenData.endValue;
        }
        public void Update()
        {
            if (Time < duration)
            {
                value = Evaluate(Toward == Direction.Horizontal.Left ? 1f - Time / duration : Time / duration);
            }
            else
            {
                tweenCallback.OnCompleteLoop();
                if (curLoopCount >= LoopCount)
                {
                    if (LoopCount != -1)
                    {
                        Complete();
                    }
                }
                if (curLoopCount < LoopCount)
                    curLoopCount++;
            }

            tweenCallback.OnUpdate();
        }
    }
}
