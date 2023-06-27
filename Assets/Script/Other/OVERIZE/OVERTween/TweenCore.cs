using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace OVERIZE
{
    internal static class TweenCore
    {
        static int tweenCount = 0;
        static Dictionary<int, TweenChain> tweenChains = new Dictionary<int, TweenChain>();
        internal static TweenChain AddTweenSetting(TweenChain tweenChain)
        {
            tweenChains.Add(tweenCount++, tweenChain);
            return tweenChain;
        }
        internal static TweenChain RemoveTweenSetting(int tweenID)
        {
            tweenChains.Remove(tweenID);
            return null;
        }
        internal static int TweenCount { get =>  tweenCount;}
        internal static List<TweenChain> TweenChains { get =>  tweenChains.Values.ToList();}
        internal static List<int> TweenIDs { get =>  tweenChains.Keys.ToList();}
    }
}
