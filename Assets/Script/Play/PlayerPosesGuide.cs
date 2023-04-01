using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerPosesGuide : MonoBehaviour
{
    Handy handy;
    // WorldInfo worldInfo;
    public GameObject playerPosDotPrefab;
    List<List<GameObject>> playerPosDots;
    List<List<SpriteRenderer>> playerPosDotsRend;
    GameManager GM;
    void Awake()
    {
        GM = GameManager.Property;
        handy = Handy.Property;
        playerPosDots = new List<List<GameObject>>();
        playerPosDotsRend = new List<List<SpriteRenderer>>();
        for (int i = 0; i < handy.GetPlayerCount(); i++)
        {
            playerPosDots.Add(new List<GameObject>());
            playerPosDotsRend.Add(new List<SpriteRenderer>());
            for (int j = 0; j < handy.GetMaxStdDegCount(i); j++)
            {
                playerPosDots[i].Add(Instantiate(playerPosDotPrefab, transform));
                playerPosDotsRend[i].Add(playerPosDots[i][j].GetComponent<SpriteRenderer>());
                playerPosDots[i][j].transform.position = handy.GetWorldInfo(GM.curWorldInfoIndex).centerInfo.pos;
                playerPosDots[i][j].SetActive(false);
            }

        }
    }
    void Update()
    {
        // handy.RepeatCode((i) => worldInfo = handy.GetWorldInfo(i), handy.GetTotalMaxPlayerIndex() + 1);
        // float playerCenterRadius = handy.GetScaleAverage(handy.GetPlayerCenter().transform.localScale.x * handy.GetPlayerCenterRend().sprite.texture.width, handy.GetPlayerCenter().transform.localScale.y * handy.GetPlayerCenterRend().sprite.texture.height) * 0.005f;

        for (int i = 0; i < handy.GetPlayerCount(); i++)
        {
            if (handy.GetPlayer(i).activeSelf)
            {
                for (int j = 0; j < handy.GetWorldInfo(GM.curWorldInfoIndex).playerInfo[i].stdDegs.Count; j++)
                {
                    playerPosDots[i][j].SetActive(false);
                    playerPosDotsRend[i][j].color = handy.GetColor01(handy.GetWorldInfo(GM.curWorldInfoIndex).playerInfo[i].posesGuideColor);
                    playerPosDots[i][j].transform.position = handy.GetCircularPos(handy.GetWorldInfo(GM.curWorldInfoIndex).playerInfo[i].stdDegs[j], /* Mathf.Clamp( */handy.GetPlayerScript(i).curRadius/*  - playerCenterRadius, 0f, handy.GetPlayerScript().curRadius) */, handy.GetWorldInfo(GM.curWorldInfoIndex).centerInfo.pos);
                    Vector2 playerPos = handy.GetCircularPos(handy.GetPlayerScript(i).curDeg, handy.GetPlayerScript(i).curRadius, handy.GetWorldInfo(GM.curWorldInfoIndex).centerInfo.pos);
                    if (!handy.CheckObjInOtherObj(playerPosDots[i][j], playerPos, handy.GetPlayerCenter(i).transform.localScale, handy.GetSpritePixels(playerPosDotsRend[i][j].sprite), handy.GetSpritePixels(handy.GetPlayerCenterRend(i).sprite)))
                    {
                        playerPosDots[i][j].SetActive(true);
                    }
                }

            }
        }
    }
}
