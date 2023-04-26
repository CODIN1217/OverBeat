using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using TweenValue;

[Serializable]
public class TweenInfo<T>
{
    [SerializeField] TweenType<T> _startValue;
    [SerializeField] TweenType<T> _endValue;
    [SerializeField] float _duration;
    [SerializeField] AnimationCurve _ease;
    /* public TweenInfo()
    {
        _duration = 0.5f;
        _ease = AnimationCurve.Linear(0f, 0f, 1f, 1f);
    } */
    public TweenInfo(T startValue, T endValue, float duration, AnimationCurve ease)
    {
        _startValue = new TweenType<T>(startValue);
        _endValue = new TweenType<T>(endValue);
        _duration = duration;
        _ease = ease;
    }
    public TweenInfo(Func<T> startValue, Func<T> endValue, float duration, AnimationCurve ease)
    {
        _startValue = new TweenType<T>(startValue);
        _endValue = new TweenType<T>(endValue);
        _duration = duration;
        _ease = ease;
    }
    public T startValue { get { return _startValue.value; }}
    public T endValue { get { return _endValue.value; }}
    public float duration { get { return _duration; } set { _duration = value; } }
    public AnimationCurve ease { get { return _ease; } set { _ease = value; } }
}
