using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Boundary : MonoBehaviour
{
    Handy handy;
    WorldInfo worldInfo;
    public GameObject boundaryLine;
    public GameObject boundaryCover;
    public GameObject boundaryMask;
    public Image boundaryLineImage;
    public Image boundaryCoverImage;
    public Image boundaryMaskImage;
    void Awake() {
        handy = Handy.Property;
    }
    void Update()
    {
        worldInfo = handy.GetWorldInfo(0);
        boundaryCoverImage.color = worldInfo.BoundaryInfo.CoverColor == null ? handy.GetColor01(worldInfo.CameraInfo.BGColor) : handy.GetColor01((Color)worldInfo.BoundaryInfo.CoverColor);
        boundaryLineImage.color = handy.GetColor01(worldInfo.BoundaryInfo.LineColor);
        transform.localScale = (/* worldInfo.boundaryScale == null ? Vector2.one * (worldInfo.noteStartRadius > worldInfo.playerRadius  ? worldInfo.noteStartRadius * 0.2f : worldInfo.playerRadius / 1.5f) :  */worldInfo.BoundaryInfo.Scale) / worldInfo.CameraInfo.Size;
    }
}
