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
        for (int i = 0; i <= handy.GetTotalMaxPlayerIndex(); i++)
        {
            if (GameManager.Property.GetIsProperKeyDown(i))
            {
                SetJudgmentText(i);
            }
        }
    }
    public void SetJudgmentText(int playerIndex, JudgmentType? judgmentType = null)
    {
        if (judgmentType != null)
            GameManager.Property.judgmentTypes[playerIndex] = (JudgmentType)judgmentType;
        GameObject newJudgmentText = Instantiate(judgmentTextPrefab, transform);
        JudgmentText newJudgmentTextScript = newJudgmentText.GetComponent<JudgmentText>();
        newJudgmentTextScript.playerIndex = playerIndex;
    }
}
