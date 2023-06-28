using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OVERIZE
{
    public static class OVERTween
    {
        public static TweenChain Add(TweenData tweenData, float duration, Setter setter = null)
        {
            TweenSetting tweenSetting = new TweenSetting(tweenData, duration);
            if (setter != null)
                TweenUpdater.Member.UpdateValue(setter, tweenSetting);
            return TweenCore.AddTweenSetting(tweenSetting);
        }
        public static TweenChain Add(TweenAble startValue, TweenAble endValue, TweenEase ease, float duration, Setter setter = null)
        => Add(new TweenData(startValue, endValue, ease), duration, setter);

        public static int TweenCount() => TweenCore.TweenCount;
        public static List<int> TweenIDs() => TweenCore.TweenIDs;
        public static List<TweenChain> TweenChains() => TweenCore.TweenChains;
        public static List<TweenSetting> TweenSettings()
        {
            List<TweenSetting> tweenSettings = new List<TweenSetting>();
            foreach (var TC in TweenCore.TweenChains)
                tweenSettings.AddRange(TC.TweenSettings);
            return tweenSettings;
        }
        public static void SetUpdateMode(UpdateMode updateMode) => TweenUpdater.Member.updateMode = updateMode;
        public static void ExecuteInEditMode() => TweenUpdater.Member.isExecuteInEditMode = true;
        public static void ExecuteInRunTimeOnly() => TweenUpdater.Member.isExecuteInEditMode = false;
        public static void SetTimeAsUnscaled() => TweenUpdater.Member.isUnscaledTime = true;
        public static void SetTimeAsScaled() => TweenUpdater.Member.isUnscaledTime = false;
    }
}
