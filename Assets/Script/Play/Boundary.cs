using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Boundary : MonoBehaviour
{
    Handy handy;
    WorldInfo worldInfo;
    public GameObject boundaryCover;
    public GameObject boundaryMask;
    public Image boundaryCoverImage;
    public Image boundaryMaskImage;
    void Awake() {
        handy = Handy.Property;
        // boundaryCoverImage = boundaryCover.GetComponent<Image>();
        // boundaryMaskImage = boundaryMask.GetComponent<Image>();
    }
    void Update()
    {
        worldInfo = handy.GetWorldInfo();
        boundaryCoverImage.color = worldInfo.boundaryCoverColor;
        transform.localScale = (worldInfo.boundaryScale == null ? Vector2.one * (worldInfo.noteStartRadius > worldInfo.playerRadius  ? worldInfo.noteStartRadius * 0.2f : worldInfo.playerRadius / 1.5f) : (Vector2)worldInfo.boundaryScale) / worldInfo.cameraSize;
    }
}
