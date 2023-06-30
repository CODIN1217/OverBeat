using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using OVERIZE;

namespace OVERIZE
{
    public class Tween : TweenPreference
    {
        bool isPlaying;
        public bool IsPlaying { get => isPlaying; internal set => isPlaying = value; }
        bool isComplete;
        public bool IsComplete { get => isComplete; internal set => isComplete = value; }
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