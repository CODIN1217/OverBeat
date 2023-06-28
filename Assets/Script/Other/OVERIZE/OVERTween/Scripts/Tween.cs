using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using OVERIZE;

namespace OVERIZE
{
    public class Tween
    {
        int loopCount;
        public int LoopCount { get => loopCount; set => loopCount = OVERMath.ClampMin(value, 0); }
        public void InfiniteLoop() => loopCount = -1;
        Direction.Horizontal toward;
        public Direction.Horizontal Toward { get => toward; set => toward = value; }
        bool isPlaying;
        public bool IsPlaying { get => isPlaying; internal set => isPlaying = value; }
        bool isComplete;
        public bool IsComplete { get => isComplete; internal set => isComplete = value; }
        protected TweenCallback tweenCallback;
        public virtual void Init()
        {
            LoopCount = 1;
            Toward = Direction.Horizontal.Right;
            IsPlaying = false;
            IsComplete = false;
            tweenCallback = new TweenCallback();
        }
        public virtual void Play()
        {
            if (!IsPlaying)
            {
                IsPlaying = true;

                tweenCallback.OnPlay();
            }
        }
        public virtual void Complete()
        {
            if (!IsComplete)
            {
                IsComplete = true;

                tweenCallback.OnComplete();
            }
        }
        public Tween OnPlay(CallBack callBack)
        {
            tweenCallback.OnPlay += callBack;
            return this;
        }
        public Tween OnUpdate(CallBack callBack)
        {
            tweenCallback.OnUpdate += callBack;
            return this;
        }
        public Tween OnComplete(CallBack callBack)
        {
            tweenCallback.OnComplete += callBack;
            return this;
        }
        public Tween OnCompleteLoop(CallBack callBack)
        {
            tweenCallback.OnCompleteLoop += callBack;
            return this;
        }
    }
}