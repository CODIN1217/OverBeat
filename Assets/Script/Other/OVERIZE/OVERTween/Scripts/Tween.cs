using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using OVERIZE;

namespace OVERIZE
{
    public abstract class Tween : TweenPreference, ITween
    {
        int loopCount;
        public int LoopCount { get => loopCount; set => loopCount = OVERMath.ClampMin(value, 0); }
        // int chainedIndex = -1;
        // public int ChainedIndex => chainedIndex;
        float time;
        public virtual float Time { get => time; set => time = value; }
        float deltaTime;
        public float DeltaTime { get => deltaTime; set => deltaTime = value * Speed; }
        float startTime;
        public float StartTime { get => startTime; set => startTime = value; }
        float endTime;
        public float EndTime { get => endTime; set => endTime = value; }
        public virtual float Duration => EndTime - StartTime;
        public bool IsStarted => LoopedCount > 0;
        bool isPlaying;
        public bool IsPlaying { get => isPlaying; set => isPlaying = value; }
        bool isComplete;
        public bool IsComplete { get => isComplete; set => isComplete = value; }
        public bool IsPause { get => !IsPlaying && !IsComplete; set { if (!IsComplete) IsPlaying = !value; } }
        public bool IsTweenAble => Time >= 0f && Time <= Duration;
        bool isRewind;
        public bool IsRewind { get => isRewind; set => isRewind = value; }
        bool isInfiniteLoop;
        public virtual bool IsInfiniteLoop { get => isInfiniteLoop; set => isInfiniteLoop = value; }
        public bool IsChained => TweenChain != null;
        TweenChain tweenChain;
        public TweenChain TweenChain { get => tweenChain; set => tweenChain = value; }
        Direction.Horizontal toward;
        public Direction.Horizontal Toward { get => toward; set => toward = value; }
        TweenID tweenID;
        public TweenID TweenID { get => tweenID; set { if (!isSetted) { tweenID = value; } isSetted = true; } }
        int loopedCount;
        public int LoopedCount { get => loopedCount; set => loopedCount = value; }
        bool isSetted;
        public override void Init()
        {
            base.Init();
            IsInfiniteLoop = false;
            LoopCount = 1;
            LoopedCount = 0;
            Toward = Direction.Horizontal.Right;
            IsPlaying = false;
            isRewind = false;
            IsComplete = false;
        }
        public virtual ITween InitLoop()
        {
            Time = 0f;
            return this;
        }
        public virtual ITween Play()
        {
            if (IsTweenAble)
            {
                if (LoopedCount == 0)
                {
                    LoopedCount++;
                    OnStart();
                }
                if (!IsPlaying)
                {
                    IsPlaying = true;
                    OnPlay();
                }
            }
            return this;
        }
        public virtual ITween Pause()
        {
            if (!IsPause)
            {
                IsPause = true;
                OnPause();
            }
            return this;
        }
        public virtual ITween Rewind()
        {
            if (!IsRewind)
            {
                IsRewind = true;
                Toward = Direction.Horizontal.Left;
                Utility.Swap(StartTime, EndTime, (T) => StartTime = T, (T) => EndTime = T);
                OnRewind();
            }
            return this;
        }
        public virtual ITween Complete()
        {
            if (!IsComplete)
            {
                IsPlaying = false;
                IsComplete = true;
                OnComplete();
            }
            return this;
        }
        public abstract ITween CompleteLoop();
        public virtual ITween Kill()
        {
            IsPlaying = false;
            TweenCore.RemoveTweenSetting(TweenID);
            return this;
        }
        public virtual ITween ManualUpdate()
        {
            OnUpdate();
            if (!IsChained)
                Time += (float)Toward * DeltaTime;
            return this;
        }
        public virtual void Defer(float time)
        {
            StartTime += time;
            EndTime += time;
        }
    }
}