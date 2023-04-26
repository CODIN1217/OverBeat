using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System;

namespace TweenValue
{
    [Serializable]
    public class TweenType<T>
    {
        [SerializeField]
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
        public T value { get { if (isAuto) return _autoValue(); else return _value; }}
    }

    public class TweeningInfo
    {
        public object startValue;
        public object endValue;
        public object curValue;
        public float duration;
        public AnimationCurve ease;
        public Sequence valueTween;
        public TweeningInfo(TweenInfo<float> tweenInfo, float? duration = null, float prependInterval = 0f)
        {
            startValue = tweenInfo.startValue;
            endValue = tweenInfo.endValue;
            curValue = startValue;
            if (duration == null)
                duration = tweenInfo.duration;
            this.duration = (float)duration;
            ease = tweenInfo.ease;
            // if (processValue == null)
            //     processValue = (v) => v;
            valueTween = DOTween.Sequence()
            .PrependInterval(prependInterval)
            .Append(DOTween.To(() => tweenInfo.startValue, (v) => curValue = v/* processValue(v) */, tweenInfo.endValue, tweenInfo.duration)
            .SetEase(tweenInfo.ease))
            .Pause();
        }
        public TweeningInfo(TweenInfo<Vector2> tweenInfo, float? duration = null, float prependInterval = 0f)
        {
            startValue = tweenInfo.startValue;
            endValue = tweenInfo.endValue;
            curValue = startValue;
            if (duration == null)
                duration = tweenInfo.duration;
            this.duration = (float)duration;
            ease = tweenInfo.ease;
            // if (processValue == null)
            //     processValue = (v) => v;
            valueTween = DOTween.Sequence()
            .PrependInterval(prependInterval)
            .Append(DOTween.To(() => tweenInfo.startValue, (v) => curValue = v, tweenInfo.endValue, tweenInfo.duration)
            .SetEase(tweenInfo.ease))
            .Pause();
        }
        public TweeningInfo(TweenInfo<Vector3> tweenInfo, float? duration = null, float prependInterval = 0f)
        {
            startValue = tweenInfo.startValue;
            endValue = tweenInfo.endValue;
            curValue = startValue;
            if (duration == null)
                duration = tweenInfo.duration;
            this.duration = (float)duration;
            ease = tweenInfo.ease;
            // if (processValue == null)
            //     processValue = (v) => v;
            valueTween = DOTween.Sequence()
            .PrependInterval(prependInterval)
            .Append(DOTween.To(() => tweenInfo.startValue, (v) => curValue = v, tweenInfo.endValue, tweenInfo.duration)
            .SetEase(tweenInfo.ease))
            .Pause();
        }
        public TweeningInfo(TweenInfo<Color> tweenInfo, float? duration = null, float prependInterval = 0f)
        {
            startValue = tweenInfo.startValue;
            endValue = tweenInfo.endValue;
            curValue = startValue;
            if (duration == null)
                duration = tweenInfo.duration;
            this.duration = (float)duration;
            ease = tweenInfo.ease;
            // if (processValue == null)
            //     processValue = (v) => v;
            valueTween = DOTween.Sequence()
            .PrependInterval(prependInterval)
            .Append(DOTween.To(() => tweenInfo.startValue, (v) => curValue = v, tweenInfo.endValue, tweenInfo.duration)
            .SetEase(tweenInfo.ease))
            .Pause();
        }
        public void Play(){
            valueTween.Play();
        }
        public void Pause(){
            valueTween.Pause();
        }
        public void Goto(float to){
            valueTween.Goto(to);
        }
    }
}