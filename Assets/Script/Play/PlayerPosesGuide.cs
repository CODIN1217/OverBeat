using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerPosesGuide : MonoBehaviour
{
    Handy handy;
    WorldInfo worldInfo;
    public GameObject playerPosDotPrefab;
    List<GameObject> playerPosDots;
    List<SpriteRenderer> playerPosDotsRend;
    void Awake()
    {
        handy = Handy.Property;
        playerPosDots = new List<GameObject>();
        playerPosDotsRend = new List<SpriteRenderer>();
        for (int i = 0; i < handy.GetMaxStdDegCount(); i++)
        {
            playerPosDots.Add(Instantiate(playerPosDotPrefab, transform));
            playerPosDotsRend.Add(playerPosDots[i].GetComponent<SpriteRenderer>());
            playerPosDots[i].transform.position = handy.GetWorldInfo().centerPos;
            playerPosDots[i].SetActive(false);
        }
    }
    void Update()
    {
        worldInfo = handy.GetWorldInfo();
        foreach (var PPD in playerPosDots)
        {
            PPD.transform.position = worldInfo.centerPos;
            PPD.SetActive(false);
        }
        // float playerCenterRadius = handy.GetScaleAverage(handy.GetPlayerCenter().transform.localScale.x * handy.GetPlayerCenterRend().sprite.texture.width, handy.GetPlayerCenter().transform.localScale.y * handy.GetPlayerCenterRend().sprite.texture.height) * 0.005f;
        for (int i = 0; i < worldInfo.stdDegs.Count; i++)
        {
            playerPosDotsRend[i].color = handy.GetColor01(worldInfo.playerPosesGuideColor);
            playerPosDots[i].transform.position = handy.GetCircularPos(worldInfo.stdDegs[i], /* Mathf.Clamp( */handy.GetPlayerScript().curRadius/*  - playerCenterRadius, 0f, handy.GetPlayerScript().curRadius) */, worldInfo.centerPos);
            for (int j = 0; j <= worldInfo.playerIndex; j++)
            {
                Vector2 playerPos = handy.GetCircularPos(handy.GetPlayerScript(j).curDeg, handy.GetPlayerScript().curRadius, worldInfo.centerPos);
                if (!handy.CheckObjInOtherObj(playerPosDots[i], playerPos, handy.GetPlayerCenter(j).transform.localScale, handy.GetSpritePixels(playerPosDotsRend[i].sprite), handy.GetSpritePixels(handy.GetPlayerCenterRend(j).sprite)))
                {
                    playerPosDots[i].SetActive(true);
                }
            }
        }
    }
}
