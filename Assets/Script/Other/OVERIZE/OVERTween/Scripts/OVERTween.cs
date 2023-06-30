using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OVERIZE
{
    public static class OVERTween
    {
        public static TweenChain Chain() => new TweenChain();
        public static TweenSetting Create(TweenData tweenData, float duration, Setter setter = null)
        {
            TweenSetting tweenSetting = new TweenSetting(tweenData, duration);
            if (setter != null)
                TweenUpdater.Member.UpdateValue(setter, tweenSetting);
            return TweenCore.AddTweenSetting(tweenSetting);
        }
        public static TweenSetting Create(TweenAble startValue, TweenAble endValue, TweenEase ease, float duration, Setter setter = null)
        => Create(new TweenData(startValue, endValue, ease), duration, setter);

        public static int TweenCount() => TweenCore.TweenCount;
        public static List<TweenID> TweenIDs() => TweenCore.TweenIDs;
        public static List<TweenSetting> TweenSettings() => TweenCore.TweenSettings;
        public static void ManualUpdate(UpdateMode updateMode, float deltaTime = 0f) => TweenUpdater.Member.ManualUpdate(updateMode, deltaTime);
        public static void ManualUpdate(float deltaTime = 0f) => TweenUpdater.Member.ManualUpdate(deltaTime);
        public static void TweenPreference(TweenPreference tweenPreference) => TweenCore.TweenPreference = tweenPreference;
        public static TweenPreference TweenPreference() => TweenCore.TweenPreference;
    }
}
