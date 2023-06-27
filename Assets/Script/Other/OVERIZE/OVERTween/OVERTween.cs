using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OVERIZE
{
    public static class OVERTween
    {
        public static TweenChain Add(TweenData tweenData, float duration) => TweenCore.AddTweenSetting(new TweenSetting(tweenData, duration));
        public static TweenChain Add(TweenAble startValue, TweenAble endValue, TweenEase ease, float duration) => TweenCore.AddTweenSetting(new TweenSetting(new TweenData(startValue, endValue, ease), duration));
    }
}
