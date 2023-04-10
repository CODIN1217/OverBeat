using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

// [ExecuteInEditMode]
public class WorldReader : MonoBehaviour
{
    public GameObject worldInfosObj;
    public List<WorldInfo> worldInfos;
    // public Hashtable objectInfos;
    // public List<WorldInfo.CameraInfo> cameraInfos;
    // public List<WorldInfo.CountDownInfo> countDownInfos;
    // public List<WorldInfo.PlayerInfo> playerInfos;
    // public List<WorldInfo.NoteInfo> noteInfos;
    // public List<WorldInfo.CenterInfo> centerInfos;
    // public List<WorldInfo.BoundaryInfo> boundaryInfos;
    // public List<WorldInfo.JudgmentInfo> judgmentInfos;
    // public List<WorldInfo.CreditInfo> creditInfos;
    // Handy handy;
    void Awake()
    {
        /* handy = Handy.Property;
        worldInfos = new List<WorldInfo>();
        for (int i = 0; i < 5; i++)
        {
            worldInfos.Add(new WorldInfo());
            int playerIndex = (int)Mathf.Clamp(i - 1, 0f, float.MaxValue);
            playerIndex -= (int)(2f * Mathf.Clamp(Mathf.Floor((float)playerIndex * 0.5f), 0f, float.MaxValue));
            worldInfos[i].PlayerInfo.Index = playerIndex;
            for (int j = 0; j < worldInfos[i].NoteInfo.NextDegIndexes.Count; j++)
            {
                int stdDegIndex = (int)Mathf.Clamp(i + j - 1, 0f, float.MaxValue);
                stdDegIndex -= (int)(4f * Mathf.Clamp(Mathf.Floor((float)stdDegIndex * 0.25f), 0f, float.MaxValue));
                worldInfos[i].NoteInfo.NextDegIndexes[j] = stdDegIndex;
            }
            worldInfos[i].NoteInfo.AwakeTimes = (float)(Mathf.Clamp(i - 1, 0f, float.MaxValue));
        } */
        worldInfos = new List<WorldInfo>();
        for (int i = 0; i < worldInfosObj.transform.childCount; i++)
        {
            if (worldInfosObj.transform.GetChild(i).gameObject.activeSelf)
                worldInfos.Add(worldInfosObj.transform.GetChild(i).GetComponent<WorldInfo>());
        }

        // objectInfos = new Hashtable();
    }
    void Update()
    {

    }
}
