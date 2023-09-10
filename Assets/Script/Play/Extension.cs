using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

namespace OVERIZE
{
    public static class Extension
    {
        public static bool IsAll<T>(this IEnumerable<T> array, T value)
        {
            foreach (var element in array)
                if (element.Equals(value))
                    return true;
            return false;
        }
        public static OrderedElement<T>[] Order<T>(this T[] array, bool descendingOrder = false)
        {
            var orderedArray = array.Select((item, index) => new OrderedElement<T>(index, item)).OrderBy(e => e.Value);
            if (descendingOrder)
                orderedArray.Reverse();
            return orderedArray.ToArray();
        }
        public static T[] SetLength<T>(this T[] array, int length)
        {
            T[] arrayTemp = new T[length];
            for (int i = 0; i < length; i++)
            {
                if (i >= array.Length)
                    break;
                arrayTemp[i] = array[i];
            }
            return arrayTemp;
        }
        public static List<T> SetLength<T>(this List<T> list, int length)
        {
            List<T> listTemp = new List<T>();
            for (int i = 0; i < length; i++)
            {
                if (i >= list.Count)
                    listTemp.Add(default);
                listTemp.Add(list[i]);
            }
            return listTemp;
        }
        public static (T, U) Set<T, U>(this ref (T, U) tuple, T value) => tuple = Set(tuple, value);
        public static (T, U) Set<T, U>(this ref (T, U) tuple, U value) => tuple = Set(tuple, value);
        public static (T, U, V) Set<T, U, V>(this ref (T, U, V) tuple, T value) => tuple = Set(tuple, value);
        public static (T, U, V) Set<T, U, V>(this ref (T, U, V) tuple, U value) => tuple = Set(tuple, value);
        public static (T, U, V) Set<T, U, V>(this ref (T, U, V) tuple, V value) => tuple = Set(tuple, value);

        static (T, U) Set<T, U>((T, U) tuple, T value) => (value, tuple.Item2);
        static (T, U) Set<T, U>((T, U) tuple, U value) => (tuple.Item1, value);
        static (T, U, V) Set<T, U, V>((T, U, V) tuple, T value) => (value, tuple.Item2, tuple.Item3);
        static (T, U, V) Set<T, U, V>((T, U, V) tuple, U value) => (tuple.Item1, value, tuple.Item3);
        static (T, U, V) Set<T, U, V>((T, U, V) tuple, V value) => (tuple.Item1, tuple.Item2, value);

        public static bool Contains<T, U>(this List<(T, U)> tuples, T value) => (from tuple in tuples select tuple.Item1).Contains(value);
        public static bool Contains<T, U>(this List<(T, U)> tuples, U value) => (from tuple in tuples select tuple.Item2).Contains(value);
        public static bool Contains<T, U, V>(this List<(T, U, V)> tuples, T value) => (from tuple in tuples select tuple.Item1).Contains(value);
        public static bool Contains<T, U, V>(this List<(T, U, V)> tuples, U value) => (from tuple in tuples select tuple.Item2).Contains(value);
        public static bool Contains<T, U, V>(this List<(T, U, V)> tuples, V value) => (from tuple in tuples select tuple.Item3).Contains(value);

        public static Vector2 Reciprocal(this Vector2 denominator) => new Vector2(1f / denominator.x, 1f / denominator.y);
        public static Vector3 Reciprocal(this Vector3 denominator) => new Vector3(1f / denominator.x, 1f / denominator.y, 1f / denominator.z);
        public static Color Reciprocal(this Color denominator) => new Color(1f / denominator.r, 1f / denominator.g, 1f / denominator.b, 1f / denominator.a);
        public static Vector2 Reciprocal(this Vector2 denominator, Vector2 numerator) => Vector2.Scale(new Vector2(1f / denominator.x, 1f / denominator.y), numerator);
        public static Vector3 Reciprocal(this Vector3 denominator, Vector3 numerator) => Vector3.Scale(new Vector3(1f / denominator.x, 1f / denominator.y, 1f / denominator.z), numerator);
        public static Color Reciprocal(this Color denominator, Color numerator) => new Color(1f / denominator.r, 1f / denominator.g, 1f / denominator.b, 1f / denominator.a) * numerator;

        public static int[] GetLengthes<T>(this T[][] array)
        {
            int[] lengthes = new int[array.Length];
            for (int i = 0; i < array.Length; i++)
                lengthes[i] = array[i].Length;
            return lengthes;
        }
        public static T[] SetMonoValue<T>(this T[] array, T monoValue)
        {
            for (int i = 0; i < array.Length; i++)
                array[i] = monoValue;
            return array;
        }
        public static T[][] SetMonoValue<T>(this T[][] array, T monoValue)
        {
            foreach (var arr in array)
                for (int i = 0; i < arr.Length; i++)
                    arr[i] = monoValue;
            return array;
        }
        public static T[] Copy<T>(this T[] array)
        {
            T[] arrayTemp = new T[array.Length];
            Array.Copy(array, arrayTemp, array.Length);
            return arrayTemp;
        }
        public static T[][] Copy<T>(this T[][] array)
        {
            T[][] arrayTemp = new T[array.Length][];
            Array.Copy(array, arrayTemp, array.Length);
            for (int i = 0; i < array.Length; i++)
            {
                arrayTemp[i] = new T[array[i].Length];
                Array.Copy(array[i], arrayTemp[i], array[i].Length);
            }
            return arrayTemp;
        }
    }
}