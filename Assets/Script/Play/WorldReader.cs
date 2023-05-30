using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class WorldReader : MonoBehaviour
{
    public WorldInfo worldInfo;
    void Awake()
    {
        worldInfo = new WorldInfo();
    }
}
