using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OVERIZE
{
    public interface ITween : ITimeSetting, ITweenCallback, ITweenBehavior
    {
        int LoopCount { get; set; }
        int LoopedCount { get; set; }
        float Time { get; set; }
        float DeltaTime { get; set; }
        float Speed { get; set; }
        bool IsPlaying { get; set; }
        bool IsPause { get; set; }
        bool IsRewind { get; set; }
        bool IsComplete { get; set; }
        bool IsUnscaledTime { get; set; }
        bool IsInfiniteLoop { get; set; }
        bool IsAutoKill { get; set; }
        bool IsTweenAble { get; }
        TweenID TweenID { get; set; }
        TweenChain TweenChain { get; set; }
        AutoPlay AutoPlay { get; set; }
        LoopType LoopType { get; set; }
        Direction.Horizontal Toward { get; set; }
        UpdateMode UpdateMode { get; set; }
        ExecuteMode ExecuteMode { get; set; }
    }
}