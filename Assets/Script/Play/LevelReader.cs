using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class LevelReader : MonoBehaviour
{
    public GameObject levelInfosObj;
    GameObject levelInfoInstance;
    public List<LevelInfo> levelInfos;
    void Awake()
    {
        levelInfoInstance = Instantiate(levelInfosObj, transform);
        levelInfos = new List<LevelInfo>();
        for (int i = 0; i < levelInfoInstance.transform.childCount; i++)
        {
            if (levelInfoInstance.transform.GetChild(i).gameObject.activeSelf)
                levelInfos.Add(levelInfoInstance.transform.GetChild(i).GetComponent<LevelInfo>());
        }
    }
}
