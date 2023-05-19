using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class DottedLine : MonoBehaviour
{
    public List<Vector3> poses;
    LineRenderer lineRenderer;
    AnimationCurve aniCurv;
    public List<float> curvVert01s;
    public float curvVertMultiplier;
    void Awake()
    {
        if (curvVert01s.Count <= 1f)
        {
            float curvVert01 = curvVert01s.Count == 1 ? curvVert01s[0] : 1f;
            curvVert01s = new List<float>() { curvVert01, curvVert01 };
        }
        if (curvVertMultiplier < 0f)
        {
            curvVertMultiplier = 0f;
        }
        lineRenderer = GetComponent<LineRenderer>();
        aniCurv = new AnimationCurve();
        for (int i = 0; i < curvVert01s.Count; i++)
        {
            aniCurv.AddKey((float)i / (float)(curvVert01s.Count - 1), curvVert01s[i]);
        }
        lineRenderer.widthCurve = aniCurv;
        lineRenderer.widthMultiplier = curvVertMultiplier;
    }
    void Update()
    {
        if (poses != null)
            lineRenderer.positionCount = poses.Count;
        lineRenderer.SetPositions(poses.Cast<Vector3>().ToArray());
    }
    public void SetRepeatCount(float repeatCount)
    {
        lineRenderer.material.SetFloat("_Rep", repeatCount);
    }
}
