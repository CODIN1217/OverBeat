using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OVERIZE
{
    public static class OVERTween
    {
        public static TweenChain Chain() => TweenCore.AddTweenSetting(new TweenChain());
        public static TweenSetting Create(TweenData tweenData, float duration, Setter<TweenAble> setter = null)
        {
            TweenSetting tweenSetting = new TweenSetting(tweenData, duration);
            if (setter != null)
                Updater.Member<TweenUpdater>().OnUpdate += () => { if (!tweenSetting.IsComplete) setter(tweenSetting.Value); };
            return TweenCore.AddTweenSetting(tweenSetting);
        }
        public static TweenSetting Create(TweenAble startValue, TweenAble endValue, TweenEase ease, float duration, Setter<TweenAble> setter = null)
        => Create(new TweenData(startValue, endValue, ease), duration, setter);

        public static int TweenCount => TweenCore.TweenCount;
        public static List<TweenID> TweenIDs => TweenCore.TweenIDs;
        public static List<ITween> Tweens => TweenCore.Tweens;
        public static void ManualUpdate(UpdateMode updateMode, float deltaTime = 0f) => Updater.Member<TweenUpdater>().ManualUpdate(updateMode, deltaTime);
        public static void ManualUpdate(float deltaTime = 0f) => Updater.Member<TweenUpdater>().ManualUpdate(deltaTime);
        public static TweenPreference Preference { get => TweenCore.TweenPreference; set => TweenCore.TweenPreference = value; }
        public static float DeltaTime(UpdateMode updateMode, bool isUnscaledTime = false) => Updater.Member<TweenUpdater>().DeltaTime(updateMode, isUnscaledTime);
        public static void CompleteLoop(ITween tween)
        {
            tween.OnCompleteLoop();
            if (!tween.IsInfiniteLoop && tween.LoopedCount >= tween.LoopCount)
                tween.Complete();
            else
            {
                tween.InitLoop();
                if (tween.LoopType == LoopType.Yoyo)
                    tween.Rewind();
                tween.LoopedCount++;
            }
        }
    }
}
