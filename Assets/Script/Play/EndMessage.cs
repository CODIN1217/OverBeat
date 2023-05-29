using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using TweenManager;
using System.Threading.Tasks;

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
        alphaInfo = new TweeningInfo(new TweenInfo<float>(0f, 1f, AnimationCurve.Linear(0f, 0f, 1f, 1f)), 0.5f);
    }
    void UpdateGameOverTweenValue()
    {
        if (!TweenMethod.IsInfoNull(alphaInfo))
            alpha = ((TweenerInfo<float>)alphaInfo).curValue;
    }
    void TryKillGameOverTween()
    {
        TweenMethod.TryKillTween(alphaInfo);
    }
    void PlayGameOverTween()
    {
        TweenMethod.TryPlayTween(alphaInfo);
    }
    public void SetEndMessage()
    {
        TryKillGameOverTween();
        InitGameOverTween();
        PlayGameOverTween();

        PM.StartCoroutine(InitGameOverScript());
    }
    IEnumerator InitGameOverScript()
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
        yield return new WaitUntil(() => !PM.isClearWorld && !PM.isGameOver);
        TryKillGameOverTween();
        alpha = 0f;
        mainMessage_TMP.text = "";
        subMessage_TMP.text = "";
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