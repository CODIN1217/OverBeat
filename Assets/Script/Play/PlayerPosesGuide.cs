using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class PlayerPosesGuide : MonoBehaviour
{
    Handy handy;
    public GameObject playerPosDotPrefab;
    List<GameObject> playerPosDots;
    List<SpriteRenderer> playerPosDotsRend;
    PlayGameManager playGM;
    void Awake()
    {
        playGM = PlayGameManager.Property;
        handy = Handy.Property;
        playerPosDots = new List<GameObject>();
        playerPosDotsRend = new List<SpriteRenderer>();
        for (int i = 0; i < playGM.GetMaxPlayerCount(); i++)
        {
            playerPosDots.Add(Instantiate(playerPosDotPrefab, transform));
            playerPosDotsRend.Add(playerPosDots[i].GetComponent<SpriteRenderer>());
            playerPosDotsRend[i].sortingOrder = playGM.GetMaxPlayerIndex() - i;
            playerPosDots[i].SetActive(false);
        }
    }
    void Update()
    {
        for (int i = 0; i < playGM.GetMaxPlayerCount(); i++)
        {
            WorldInfo curWorldInfo = playGM.GetWorldInfo(i, playGM.closestNoteIndex[i]);
            playerPosDots[i].transform.position = handy.GetCircularPos(playGM.closestNoteScripts[i].curDeg, playGM.GetPlayerScript(i).curRadius, playGM.centerScript.pos);
            playerPosDots[i].SetActive(true);
        }
    }
}
