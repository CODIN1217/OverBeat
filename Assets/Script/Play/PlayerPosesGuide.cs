using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerPosesGuide : MonoBehaviour
{
    Handy handy;
    WorldInfo worldInfo;
    public GameObject playerPosesLine;
    List<GameObject> playerPosesLines;
    List<SpriteRenderer> playerPosesLinesRenderer;
    // List<DottedLine> playerPosesLinesInfo;
    void Awake()
    {
        handy = Handy.Property;
        playerPosesLines = new List<GameObject>();
        playerPosesLinesRenderer = new List<SpriteRenderer>();
        // playerPosesLinesInfo = new List<DottedLine>();
        for (int i = 0; i < handy.GetMaxStdDegCount(); i++)
        {
            playerPosesLines.Add(Instantiate(playerPosesLine, transform));
            playerPosesLinesRenderer.Add(playerPosesLines[i].GetComponent<SpriteRenderer>());
            playerPosesLines[i].transform.position = handy.GetWorldInfo().centerPos;
            playerPosesLines[i].SetActive(false);
            // playerPosesLinesInfo.Add(playerPosesLines[i].GetComponent<DottedLine>());
            // playerPosesLinesInfo[i].poses = new List<Vector3>(){worldInfo.centerPos, handy.GetCircularPos(worldInfo.stdDegs[i], Mathf.Clamp(handy.playerScript.radius - playerCenterRadius, 0f, handy.playerScript.radius), worldInfo.centerPos)};
            // playerPosesLinesInfo[i].SetRepeatCount(2.545f * handy.playerScript.radius);
        }
    }
    void Update()
    {
        worldInfo = handy.GetWorldInfo();
        foreach (var PPL in playerPosesLines)
        {
            PPL.transform.position = worldInfo.centerPos;
            PPL.SetActive(false);
        }
        float playerCenterRadius = handy.GetScaleAverage(handy.playerCenter.transform.localScale) * handy.GetScaleAverage(new Vector2(handy.playerCenterRenderer.sprite.texture.width, handy.playerCenterRenderer.sprite.texture.height)) * 0.005f;
        for (int i = 0; i < worldInfo.stdDegs.Count; i++)
        {
            playerPosesLines[i].transform.position = handy.GetCircularPos(worldInfo.stdDegs[i], Mathf.Clamp(handy.playerScript.radius - playerCenterRadius, 0f, handy.playerScript.radius), worldInfo.centerPos);
            if (!handy.CheckObjInOtherObj(playerPosesLines[i], handy.playerCenter, new Vector2(playerPosesLinesRenderer[i].sprite.texture.width, playerPosesLinesRenderer[i].sprite.texture.height), new Vector2(handy.playerCenterRenderer.sprite.texture.width, handy.playerCenterRenderer.sprite.texture.height))/* i != handy.GetWorldInfo(handy.noteIndex - 1).nextDegIndex */)
            {
                playerPosesLines[i].SetActive(true);
            }
        }
    }
}
