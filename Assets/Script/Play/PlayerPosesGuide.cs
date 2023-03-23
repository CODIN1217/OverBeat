using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerPosesGuide : MonoBehaviour
{
    Handy handy;
    // WorldInfo worldInfo;
    public GameObject playerPosDotPrefab;
    List<GameObject> playerPosDots;
    List<SpriteRenderer> playerPosDotsRend;
    void Awake()
    {
        handy = Handy.Property;
        playerPosDots = new List<GameObject>();
        playerPosDotsRend = new List<SpriteRenderer>();
        for (int i = 0; i <= handy.GetTotalMaxPlayerIndex(); i++)
        {
            for (int j = 0; j < handy.GetMaxStdDegCount(i); j++)
            {
                playerPosDots.Add(Instantiate(playerPosDotPrefab, transform));
                playerPosDotsRend.Add(playerPosDots[j].GetComponent<SpriteRenderer>());
                playerPosDots[j].transform.position = handy.GetWorldInfo(i).CenterInfo.Pos;
                playerPosDots[j].SetActive(false);
            }

        }
    }
    void Update()
    {
        // handy.RepeatCode((i) => worldInfo = handy.GetWorldInfo(i), handy.GetTotalMaxPlayerIndex() + 1);
        // float playerCenterRadius = handy.GetScaleAverage(handy.GetPlayerCenter().transform.localScale.x * handy.GetPlayerCenterRend().sprite.texture.width, handy.GetPlayerCenter().transform.localScale.y * handy.GetPlayerCenterRend().sprite.texture.height) * 0.005f;

        for (int i = 0; i <= handy.GetTotalMaxPlayerIndex(); i++)
        {
            if (handy.GetPlayer(i).activeSelf)
            {
                for (int j = 0; j < handy.GetWorldInfo(i).PlayerInfo.StdDegs.Count; j++)
                {
                    playerPosDots[j].SetActive(false);
                    playerPosDotsRend[j].color = handy.GetColor01(handy.GetWorldInfo(i).PlayerInfo.PosesGuideColor);
                    playerPosDots[j].transform.position = handy.GetCircularPos(handy.GetWorldInfo(i).PlayerInfo.StdDegs[j], /* Mathf.Clamp( */handy.GetPlayerScript(i).curRadius/*  - playerCenterRadius, 0f, handy.GetPlayerScript().curRadius) */, handy.GetWorldInfo(i).CenterInfo.Pos);
                    Vector2 playerPos = handy.GetCircularPos(handy.GetPlayerScript(i).curDeg, handy.GetPlayerScript(i).curRadius, handy.GetWorldInfo(i).CenterInfo.Pos);
                    if (!handy.CheckObjInOtherObj(playerPosDots[j], playerPos, handy.GetPlayerCenter(i).transform.localScale, handy.GetSpritePixels(playerPosDotsRend[j].sprite), handy.GetSpritePixels(handy.GetPlayerCenterRend(i).sprite)))
                    {
                        playerPosDots[j].SetActive(true);
                    }
                }

            }
        }
    }
}
