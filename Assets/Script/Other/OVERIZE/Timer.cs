using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

namespace OVERIZE
{
    [ExecuteInEditMode]
    public class Timer
    {
        // float[] delays;
        OrderedElement<float>[] eventTimesOrdered;
        CallBack onUpdate;
        Updater updater;
        Action<int> callBack;
        int playIndex;
        float totalTime;
        float tarTime;
        public Timer(Updater updater, Action<int> callBack, bool isUnscaledTime = false, params float[] eventTimes)
        {
            this.updater = updater;
            this.callBack = callBack;
            // this.delays = delays;
            eventTimesOrdered = eventTimes.Order();
            if (eventTimesOrdered.Length > 0)
                tarTime = eventTimesOrdered[0].Value;
            onUpdate = () =>
            {
                CheckEvent();
                totalTime += isUnscaledTime ? Time.unscaledDeltaTime : Time.deltaTime;
            };
        }
        void CheckEvent()
        {
            if (playIndex < eventTimesOrdered.Length)
            {
                if (totalTime >= tarTime)
                {
                    callBack(eventTimesOrdered[playIndex].OrigIndex);
                    playIndex++;
                    tarTime += eventTimesOrdered[playIndex].Value;
                    CheckEvent();
                }
            }
        }
        /* bool isElapsed = true;
        public Timer(Updater updater, Func<int, bool> callBack, bool isUnscaledTime = false, params float[] delays)
        {
            this.updater = updater;
            this.delays = delays;
            if (delays.Length > 0)
                tarTime = delays[0];
            onUpdate = () =>
            {
                if (playIndex < delays.Length)
                {
                    if (totalTime >= tarTime)
                    {
                        if (callBack(playIndex))
                        {
                            playIndex++;
                            tarTime += delays[playIndex];
                            isElapsed = true;
                        }
                        else
                            isElapsed = false;
                    }
                    if (isElapsed)
                        totalTime += isUnscaledTime ? Time.unscaledDeltaTime : Time.deltaTime;
                }
            };
        } */
        public void Reset(bool isStop = true)
        {
            playIndex = 0;
            totalTime = 0f;
            if (eventTimesOrdered.Length > 0)
                tarTime = eventTimesOrdered[0].Value;
            if (isStop)
                Stop();
        }
        public void Play() => updater.OnUpdate += onUpdate;
        public void Stop() => updater.OnUpdate -= onUpdate;
    }
}