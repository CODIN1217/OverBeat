using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TweenManager;

public class Center : MonoBehaviour, ITweener, PlayManager.ITweenerInPlay, IGameObject
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
        PM.AddGO(this).AddTweenerGO(this).AddTweenerInPlayGO(this);
    }
    void Update()
    {
        centerImage.fillAmount = Mathf.Lerp(centerImage.fillAmount, PM.HP01, Time.deltaTime * 4f);
    }
    public void InitTween()
    {
        worldInfo = PM.GetWorldInfo(PM.worldInfoIndex);

        TweenMethod.TryKillTween(scaleInfo);
        scaleInfo = new TweeningInfo(worldInfo.centerInfo.scaleTween, PM.GetHoldNoteSecs(PM.worldInfoIndex));

        TweenMethod.TryKillTween(posInfo);
        posInfo = new TweeningInfo(worldInfo.centerInfo.posTween, PM.GetHoldNoteSecs(PM.worldInfoIndex));

        TweenMethod.TryKillTween(colorInfo);
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
        TweenMethod.PlayTweens(scaleInfo, posInfo, colorInfo);
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
