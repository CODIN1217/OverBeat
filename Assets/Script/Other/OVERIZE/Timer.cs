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
        public Timer(Updater updater, bool isUnscaledTime, Direction.Horizontal toward) => new Timer(updater, () => isUnscaledTime, () => toward);
        public Timer(Updater updater, Getter<bool> isUnscaledTime, Getter<Direction.Horizontal> toward)
        {
            this.updater = updater;
            onUpdate = () => time += (float)toward() * (isUnscaledTime() ? UnityEngine.Time.unscaledDeltaTime : UnityEngine.Time.deltaTime);
        }
        public virtual Timer Play()
        {
            updater.OnUpdate += onUpdate;
            return this;
        }
        public virtual Timer Stop()
        {
            updater.OnUpdate -= onUpdate;
            return this;
        }
        public virtual Timer Reset(bool isStop = true)
        {
            time = 0f;
            if (isStop)
                Stop();
            return this;
        }
    }
}
