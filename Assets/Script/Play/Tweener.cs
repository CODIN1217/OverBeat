using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class Tweener
{
    [SerializeField] float _duration;
    [SerializeField] AnimationCurve _ease;
    public Tweener()
    {
        _duration = 0f;
        _ease = AnimationCurve.Linear(0f, 0f, 1f, 1f);
    }
    public Tweener(float duration)
    {
        _duration = Mathf.Clamp(duration, 0f, float.MaxValue);
        _ease = AnimationCurve.Linear(0f, 0f, 1f, 1f);
    }
    public float duration { get { return _duration; } set { _duration = value; } }
    public AnimationCurve ease { get { return _ease; } set { _ease = value; } }
}
