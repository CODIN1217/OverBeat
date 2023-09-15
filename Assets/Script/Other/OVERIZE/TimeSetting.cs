using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OVERIZE
{
    public class TimeSetting
    {
        float startTime;
        public float StartTime { get => startTime; set => startTime = value; }
        public float EndTime => StartTime + Duration;
        float duration;
        public virtual float Duration { get => duration; set => duration = value; }
        public TimeSetting(float startTime, float duration)
        {
            this.startTime = startTime;
            this.duration = duration;
        }
    }
}