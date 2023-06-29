using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OVERIZE
{
    public class TweenPreference
    {
        bool isLoopInEditMode;
        public bool IsLoopInEditMode { get => isLoopInEditMode; set => isLoopInEditMode = value; }
        bool isUnscaledTime;
        public bool IsUnscaledTime { get => isUnscaledTime; set => isUnscaledTime = value; }
        int loopCount;
        public int LoopCount { get => loopCount; set => loopCount = OVERMath.ClampMin(value, 0); }
        public void InfiniteLoop() => loopCount = -1;
        Direction.Horizontal toward;
        public Direction.Horizontal Toward { get => toward; set => toward = value; }
    }
}
