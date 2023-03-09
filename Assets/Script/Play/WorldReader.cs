using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class WorldReader : MonoBehaviour
{
    public List<WorldInfo> worldInfos;
    public int notesCount;
    Handy handy;
    void Awake()
    {
        handy = Handy.Property;
        worldInfos = new List<WorldInfo>();
        for(int i = 0; i < 5; i++){
            worldInfos.Add(new WorldInfo());
            int stdDegIndex = i;
            stdDegIndex -= (int)(4f * Mathf.Clamp(Mathf.Floor((float)stdDegIndex * 0.25f), 0f, float.MaxValue));
            worldInfos[i].nextDegIndex = stdDegIndex;
            // worldInfos[i].nextDeg = worldInfos[i].stdDegs[stdDegIndex]/* handy.GetCorrectDegMaxIs0(i * 90f) */;
        }
        notesCount = worldInfos.Count - 1;
    }
    void Update()
    {
        
    }
}
