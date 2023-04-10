using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JudgmentGen : MonoBehaviour
{
    public GameObject judgmentTextPrefab;
    Handy handy;
    GameManager GM;
    void Awake()
    {
        GM = GameManager.Property;
        handy = Handy.Property;
    }
    void Update()
    {
        /* for (int i = 0; i < handy.GetPlayerCount(); i++)
        {
            if (GM.GetIsProperKeyDown(i))
            {
                SetJudgmentText(i, GM.judgmentTypes[i]);
            }
        } */
    }
    public void SetJudgmentText(int playerIndex, JudgmentType judgmentType)
    {
        /* if (GM.judgmentTypes != null)
        {
            // if (GM.judgmentTypes.Length > playerIndex)
                GM.judgmentTypes[playerIndex] = (JudgmentType)judgmentType;
            // else
            //     GM.judgmentTypes.Insert(playerIndex, (JudgmentType)judgmentType);
        } */
        // else
        // {
        // GM.judgmentTypes = new JudgmentType[handy.GetTotalMaxPlayerIndex() + 1];
        // GM.judgmentTypes.Insert(playerIndex, (JudgmentType)judgmentType);
        // GM.judgmentTypes[playerIndex] = (JudgmentType)judgmentType;
        // }
        GameObject newJudgmentText = Instantiate(judgmentTextPrefab, transform);
        JudgmentText newJudgmentTextScript = newJudgmentText.GetComponent<JudgmentText>();
        newJudgmentTextScript.playerIndex = playerIndex;
        newJudgmentTextScript.judgmentType = judgmentType;
    }
}
