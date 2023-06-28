using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OVERIZE
{
    public struct TweenEase
    {
        readonly AnimationCurve curve;
        public TweenEase(AnimationCurve curve)
        {
            this.curve = new AnimationCurve(curve.keys);
        }
        internal float this[float time] { get { return curve.Evaluate(time); } }
    }
}