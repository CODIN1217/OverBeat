using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System;

namespace TweenManager
{
    public interface ITweener
    {
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
        public TweeningInfo SetForward()
        {
            bool isPlaying = tweener.IsPlaying();
            tweener.PlayForward();
            if (!isPlaying)
                tweener.Pause();
            return this;
        }
        public TweeningInfo SetBackward()
        {
            bool isPlaying = tweener.IsPlaying();
            tweener.PlayBackwards();
            if (!isPlaying)
                tweener.Pause();
            return this;
        }
        public TweeningInfo Append(TweeningInfo tweeningInfo)
        {
            tweener.Append(tweeningInfo.tweener);
            return this;
        }
        public TweeningInfo AppendInterval(float interval)
        {
            tweener.AppendInterval(interval);
            return this;
        }
        public TweeningInfo Play()
        {
            if (tweener != null)
                tweener.Play();
            return this;
        }
        public TweeningInfo Pause()
        {
            if (tweener != null)
                tweener.Pause();
            return this;
        }
        public TweeningInfo Complete()
        {
            tweener.Complete();
            return this;
        }
        public TweeningInfo OnStart(Action action)
        {
            tweener.OnStart(() => action());
            return this;
        }
        public TweeningInfo OnPlay(Action action)
        {
            tweener.OnPlay(() => action());
            return this;
        }
        public TweeningInfo OnUpdate(Action action)
        {
            tweener.OnUpdate(() => action());
            return this;
        }
        public TweeningInfo OnComplete(Action action)
        {
            tweener.OnComplete(() => action());
            return this;
        }
        public bool IsActive()
        {
            return tweener.IsActive();
        }
        public bool IsBackwards()
        {
            return tweener.IsBackwards();
        }
        public bool IsComplete()
        {
            return tweener.IsComplete();
        }
        public bool IsInitialized()
        {
            return tweener.IsInitialized();
        }
        public bool IsPlaying()
        {
            return tweener.IsPlaying();
        }
        public TweeningInfo Goto(float toSecs)
        {
            bool isPlaying = tweener.IsPlaying();
            tweener.Goto(toSecs, isPlaying);
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
        public static bool IsInfoNull(TweeningInfo tweeningInfo)
        {
            if (tweeningInfo != null)
                if (tweeningInfo.tweener != null)
                    return false;
            return true;
        }
        public static void PlayTween(TweeningInfo tweeningInfos)
        {
            if (tweeningInfos != null)
                tweeningInfos.Play();
        }
        public static void PlayTweens(params TweeningInfo[] tweeningInfos)
        {
            foreach (var TI in tweeningInfos)
                PlayTween(TI);
        }
        public static void PauseTween(TweeningInfo tweeningInfos)
        {
            if (tweeningInfos != null)
                tweeningInfos.Pause();
        }
        public static void PauseTweens(params TweeningInfo[] tweeningInfos)
        {
            foreach (var TI in tweeningInfos)
                PauseTween(TI);
        }
        public static void TryKillTween(TweeningInfo tweeningInfo, bool isComplete = true)
        {
            if (tweeningInfo != null)
            {
                if (tweeningInfo.tweener != null)
                {
                    tweeningInfo.tweener.Kill(isComplete);
                    tweeningInfo.tweener = null;
                    tweeningInfo = null;
                }
            }
        }
        public static void TryKillTweens(params TweeningInfo[] tweeningInfos)
        {
            foreach (var TI in tweeningInfos)
                TryKillTween(TI);
        }
    }
}