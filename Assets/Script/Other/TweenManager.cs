using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System;

namespace TweenManager
{
    public interface ITweenerInfo
    {
        void InitTween();
        void UpdateTweenValue();
        void PlayTween();
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
        /* public TweenInfo (TweenInfo<T> tweenInfo)
        {
            startValue = tweenInfo.startValue;
            endValue = tweenInfo.endValue;
            ease = new AnimationCurve(tweenInfo.ease.keys);
        }
        public TweenInfo (TweenerInfo<T> tweenerInfo)
        {
            startValue = tweenerInfo.startValue;
            endValue = tweenerInfo.endValue;
            ease = new AnimationCurve(tweenerInfo.ease.keys);
        } */
        public TweenInfo<T> Clone()
        {
            return new TweenInfo<T>(startValue, endValue, ease);
        }
    }
    public class TweenerInfo<T> : TweenInfo<T>
    {
        public T curValue;
        float duration;
        // public Sequence tweener;
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
        public static float GetTweenValue(TweenInfo<float> tweenInfo, float progress) => tweenInfo.startValue + (tweenInfo.endValue - tweenInfo.startValue) * tweenInfo.ease.Evaluate(progress);
        public static Vector2 GetTweenValue(TweenInfo<Vector2> tweenInfo, float progress) => tweenInfo.startValue + (tweenInfo.endValue - tweenInfo.startValue) * tweenInfo.ease.Evaluate(progress);
        public static Vector3 GetTweenValue(TweenInfo<Vector3> tweenInfo, float progress) => tweenInfo.startValue + (tweenInfo.endValue - tweenInfo.startValue) * tweenInfo.ease.Evaluate(progress);
        public static Color GetTweenValue(TweenInfo<Color> tweenInfo, float progress) => tweenInfo.startValue + (tweenInfo.endValue - tweenInfo.startValue) * tweenInfo.ease.Evaluate(progress);

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
    }
    /* public class TweeningInfo
    {
        object startValue;
        object endValue;
        Func<object> _curValue;
        float duration;
        AnimationCurve ease;
        public Sequence tweener;
        public object curValue { get { return _curValue(); } }
        public TweeningInfo(TweenInfo<float> tweenInfo, float duration, Func<float, float> processEndValue = null, Func<object> value = null)
        {
            startValue = tweenInfo.startValue;
            endValue = tweenInfo.endValue;
            _curValue = () => startValue;
            this.duration = duration;
            ease = tweenInfo.ease;
            tweener = DOTween.Sequence()
            .Append(DOTween.To(() => tweenInfo.startValue, (v) => { _curValue = () => v; }, processEndValue(tweenInfo.endValue), duration)
            .SetEase(tweenInfo.ease));
        }
        public TweeningInfo(TweenInfo<Vector2> tweenInfo, float duration, Func<Vector2, Vector2> processEndValue = null, Func<object> value = null)
        {
            startValue = tweenInfo.startValue;
            endValue = tweenInfo.endValue;
            _curValue = () => startValue;
            this.duration = duration;
            ease = tweenInfo.ease;
            tweener = DOTween.Sequence()
            .Append(DOTween.To(() => tweenInfo.startValue, (v) => { _curValue = () => v; }, processEndValue(tweenInfo.endValue), duration)
            .SetEase(tweenInfo.ease));
        }
        public TweeningInfo(TweenInfo<Vector3> tweenInfo, float duration, Func<Vector3, Vector3> processEndValue = null, Func<object> value = null)
        {
            startValue = tweenInfo.startValue;
            endValue = tweenInfo.endValue;
            _curValue = () => startValue;
            this.duration = duration;
            ease = tweenInfo.ease;
            tweener = DOTween.Sequence()
            .Append(DOTween.To(() => tweenInfo.startValue, (v) => { _curValue = () => v; }, processEndValue(tweenInfo.endValue), duration)
            .SetEase(tweenInfo.ease));
        }
        public TweeningInfo(TweenInfo<Color> tweenInfo, float duration, Func<Color, Color> processEndValue = null, Func<object> value = null)
        {
            startValue = tweenInfo.startValue;
            endValue = tweenInfo.endValue;
            _curValue = () => startValue;
            this.duration = duration;
            ease = tweenInfo.ease;
            tweener = DOTween.Sequence()
            .Append(DOTween.To(() => tweenInfo.startValue, (v) => { _curValue = () => v; }, processEndValue(tweenInfo.endValue), duration)
            .SetEase(tweenInfo.ease));
        }
    } */
}