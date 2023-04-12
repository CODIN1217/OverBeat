using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ProgressBar : MonoBehaviour
{
    public GameObject ProgressBarBGImage;
    public GameObject ProgressBarBGVertex;
    public GameObject ProgressBarVertex0;
    public GameObject ProgressBarVertex1;
    public Image ProgressBarBGImageRend;
    public Image ProgressBarBGVertexRend;
    public Image ProgressBarVertexRend0;
    public Image ProgressBarVertexRend1;
    Handy handy;
    GameManager GM;
    void Awake() {
        handy = Handy.Property;
        GM = GameManager.Property;
    }
    void Update()
    {
        Vector2 ProgressBarBGImagePixels = handy.GetSpritePixels(ProgressBarBGImageRend.sprite);
        Vector2 ProgressBarBGVertexPixels = handy.GetSpritePixels(ProgressBarBGVertexRend.sprite);
        float halfRatio = ProgressBarBGVertexPixels.x / ProgressBarBGImagePixels.x * 0.5f;
        ProgressBarBGImageRend.fillAmount = GM.progress01 + halfRatio;
        ProgressBarBGVertex.transform.localPosition = new Vector2(ProgressBarBGImagePixels.x * (GM.progress01 - 0.5f) + (GM.progress01 + halfRatio >= 1f ? 1f - GM.progress01 - halfRatio : ProgressBarBGVertexPixels.x * 0.5f), 0f);
        ProgressBarVertexRend0.transform.localPosition = new Vector2((ProgressBarBGVertexPixels.x - ProgressBarBGImagePixels.x) * 0.5f, 0f);
        ProgressBarVertexRend1.transform.localPosition = new Vector2((ProgressBarBGImagePixels.x - ProgressBarBGVertexPixels.x) * 0.5f, 0f);
    }
}
