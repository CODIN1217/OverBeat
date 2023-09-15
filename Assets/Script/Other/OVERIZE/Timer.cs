using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace OVERIZE
{
    public class Timer
    {
        float time;
        public float Time => time;
        Updater updater;
        protected Updater Updater => updater;
        CallBack onUpdate;
        public Timer OnUpdate(CallBack onUpdate)
        {
            this.onUpdate += onUpdate;
            return this;
        }
        public Timer(Updater updater, bool isUnscaledTime, Direction.Horizontal toward)
        {
            this.updater = updater;
            onUpdate = () => time += (float)toward * (isUnscaledTime ? UnityEngine.Time.unscaledDeltaTime : UnityEngine.Time.deltaTime);
        }
        public Timer(Updater updater, Func<bool> isUnscaledTime, Func<Direction.Horizontal> toward)
        {
            this.updater = updater;
            onUpdate = () => time += (float)toward() * (isUnscaledTime() ? UnityEngine.Time.unscaledDeltaTime : UnityEngine.Time.deltaTime);
        }
        public virtual void Play() => updater.OnUpdate += onUpdate;
        public virtual void Stop() => updater.OnUpdate -= onUpdate;
        public virtual void Reset(bool isStop = true)
        {
            time = 0f;
            if (isStop)
                Stop();
        }
    }
}
