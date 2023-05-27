using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using TMPro;
using System.Text;
using DG.Tweening;
using TweenManager;
using System.Diagnostics;
using Debug = UnityEngine.Debug;
using System.Linq;

public static class Handy
{
    public static Vector2 GetCircularPos(float deg, float radius, Vector2? centerPos = null)
    {
        if (centerPos == null)
            centerPos = Vector2.zero;
        float rad = deg * Mathf.Deg2Rad;
        return new Vector2(Mathf.Sin(rad), Mathf.Cos(rad)) * radius + (Vector2)centerPos;
    }
    public static float GetAverageAbsScale(GameObject go)
    {
        return (Mathf.Abs(go.transform.localScale.x) + Mathf.Abs(go.transform.localScale.y)) * 0.5f;
    }
    public static float GetAverageAbsScale(Vector2 scale)
    {
        return (Mathf.Abs(scale.x) + Mathf.Abs(scale.y)) * 0.5f;
    }
    public static float GetAverageAbsScale(float x, float y)
    {
        return (Mathf.Abs(x) + Mathf.Abs(y)) * 0.5f;
    }


    public static float GetBiggerAbsScale(GameObject go)
    {
        return Mathf.Abs(go.transform.localScale.x) > Mathf.Abs(go.transform.localScale.y) ? Mathf.Abs(go.transform.localScale.x) : Mathf.Abs(go.transform.localScale.y);
    }
    public static float GetBiggerAbsScale(Vector2 scale)
    {
        return Mathf.Abs(scale.x) > Mathf.Abs(scale.y) ? Mathf.Abs(scale.x) : Mathf.Abs(scale.y);
    }
    public static float GetBiggerAbsScale(float x, float y)
    {
        return Mathf.Abs(x) > Mathf.Abs(y) ? Mathf.Abs(x) : Mathf.Abs(y);
    }


    public static float GetSmallerAbsScale(GameObject go)
    {
        return Mathf.Abs(go.transform.localScale.x) < Mathf.Abs(go.transform.localScale.y) ? Mathf.Abs(go.transform.localScale.x) : Mathf.Abs(go.transform.localScale.y);
    }
    public static float GetSmallerAbsScale(Vector2 scale)
    {
        return Mathf.Abs(scale.x) < Mathf.Abs(scale.y) ? Mathf.Abs(scale.x) : Mathf.Abs(scale.y);
    }
    public static float GetSmallerAbsScale(float x, float y)
    {
        return Mathf.Abs(x) < Mathf.Abs(y) ? Mathf.Abs(x) : Mathf.Abs(y);
    }
    public static Color GetColor(Color color, float alpha)
    {
        return new Color(color.r, color.g, color.b, alpha);
    }
    public static SpriteRenderer GetColor(SpriteRenderer rend, float alpha)
    {
        rend.color = GetColor(rend.color, alpha);
        return rend;
    }
    public static LineRenderer GetColor(LineRenderer rend, float alpha)
    {
        rend.startColor = GetColor(rend.startColor, alpha);
        rend.endColor = GetColor(rend.endColor, alpha);
        return rend;
    }
    public static TextMeshProUGUI GetColor(TextMeshProUGUI rend, float alpha)
    {
        rend.color = GetColor(rend.color, alpha);
        return rend;
    }
    public static Color GetCorrectedColor(Color color, float minValue = 0f, float maxValue = 1f)
    {
        return new Color(Mathf.Clamp(color.r, minValue, maxValue), Mathf.Clamp(color.g, minValue, maxValue), Mathf.Clamp(color.b, minValue, maxValue), Mathf.Clamp(color.a, minValue, maxValue));
    }
    public static Vector2 GetImagePixels(Sprite sprite)
    {
        return new Vector2(sprite.texture.width, sprite.texture.height);
    }
    public static Vector2 GetImagePixels(SpriteRenderer rend)
    {
        return GetImagePixels(rend.sprite);
    }
    public static Vector2 GetImagePixels(Image rend)
    {
        return GetImagePixels(rend.sprite);
    }
    public static bool CheckColliding(Vector2 objAPos, Vector2 objAScale, Vector2 objAPixelCount, Vector2 objBPos, Vector2 objBScale, Vector2 objBPixelCount)
    {
        if (Vector2.Distance(objAPos, objBPos) <= (Handy.GetAverageAbsScale(Handy.MultiplyVector(objAScale, objAPixelCount)) + Handy.GetAverageAbsScale(Handy.MultiplyVector(objBScale, objBPixelCount))) * 0.005f)
        {
            return true;
        }
        return false;
    }
    public static IEnumerator WaitCodeForFixedUpdateCo(Action PlayCode)
    {
        yield return new WaitForFixedUpdate();
        PlayCode();
    }
    public static IEnumerator WaitCodeForUpdateCo(Action PlayCode)
    {
        yield return null;
        PlayCode();
    }
    public static IEnumerator WaitCodeForSecsCo(float waitSecs, Action code)
    {
        yield return new WaitForSeconds(waitSecs);
        code();
    }
    public static IEnumerator WaitCodeUntilCo(Func<bool> predicate, Action code)
    {
        yield return new WaitUntil(predicate);
        code();
    }
    public static void RepeatCode(Action code, int count, int initIndex = 0)
    {
        for (int i = initIndex; i < count; i++)
        {
            code();
        }
    }
    public static void RepeatCode(Action<int> code, int count, int initIndex = 0)
    {
        for (int i = initIndex; i < count; i++)
        {
            code(i);
        }
    }
    public static void RepeatCode<T>(Action<int, T> code, List<T> list, int initIndex = 0)
    {
        for (int i = initIndex; i < list.Count; i++)
        {
            code(i, list[i]);
        }
    }
    public static void RepeatCode<T>(Action<T> code, List<T> list, int initIndex = 0)
    {
        for (int i = initIndex; i < list.Count; i++)
        {
            code(list[i]);
        }
    }
    public static float GetCorrectedDegMaxIs0(float deg)
    {
        if (deg < 0f || deg >= 360f)
        {
            deg -= Mathf.Sign(deg) * 360f * Mathf.Clamp(Mathf.Abs(Mathf.Floor(deg / 360f)), 1f, float.MaxValue);
        }
        return deg;
    }
    public static float GetCorrectedDegMaxIs360(float deg)
    {
        if (deg <= 0f || deg > 360f)
        {
            deg -= (deg == 0 ? -1f : Mathf.Sign(deg)) * 360f * Mathf.Clamp(Mathf.Abs(Mathf.Floor(deg / 360f)), 1f, float.MaxValue);
        }
        return deg;
    }
    public static float GetDegDistance(float tarDeg, float curDeg, bool maxIs360, int direction)
    {
        float distanceDeg = (tarDeg - curDeg) * direction;
        return maxIs360 ? GetCorrectedDegMaxIs360(distanceDeg) : GetCorrectedDegMaxIs0(distanceDeg);
    }
    public static float GetSign0IsPlus(float value)
    {
        return value < 0f ? -1f : 1f;
    }
    public static float GetSign0IsZero(float value)
    {
        if (value != 0f)
            return Mathf.Sign(value);
        return 0f;
    }
    public static float GetSign0IsMinus(float value)
    {
        return value <= 0f ? -1f : 1f;
    }
    public static Vector2 MultiplyVector(Vector2 va, Vector2 vb)
    {
        return new Vector2(va.x * vb.x, va.y * vb.y);
    }
    public static Vector2 GetCorrectedVector(Vector2 vector, float minValue, float maxValue)
    {
        return new Vector2(Mathf.Clamp(vector.x, minValue, maxValue), Mathf.Clamp(vector.y, minValue, maxValue));
    }
    public static float GetPosesDistance(List<Vector2> poses)
    {
        float length = 0f;
        for (int i = 0; i < poses.Count; i++)
        {
            length += Vector2.Distance(poses[Handy.GetBeforeIndex(i)], poses[i]);
        }
        return length;
    }
    public static List<Vector3> ConvertListVector(List<Vector2> vector2s)
    {
        List<Vector3> vector3s = new List<Vector3>();
        foreach (var vector2 in vector2s)
            vector3s.Add(vector2);
        return vector3s;
    }
    public static List<Vector2> ConvertListVector(List<Vector3> vector3s)
    {
        List<Vector2> vector2s = new List<Vector2>();
        foreach (var vector3 in vector3s)
            vector2s.Add(vector3);
        return vector2s;
    }
    public static float CeilValue(float value, int place)
    {
        return Mathf.Ceil(value * Mathf.Pow(10f, place - 1)) / Mathf.Pow(10f, place - 1);
    }
    public static float RoundValue(float value, int place)
    {
        return Mathf.Round(value * Mathf.Pow(10f, place - 1)) / Mathf.Pow(10f, place - 1);
    }
    public static float FloorValue(float value, int place)
    {
        return Mathf.Floor(value * Mathf.Pow(10f, place - 1)) / Mathf.Pow(10f, place - 1);
    }
    public static void WriteLog<T>(params T[] contents)
    {
        StringBuilder text = new StringBuilder();
        foreach (var cont in contents)
            text.Append(cont.ToString() + "    ");
        Debug.Log(text);
    }
    public static int GetBeforeIndex(int index, int initIndex = 0)
    {
        return index <= initIndex ? initIndex : index - 1;
    }
    public static int GetNextIndex(int index, int maxIndex = int.MaxValue)
    {
        return index >= maxIndex ? maxIndex : index + 1;
    }
    public static int GetCorrectedIndex(int index, int maxIndex = int.MaxValue, int minIndex = 0)
    {
        return (int)Mathf.Clamp(index, minIndex, maxIndex);
    }
    public static T[] GetArray<T>(params T[] parms)
    {
        return parms;
    }
    public static int TryGetArrayLength<T>(T[] array)
    {
        if (array != null)
            return array.Length;
        return 0;
    }
    public static string GetPredicateName(string[] predicateNames, int? index = null)
    {
        StringBuilder predicateName = new StringBuilder();
        for (int i = 0; i < predicateNames.Length; i++)
        {
            predicateName.Append(predicateNames[i] + (i == predicateNames.Length - 1 ? null : " / "));
        }
        if (index != null)
            predicateName.Append($" ( Index : {index} )");
        return predicateName.ToString();
    }
    public static void SetDottedLine(DottedLine dottedLine, List<Vector2> poses, float? posesLength = null)
    {
        dottedLine.poses = Handy.ConvertListVector(poses);
    }
}