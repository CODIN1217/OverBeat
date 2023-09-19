using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OVERIZE
{
    public interface ITimeSetting
    {
        float StartTime { get; set; }
        float EndTime { get; set; }
        float Duration { get; }
        void Defer(float time);
    }
    public class TimeSetting : ITimeSetting
    {
        float startTime;
        public float StartTime { get => startTime; set => startTime = value; }
        float endTime;
        public float EndTime { get => endTime; set => endTime = value; }
        public virtual float Duration => EndTime - StartTime;
        public virtual void Defer(float time)
        {
            StartTime += time;
            EndTime += time;
        }
        public TimeSetting(float startTime, float endTime)
        {
            this.startTime = startTime;
            this.endTime = endTime;
        }
    }
}