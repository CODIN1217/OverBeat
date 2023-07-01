using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OVERIZE
{
    public interface ITween
    {
        TweenID TweenID { get; set; }
        float DeltaTime { get; set; }
        bool IsPlaying { get; set; }
        bool IsComplete { get; set; }
        bool IsInfiniteLoopInEditMode { get; set; }
        bool IsUnscaledTime { get; set; }
        bool IsInfiniteLoop { get; set; }
        bool IsAutoKill { get; set; }
        int LoopCount { get; set; }
        Direction.Horizontal Toward { get; set; }
        float Speed { get; set; }
        UpdateMode UpdateMode { get; set; }
        ExecuteMode ExecuteMode { get; set; }
        void Init();
        void InitLoop();
        void Play();
        void ManualUpdate();
        void Complete();
        void Kill(bool isComplete = true);
        TweenCallback OnStart(CallBack callBack);
        TweenCallback OnPlay(CallBack callBack);
        TweenCallback OnUpdate(CallBack callBack);
        TweenCallback OnComplete(CallBack callBack);
        TweenCallback OnCompleteLoop(CallBack callBack);
    }
}