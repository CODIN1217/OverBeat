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

namespace Handy
{
    namespace Transform
    {
        public static class PosMethod
        {
            public static Vector2 GetCircularPos(float deg, float radius, Vector2? centerPos = null)
            {
                if (centerPos == null)
                    centerPos = Vector2.zero;
                float rad = deg * Mathf.Deg2Rad;
                return new Vector2(Mathf.Sin(rad), Mathf.Cos(rad)) * radius + (Vector2)centerPos;
            }
        }
        public static class ScaleMethod
        {
            public static float GetScaleAbsAverage(GameObject go)
            {
                return (Mathf.Abs(go.transform.localScale.x) + Mathf.Abs(go.transform.localScale.y)) * 0.5f;
            }
            public static float GetScaleAbsAverage(Vector2 scale)
            {
                return (Mathf.Abs(scale.x) + Mathf.Abs(scale.y)) * 0.5f;
            }
            public static float GetScaleAbsAverage(float x, float y)
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
        }
    }
    namespace Renderer
    {
        public static class ColorMethod
        {
            public static void FadeColor(UnityEngine.Renderer renderer, float alpha)
            {
                renderer.material.color = new Color(renderer.material.color.r, renderer.material.color.g, renderer.material.color.b, alpha);
            }
            public static void FadeColor(LineRenderer renderer, float alpha)
            {
                renderer.startColor = new Color(renderer.startColor.r, renderer.startColor.g, renderer.startColor.b, alpha);
                renderer.endColor = new Color(renderer.endColor.r, renderer.endColor.g, renderer.endColor.b, alpha);
            }
            public static void FadeColor(TextMeshProUGUI renderer, float alpha)
            {
                renderer.color = new Color(renderer.color.r, renderer.color.g, renderer.color.b, alpha);
            }
            public static Color GetCorrectRGBA(Color RGBA, float minRGBA = 0f, float maxRGBA = 1f)
            {
                return new Color(Mathf.Clamp(RGBA.r, minRGBA, maxRGBA), Mathf.Clamp(RGBA.g, minRGBA, maxRGBA), Mathf.Clamp(RGBA.b, minRGBA, maxRGBA), Mathf.Clamp(RGBA.a, minRGBA, maxRGBA));
            }
            public static Color? GetCorrectRGBA(Color? RGBA, float minRGBA = 0f, float maxRGBA = 1f)
            {
                if (RGBA == null)
                    return null;
                return GetCorrectRGBA((Color)RGBA, minRGBA, maxRGBA);
            }
        }
        public static class SpriteMethod
        {
            public static Vector2 GetSpritePixels(Sprite sprite)
            {
                return new Vector2(sprite.texture.width, sprite.texture.height);
            }
        }
    }
    namespace Physics
    {
        public static class CollidingMethod
        {
            public static bool CheckColliding(Vector2 objAPos, Vector2 objAScale, Vector2 objAPixelCount, Vector2 objBPos, Vector2 objBScale, Vector2 objBPixelCount)
            {
                if (Vector2.Distance(objAPos, objBPos) <= (Transform.ScaleMethod.GetScaleAbsAverage(Math.VectorMethod.MultiplyXByX_YByY(objAScale, objAPixelCount)) + Transform.ScaleMethod.GetScaleAbsAverage(Math.VectorMethod.MultiplyXByX_YByY(objBScale, objBPixelCount))) * 0.005f)
                {
                    return true;
                }
                return false;
            }
        }
    }
    namespace ProcessCode
    {
        public static class WaitCodeMethod
        {
            public static IEnumerator WaitCodeWaitForFixedUpdateCo(Action PlayCode)
            {
                yield return new WaitForFixedUpdate();
                PlayCode();
            }
            public static IEnumerator WaitCodeWaitForUpdateCo(Action PlayCode)
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
        }
        public static class RepeatCodeMethod
        {
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
        }
    }
    namespace Math
    {
        public static class DegMethod
        {
            public static float CorrectDegMaxIs0(float deg)
            {
                if (deg < 0f || deg >= 360f)
                {
                    deg -= Mathf.Sign(deg) * 360f * Mathf.Clamp(Mathf.Abs(Mathf.Floor(deg / 360f)), 1f, float.MaxValue);
                }
                return deg;
            }
            public static float CorrectDegMaxIs360(float deg)
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
                return maxIs360 ? CorrectDegMaxIs360(distanceDeg) : CorrectDegMaxIs0(distanceDeg);
            }
        }
        public static class SignMethod
        {
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
        }
        public static class VectorMethod
        {
            public static Vector2 MultiplyXByX_YByY(Vector2 va, Vector2 vb)
            {
                return new Vector2(va.x * vb.x, va.y * vb.y);
            }
            public static Vector2 GetCorrectXY(Vector2 XY, float minXY, float maxXY)
            {
                return new Vector2(Mathf.Clamp(XY.x, minXY, maxXY), Mathf.Clamp(XY.y, minXY, maxXY));
            }
            public static Vector2? GetCorrectXY(Vector2? XY, float minXY, float maxXY)
            {
                if (XY == null)
                    return null;
                return new Vector2(Mathf.Clamp(((Vector2)XY).x, minXY, maxXY), Mathf.Clamp(((Vector2)XY).y, minXY, maxXY));
            }
            public static float GetDistance(List<Vector2> poses)
            {
                float length = 0f;
                for (int i = 0; i < poses.Count; i++)
                {
                    length += Vector2.Distance(poses[Handy.IndexMethod.GetBeforeIndex(i)], poses[i]);
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
        }
    }
    public static class LogMethod
    {
        public static void WriteLog(params object[] contents)
        {
            StringBuilder text = new StringBuilder();
            foreach (var cont in contents)
                text.Append(cont.ToString() + "    ");
            Debug.Log(text);
        }
    }
    public static class IndexMethod
    {
        public static int GetBeforeIndex(int index, int initIndex = 0)
        {
            return index <= initIndex ? initIndex : index - 1;
        }
        public static int GetNextIndex(int index, int maxIndex = int.MaxValue)
        {
            return index >= maxIndex ? maxIndex : index + 1;
        }
        public static int CorrectIndex(int index, int maxIndex = int.MaxValue, int minIndex = 0)
        {
            return (int)Mathf.Clamp(index, minIndex, maxIndex);
        }
    }
    public static class ArrayMethod
    {
        public static T[] GetParams<T>(params T[] parms) => parms;
    }
    public static class ReflectionMethod
    {
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
    }
    public static class LineRendMethod
    {
        public static void SetDottedLine(DottedLine dottedLine, List<Vector2> poses, float? posesLength = null)
        {
            dottedLine.poses = Handy.Math.VectorMethod.ConvertListVector(poses);
            dottedLine.SetRepeatCount((posesLength == null ? Handy.Math.VectorMethod.GetDistance(poses) : (float)posesLength) * 2.44f);
        }
    }
}