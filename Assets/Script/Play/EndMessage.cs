using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using TweenManager;

public class EndMessage : MonoBehaviour, IGameObject
{
    [SerializeField]
    GameObject mainMessage;
    [SerializeField]
    GameObject subMessage;
    CanvasGroup endMessageUIGroup;
    TextMeshProUGUI mainMessage_TMP;
    TextMeshProUGUI subMessage_TMP;
    RectTransform mainMessageRect;
    RectTransform subMessageRect;
    PlayManager PM;

    float alpha;

    TweeningInfo alphaInfo;

    [SerializeField]
    AnimationCurve alphaEase;
    [SerializeField]
    float alphaDuration;
    [SerializeField]
    string[] cheerTexts;
    void Awake()
    {
        PM = PlayManager.Member;

        endMessageUIGroup = GetComponent<CanvasGroup>();
        mainMessage_TMP = mainMessage.GetComponent<TextMeshProUGUI>();
        subMessage_TMP = subMessage.GetComponent<TextMeshProUGUI>();
        mainMessageRect = mainMessage.GetComponent<RectTransform>();
        subMessageRect = subMessage.GetComponent<RectTransform>();
    }
    void LateUpdate()
    {
        UpdateGameOverTweenValue();
    }
    void InitGameOverTween()
    {
        alphaInfo = new TweeningInfo(new TweenInfo<float>(0f, 1f, alphaEase), alphaDuration);
    }
    void UpdateGameOverTweenValue()
    {
        if (!TweenMethod.IsInfosNull(alphaInfo))
            alpha = ((TweenerInfo<float>)alphaInfo).curValue;
    }
    void TryKillGameOverTween()
    {
        TweenMethod.TryKillTweens(alphaInfo);
    }
    void PlayGameOverTween()
    {
        TweenMethod.TryPlayTweens(alphaInfo);
    }
    public void SetEndMessage()
    {
        TryKillGameOverTween();
        InitGameOverTween();
        PlayGameOverTween();

        InitGameOverScript();
    }
    void InitGameOverScript()
    {
        alpha = 0f;
        PM.AddGO(this);
        if (PM.isClearWorld)
        {
            mainMessage_TMP.text = $"Clear \"{PM.worldInfo.creditInfo.levelName}\"!";
            subMessage_TMP.text = PM.accuracy01 >= 1f ? "Best Accuracy!" : $"Accuracy : {PM.accuracy01.ToString("0%")}";
        }
        else if (PM.isGameOver)
        {
            mainMessage_TMP.text = $"YOU REACH {PM.progress01.ToString("0%")}";
            if (cheerTexts.Length > 0)
            {
                subMessage_TMP.text = cheerTexts[Random.Range(0, cheerTexts.Length)];
            }
        }
    }
    public void UpdateTransform()
    {
        mainMessageRect.localPosition = new Vector2(0f, mainMessageRect.sizeDelta.y * 0.5f);
        subMessageRect.localPosition = new Vector2(0f, subMessageRect.sizeDelta.y * -0.5f);
    }
    public void UpdateRenderer()
    {
        endMessageUIGroup.alpha = alpha;
    }
}