using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using TweenValue;

[Serializable]
public class TweenInfo<T>
{
    [SerializeField] T _startValue;
    [SerializeField] T _endValue;
    [SerializeField] float _duration;
    [SerializeField] AnimationCurve _ease;
    public bool isAuto;
    /* public TweenInfo()
    {
        _duration = 0.5f;
        _ease = AnimationCurve.Linear(0f, 0f, 1f, 1f);
    } */
    public TweenInfo(T startValue, T endValue, float duration, AnimationCurve ease)
    {
        isAuto = false;
        _startValue = startValue;
        _endValue = endValue;
        _duration = duration;
        _ease = ease;
    }
    public TweenInfo()
    {
        isAuto = true;
    }
    /* public TweenInfo<T> SetAutoValue(TweenInfo<T> tweenInfo)
    {
        _startValue.SetAutoValue(() => tweenInfo.startValue);
        _endValue.SetAutoValue(() => tweenInfo.endValue);
        return this;
    }
    public TweenInfo<T> SetAutoValue(Func<T> startValue, Func<T> endValue)
    {
        _startValue.SetAutoValue(startValue);
        _endValue.SetAutoValue(endValue);
        return this;
    } */
    /* public TweenInfo(Func<T> startValue, Func<T> endValue, float duration, AnimationCurve ease)
    {
        _startValue = new TweenType<T>(startValue);
        _endValue = new TweenType<T>(endValue);
        _duration = duration;
        _ease = ease;
    } */
    public T startValue { get { return _startValue; } }
    public T endValue { get { return _endValue; } }
    public float duration { get { return _duration; } set { _duration = value; } }
    public AnimationCurve ease { get { return _ease; } set { _ease = value; } }
}
