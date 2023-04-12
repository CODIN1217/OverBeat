using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseCamera : MonoBehaviour
{
    Camera baseCamera;
    public readonly float stdSize;
    Handy handy;
    WorldInfo worldInfo;
    GameManager GM;
    void Awake()
    {
        GM = GameManager.Property;
        handy = Handy.Property;
        baseCamera = Camera.main;
    }
    void Update()
    {
        if (GM.isBreakUpdate())
            return;
        worldInfo = handy.GetWorldInfo(GM.curWorldInfoIndex);
        baseCamera.orthographicSize = stdSize * worldInfo.cameraInfo.size;
        baseCamera.backgroundColor = handy.GetColor01(worldInfo.cameraInfo.BGColor);
        transform.rotation = Quaternion.Euler(0f, 0f, handy.GetCorrectDegMaxIs0(worldInfo.cameraInfo.rotation));
    }
    BaseCamera()
    {
        stdSize = 5.4f;
    }
}
