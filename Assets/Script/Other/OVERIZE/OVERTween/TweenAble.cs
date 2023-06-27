using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OVERIZE
{
    public struct TweenAble
    {
        internal enum TweenType {Null, Int, Float, Double, Vector3, Vector2, Color }
        internal TweenType tweenType;
        int? _value_int;
        float? _value_float;
        double? _value_double;
        Vector2? _value_Vector2;
        Vector3? _value_Vector3;
        Color? _value_Color;
        TweenAble(TweenType tweenType)
        {
            _value_int = 0;
            _value_float = 0f;
            _value_double = 0d;
            _value_Vector2 = Vector2.zero;
            _value_Vector3 = Vector3.zero;
            _value_Color = Color.clear;
            this.tweenType = tweenType;
        }
        TweenAble(int value)
        {
            _value_int = value;
            _value_float = 0f;
            _value_double = 0d;
            _value_Vector2 = Vector2.zero;
            _value_Vector3 = Vector3.zero;
            _value_Color = Color.clear;
            tweenType = TweenType.Int;
        }
        TweenAble(float value)
        {
            _value_int = 0;
            _value_float = value;
            _value_double = 0d;
            _value_Vector2 = Vector2.zero;
            _value_Vector3 = Vector3.zero;
            _value_Color = Color.clear;
            tweenType = TweenType.Float;
        }
        TweenAble(double value)
        {
            _value_int = 0;
            _value_float = 0f;
            _value_double = value;
            _value_Vector2 = Vector2.zero;
            _value_Vector3 = Vector3.zero;
            _value_Color = Color.clear;
            tweenType = TweenType.Double;
        }
        TweenAble(Vector2 value)
        {
            _value_int = 0;
            _value_float = 0f;
            _value_double = 0d;
            _value_Vector2 = value;
            _value_Vector3 = Vector3.zero;
            _value_Color = Color.clear;
            tweenType = TweenType.Vector2;
        }
        TweenAble(Vector3 value)
        {
            _value_int = 0;
            _value_float = 0f;
            _value_double = 0d;
            _value_Vector2 = Vector2.zero;
            _value_Vector3 = value;
            _value_Color = Color.clear;
            tweenType = TweenType.Vector3;
        }
        TweenAble(Color value)
        {
            _value_int = 0;
            _value_float = 0f;
            _value_double = 0d;
            _value_Vector2 = Vector2.zero;
            _value_Vector3 = Vector3.zero;
            _value_Color = value;
            tweenType = TweenType.Color;
        }
        public static implicit operator TweenAble(int value) => new TweenAble(value);
        public static implicit operator TweenAble(float value) => new TweenAble(value);
        public static implicit operator TweenAble(double value) => new TweenAble(value);
        public static implicit operator TweenAble(Vector2 value) => new TweenAble(value);
        public static implicit operator TweenAble(Vector3 value) => new TweenAble(value);
        public static implicit operator TweenAble(Color value) => new TweenAble(value);

        public static implicit operator int(TweenAble TA) => (int)TA._value_int;
        public static implicit operator float(TweenAble TA) => (float)TA._value_float;
        public static implicit operator double(TweenAble TA) => (double)TA._value_double;
        public static implicit operator Vector2(TweenAble TA) => (Vector2)TA._value_Vector2;
        public static implicit operator Vector3(TweenAble TA) => (Vector3)TA._value_Vector3;
        public static implicit operator Color(TweenAble TA) => (Color)TA._value_Color;
    }
}
