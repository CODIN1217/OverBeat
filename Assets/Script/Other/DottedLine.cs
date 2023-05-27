using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class DottedLine : MonoBehaviour
{
    LineRenderer lineRenderer;
    public List<Vector3> poses;
    public float widthMultiplier;
    void Awake()
    {
        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.startWidth = 1f;
        lineRenderer.endWidth = 1f;
    }
    void Update()
    {
        if (poses != null)
            lineRenderer.positionCount = poses.Count;
        lineRenderer.SetPositions(poses.Cast<Vector3>().ToArray());
        if (widthMultiplier < 0f)
            widthMultiplier = 0f;
        lineRenderer.widthMultiplier = widthMultiplier;
        SetLineWidth(widthMultiplier);
    }
    public void SetLineWidth(float width)
    {
        lineRenderer.material.mainTextureScale = new Vector2(1f / width, 1f);
    }
    public void SetColor(Color color)
    {
        lineRenderer.startColor = color;
        lineRenderer.endColor = color;
    }
}
