using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System;

namespace TweenValue
{
    public class TweeningInfo
    {
        object startValue;
        object endValue;
        Func<object> _curValue;
        float duration;
        AnimationCurve ease;
        public Sequence tweener;
        public object curValue { get { return _curValue(); } }
        public TweeningInfo(TweenInfo<float> tweenInfo, float duration, Func<object> value = null)
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
                this.duration = duration;
                ease = tweenInfo.ease;
                tweener = DOTween.Sequence()
                .Append(DOTween.To(() => tweenInfo.startValue, (v) => { _curValue = () => v; }, tweenInfo.endValue, duration)
                .SetEase(tweenInfo.ease))
                .Pause();
            }
        }
        public TweeningInfo(TweenInfo<Vector2> tweenInfo, float duration, Func<object> value = null)
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
                this.duration = duration;
                ease = tweenInfo.ease;
                tweener = DOTween.Sequence()
                .Append(DOTween.To(() => tweenInfo.startValue, (v) => { _curValue = () => v; }, tweenInfo.endValue, duration)
                .SetEase(tweenInfo.ease))
                .Pause();
            }
        }
        public TweeningInfo(TweenInfo<Vector3> tweenInfo, float duration, Func<object> value = null)
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
                this.duration = duration;
                ease = tweenInfo.ease;
                tweener = DOTween.Sequence()
                .Append(DOTween.To(() => tweenInfo.startValue, (v) => { _curValue = () => v; }, tweenInfo.endValue, duration)
                .SetEase(tweenInfo.ease))
                .Pause();
            }
        }
        public TweeningInfo(TweenInfo<Color> tweenInfo, float duration, Func<object> value = null)
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
                this.duration = duration;
                ease = tweenInfo.ease;
                tweener = DOTween.Sequence()
                .Append(DOTween.To(() => tweenInfo.startValue, (v) => { _curValue = () => v; }, tweenInfo.endValue, duration)
                .SetEase(tweenInfo.ease))
                .Pause();
            }
        }
    }
}