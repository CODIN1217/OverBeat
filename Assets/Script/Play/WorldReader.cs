using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class WorldReader : MonoBehaviour
{
    public GameObject worldInfosObj;
    public List<WorldInfo> worldInfos;
    void Awake()
    {
        worldInfos = new List<WorldInfo>();
        for (int i = 0; i < worldInfosObj.transform.childCount; i++)
        {
            if (worldInfosObj.transform.GetChild(i).gameObject.activeSelf)
                worldInfos.Add(worldInfosObj.transform.GetChild(i).GetComponent<WorldInfo>());
        }
    }
}
