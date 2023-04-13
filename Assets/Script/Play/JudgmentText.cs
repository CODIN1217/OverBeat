using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using DG.Tweening;

public enum JudgmentType { Perfect, Great, Good, Bad, Miss }
public class JudgmentText : MonoBehaviour
{
    public int playerIndex;
    public JudgmentType judgmentType;
    TextMeshProUGUI judgmentText_TMP;
    RectTransform judgmentText_rect;
    Vector2 stdPlayerPos;
    Vector2 stdPlayerScale;
    Handy handy;
    PlayGameManager playGM;
    Sequence FadeTweener;
    bool isAwake;
    void Awake()
    {
        isAwake = true;
    }
    void Update()
    {
        if (isAwake)
        {
            judgmentText_TMP = GetComponent<TextMeshProUGUI>();
            judgmentText_rect = GetComponent<RectTransform>();
            handy = Handy.Property;
            playGM = PlayGameManager.Property;
            judgmentText_TMP.text = judgmentType.ToString();
            stdPlayerPos = playGM.GetPlayer(playerIndex).transform.position;
            stdPlayerScale = playGM.GetPlayer(playerIndex).transform.localScale;
            FadeTweener = DOTween.Sequence()
            .Append(judgmentText_TMP.DOFade(1f, 0.05f))
            .AppendInterval(0.2f)
            .Append(judgmentText_TMP.DOFade(0f, 0.05f))
            .OnComplete(() => gameObject.SetActive(false));
            judgmentText_rect.localPosition = 100f * (stdPlayerPos + new Vector2(0, 0.61f * handy.GetScaleAbsAverage(stdPlayerScale) + judgmentText_rect.sizeDelta.y * 0.005f));
            isAwake = false;
        }
    }
}
