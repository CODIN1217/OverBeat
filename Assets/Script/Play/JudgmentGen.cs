using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JudgmentGen : MonoBehaviour
{
    public GameObject judgmentTextPrefab;
    PlayManager PM;
    void Awake()
    {
        PM = PlayManager.Property;
    }
    public void SetJudgmentText(int playerIndex, JudgmentType judgmentType)
    {
        GameObject newJudgmentText = Instantiate(judgmentTextPrefab, transform);
        JudgmentText newJudgmentTextScript = newJudgmentText.GetComponent<JudgmentText>();
        newJudgmentTextScript.playerIndex = playerIndex;
        newJudgmentTextScript.judgmentType = judgmentType;
    }
}
