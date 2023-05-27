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
        /* for (int i = 0; i < levelInfosObj.transform.childCount; i++)
        {
            if (levelInfosObj.transform.GetChild(i).gameObject.activeSelf)
                levelInfos.Add(levelInfosObj.transform.GetChild(i).GetComponent<LevelInfo>());
        } */
    }
}
