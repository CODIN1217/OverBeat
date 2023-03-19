using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using DG.Tweening;

public enum JudgmentType { Perfect, Great, Good, Bad, Miss }
public class JudgmentText : MonoBehaviour
{
    JudgmentType judgmentType;
    TextMeshProUGUI judgmentText_TMP;
    Vector2 stdPlayerPos;
    Vector2 stdPlayerScale;
    Handy handy;
    Sequence FadeTweener;
    bool isAwake;
    void Awake()
    {
        judgmentText_TMP = GetComponent<TextMeshProUGUI>();
        handy = Handy.Property;
        judgmentType = GameManager.Property.judgmentType;
        judgmentText_TMP.text = judgmentType.ToString();
        stdPlayerPos = handy.GetPlayer().transform.position;
        stdPlayerScale = handy.GetPlayer().transform.localScale;
        FadeTweener = DOTween.Sequence()
        .Append(judgmentText_TMP.DOFade(1f, 0.05f))
        .AppendInterval(0.2f)
        .Append(judgmentText_TMP.DOFade(0f, 0.05f))
        .OnComplete(() => gameObject.SetActive(false))
        .SetUpdate(true);
        transform.position = stdPlayerPos + new Vector2(0, 0.61f * handy.GetScaleAverage(stdPlayerScale) + GetComponent<RectTransform>().sizeDelta.y * 0.5f);
    }
}
