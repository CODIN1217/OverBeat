using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OVERIZE
{
    public struct TweenAble
    {
        internal enum TweenType { Null = 0, Int = 1 << 0, Float = 1 << 1, Double = 1 << 2, Vector2 = 1 << 3, Vector3 = 1 << 4, Color = 1 << 5 }
        internal TweenType tweenType;
        int? _value_int;
        float? _value_float;
        double? _value_double;
        Vector2? _value_Vector2;
        Vector3? _value_Vector3;
        Color? _value_Color;
        TweenAble(TweenType tweenType)
        {
            _value_int = null;
            _value_float = null;
            _value_double = null;
            _value_Vector2 = null;
            _value_Vector3 = null;
            _value_Color = null;
            this.tweenType = tweenType;
        }
        TweenAble(int value)
        {
            _value_int = value;
            _value_float = null;
            _value_double = null;
            _value_Vector2 = null;
            _value_Vector3 = null;
            _value_Color = null;
            tweenType = TweenType.Int;
        }
        TweenAble(float value)
        {
            _value_int = null;
            _value_float = value;
            _value_double = null;
            _value_Vector2 = null;
            _value_Vector3 = null;
            _value_Color = null;
            tweenType = TweenType.Float;
        }
        TweenAble(double value)
        {
            _value_int = null;
            _value_float = null;
            _value_double = value;
            _value_Vector2 = null;
            _value_Vector3 = null;
            _value_Color = null;
            tweenType = TweenType.Double;
        }
        TweenAble(Vector2 value)
        {
            _value_int = null;
            _value_float = null;
            _value_double = null;
            _value_Vector2 = value;
            _value_Vector3 = null;
            _value_Color = null;
            tweenType = TweenType.Vector2;
        }
        TweenAble(Vector3 value)
        {
            _value_int = null;
            _value_float = null;
            _value_double = null;
            _value_Vector2 = null;
            _value_Vector3 = value;
            _value_Color = null;
            tweenType = TweenType.Vector3;
        }
        TweenAble(Color value)
        {
            _value_int = null;
            _value_float = null;
            _value_double = null;
            _value_Vector2 = null;
            _value_Vector3 = null;
            _value_Color = value;
            tweenType = TweenType.Color;
        }
        public static implicit operator TweenAble(int value) => new TweenAble(value);
        public static implicit operator TweenAble(float value) => new TweenAble(value);
        public static implicit operator TweenAble(double value) => new TweenAble(value);
        public static implicit operator TweenAble(Vector2 value) => new TweenAble(value);
        public static implicit operator TweenAble(Vector3 value) => new TweenAble(value);
        public static implicit operator TweenAble(Color value) => new TweenAble(value);

        public static explicit operator int(TweenAble TA) => (int)TA._value_int;
        public static explicit operator float(TweenAble TA) => (float)TA._value_float;
        public static explicit operator double(TweenAble TA) => (double)TA._value_double;
        public static explicit operator Vector2(TweenAble TA) => (Vector2)TA._value_Vector2;
        public static explicit operator Vector3(TweenAble TA) => (Vector3)TA._value_Vector3;
        public static explicit operator Color(TweenAble TA) => (Color)TA._value_Color;

        /* public T ByType<T>(Func<TweenType, T> code)
        {
            for (int i = 0; i < Handy.Enums(typeof(TweenType)).Length - 1; i++)
            {
                if (((int)tweenType & (1 << i)) != 0)
                    return code((TweenType)(1 << i));
            }
            return code(TweenType.Null);
        } */

        public static TweenAble operator +(TweenAble TA1, TweenAble TA2)
        {
            if ((((int)TA1.tweenType & (int)TA2.tweenType) & (1 << 0)) != 0)
                return (int)(TA1._value_int + TA2._value_int);
            if ((((int)TA1.tweenType & (int)TA2.tweenType) & (1 << 1)) != 0)
                return (float)(TA1._value_float + TA2._value_float);
            if ((((int)TA1.tweenType & (int)TA2.tweenType) & (1 << 2)) != 0)
                return (double)(TA1._value_double + TA2._value_double);
            if ((((int)TA1.tweenType & (int)TA2.tweenType) & (1 << 3)) != 0)
                return (Vector2)(TA1._value_Vector2 + TA2._value_Vector2);
            if ((((int)TA1.tweenType & (int)TA2.tweenType) & (1 << 4)) != 0)
                return (Vector3)(TA1._value_Vector3 + TA2._value_Vector3);
            if ((((int)TA1.tweenType & (int)TA2.tweenType) & (1 << 5)) != 0)
                return (Color)(TA1._value_Color + TA2._value_Color);
            return new TweenAble(TweenType.Null);
        }
        public static TweenAble operator -(TweenAble TA1, TweenAble TA2)
        {
            if ((((int)TA1.tweenType & (int)TA2.tweenType) & (1 << 0)) != 0)
                return (int)(TA1._value_int - TA2._value_int);
            if ((((int)TA1.tweenType & (int)TA2.tweenType) & (1 << 1)) != 0)
                return (float)(TA1._value_float - TA2._value_float);
            if ((((int)TA1.tweenType & (int)TA2.tweenType) & (1 << 2)) != 0)
                return (double)(TA1._value_double - TA2._value_double);
            if ((((int)TA1.tweenType & (int)TA2.tweenType) & (1 << 3)) != 0)
                return (Vector2)(TA1._value_Vector2 - TA2._value_Vector2);
            if ((((int)TA1.tweenType & (int)TA2.tweenType) & (1 << 4)) != 0)
                return (Vector3)(TA1._value_Vector3 - TA2._value_Vector3);
            if ((((int)TA1.tweenType & (int)TA2.tweenType) & (1 << 5)) != 0)
                return (Color)(TA1._value_Color - TA2._value_Color);
            return new TweenAble(TweenType.Null);
        }
        public static TweenAble operator *(TweenAble TA, float V)
        {
            if (((int)TA.tweenType & (1 << 0)) != 0)
                return (int)TA._value_int * (int)V;
            if (((int)TA.tweenType & (1 << 1)) != 0)
                return (float)TA._value_float * V;
            if (((int)TA.tweenType & (1 << 2)) != 0)
                return (double)TA._value_double * V;
            if (((int)TA.tweenType & (1 << 3)) != 0)
                return (Vector2)TA._value_Vector2 * V;
            if (((int)TA.tweenType & (1 << 4)) != 0)
                return (Vector3)TA._value_Vector3 * V;
            if (((int)TA.tweenType & (1 << 5)) != 0)
                return (Color)TA._value_Color * V;
            return new TweenAble(TweenType.Null);
        }
        public static TweenAble operator /(TweenAble TA, float V)
        {
            if (((int)TA.tweenType & (1 << 0)) != 0)
                return (int)TA._value_int / (int)V;
            if (((int)TA.tweenType & (1 << 1)) != 0)
                return (float)TA._value_float / V;
            if (((int)TA.tweenType & (1 << 2)) != 0)
                return (double)TA._value_double / V;
            if (((int)TA.tweenType & (1 << 3)) != 0)
                return (Vector2)TA._value_Vector2 / V;
            if (((int)TA.tweenType & (1 << 4)) != 0)
                return (Vector3)TA._value_Vector3 / V;
            if (((int)TA.tweenType & (1 << 5)) != 0)
                return (Color)TA._value_Color / V;
            return new TweenAble(TweenType.Null);
        }
        public static TweenAble operator *(TweenAble TA1, TweenAble TA2)
        {
            if ((((int)TA1.tweenType & (int)TA2.tweenType) & (1 << 0)) != 0)
                return (int)(TA1._value_int * TA2._value_int);
            if ((((int)TA1.tweenType & (int)TA2.tweenType) & (1 << 1)) != 0)
                return (float)(TA1._value_float * TA2._value_float);
            if ((((int)TA1.tweenType & (int)TA2.tweenType) & (1 << 2)) != 0)
                return (double)(TA1._value_double * TA2._value_double);
            if ((((int)TA1.tweenType & (int)TA2.tweenType) & (1 << 3)) != 0)
                return Vector2.Scale((Vector2)TA1._value_Vector2, (Vector2)TA2._value_Vector2);
            if ((((int)TA1.tweenType & (int)TA2.tweenType) & (1 << 4)) != 0)
                return Vector3.Scale((Vector3)TA1._value_Vector3, (Vector3)TA2._value_Vector3);
            if ((((int)TA1.tweenType & (int)TA2.tweenType) & (1 << 5)) != 0)
                return (Color)(TA1._value_Color * TA2._value_Color);
            return new TweenAble(TweenType.Null);
        }
        public static TweenAble operator /(TweenAble TA1, TweenAble TA2)
        {
            if ((((int)TA1.tweenType & (int)TA2.tweenType) & (1 << 0)) != 0)
                return (int)(TA1._value_int / TA2._value_int);
            if ((((int)TA1.tweenType & (int)TA2.tweenType) & (1 << 1)) != 0)
                return (float)(TA1._value_float / TA2._value_float);
            if ((((int)TA1.tweenType & (int)TA2.tweenType) & (1 << 2)) != 0)
                return (double)(TA1._value_double / TA2._value_double);
            if ((((int)TA1.tweenType & (int)TA2.tweenType) & (1 << 3)) != 0)
                return ((Vector2)TA2._value_Vector2).Reciprocal((Vector2)TA1._value_Vector2);
            if ((((int)TA1.tweenType & (int)TA2.tweenType) & (1 << 4)) != 0)
                return ((Vector3)TA2._value_Vector3).Reciprocal((Vector3)TA1._value_Vector3);
            if ((((int)TA1.tweenType & (int)TA2.tweenType) & (1 << 5)) != 0)
                return ((Color)TA2._value_Color).Reciprocal((Color)TA1._value_Color);
            return new TweenAble(TweenType.Null);
        }
    }
}
