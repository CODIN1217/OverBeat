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
        for (int i = 0; i < 5; i++)
        {
            worldInfos.Add(new WorldInfo());
            int playerIndex = i;
            playerIndex -= (int)(2f * Mathf.Clamp(Mathf.Floor((float)playerIndex * 0.5f), 0f, float.MaxValue));
            worldInfos[i].playerIndex = playerIndex;
            for (int j = 0; j < worldInfos[i].nextDegIndex.Count; j++)
            {
                int stdDegIndex = i + j;
                stdDegIndex -= (int)(4f * Mathf.Clamp(Mathf.Floor((float)stdDegIndex * 0.25f), 0f, float.MaxValue));
                worldInfos[i].nextDegIndex[j] = stdDegIndex;
            }
            worldInfos[i].awakeTime = (float)(Mathf.Clamp(i/*  * 2 */ - 1, 0f, float.MaxValue));
        }
        notesCount = worldInfos.Count - 1;
    }
    void Update()
    {

    }
}
