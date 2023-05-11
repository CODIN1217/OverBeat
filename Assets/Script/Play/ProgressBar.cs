using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ProgressBar : MonoBehaviour
{
    public GameObject PBBGImage;
    public GameObject PBVertex0;
    public GameObject PBVertex1;
    public Image PBBGImageRend;
    public Image PBVertexRend0;
    public Image PBVertexRend1;
    public RectTransform PBBGImageRect;
    Handy handy;
    PlayManager PM;
    void Awake() {
        handy = Handy.Property;
        PM = PlayManager.Property;
    }
    void Update()
    {
        Vector2 PBBGImagePixels = handy.GetSpritePixels(PBBGImageRend.sprite);
        Vector2 PBBGVertex0Pixels = handy.GetSpritePixels(PBVertexRend0.sprite);
        Vector2 PBBGVertex1Pixels = handy.GetSpritePixels(PBVertexRend1.sprite);
        PBBGImageRect.sizeDelta = new Vector2(PBBGImagePixels.x * PM.progress01, PBBGImagePixels.y);
        PBBGImageRect.localPosition = new Vector2((PBBGImageRect.sizeDelta.x - PBBGImagePixels.x) * 0.5f, 0f);
        PBVertexRend0.transform.localPosition = new Vector2((PBBGVertex0Pixels.x - PBBGImagePixels.x) * 0.5f, 0f);
        PBVertexRend1.transform.localPosition = new Vector2((PBBGImagePixels.x - PBBGVertex1Pixels.x) * 0.5f, 0f);
        PBBGImageRend.color = PM.GetPlayerScript(0).centerColor;
        PBVertexRend0.color = PM.GetPlayerScript(0).sideColor;
        PBVertexRend1.color = PM.GetPlayerScript(0).sideColor;
    }
}
