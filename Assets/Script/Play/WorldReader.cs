using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class WorldReader : MonoBehaviour
{
    public GameObject worldInfosObj;
    GameObject worldInfoInstance;
    public List<WorldInfo> worldInfos;
    void Awake()
    {
        worldInfoInstance = Instantiate(worldInfosObj, transform);
        worldInfos = new List<WorldInfo>();
        for (int i = 0; i < worldInfoInstance.transform.childCount; i++)
        {
            if (worldInfoInstance.transform.GetChild(i).gameObject.activeSelf)
                worldInfos.Add(worldInfoInstance.transform.GetChild(i).GetComponent<WorldInfo>());
        }
    }
}
