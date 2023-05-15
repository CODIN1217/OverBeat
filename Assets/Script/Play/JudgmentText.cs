using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using DG.Tweening;
using TweenManager;

public enum JudgmentType { Perfect, Good, Bad, Miss }
public class JudgmentText : MonoBehaviour
{
    float fade;
    public int playerIndex;
    public JudgmentType judgmentType;
    TextMeshProUGUI judgmentText_TMP;
    RectTransform judgmentText_rect;
    Vector2 stdPlayerPos;
    Vector2 stdPlayerScale;
    PlayManager PM;
    TweeningInfo fadeInfo;
    WorldInfo worldInfo;
    bool isAwake;
    void Awake()
    {
        PM = PlayManager.Property;
        isAwake = true;
    }
    void Update()
    {
        if (PM.isPause)
            return;
        if (isAwake)
        {
            judgmentText_TMP = GetComponent<TextMeshProUGUI>();
            judgmentText_rect = GetComponent<RectTransform>();
            PM = PlayManager.Property;
            worldInfo = PM.GetWorldInfo(PM.worldInfoIndex);
            judgmentText_TMP.text = judgmentType.ToString();
            stdPlayerPos = (Vector2)PM.GetPlayer(playerIndex).transform.position * 100f;
            stdPlayerScale = PM.GetPlayer(playerIndex).transform.localScale;
            judgmentText_TMP.color = worldInfo.judgmentInfo.judgmentColors[(int)judgmentType];
            Handy.Renderer.ColorMethod.FadeColor(judgmentText_TMP, 0f);
            fadeInfo = new TweeningInfo(new TweenInfo<float>(0f, 1f, AnimationCurve.Linear(0f, 0f, 1f, 1f)), 0.1f)
            .AppendInterval(0.1f)
            .Append(new TweeningInfo(new TweenInfo<float>(1f, 0f, AnimationCurve.Linear(0f, 0f, 1f, 1f)), 0.1f))
            .OnComplete(() => {gameObject.SetActive(false); TweenMethod.TryKillTween(fadeInfo);})
            .Play();
            judgmentText_rect.localPosition = stdPlayerPos + new Vector2(0, 0.61f * Handy.Transform.ScaleMethod.GetScaleAbsAverage(stdPlayerScale) + judgmentText_rect.sizeDelta.y * 0.005f) * 100f;
            isAwake = false;
        }
        fade = ((TweenerInfo<float>)fadeInfo).curValue;
        Handy.Renderer.ColorMethod.FadeColor(judgmentText_TMP, fade);
    }
}
