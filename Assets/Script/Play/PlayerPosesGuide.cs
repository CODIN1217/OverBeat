using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class PlayerPosesGuide : MonoBehaviour
{
    Handy handy;
    public GameObject playerPosDotPrefab;
    List<List<GameObject>> playerPosDots;
    List<List<SpriteRenderer>> playerPosDotsRend;
    PlayGameManager playGM;
    void Awake()
    {
        playGM = PlayGameManager.Property;
        handy = Handy.Property;
        playerPosDots = new List<List<GameObject>>();
        playerPosDotsRend = new List<List<SpriteRenderer>>();
        for (int i = 0; i < playGM.GetMaxPlayerCount(); i++)
        {
            playerPosDots.Add(new List<GameObject>());
            playerPosDotsRend.Add(new List<SpriteRenderer>());
            for (int j = 0; j < playGM.GetMaxStdDegCount(i); j++)
            {
                playerPosDots[i].Add(Instantiate(playerPosDotPrefab, transform));
                playerPosDotsRend[i].Add(playerPosDots[i][j].GetComponent<SpriteRenderer>());
                playerPosDotsRend[i][j].sortingOrder = playGM.GetMaxPlayerIndex() - i;
                playerPosDots[i][j].transform.position = playGM.GetWorldInfo(playGM.curWorldInfoIndex).centerInfo.pos;
                playerPosDots[i][j].SetActive(false);
            }

        }
    }
    void Update()
    {
        if (!handy.CompareWithBeforeValue(this.name, nameof(Update), nameof(playGM.curWorldInfoIndex), playGM.curWorldInfoIndex))
        {
            // handy.WriteLog(playGM.curWorldInfoIndex);
            for (int i = 0; i < playGM.GetMaxPlayerCount(); i++)
            {
                for (int j = 0; j < playGM.GetWorldInfo(playGM.curWorldInfoIndex).playerInfo[i].stdDegs.Length; j++)
                {
                    playerPosDots[i][j].SetActive(false);
                    if (playGM.GetPlayer(i).activeSelf)
                    {
                        playerPosDots[i][j].SetActive(true);
                        playerPosDotsRend[i][j].DOColor(playGM.GetColor01WithPlayerIndex(handy.GetColor01(playGM.GetWorldInfo(playGM.curWorldInfoIndex).playerInfo[i].posesGuideColor), i), playGM.GetWorldInfo(playGM.curWorldInfoIndex).playerInfo[i].posesGuideColorTween.duration)
                        .SetEase(playGM.GetWorldInfo(playGM.curWorldInfoIndex).playerInfo[i].posesGuideColorTween.ease);
                        // playerPosDotsRend[i][j].color = playGM.GetColor01WithPlayerIndex(handy.GetColor01(playGM.GetWorldInfo(playGM.curWorldInfoIndex).playerInfo[i].posesGuideColor), i);
                        playerPosDots[i][j].transform.position = handy.GetCircularPos(playGM.GetWorldInfo(playGM.curWorldInfoIndex).playerInfo[i].stdDegs[j], playGM.GetPlayerScript(i).curRadius, playGM.GetWorldInfo(playGM.curWorldInfoIndex).centerInfo.pos);
                    }
                }
            }
            handy.SetValueForCompare(this.name, nameof(Update), nameof(playGM.curWorldInfoIndex), playGM.curWorldInfoIndex);
        }
        /* for (int i = 0; i < playGM.GetMaxPlayerCount(); i++)
        {
            for (int j = 0; j < playGM.GetWorldInfo(playGM.curWorldInfoIndex).playerInfo[i].stdDegs.Length; j++)
            {
                playerPosDots[i][j].SetActive(false);
                if (playGM.GetPlayer(i).activeSelf)
                {
                    playerPosDots[i][j].SetActive(true);
                    playerPosDotsRend[i][j].color = playGM.GetColor01WithPlayerIndex(handy.GetColor01(playGM.GetWorldInfo(playGM.curWorldInfoIndex).playerInfo[i].posesGuideColor), i);
                    playerPosDots[i][j].transform.position = handy.GetCircularPos(playGM.GetWorldInfo(playGM.curWorldInfoIndex).playerInfo[i].stdDegs[j], playGM.GetPlayerScript(i).curRadius, playGM.GetWorldInfo(playGM.curWorldInfoIndex).centerInfo.pos);
                }
            }
        } */
    }
}
