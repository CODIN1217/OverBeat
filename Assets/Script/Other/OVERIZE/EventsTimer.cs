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
        public EventsTimer(Updater updater, Action<int, float> onStartEvent = null, Action<int, float> onEndEvent = null, bool isUnscaledTime = false, Direction.Horizontal toward = Direction.Horizontal.Right, params TimeSetting[] eventTimes)
        : base(updater, isUnscaledTime, toward)
        => new EventsTimer(updater, onStartEvent, onEndEvent, () => isUnscaledTime, () => toward, () => eventTimes);
        public EventsTimer(Updater updater, Action<int, float> onStartEvent = null, Action<int, float> onEndEvent = null, Getter<bool> isUnscaledTime = null, Getter<Direction.Horizontal> toward = null, Getter<TimeSetting[]> eventTimes = null)
        : base(updater, isUnscaledTime, toward)
        {
            onStartEvent ??= (index, totalTime) => { };
            onEndEvent ??= (index, totalTime) => { };
            isUnscaledTime ??= () => false;
            toward ??= () => Direction.Horizontal.Right;
            if (eventTimes == null || eventTimes() == null)
                eventTimes = () => new TimeSetting[0];
            eventTimers = new();
            OnUpdate(() =>
            {
                for (int i = 0; i < eventTimes().Length - eventTimers.Count; i++)
                    eventTimers.Add(new EventTimer(updater, eventTimes()[i + eventTimers.Count], (time) => onStartEvent(i + eventTimers.Count, time), (time) => onEndEvent(i + eventTimers.Count, time), isUnscaledTime, toward));
            });
        }
        public override Timer Play()
        {
            base.Play();
            foreach (var ET in eventTimers)
                ET.Play();
            return this;
        }
        public override Timer Stop()
        {
            base.Stop();
            foreach (var ET in eventTimers)
                ET.Stop();
            return this;
        }
        public override Timer Reset(bool isStop = true)
        {
            base.Reset(false);
            foreach (var ET in eventTimers)
                ET.Reset(false);
            if (isStop)
                Stop();
            return this;
        }
    }
    [ExecuteInEditMode]
    public class EventTimer : Timer
    {
        float tarTime;
        public float TarTime => tarTime;
        public EventTimer(Updater updater, TimeSetting eventTime, Action<float> onStartEvent = null, Action<float> onEndEvent = null, bool isUnscaledTime = false, Direction.Horizontal toward = Direction.Horizontal.Right)
        : base(updater, isUnscaledTime, toward)
        => new EventTimer(updater, eventTime, onStartEvent, onEndEvent, () => isUnscaledTime, () => toward);
        public EventTimer(Updater updater, TimeSetting eventTime, Action<float> onStartEvent = null, Action<float> onEndEvent = null, Getter<bool> isUnscaledTime = null, Getter<Direction.Horizontal> toward = null)
        : base(updater, isUnscaledTime, toward)
        {
            onStartEvent ??= (totalTime) => { };
            onEndEvent ??= (totalTime) => { };
            isUnscaledTime ??= () => false;
            toward ??= () => Direction.Horizontal.Right;
            OnUpdate(() =>
            {
                tarTime = eventTime.StartTime + OVERMath.ClampMin(-(float)toward(), 0f) * eventTime.Duration;
                if ((float)toward() * Time >= (float)toward() * tarTime)
                    onStartEvent(Time);
                if ((float)toward() * Time >= (float)toward() * tarTime + eventTime.Duration)
                    onEndEvent(Time);
            });
        }
    }
}