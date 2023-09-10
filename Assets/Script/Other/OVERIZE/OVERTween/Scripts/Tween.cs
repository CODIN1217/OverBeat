using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using OVERIZE;

namespace OVERIZE
{
    public abstract class Tween : TweenPreference
    {
        TweenID tweenID;
        bool isSetted;
        public TweenID TweenID { get => tweenID; set { if (!isSetted) { tweenID = value; } isSetted = true; } }
        float time;
        public float Time { get => time; internal set => time = value; }
        float deltaTime;
        public float DeltaTime { get => deltaTime; set { deltaTime = value; Time += value; } }
        bool isPlaying;
        public bool IsPlaying { get => isPlaying; set => isPlaying = value; }
        bool isComplete;
        public bool IsComplete { get => isComplete; set => isComplete = value; }
        protected int curLoopCount;
        public override void Init()
        {
            base.Init();
            IsPlaying = false;
            IsComplete = false;
        }
        public virtual void Play()
        {
            if (!IsPlaying)
            {
                IsPlaying = true;

                onPlay();
            }
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