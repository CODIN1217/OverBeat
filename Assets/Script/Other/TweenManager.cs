using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System;

namespace TweenManager
{
    public interface ITweener
    {
        TweeningInfo[] GetTweens();
        void InitTween();
        void UpdateTweenValue();
        void TryKillTween();
        void GotoTween(float toSecs);
    }
    [Serializable]
    public class TweenInfo<T> : IDeepCopy<TweenInfo<T>>
    {
        [SerializeField] public T startValue;
        [SerializeField] public T endValue;
        [SerializeField] public AnimationCurve ease;
        public TweenInfo(T startValue, T endValue, AnimationCurve ease)
        {
            this.startValue = startValue;
            this.endValue = endValue;
            this.ease = new AnimationCurve(ease.keys);
        }
        public TweenInfo<T> Clone()
        {
            return new TweenInfo<T>(startValue, endValue, ease);
        }
    }
    public class TweenerInfo<T> : TweenInfo<T>
    {
        public T curValue;
        float duration;
        public TweenerInfo(TweenInfo<T> tweenInfo, float duration) : base(tweenInfo.startValue, tweenInfo.endValue, tweenInfo.ease)
        {
            this.duration = duration;
            curValue = tweenInfo.startValue;
        }
    }
    public class TweeningInfo
    {
        public Sequence tweener;

        TweenerInfos<float> tweenerInfosFloat;
        TweenerInfos<Vector2> tweenerInfosVector2;
        TweenerInfos<Vector3> tweenerInfosVector3;
        TweenerInfos<Color> tweenerInfosColor;
        public TweeningInfo(TweenInfo<float> tweenInfo, float duration) => tweenerInfosFloat = new TweenerInfos<float>().AddTweenerInfo(TweenMethod.SetTweenerInfo(tweenInfo, duration, out tweener));
        public TweeningInfo(TweenInfo<Vector2> tweenInfo, float duration) => tweenerInfosVector2 = new TweenerInfos<Vector2>().AddTweenerInfo(TweenMethod.SetTweenerInfo(tweenInfo, duration, out tweener));
        public TweeningInfo(TweenInfo<Vector3> tweenInfo, float duration) => tweenerInfosVector3 = new TweenerInfos<Vector3>().AddTweenerInfo(TweenMethod.SetTweenerInfo(tweenInfo, duration, out tweener));
        public TweeningInfo(TweenInfo<Color> tweenInfo, float duration) => tweenerInfosColor = new TweenerInfos<Color>().AddTweenerInfo(TweenMethod.SetTweenerInfo(tweenInfo, duration, out tweener));
        public static explicit operator TweenerInfo<float>(TweeningInfo tweeningInfo) { if (tweeningInfo.tweenerInfosFloat != null) return tweeningInfo.tweenerInfosFloat.tweenerInfo; return null; }
        public static explicit operator TweenerInfo<Vector2>(TweeningInfo tweeningInfo) { if (tweeningInfo.tweenerInfosVector2 != null) return tweeningInfo.tweenerInfosVector2.tweenerInfo; return null; }
        public static explicit operator TweenerInfo<Vector3>(TweeningInfo tweeningInfo) { if (tweeningInfo.tweenerInfosVector3 != null) return tweeningInfo.tweenerInfosVector3.tweenerInfo; return null; }
        public static explicit operator TweenerInfo<Color>(TweeningInfo tweeningInfo) { if (tweeningInfo.tweenerInfosColor != null) return tweeningInfo.tweenerInfosColor.tweenerInfo; return null; }
        public TweeningInfo Append(TweeningInfo tweeningInfo)
        {
            if (tweener != null)
                tweener.Append(tweeningInfo.tweener);
            return this;
        }
        public TweeningInfo AppendInterval(float interval)
        {
            if (tweener != null)
                tweener.AppendInterval(interval);
            return this;
        }
        public TweeningInfo OnStart(Action action)
        {
            if (tweener != null)
                tweener.OnStart(() => action());
            return this;
        }
        public TweeningInfo OnPlay(Action action)
        {
            if (tweener != null)
                tweener.OnPlay(() => action());
            return this;
        }
        public TweeningInfo OnUpdate(Action action)
        {
            if (tweener != null)
                tweener.OnUpdate(() => action());
            return this;
        }
        public TweeningInfo OnComplete(Action action)
        {
            if (tweener != null)
                tweener.OnComplete(() => action());
            return this;
        }
        public bool IsActive()
        {
            if (tweener != null)
                return tweener.IsActive();
            return false;
        }
        public bool IsBackwards()
        {
            if (tweener != null)
                return tweener.IsBackwards();
            return false;
        }
        public bool IsComplete()
        {
            if (tweener != null)
                return tweener.IsComplete();
            return false;
        }
        public bool IsInitialized()
        {
            if (tweener != null)
                return tweener.IsInitialized();
            return false;
        }
        public bool IsPlaying()
        {
            if (tweener != null)
                return tweener.IsPlaying();
            return false;
        }
        public TweeningInfo Goto(float toSecs)
        {
            if (tweener != null)
            {
                bool isPlaying = tweener.IsPlaying();
                tweener.Goto(toSecs, isPlaying);
            }
            return this;
        }
    }
    class TweenerInfos<T>
    {
        public static int tweenCount;
        public static Dictionary<int, TweenerInfo<T>> tweenerInfos = new Dictionary<int, TweenerInfo<T>>();

        public int tweenIndex;
        public TweenerInfo<T> tweenerInfo { get { return tweenerInfos[tweenIndex]; } set { tweenerInfos[tweenIndex] = value; } }
        public TweenerInfos<T> AddTweenerInfo(TweenerInfo<T> tweenerInfo)
        {
            tweenIndex = tweenCount;
            tweenCount++;
            tweenerInfos.Add(tweenIndex, tweenerInfo);
            return this;
        }
    }
    public static class TweenMethod
    {
        public static float GetTweenGap(TweenInfo<float> tweenInfo, float progress) => (tweenInfo.endValue - tweenInfo.startValue) * tweenInfo.ease.Evaluate(progress);
        public static Vector2 GetTweenGap(TweenInfo<Vector2> tweenInfo, float progress) => (tweenInfo.endValue - tweenInfo.startValue) * tweenInfo.ease.Evaluate(progress);
        public static Vector3 GetTweenGap(TweenInfo<Vector3> tweenInfo, float progress) => (tweenInfo.endValue - tweenInfo.startValue) * tweenInfo.ease.Evaluate(progress);
        public static Color GetTweenGap(TweenInfo<Color> tweenInfo, float progress) => (tweenInfo.endValue - tweenInfo.startValue) * tweenInfo.ease.Evaluate(progress);

        public static float GetTweenValue(TweenInfo<float> tweenInfo, float progress) => tweenInfo.startValue + GetTweenGap(tweenInfo, progress);
        public static Vector2 GetTweenValue(TweenInfo<Vector2> tweenInfo, float progress) => tweenInfo.startValue + GetTweenGap(tweenInfo, progress);
        public static Vector3 GetTweenValue(TweenInfo<Vector3> tweenInfo, float progress) => tweenInfo.startValue + GetTweenGap(tweenInfo, progress);
        public static Color GetTweenValue(TweenInfo<Color> tweenInfo, float progress) => tweenInfo.startValue + GetTweenGap(tweenInfo, progress);

        public static TweenerInfo<float> SetTweenerInfo(TweenInfo<float> tweenInfo, float duration, out Sequence tweener)
        {
            TweenerInfo<float> tweenerInfoTemp = new TweenerInfo<float>(tweenInfo, duration);
            tweener = DOTween.Sequence()
            .Append(DOTween.To(() => tweenInfo.startValue, (v) => tweenerInfoTemp.curValue = v, tweenInfo.endValue, duration)
            .SetEase(tweenInfo.ease));
            return tweenerInfoTemp;
        }
        public static TweenerInfo<Vector2> SetTweenerInfo(TweenInfo<Vector2> tweenInfo, float duration, out Sequence tweener)
        {
            TweenerInfo<Vector2> tweenerInfoTemp = new TweenerInfo<Vector2>(tweenInfo, duration);
            tweener = DOTween.Sequence()
            .Append(DOTween.To(() => tweenInfo.startValue, (v) => tweenerInfoTemp.curValue = v, tweenInfo.endValue, duration)
            .SetEase(tweenInfo.ease));
            return tweenerInfoTemp;
        }
        public static TweenerInfo<Vector3> SetTweenerInfo(TweenInfo<Vector3> tweenInfo, float duration, out Sequence tweener)
        {
            TweenerInfo<Vector3> tweenerInfoTemp = new TweenerInfo<Vector3>(tweenInfo, duration);
            tweener = DOTween.Sequence()
            .Append(DOTween.To(() => tweenInfo.startValue, (v) => tweenerInfoTemp.curValue = v, tweenInfo.endValue, duration)
            .SetEase(tweenInfo.ease));
            return tweenerInfoTemp;
        }
        public static TweenerInfo<Color> SetTweenerInfo(TweenInfo<Color> tweenInfo, float duration, out Sequence tweener)
        {
            TweenerInfo<Color> tweenerInfoTemp = new TweenerInfo<Color>(tweenInfo, duration);
            tweener = DOTween.Sequence()
            .Append(DOTween.To(() => tweenInfo.startValue, (v) => tweenerInfoTemp.curValue = v, tweenInfo.endValue, duration)
            .SetEase(tweenInfo.ease));
            return tweenerInfoTemp;
        }
        public static bool IsInfoNull(params TweeningInfo[] tweeningInfos)
        {
            foreach (var TI in tweeningInfos)
            {
                if (TI == null)
                {
                    return true;
                }
                else
                {
                    if (TI.tweener == null)
                        return true;
                }
            }
            return false;
        }
        public static void TryPlayTween(params TweeningInfo[] tweeningInfos)
        {
            foreach (var TI in tweeningInfos)
                if (TI != null)
                    if (TI.tweener != null)
                        TI.tweener.Play();
        }
        public static void TryPauseTween(params TweeningInfo[] tweeningInfos)
        {
            foreach (var TI in tweeningInfos)
                if (TI != null)
                    if (TI.tweener != null)
                        TI.tweener.Pause();
        }
        public static void TryKillTween(params TweeningInfo[] tweeningInfos)
        {
            foreach (var TI in tweeningInfos)
                if (TI != null)
                    if (TI.tweener != null)
                    {
                        TI.tweener.Kill(true);
                        TI.tweener = null;
                    }
        }
        public static void TrySetForward(params TweeningInfo[] tweeningInfos)
        {
            foreach (var TI in tweeningInfos)
            {
                if (TI.tweener != null)
                {
                    bool isPlaying = TI.tweener.IsPlaying();
                    TI.tweener.PlayForward();
                    if (!isPlaying)
                        TI.tweener.Pause();
                }
            }
        }
        public static void TrySetBackward(params TweeningInfo[] tweeningInfos)
        {
            foreach (var TI in tweeningInfos)
            {
                if (TI.tweener != null)
                {
                    bool isPlaying = TI.tweener.IsPlaying();
                    TI.tweener.PlayBackwards();
                    if (!isPlaying)
                        TI.tweener.Pause();
                }
            }
        }
        public static void TryComplete(params TweeningInfo[] tweeningInfos)
        {
            foreach (var TI in tweeningInfos)
            {
                if (TI.tweener != null)
                    TI.tweener.Complete();
            }
        }
    }
}