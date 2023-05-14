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

public class Handy : MonoBehaviour
{
    private static Handy instance = null;
    PlayManager PM;
    CompareValue<int> _compareValue_int;
    public CompareValue<int> compareValue_int
    {
        get
        {
            return _compareValue_int;
        }
    }
    void Awake()
    {
        instance = this;
        PM = PlayManager.Property;
        _compareValue_int = new CompareValue<int>();
    }
    public static Handy Property
    {
        get
        {
            return instance;
        }
    }
    public class CompareValue<T>
    {
        public CompareValue()
        {
            beforeValuesWithNames = new Dictionary<string, T>();
        }
        Dictionary<string, T> beforeValuesWithNames;
        public void SetValueForCompare(string parentsName, string methodName, string varName, T value, int? index = null)
        {
            string fullName = $"{parentsName} / {methodName} / {varName}" + (index != null ? $" / {index}" : "");
            if (beforeValuesWithNames.ContainsKey(fullName))
            {
                beforeValuesWithNames[fullName] = value;
            }
            else
            {
                beforeValuesWithNames.Add(fullName, value);
            }
        }
        public bool CompareWithBeforeValue(string parentsName, string methodName, string varName, T curValue, int? index = null)
        {
            string fullName = $"{parentsName} / {methodName} / {varName}" + (index != null ? $" / {index}" : "");
            if (beforeValuesWithNames.ContainsKey(fullName))
            {
                if (beforeValuesWithNames[fullName].Equals(curValue))
                {
                    return true;
                }
            }
            return false;
        }
    }
    public float CorrectDegMaxIs0(float deg)
    {
        if (deg < 0f || deg >= 360f)
        {
            deg -= Mathf.Sign(deg) * 360f * Mathf.Clamp(Mathf.Abs(Mathf.Floor(deg / 360f)), 1f, float.MaxValue);
        }
        return deg;
    }
    public float CorrectDegMaxIs360(float deg)
    {
        if (deg <= 0f || deg > 360f)
        {
            deg -= (deg == 0 ? -1f : Mathf.Sign(deg)) * 360f * Mathf.Clamp(Mathf.Abs(Mathf.Floor(deg / 360f)), 1f, float.MaxValue);
        }
        return deg;
    }
    public float GetDegDistance(float tarDeg, float curDeg, bool maxIs360, int direction)
    {
        float distanceDeg = (tarDeg - curDeg) * direction;
        return maxIs360 ? CorrectDegMaxIs360(distanceDeg) : CorrectDegMaxIs0(distanceDeg);
    }
    public void FadeColor(Renderer renderer, float alpha)
    {
        renderer.material.color = new Color(renderer.material.color.r, renderer.material.color.g, renderer.material.color.b, alpha);
    }
    public void FadeColor(LineRenderer renderer, float alpha)
    {
        renderer.startColor = new Color(renderer.startColor.r, renderer.startColor.g, renderer.startColor.b, alpha);
        renderer.endColor = new Color(renderer.endColor.r, renderer.endColor.g, renderer.endColor.b, alpha);
    }
    public void FadeColor(TextMeshProUGUI renderer, float alpha)
    {
        renderer.color = new Color(renderer.color.r, renderer.color.g, renderer.color.b, alpha);
    }
    public Vector2 GetCircularPos(float deg, float radius, Vector2? centerPos = null)
    {
        if (centerPos == null)
            centerPos = Vector2.zero;
        float rad = deg * Mathf.Deg2Rad;
        return new Vector2(Mathf.Sin(rad), Mathf.Cos(rad)) * radius + (Vector2)centerPos;
    }
    public void WriteLog(params object[] contents)
    {
        StringBuilder text = new StringBuilder();
        foreach (var cont in contents)
            text.Append(cont.ToString() + "    ");
        Debug.Log(text);
    }
    public float GetScaleAbsAverage(GameObject go)
    {
        return (Mathf.Abs(go.transform.localScale.x) + Mathf.Abs(go.transform.localScale.y)) * 0.5f;
    }
    public float GetScaleAbsAverage(Vector2 scale)
    {
        return (Mathf.Abs(scale.x) + Mathf.Abs(scale.y)) * 0.5f;
    }
    public float GetScaleAbsAverage(float x, float y)
    {
        return (Mathf.Abs(x) + Mathf.Abs(y)) * 0.5f;
    }
    public float GetBiggerAbsScale(GameObject go)
    {
        return Mathf.Abs(go.transform.localScale.x) > Mathf.Abs(go.transform.localScale.y) ? Mathf.Abs(go.transform.localScale.x) : Mathf.Abs(go.transform.localScale.y);
    }
    public float GetBiggerAbsScale(Vector2 scale)
    {
        return Mathf.Abs(scale.x) > Mathf.Abs(scale.y) ? Mathf.Abs(scale.x) : Mathf.Abs(scale.y);
    }
    public float GetBiggerAbsScale(float x, float y)
    {
        return Mathf.Abs(x) > Mathf.Abs(y) ? Mathf.Abs(x) : Mathf.Abs(y);
    }
    public float GetSmallerAbsScale(GameObject go)
    {
        return Mathf.Abs(go.transform.localScale.x) < Mathf.Abs(go.transform.localScale.y) ? Mathf.Abs(go.transform.localScale.x) : Mathf.Abs(go.transform.localScale.y);
    }
    public float GetSmallerAbsScale(Vector2 scale)
    {
        return Mathf.Abs(scale.x) < Mathf.Abs(scale.y) ? Mathf.Abs(scale.x) : Mathf.Abs(scale.y);
    }
    public float GetSmallerAbsScale(float x, float y)
    {
        return Mathf.Abs(x) < Mathf.Abs(y) ? Mathf.Abs(x) : Mathf.Abs(y);
    }
    public void WaitCodeUntilUpdateEnd(Action PlayCode)
    {
        StartCoroutine(WaitCodeUntilUpdateEnd_Co(PlayCode));
    }
    IEnumerator WaitCodeUntilUpdateEnd_Co(Action PlayCode)
    {
        yield return null;
        PlayCode();
    }
    public int GetBeforeIndex(int index, int initIndex = 0)
    {
        return index <= initIndex ? initIndex : index - 1;
    }
    public bool CheckColliding(Vector2 objAPos, Vector2 objAScale, Vector2 objAPixelCount, Vector2 objBPos, Vector2 objBScale, Vector2 objBPixelCount)
    {
        if (Vector2.Distance(objAPos, objBPos) <= (GetScaleAbsAverage(MultiplyXByX_YByY(objAScale, objAPixelCount)) + GetScaleAbsAverage(MultiplyXByX_YByY(objBScale, objBPixelCount))) * 0.005f)
        {
            return true;
        }
        return false;
    }
    public float GetSign0IsMin(float value)
    {
        return value <= 0f ? -1f : 1f;
    }
    public float GetSign0Is0(float value)
    {
        if (value != 0f)
            return Mathf.Sign(value);
        return 0f;
    }
    public int CorrectIndex(int index, int maxIndex = int.MaxValue, int minIndex = 0)
    {
        return (int)Mathf.Clamp(index, minIndex, maxIndex);
    }
    public Vector2 GetSpritePixels(Sprite sprite)
    {
        return new Vector2(sprite.texture.width, sprite.texture.height);
    }
    public void RepeatCode(Action<int> code, int count)
    {
        for (int i = 0; i < count; i++)
        {
            code(i);
        }
    }
    public Vector2 GetCorrectXY(Vector2 XY, float minXY, float maxXY)
    {
        return new Vector2(Mathf.Clamp(XY.x, minXY, maxXY), Mathf.Clamp(XY.y, minXY, maxXY));
    }
    public Vector2? GetCorrectXY(Vector2? XY, float minXY, float maxXY)
    {
        if (XY == null)
            return null;
        return new Vector2(Mathf.Clamp(((Vector2)XY).x, minXY, maxXY), Mathf.Clamp(((Vector2)XY).y, minXY, maxXY));
    }
    public Color GetCorrectRGBA(Color RGBA, float minRGBA = 0f, float maxRGBA = 1f)
    {
        return new Color(Mathf.Clamp(RGBA.r, minRGBA, maxRGBA), Mathf.Clamp(RGBA.g, minRGBA, maxRGBA), Mathf.Clamp(RGBA.b, minRGBA, maxRGBA), Mathf.Clamp(RGBA.a, minRGBA, maxRGBA));
    }
    public Color? GetCorrectRGBA(Color? RGBA, float minRGBA = 0f, float maxRGBA = 1f)
    {
        if (RGBA == null)
            return null;
        return GetCorrectRGBA((Color)RGBA, minRGBA, maxRGBA);
    }
    public Color2 GetCorrectRGBA2(Color2 RGBA2, float minRGBA = 0f, float maxRGBA = 1f)
    {
        return new Color2(GetCorrectRGBA(RGBA2.ca, minRGBA, maxRGBA), GetCorrectRGBA(RGBA2.cb, minRGBA, maxRGBA));
    }
    public Color2? GetCorrectRGBA2(Color2? RGBA2, float minRGBA = 0f, float maxRGBA = 1f)
    {
        if (RGBA2 == null)
            return null;
        return new Color2(GetCorrectRGBA(((Color2)RGBA2).ca, minRGBA, maxRGBA), GetCorrectRGBA(((Color2)RGBA2).cb, minRGBA, maxRGBA));
    }
    public void PlayEachCodesWithBoolens(List<bool> boolen, List<Action> codes)
    {
        if (boolen.Count < codes.Count)
            codes.RemoveRange(boolen.Count, codes.Count - 1);
        for (int i = 0; i < boolen.Count; i++)
        {
            if (boolen[i])
            {
                codes[i]();
            }
        }
    }
    public void WaitCodeForSecs(float waitSecs, Action code)
    {
        StartCoroutine(WaitCodeForSecs_Co(waitSecs, code));
    }
    IEnumerator WaitCodeForSecs_Co(float waitSecs, Action code)
    {
        yield return new WaitForSeconds(waitSecs);
        code();
    }
    public void WaitCodeUntil(Func<bool> predicate, Action code)
    {
        StartCoroutine(WaitCodeUntil_Co(predicate, code));
    }
    IEnumerator WaitCodeUntil_Co(Func<bool> predicate, Action code)
    {
        yield return new WaitUntil(predicate);
        code();
    }
    public Vector2 MultiplyXByX_YByY(Vector2 va, Vector2 vb)
    {
        return new Vector2(va.x * vb.x, va.y * vb.y);
    }
    public void InvokeFunc(string methodName, GameObject go)
    {
        go.SendMessage(methodName, SendMessageOptions.DontRequireReceiver);
    }
    public void InvokeFuncAllObj(string methodName)
    {
        GameObject[] GOs = FindObjectsOfType<GameObject>();
        foreach (var GO in GOs)
            InvokeFunc(methodName, GO);
    }
}