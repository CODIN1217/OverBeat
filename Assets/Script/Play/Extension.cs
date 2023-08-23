using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public static class Extension
{
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