using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using DG.Tweening;

public enum JudgmentType { Perfect, Good, Bad, Miss }
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
    WorldInfo worldInfo;
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
            worldInfo = playGM.GetWorldInfo(playGM.worldInfoIndex);
            judgmentText_TMP.text = judgmentType.ToString();
            stdPlayerPos = playGM.GetPlayer(playerIndex).transform.position;
            stdPlayerScale = playGM.GetPlayer(playerIndex).transform.localScale;
            judgmentText_TMP.color = worldInfo.judgmentInfo.judgmentColors[(int)judgmentType];
            handy.ChangeAlpha(judgmentText_TMP, 0f);
            FadeTweener = DOTween.Sequence()
            .Append(judgmentText_TMP.DOFade(1f, 0.05f))
            .AppendInterval(0.2f)
            .Append(judgmentText_TMP.DOFade(0f, 0.05f))
            .OnComplete(() => {gameObject.SetActive(false); handy.TryKillTween(FadeTweener);});
            judgmentText_rect.localPosition = stdPlayerPos + new Vector2(0, 0.61f * handy.GetScaleAbsAverage(stdPlayerScale) + judgmentText_rect.sizeDelta.y * 0.005f);
            isAwake = false;
        }
    }
}
