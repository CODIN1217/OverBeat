using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class TweenInfo<T>
{
    [SerializeField] TweenValue.TweenType<T> _tweenStartValue;
    [SerializeField] TweenValue.TweenType<T> _tweenEndValue;
    [SerializeField] float _duration;
    [SerializeField] AnimationCurve _ease;
    /* public TweenInfo()
    {
        _duration = 0.5f;
        _ease = AnimationCurve.Linear(0f, 0f, 1f, 1f);
    } */
    public TweenInfo(T startValue, T endValue, float duration, AnimationCurve ease)
    {
        _tweenStartValue = new TweenValue.TweenType<T>(startValue);
        _tweenEndValue = new TweenValue.TweenType<T>(endValue);
        _duration = duration;
        _ease = ease;
    }
    public TweenInfo(Func<T> startValue, Func<T> endValue, float duration, AnimationCurve ease)
    {
        _tweenStartValue = new TweenValue.TweenType<T>(startValue);
        _tweenEndValue = new TweenValue.TweenType<T>(endValue);
        _duration = duration;
        _ease = ease;
    }
    public T tweenStartValue { get { return _tweenStartValue.value; }}
    public T tweenEndValue { get { return _tweenEndValue.value; }}
    public float duration { get { return _duration; } set { _duration = value; } }
    public AnimationCurve ease { get { return _ease; } set { _ease = value; } }
}
