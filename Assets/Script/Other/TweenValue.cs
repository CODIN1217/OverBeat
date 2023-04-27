using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System;

namespace TweenValue
{
    /* [Serializable]
    public class TweenType<T>
    {
        Func<T> _value;
        public bool isAuto;
        public TweenType(T value)
        {
            isAuto = false;
            _value = () => value;
        }
        public TweenType()
        {
            isAuto = true;
            _value = null;
        }
        public void SetAutoValue(Func<T> autoValue)
        {
            if (isAuto)
                _value = autoValue;
        }
        public T value { get { return _value(); } }
    } */

    public class TweeningInfo
    {
        object startValue;
        object endValue;
        Func<object> _curValue;
        float duration;
        AnimationCurve ease;
        public Sequence tweener;
        public TweeningInfo(TweenInfo<float> tweenInfo, Func<object> value = null)
        {
            if (tweenInfo.isAuto)
            {
                if (value != null)
                {
                    _curValue = value;
                }
                else
                {
                    throw new Exception("AutoTween don't have value");
                }
            }
            else
            {
                startValue = tweenInfo.startValue;
                endValue = tweenInfo.endValue;
                _curValue = () => startValue;
                duration = tweenInfo.duration;
                ease = tweenInfo.ease;
                tweener = DOTween.Sequence()
                .Append(DOTween.To(() => tweenInfo.startValue, (v) => { _curValue = () => v; }, tweenInfo.endValue, tweenInfo.duration)
                .SetEase(tweenInfo.ease))
                .Pause();
            }
        }
        public TweeningInfo(TweenInfo<Vector2> tweenInfo, Func<object> value = null)
        {
            if (tweenInfo.isAuto)
            {
                if (value != null)
                {
                    _curValue = value;
                }
                else
                {
                    throw new Exception("AutoTween don't have value");
                }
            }
            else
            {
                startValue = tweenInfo.startValue;
                endValue = tweenInfo.endValue;
                _curValue = () => startValue;
                duration = tweenInfo.duration;
                ease = tweenInfo.ease;
                tweener = DOTween.Sequence()
                .Append(DOTween.To(() => tweenInfo.startValue, (v) => { _curValue = () => v; }, tweenInfo.endValue, tweenInfo.duration)
                .SetEase(tweenInfo.ease))
                .Pause();
            }
        }
        public TweeningInfo(TweenInfo<Vector3> tweenInfo, Func<object> value = null)
        {
            if (tweenInfo.isAuto)
            {
                if (value != null)
                {
                    _curValue = value;
                }
                else
                {
                    throw new Exception("AutoTween don't have value");
                }
            }
            else
            {
                startValue = tweenInfo.startValue;
                endValue = tweenInfo.endValue;
                _curValue = () => startValue;
                duration = tweenInfo.duration;
                ease = tweenInfo.ease;
                tweener = DOTween.Sequence()
                .Append(DOTween.To(() => tweenInfo.startValue, (v) => { _curValue = () => v; }, tweenInfo.endValue, tweenInfo.duration)
                .SetEase(tweenInfo.ease))
                .Pause();
            }
        }
        public TweeningInfo(TweenInfo<Color> tweenInfo, Func<object> value = null)
        {
            if (tweenInfo.isAuto)
            {
                if (value != null)
                {
                    _curValue = value;
                }
                else
                {
                    throw new Exception("AutoTween don't have value");
                }
            }
            else
            {
                startValue = tweenInfo.startValue;
                endValue = tweenInfo.endValue;
                _curValue = () => startValue;
                duration = tweenInfo.duration;
                ease = tweenInfo.ease;
                tweener = DOTween.Sequence()
                .Append(DOTween.To(() => tweenInfo.startValue, (v) => { _curValue = () => v; }, tweenInfo.endValue, tweenInfo.duration)
                .SetEase(tweenInfo.ease))
                .Pause();
            }
        }
        public object curValue { get { return _curValue(); } }
    }
}