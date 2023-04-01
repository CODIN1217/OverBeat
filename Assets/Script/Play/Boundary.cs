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
    GameManager GM;
    void Awake()
    {
        GM = GameManager.Property;
        handy = Handy.Property;
    }
    void Update()
    {
        worldInfo = handy.GetWorldInfo(GM.curWorldInfoIndex);
        boundaryCoverImage.color = worldInfo.boundaryInfo.coverColor == null ? handy.GetColor01(worldInfo.cameraInfo.BGColor) : handy.GetColor01((Color)worldInfo.boundaryInfo.coverColor);
        boundaryLineImage.color = handy.GetColor01(worldInfo.boundaryInfo.lineColor);
        transform.localScale = (/* worldInfo.boundaryScale == null ? Vector2.one * (worldInfo.noteStartRadius > worldInfo.playerRadius  ? worldInfo.noteStartRadius * 0.2f : worldInfo.playerRadius / 1.5f) :  */worldInfo.boundaryInfo.scale) / worldInfo.cameraInfo.size;
    }
}
