using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using OVERIZE;

namespace OVERIZE
{
    public abstract class Tween : TweenPreference
    {
        int loopCount;
        public int LoopCount { get => loopCount; set => loopCount = OVERMath.ClampMin(value, 0); }
        float time;
        public float Time { get => time; internal set => time = value; }
        float deltaTime;
        public float DeltaTime { get => deltaTime; set { deltaTime = value * Speed; Time += (float)Toward * deltaTime; } }
        bool isPlaying;
        public bool IsPlaying { get => isPlaying; set => isPlaying = value; }
        bool isComplete;
        public bool IsComplete { get => isComplete; set => isComplete = value; }
        public bool IsPause { get => !IsPlaying && !IsComplete; set { if (!IsComplete) IsPlaying = !value; } }
        bool isRewind;
        public bool IsRewind { get => isRewind; set => isRewind = value; }
        bool isInfiniteLoop;
        public virtual bool IsInfiniteLoop { get => isInfiniteLoop; set => isInfiniteLoop = value; }
        Direction.Horizontal toward;
        public Direction.Horizontal Toward { get => toward; set => toward = value; }
        TweenID tweenID;
        public TweenID TweenID { get => tweenID; set { if (!isSetted) { tweenID = value; } isSetted = true; } }
        protected int curLoopCount;
        bool isSetted;
        public override void Init()
        {
            base.Init();
            IsInfiniteLoop = false;
            LoopCount = 1;
            curLoopCount = 0;
            Toward = Direction.Horizontal.Right;
            IsPlaying = false;
            isRewind = false;
            IsComplete = false;
        }
        public virtual void Play()
        {
            if (curLoopCount == 0)
            {
                curLoopCount++;
                onStart();
            }
            if (!IsPlaying)
            {
                IsPlaying = true;

                onPlay();
            }
        }
        public virtual void Pause()
        {
            if (!IsPause)
            {
                IsPause = true;

                onPause();
            }
        }
        public virtual void Rewind(bool isPlay = true)
        {
            if (!IsRewind)
            {
                IsRewind = true;
                Toward = Direction.Horizontal.Left;

                onRewind();
            }
            if (isPlay)
                Play();
        }
        public virtual void Complete()
        {
            if (!IsComplete)
            {
                IsPlaying = false;
                IsComplete = true;

                onComplete();
            }
        }
    }
}