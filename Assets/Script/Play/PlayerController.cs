using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public GameObject playerPrefab;
    public List<GameObject> players;
    public List<Player> playerScripts;
    public List<GameObject> playerSides;
    public List<SpriteRenderer> playerSideRends;
    public List<GameObject> playerCenters;
    public List<SpriteRenderer> playerCenterRends;
    public int maxPlayerIndex;
    PlayManager PM;
    void Awake() {
        PM = PlayManager.Member;
        players = new List<GameObject>();
        playerScripts = new List<Player>();
        playerSides = new List<GameObject>();
        playerSideRends = new List<SpriteRenderer>();
        playerCenters = new List<GameObject>();
        playerCenterRends = new List<SpriteRenderer>();
        for(int i = 0; i < PM.GetMaxPlayerCount(); i++){
            players.Add(Instantiate(playerPrefab, transform));
            playerScripts.Add(players[i].GetComponent<Player>());
            playerSides.Add(playerScripts[i].playerSide);
            playerSideRends.Add(playerSides[i].GetComponent<SpriteRenderer>());
            playerCenters.Add(playerScripts[i].playerCenter);
            playerCenterRends.Add(playerCenters[i].GetComponent<SpriteRenderer>());
            playerSideRends[i].sortingOrder =  (PM.GetMaxPlayerIndex() - i) * 2 + 1 + PM.GetMaxPlayerCount();
            playerCenterRends[i].sortingOrder = (PM.GetMaxPlayerIndex() - i) * 2 + PM.GetMaxPlayerCount();
            playerScripts[i].playerIndex = i;
        }
    }
}
