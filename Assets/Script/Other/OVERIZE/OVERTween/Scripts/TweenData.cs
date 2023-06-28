using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OVERIZE
{
    public struct TweenData
    {
        public TweenAble startValue;
        public TweenAble endValue;
        public TweenEase ease;

        public TweenData(TweenAble startValue, TweenAble endValue, TweenEase ease)
        {
            this.startValue = startValue;
            this.endValue = endValue;
            this.ease = ease;
        }
    }
}