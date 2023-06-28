using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OVERIZE
{
    public static partial class OVERMath
    {
        public static int Sign(int value)
        {
            if (value != 0)
                if (value < 0)
                    return -1;
                else
                    return 1;
            return 0;
        }
        public static float Sign(float value)
        {
            if (value != 0f)
                if (value < 0f)
                    return -1f;
                else
                    return 1f;
            return 0f;
        }
        public static int Clamp(int value, int min, int max)
        {
            if (value < min)
                return min;
            if (value > max)
                return max;
            return value;
        }
        public static float Clamp(float value, float min, float max)
        {
            if (value < min)
                return min;
            if (value > max)
                return max;
            return value;
        }
        public static int Clamp01(int value)
        {
            return Clamp(value, 0, 1);
        }
        public static float Clamp01(float value)
        {
            return Clamp(value, 0, 1);
        }
        public static int ClampMin(int value, int min)
        {
            if (value < min)
                return min;
            return value;
        }
        public static float ClampMin(float value, float min)
        {
            if (value < min)
                return min;
            return value;
        }
        public static int ClampMax(int value, int max)
        {
            if (value > max)
                return max;
            return value;
        }
        public static float ClampMax(float value, float max)
        {
            if (value > max)
                return max;
            return value;
        }
    }
}
