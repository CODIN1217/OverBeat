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
        static Dictionary<TweenID, ITween> tweens = new Dictionary<TweenID, ITween>();
        internal static List<TweenID> TweenIDs { get => tweens.Keys.ToList(); }
        internal static List<ITween> Tweens { get => tweens.Values.ToList(); }
        static TweenPreference tweenPreference = new TweenPreference();
        internal static TweenPreference TweenPreference { get => tweenPreference; set => tweenPreference = value;}
        internal static ITween AddTweenSetting(ITween tween)
        {
            tween.TweenID = tweenCount++;
            tweens.Add(tween.TweenID, tween);
            return tween;
        }
        internal static void RemoveTweenSetting(TweenID tweenID)
        {
            tweens.Remove(tweenID);
        }
    }
}
