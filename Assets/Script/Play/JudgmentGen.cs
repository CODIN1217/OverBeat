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
        // if (GameManager.Property.isPause)
        //     return;
        /* if (GameManager.Property.numberOfInputKeys > 0)
        {
            SetJudgmentText();
        } */
        if (GameManager.Property.GetIsProperKeyDown())
        {
            SetJudgmentText();
        }
        // GameManager.Property.OncePlayCodeWhenInput(() => SetJudgmentText());
    }
    public void SetJudgmentText(JudgmentType? judgmentType = null)
    {
        if (judgmentType != null)
            GameManager.Property.judgmentType = (JudgmentType)judgmentType;
        GameObject newJudgmentText = Instantiate(judgmentTextPrefab, transform);
        // handy.WriteLog(GameManager.Property.judgmentType, GameManager.Property.elapsedTimeWhenNeedlessInput01, GameManager.Property.elapsedTimeWhenNeedInput01, handy.closestNoteScript.noteLengthTime);
    }
}
