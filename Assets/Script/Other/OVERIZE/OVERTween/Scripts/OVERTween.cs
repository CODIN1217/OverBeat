using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OVERIZE
{
    public static class OVERTween
    {
        public static TweenChain Chain() => new TweenChain();
        public static ITween Create(TweenData tweenData, float duration, Setter<TweenAble> setter = null)
        {
            TweenSetting tweenSetting = new TweenSetting(tweenData, duration);
            if (setter != null)
                Updater.Member<TweenUpdater>().OnUpdate += () => { if (!tweenSetting.IsComplete) setter(tweenSetting.Value); };
            // Updater.Member<TweenUpdater>().UpdateValue(setter, tweenSetting);
            return TweenCore.AddTweenSetting(tweenSetting);
        }
        public static ITween Create(TweenAble startValue, TweenAble endValue, TweenEase ease, float duration, Setter<TweenAble> setter = null)
        => Create(new TweenData(startValue, endValue, ease), duration, setter);

        public static int TweenCount => TweenCore.TweenCount;
        public static List<TweenID> TweenIDs => TweenCore.TweenIDs;
        public static List<ITween> Tweens => TweenCore.Tweens;
        public static void ManualUpdate(UpdateMode updateMode, float deltaTime = 0f) => Updater.Member<TweenUpdater>().ManualUpdate(updateMode, deltaTime);
        public static void ManualUpdate(float deltaTime = 0f) => Updater.Member<TweenUpdater>().ManualUpdate(deltaTime);
        // public static void TweenPreference(TweenPreference tweenPreference) => TweenCore.TweenPreference = tweenPreference;
        // public static TweenPreference TweenPreference() => TweenCore.TweenPreference;
        public static TweenPreference Preference { get => TweenCore.TweenPreference; set => TweenCore.TweenPreference = value; }
        public static float DeltaTime(UpdateMode updateMode, bool isUnscaledTime = false) => Updater.Member<TweenUpdater>().DeltaTime(updateMode, isUnscaledTime);
    }
}
