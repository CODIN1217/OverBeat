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
        if (GameManager.Property.GetIsProperKeyDown())
        {
            SetJudgmentText();
        }
    }
    public void SetJudgmentText(JudgmentType? judgmentType = null)
    {
        if (judgmentType != null)
            GameManager.Property.judgmentType = (JudgmentType)judgmentType;
        GameObject newJudgmentText = Instantiate(judgmentTextPrefab, transform);
    }
}
