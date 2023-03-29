using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseCamera : MonoBehaviour
{
    Camera baseCamera;
    public readonly float stdSize;
    Handy handy;
    WorldInfo worldInfo;
    void Awake()
    {
        handy = Handy.Property;
        baseCamera = Camera.main;
    }
    void Update()
    {
        worldInfo = handy.GetWorldInfo();
        baseCamera.orthographicSize = stdSize * worldInfo.cameraInfo.size;
        baseCamera.backgroundColor = handy.GetColor01(worldInfo.cameraInfo.BGColor);
        transform.rotation = Quaternion.Euler(0f, 0f, handy.GetCorrectDegMaxIs0(worldInfo.cameraInfo.rotation));
    }
    BaseCamera()
    {
        stdSize = 5.4f;
    }
}
