using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OVERIZE
{
    public class TweenSetting : Tween, ITween
    {
        bool isChained;
        internal bool IsChained { get => isChained; set => isChained = value; }

        public override bool IsInfiniteLoopInEditMode { get => base.IsInfiniteLoopInEditMode && !IsChained; set => base.IsInfiniteLoopInEditMode = value && !IsChained; }
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

        public TweenAble Evaluate(float time)
        {
            return tweenData.startValue + (tweenData.endValue - tweenData.startValue) * tweenData.ease[Toward == Direction.Horizontal.Left ? 1f - time / Duration : time / Duration];
        }
        public override void Init()
        {
            base.Init();
            InitLoop();
            curLoopCount = 0;
        }
        public void InitLoop()
        {
            value = tweenData.startValue;
            Time = 0f;
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
        public void Kill(bool isComplete = true)
        {
            if (isComplete)
                Complete();
            IsPlaying = false;
            TweenCore.RemoveTweenSetting(TweenID);
        }
        public void ManualUpdate()
        {
            if (Time < Duration)
            {
                Value = Evaluate(Time);
            }
            else
                TweenUpdater.Member.StartCoroutine(CompleteCo());
            onUpdate();
        }
        public TweenSetting SetName(string name)
        {
            TweenID.Name = name;
            return this;
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
    }
}
