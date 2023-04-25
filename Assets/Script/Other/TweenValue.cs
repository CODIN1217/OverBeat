using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System;

namespace TweenValue
{
    public class TweenType<T>
    {
        T _value;
        Func<T> _autoValue;
        public bool isAuto;
        public TweenType(T value)
        {
            isAuto = false;
            _value = value;
        }
        public TweenType(Func<T> autoValue)
        {
            isAuto = true;
            _autoValue = autoValue;
        }
        public T value { get { if (isAuto) return _autoValue(); else return _value; } }
    }
    
    public class TweeningInfo
    {
        public object startValue;
        public object endValue;
        public object curValue;
        public float duration;
        public AnimationCurve ease;
        public Sequence valueTween;
        public TweeningInfo(TweenInfo<float> tweenInfo, Func<float, float> processValue = null)
        {
            this.startValue = tweenInfo.tweenStartValue;
            this.endValue = tweenInfo.tweenEndValue;
            this.duration = tweenInfo.duration;
            this.ease = tweenInfo.ease;
            if (processValue == null)
                processValue = (v) => v;
            valueTween = DOTween.Sequence()
            .Append(DOTween.To(() => tweenInfo.tweenStartValue, (v) => curValue = processValue(v), tweenInfo.tweenEndValue, tweenInfo.duration))
            .SetEase(tweenInfo.ease);
        }
        public TweeningInfo(TweenInfo<Vector2> tweenInfo, Func<Vector2, Vector2> processValue = null)
        {
            this.startValue = tweenInfo.tweenStartValue;
            this.endValue = tweenInfo.tweenEndValue;
            this.duration = tweenInfo.duration;
            this.ease = tweenInfo.ease;
            if (processValue == null)
                processValue = (v) => v;
            valueTween = DOTween.Sequence()
            .Append(DOTween.To(() => tweenInfo.tweenStartValue, (v) => curValue = processValue(v), tweenInfo.tweenEndValue, tweenInfo.duration))
            .SetEase(tweenInfo.ease);
        }
        public TweeningInfo(TweenInfo<Vector3> tweenInfo, Func<Vector3, Vector3> processValue = null)
        {
            this.startValue = tweenInfo.tweenStartValue;
            this.endValue = tweenInfo.tweenEndValue;
            this.duration = tweenInfo.duration;
            this.ease = tweenInfo.ease;
            if (processValue == null)
                processValue = (v) => v;
            valueTween = DOTween.Sequence()
            .Append(DOTween.To(() => tweenInfo.tweenStartValue, (v) => curValue = processValue(v), tweenInfo.tweenEndValue, tweenInfo.duration))
            .SetEase(tweenInfo.ease);
        }
        public TweeningInfo(TweenInfo<Color> tweenInfo, Func<Color, Color> processValue = null)
        {
            this.startValue = tweenInfo.tweenStartValue;
            this.endValue = tweenInfo.tweenEndValue;
            this.duration = tweenInfo.duration;
            this.ease = tweenInfo.ease;
            if (processValue == null)
                processValue = (v) => v;
            valueTween = DOTween.Sequence()
            .Append(DOTween.To(() => tweenInfo.tweenStartValue, (v) => curValue = processValue(v), tweenInfo.tweenEndValue, tweenInfo.duration))
            .SetEase(tweenInfo.ease);
        }
    }
}