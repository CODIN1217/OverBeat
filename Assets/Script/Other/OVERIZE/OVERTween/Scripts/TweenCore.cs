using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace OVERIZE
{
    internal static class TweenCore
    {
        static int tweenCount = 0;
        internal static int TweenCount { get => tweenCount; }
        static Dictionary<TweenID, TweenSetting> tweenSettings = new Dictionary<TweenID, TweenSetting>();
        internal static List<TweenID> TweenIDs { get => tweenSettings.Keys.ToList(); }
        internal static List<TweenSetting> TweenSettings { get => tweenSettings.Values.ToList(); }
        static TweenPreference tweenPreference = new TweenPreference();
        internal static TweenPreference TweenPreference { get => tweenPreference; set => tweenPreference = value;}
        internal static TweenSetting AddTweenSetting(TweenSetting tweenSetting)
        {
            tweenSetting.TweenID = tweenCount++;
            tweenSettings.Add(tweenSetting.TweenID, tweenSetting);
            return tweenSetting;
        }
        internal static void RemoveTweenSetting(TweenID tweenID)
        {
            tweenSettings.Remove(tweenID);
        }
    }
}
