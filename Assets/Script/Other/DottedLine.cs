using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class DottedLine : MonoBehaviour
{
    LineRenderer lineRenderer;
    AnimationCurve aniCurv;
    public List<Vector3> poses;
    public List<float> curvVert01s;
    public float curvVertMultiplier;
    void Awake()
    {
        lineRenderer = GetComponent<LineRenderer>();
        aniCurv = new AnimationCurve();
    }
    void Update()
    {
        if (poses != null)
            lineRenderer.positionCount = poses.Count;
        lineRenderer.SetPositions(poses.Cast<Vector3>().ToArray());
        if (curvVert01s.Count <= 1f)
        {
            float curvVert01 = curvVert01s.Count == 1 ? curvVert01s[0] : 1f;
            curvVert01s = new List<float>() { curvVert01, curvVert01 };
        }
        if (curvVertMultiplier < 0f)
        {
            curvVertMultiplier = 0f;
        }
        for (int i = 0; i < curvVert01s.Count; i++)
        {
            aniCurv.AddKey((float)i / (float)(curvVert01s.Count - 1), curvVert01s[i]);
        }
        lineRenderer.widthCurve = aniCurv;
        lineRenderer.widthMultiplier = curvVertMultiplier;
    }
    public void SetRepeatCount(float repeatCount)
    {
        lineRenderer.material.SetFloat("_Rep", repeatCount / (curvVertMultiplier * 5f));
    }
    public void SetColor(Color startColor, Color endColor)
    {
        lineRenderer.startColor = startColor;
        lineRenderer.endColor = endColor;
    }
}
