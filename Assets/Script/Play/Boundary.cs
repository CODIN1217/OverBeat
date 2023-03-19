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
        worldInfo = handy.GetWorldInfo();
        boundaryCoverImage.color = worldInfo.boundaryCoverColor == null ? handy.GetColor01(worldInfo.cameraBGColor) : handy.GetColor01((Color)worldInfo.boundaryCoverColor);
        boundaryLineImage.color = handy.GetColor01(worldInfo.boundaryLineColor);
        transform.localScale = (/* worldInfo.boundaryScale == null ? Vector2.one * (worldInfo.noteStartRadius > worldInfo.playerRadius  ? worldInfo.noteStartRadius * 0.2f : worldInfo.playerRadius / 1.5f) :  */worldInfo.boundaryScale) / worldInfo.cameraSize;
    }
}
