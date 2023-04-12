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
        for (int i = 0; i < handy.GetMaxPlayerCount(); i++)
        {
            playerPosDots.Add(new List<GameObject>());
            playerPosDotsRend.Add(new List<SpriteRenderer>());
            for (int j = 0; j < handy.GetMaxStdDegCount(i); j++)
            {
                playerPosDots[i].Add(Instantiate(playerPosDotPrefab, transform));
                playerPosDotsRend[i].Add(playerPosDots[i][j].GetComponent<SpriteRenderer>());
                playerPosDotsRend[i][j].sortingOrder = handy.GetMaxPlayerIndex() - i;
                playerPosDots[i][j].transform.position = handy.GetWorldInfo(GM.curWorldInfoIndex).centerInfo.pos;
                playerPosDots[i][j].SetActive(false);
            }

        }
    }
    void Update()
    {
        // handy.RepeatCode((i) => worldInfo = handy.GetWorldInfo(i), handy.GetTotalMaxPlayerIndex() + 1);
        // float playerCenterRadius = handy.GetScaleAverage(handy.GetPlayerCenter().transform.localScale.x * handy.GetPlayerCenterRend().sprite.texture.width, handy.GetPlayerCenter().transform.localScale.y * handy.GetPlayerCenterRend().sprite.texture.height) * 0.005f;

        for (int i = 0; i < handy.GetMaxPlayerCount(); i++)
        {
            for (int j = 0; j < handy.GetWorldInfo(GM.curWorldInfoIndex).playerInfo[i].stdDegs.Length; j++)
            {
                playerPosDots[i][j].SetActive(false);
                if (handy.GetPlayer(i).activeSelf)
                {
                    playerPosDots[i][j].SetActive(true);
                    playerPosDotsRend[i][j].color = handy.GetColor01WithPlayerIndex(handy.GetColor01(handy.GetWorldInfo(GM.curWorldInfoIndex).playerInfo[i].posesGuideColor), i);
                    playerPosDots[i][j].transform.position = handy.GetCircularPos(handy.GetWorldInfo(GM.curWorldInfoIndex).playerInfo[i].stdDegs[j], handy.GetPlayerScript(i).curRadius, handy.GetWorldInfo(GM.curWorldInfoIndex).centerInfo.pos);
                    /* Vector2 playerPos = handy.GetCircularPos(handy.GetPlayerScript(i).curDeg, handy.GetPlayerScript(i).curRadius, handy.GetWorldInfo(GM.curWorldInfoIndex).centerInfo.pos);
                    if (!handy.CheckObjInOtherObj(playerPosDots[i][j], playerPos, handy.GetPlayerCenter(i).transform.localScale, handy.GetSpritePixels(playerPosDotsRend[i][j].sprite), handy.GetSpritePixels(handy.GetPlayerCenterRend(i).sprite)))
                    {
                    } */
                }
            }
        }
    }
}
