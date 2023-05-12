using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TweenManager;

public class Center : MonoBehaviour, ITweenerInfo, IGameObject
{
    Image centerImage;
    WorldInfo worldInfo;
    Handy handy;
    PlayManager PM;

    public Vector2 scale;
    public Vector2 pos;
    public Color color;

    public TweeningInfo scaleInfo;
    public TweeningInfo posInfo;
    public TweeningInfo colorInfo;
    void Awake()
    {
        PM = PlayManager.Property;
        handy = Handy.Property;
        centerImage = GetComponent<Image>();
        PM.initTweenEvent += InitTween;
        PM.playHoldTweenEvent += PlayHoldTween;
    }
    void Update()
    {
        UpdateTweenValue();
        if (PM.isPause)
            return;
        centerImage.fillAmount = Mathf.Lerp(centerImage.fillAmount, PM.HP01, Time.deltaTime * 4f);
    }
    void LateUpdate()
    {
        if (PM.worldInfoIndex == 0)
            return;
        UpdateTransform();
        UpdateRenderer();
    }
    public void InitTween()
    {
        worldInfo = PM.GetWorldInfo(PM.worldInfoIndex);

        handy.TryKillTween(scaleInfo);
        scaleInfo = new TweeningInfo(worldInfo.centerInfo.scaleTween, PM.GetHoldNoteSecs(PM.worldInfoIndex));

        handy.TryKillTween(posInfo);
        posInfo = new TweeningInfo(worldInfo.centerInfo.posTween, PM.GetHoldNoteSecs(PM.worldInfoIndex));

        handy.TryKillTween(colorInfo);
        colorInfo = new TweeningInfo(worldInfo.centerInfo.colorTween, PM.GetHoldNoteSecs(PM.worldInfoIndex));
    }
    public void UpdateTweenValue()
    {
        scale = ((TweenerInfo<Vector2>)scaleInfo).curValue;
        pos = ((TweenerInfo<Vector2>)posInfo).curValue;
        color = ((TweenerInfo<Color>)colorInfo).curValue;
    }
    public void PlayWaitTween() { }
    public void PlayHoldTween()
    {
        handy.PlayTweens(scaleInfo, posInfo, colorInfo);
    }
    public void UpdateTransform()
    {
        transform.localScale = scale;
        transform.localPosition = pos;
    }
    public void UpdateRenderer()
    {
        centerImage.color = color;
    }
}
