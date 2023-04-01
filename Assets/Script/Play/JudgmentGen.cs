using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JudgmentGen : MonoBehaviour
{
    public GameObject judgmentTextPrefab;
    Handy handy;
    void Awake()
    {
        handy = Handy.Property;
    }
    void Update()
    {
        /* for (int i = 0; i < handy.GetPlayerCount(); i++)
        {
            if (GameManager.Property.GetIsProperKeyDown(i))
            {
                SetJudgmentText(i, GameManager.Property.judgmentTypes[i]);
            }
        } */
    }
    public void SetJudgmentText(int playerIndex, JudgmentType judgmentType)
    {
        /* if (GameManager.Property.judgmentTypes != null)
        {
            // if (GameManager.Property.judgmentTypes.Length > playerIndex)
                GameManager.Property.judgmentTypes[playerIndex] = (JudgmentType)judgmentType;
            // else
            //     GameManager.Property.judgmentTypes.Insert(playerIndex, (JudgmentType)judgmentType);
        } */
        // else
        // {
            // GameManager.Property.judgmentTypes = new JudgmentType[handy.GetTotalMaxPlayerIndex() + 1];
            // GameManager.Property.judgmentTypes.Insert(playerIndex, (JudgmentType)judgmentType);
                // GameManager.Property.judgmentTypes[playerIndex] = (JudgmentType)judgmentType;
        // }
        GameObject newJudgmentText = Instantiate(judgmentTextPrefab, transform);
        JudgmentText newJudgmentTextScript = newJudgmentText.GetComponent<JudgmentText>();
        newJudgmentTextScript.playerIndex = playerIndex;
        newJudgmentTextScript.judgmentType = judgmentType;
    }
}
