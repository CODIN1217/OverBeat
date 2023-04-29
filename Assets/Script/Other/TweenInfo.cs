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
    [SerializeField] AnimationCurve _ease;
    public bool isAuto;
    public TweenInfo(T startValue, T endValue, AnimationCurve ease)
    {
        isAuto = false;
        _startValue = startValue;
        _endValue = endValue;
        _ease = ease;
    }
    public TweenInfo()
    {
        isAuto = true;
    }
    public T startValue { get { return _startValue; } }
    public T endValue { get { return _endValue; } }
    public AnimationCurve ease { get { return _ease; } set { _ease = value; } }
}
