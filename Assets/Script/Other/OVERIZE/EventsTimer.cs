using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

namespace OVERIZE
{
    [ExecuteInEditMode]
    public class EventsTimer : Timer
    {
        List<EventTimer> eventTimers;
        public EventsTimer(Updater updater, Action<int, float> onStartEvent, Action<int, float> onEndEvent, bool isUnscaledTime, Direction.Horizontal toward, params TimeSetting[] eventTimeSettings)
        : base(updater, isUnscaledTime, toward)
        => new EventsTimer(updater, onStartEvent, onEndEvent, () => isUnscaledTime, () => toward, () => eventTimeSettings);
        public EventsTimer(Updater updater, Action<int, float> onStartEvent, Action<int, float> onEndEvent, Func<bool> isUnscaledTime, Func<Direction.Horizontal> toward, Func<TimeSetting[]> eventTimeSettings)
        : base(updater, isUnscaledTime, toward)
        {
            eventTimers = new();
            OnUpdate(() =>
            {
                for (int i = 0; i < eventTimeSettings().Length - eventTimers.Count; i++)
                    eventTimers.Add(new EventTimer(updater, (time) => onStartEvent(i + eventTimers.Count, time), (time) => onEndEvent(i + eventTimers.Count, time), () => eventTimeSettings()[i + eventTimers.Count], isUnscaledTime, toward));
            });
        }
        public override void Play()
        {
            base.Play();
            // updater.OnUpdate += onUpdate;
            foreach (var ET in eventTimers)
                ET.Play();
        }
        public override void Stop()
        {
            base.Stop();
            // updater.OnUpdate -= onUpdate;
            foreach (var ET in eventTimers)
                ET.Stop();
        }
        public override void Reset(bool isStop = true)
        {
            base.Reset(false);
            foreach (var ET in eventTimers)
                ET.Reset(false);
            if (isStop)
                Stop();
        }
    }
    [ExecuteInEditMode]
    public class EventTimer : Timer
    {
        float tarTime;
        public float TarTime => tarTime;
        public EventTimer(Updater updater, Action<float> onStartEvent, Action<float> onEndEvent, TimeSetting eventTime, bool isUnscaledTime = false, Direction.Horizontal toward = Direction.Horizontal.Right)
        : base(updater, isUnscaledTime, toward)
        => new EventTimer(updater, onStartEvent, onEndEvent, () => eventTime, () => isUnscaledTime, () => toward);
        public EventTimer(Updater updater, Action<float> onStartEvent, Action<float> onEndEvent, Func<TimeSetting> eventTime, Func<bool> isUnscaledTime, Func<Direction.Horizontal> toward)
        : base(updater, isUnscaledTime, toward)
        {
            // this.updater = updater;
            OnUpdate(() =>
            {
                tarTime = eventTime().EndTime - OVERMath.ClampMin((float)toward(), 0f) * eventTime().Duration;
                if ((float)toward() * Time >= (float)toward() * tarTime)
                    onStartEvent(Time);
                if ((float)toward() * Time >= (float)toward() * tarTime + eventTime().Duration)
                    onEndEvent(Time);
            });
        }
        // public void Play() => updater.OnUpdate += onUpdate;
        // public void Stop() => updater.OnUpdate -= onUpdate;
        // public void Reset(bool isStop = true)
        // {
        //     time = 0f;
        //     if (isStop)
        //         Stop();
        // }
    }
}