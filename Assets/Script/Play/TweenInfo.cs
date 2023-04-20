using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class TweenInfo<T>
{
    [SerializeField] T _value;
    [SerializeField] float _duration;
    [SerializeField] AnimationCurve _ease;
    /* public TweenInfo()
    {
        _duration = 0.5f;
        _ease = AnimationCurve.Linear(0f, 0f, 1f, 1f);
    } */
    public TweenInfo(T value, float duration, AnimationCurve ease)
    {
        _value = value;
        _duration = duration;
        _ease = ease;
    }
    public T value { get { return _value; } set { _value = value; } }
    public float duration { get { return _duration; } set { _duration = value; } }
    public AnimationCurve ease { get { return _ease; } set { _ease = value; } }
}
