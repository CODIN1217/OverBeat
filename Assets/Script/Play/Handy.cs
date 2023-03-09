using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using TMPro;
using System.Text;

public class Handy : MonoBehaviour
{
    public GameObject countDown;
    public CountDown countDownScript;
    public GameObject noteGenerator;
    public NoteGenerator noteGeneratorScript;
    public GameObject worldReader;
    public WorldReader worldReaderScript;
    public GameObject player;
    public Player playerScript;
    public GameObject playerSide;
    public SpriteRenderer playerSideRenderer;
    public GameObject playerCenter;
    public SpriteRenderer playerCenterRenderer;
    public GameObject judgmentGen;
    public JudgmentGen judgmentGenScript;
    public GameObject boundary;
    public Boundary boundaryScript;
    public GameObject baseCamera;
    public BaseCamera baseCameraScript;
    public GameObject closestNote;
    public NotePrefab closestNoteScript;
    public int noteIndex;
    public float judgmentRange;
    public float bestJudgmentRange;
    public readonly float minJudgmentRange;
    public readonly float maxJudgmentRange;
    int stdDegCount;
    int stdDegCount_temp;
    List<float> stdDegs;
    List<float> stdDegs_temp;
    // bool needInput_handy;
    // public bool needInput_temp;
    private static Handy instance = null;
    void Awake()
    {
        instance = this;
        stdDegs = new List<float>();
        stdDegs_temp = new List<float>();
    }
    void Update()
    {
        closestNote = noteGeneratorScript.closestNote;
        closestNoteScript = noteGeneratorScript.closestNoteScript;
        noteIndex = closestNoteScript.myNoteIndex;
        judgmentRange = Mathf.Clamp(GetWorldInfo(noteIndex).judgmentRange, minJudgmentRange, maxJudgmentRange);
        bestJudgmentRange = judgmentRange * 0.2f;
    }
    void LateUpdate()
    {
        // needInput_handy = needInput_temp;
        stdDegCount = stdDegCount_temp;
        stdDegs = stdDegs_temp;
    }
    public static Handy Property
    {
        get
        {
            return instance;
        }
    }
    public float GetCorrectDegMaxIs0(float deg)
    {
        if (deg < 0f || deg >= 360f)
        {
            deg -= Mathf.Sign(deg) * 360f * Mathf.Clamp(Mathf.Abs(Mathf.Floor(deg / 360f)), 1f, float.MaxValue);
        }
        return deg;
    }
    public float GetCorrectDegMaxIs360(float deg)
    {
        if (deg <= 0f || deg > 360f)
        {
            deg -= (deg == 0 ? -1f : Mathf.Sign(deg)) * 360f * Mathf.Clamp(Mathf.Abs(Mathf.Floor(deg / 360f)), 1f, float.MaxValue);
        }
        return deg;
    }
    public WorldInfo GetWorldInfo(int? index = null)
    {
        if (index == null)
            index = noteIndex;
        return worldReaderScript.worldInfos[Mathf.Clamp((int)index, 0, worldReaderScript.notesCount)];
    }
    public float GetDistanceDeg(float tarDeg, float curDeg, bool maxIs360)
    {
        float distanceDeg = (tarDeg - curDeg) * (float)GetWorldInfo(noteIndex).direction;
        return maxIs360 ? GetCorrectDegMaxIs360(distanceDeg) : GetCorrectDegMaxIs0(distanceDeg);
    }
    public Vector2 GetCircularPos(float deg, float radius, Vector2? centerPos = null)
    {
        if (centerPos == null)
            centerPos = Vector2.zero;
        float rad = deg * Mathf.Deg2Rad;
        return new Vector2(Mathf.Sin(rad), Mathf.Cos(rad)) * radius + (Vector2)centerPos;
    }
    public float GetNextDeg(int? index = null)
    {
        if (index == null)
            index = noteIndex;
        // return GetWorldInfo(noteIndex).nextDeg;
        return GetWorldInfo(index).stdDegs[GetWorldInfo(index).nextDegIndex];
    }
    public float GetNextDeg(WorldInfo worldInfo)
    {
        return worldInfo.stdDegs[worldInfo.nextDegIndex];
    }
    public float GetBeforeDeg(int? index = null)
    {
        if (index == null)
            index = noteIndex;
        // return GetWorldInfo(noteIndex - 1).nextDeg;
        return GetWorldInfo(index - 1).stdDegs[GetWorldInfo(index - 1).nextDegIndex];
    }
    public float GetNoteWaitTime(int? index = null)
    {
        if (index == null)
            index = noteIndex;
        return noteGeneratorScript.noteWaitTimes[Mathf.Clamp((int)index, 0, worldReaderScript.notesCount)];
    }
    public float GetNoteLengthTime(int? index = null)
    {
        if (index == null)
            index = noteIndex;
        return noteGeneratorScript.noteLengthTimes[Mathf.Clamp((int)index, 0, worldReaderScript.notesCount)];
    }
    public GameObject GetNote(int? index = null)
    {
        if (index == null)
            index = noteIndex;
        return noteGeneratorScript.notes[Mathf.Clamp((int)index, 0, worldReaderScript.notesCount)];
    }
    public NotePrefab GetNoteScripts(int? index = null)
    {
        if (index == null)
            index = noteIndex;
        return noteGeneratorScript.noteScripts[Mathf.Clamp((int)index, 0, worldReaderScript.notesCount)];
    }
    public void WriteLog(params object[] contents)
    {
        StringBuilder text = new StringBuilder();
        foreach (var cont in contents)
            text.Append(cont.ToString() + "    ");
        Debug.Log(text);
    }
    public void ChangeAlpha(Renderer renderer, float alpha)
    {
        renderer.material.color = new Color(renderer.material.color.r, renderer.material.color.g, renderer.material.color.b, alpha);
    }
    public void ChangeAlpha(LineRenderer renderer, float alpha)
    {
        renderer.startColor = new Color(renderer.startColor.r, renderer.startColor.g, renderer.startColor.b, alpha);
        renderer.endColor = new Color(renderer.endColor.r, renderer.endColor.g, renderer.endColor.b, alpha);
    }
    public float GetScaleAverage(GameObject go)
    {
        return (Mathf.Abs(go.transform.localScale.x) + Mathf.Abs(go.transform.localScale.y)) * 0.5f;
    }
    public float GetScaleAverage(Vector2 scale)
    {
        return (Mathf.Abs(scale.x) + Mathf.Abs(scale.y)) * 0.5f;
    }
    public float GetBiggerAbsScale(GameObject go)
    {
        return Mathf.Abs(go.transform.localScale.x) > Mathf.Abs(go.transform.localScale.y) ? go.transform.localScale.x : go.transform.localScale.y;
    }
    public float GetBiggerAbsScale(Vector2 scale)
    {
        return Mathf.Abs(scale.x) > Mathf.Abs(scale.y) ? scale.x : scale.y;
    }
    public float GetJudgmentValue()
    {
        float judgmentValue = 1f;
        if (GameManager.Property.elapsedTimeWhenNeedInput01 > 0f)
            judgmentValue = GameManager.Property.elapsedTimeWhenNeedInput01;
        else if (GameManager.Property.elapsedTimeWhenNeedlessInput01 >= 1f - judgmentRange)
            judgmentValue = 1f - GameManager.Property.elapsedTimeWhenNeedlessInput01;
        return judgmentValue;
    }
    public void WaitCodeUntilUpdateEnd(Action PlayCode)
    {
        StartCoroutine(WaitCodeUntilUpdateEnd_Co(PlayCode));
    }
    IEnumerator WaitCodeUntilUpdateEnd_Co(Action PlayCode)
    {
        yield return null;
        PlayCode();
    }
    public int GetBeforeIndex(int index, int initIndex = 0)
    {
        return index <= initIndex ? initIndex : index - 1;
    }
    public bool CheckObjInOtherObj(GameObject includedObj, GameObject includeObj, Vector2 includedObjImagePixelCount, Vector2 includeObjImagePixelCount)
    {
        if (Vector2.Distance(includedObj.transform.position, includeObj.transform.position) <= GetScaleAverage(includeObj) * (includeObjImagePixelCount.x + includeObjImagePixelCount.y) * 0.0025f + GetScaleAverage(includedObj) * (includedObjImagePixelCount.x + includedObjImagePixelCount.y) * 0.0025f)
        {
            return true;
        }
        return false;
    }
    public bool CheckObjInOtherObj(Vector2 includedObjPos/* , Vector2 includedObjScale */, GameObject includeObj, Vector2 includeObjImagePixelCount)
    {
        if (Vector2.Distance(includedObjPos, includeObj.transform.position)/*  - (GetScaleAverage(includedObjScale) + GetScaleAverage(includeObj)) * 0.5f */ <= GetScaleAverage(includeObj) * (includeObjImagePixelCount.x + includeObjImagePixelCount.y) * 0.0025f)
        {
            return true;
        }
        return false;
    }
    public bool CheckObjInOtherObj(Vector2 includedObjPos, Vector2 includeObjPos, Vector2 includeObjScale, Vector2 includeObjImagePixelCount)
    {
        if (Vector2.Distance(includedObjPos, includeObjPos) <= GetScaleAverage(includeObjScale) * (includeObjImagePixelCount.x + includeObjImagePixelCount.y) * 0.0025f)
        {
            return true;
        }
        return false;
    }
    public float GetSign0IsMin(float value)
    {
        return value <= 0f ? -1f : 1f;
    }
    public bool GetIsStdDegsChanged(){
        bool isStdDegsChanged = false;
        if(stdDegCount != GetWorldInfo().stdDegs.Count){
            isStdDegsChanged = true;
        }
        else{
            for(int i = 0; i < GetWorldInfo().stdDegs.Count; i++){
                if(GetWorldInfo().stdDegs[i] != stdDegs[i]){
                    isStdDegsChanged = true;
                }
            }
        }
        stdDegCount_temp = GetWorldInfo().stdDegs.Count;
        stdDegs_temp = GetWorldInfo().stdDegs;
        return isStdDegsChanged;
    }
    public int GetMaxStdDegCount(){
        int maxStdDegCount = 0;
        for(int i = 0; i < worldReaderScript.notesCount; i++){
            if(maxStdDegCount < GetWorldInfo(i).stdDegs.Count){
                maxStdDegCount = GetWorldInfo(i).stdDegs.Count;
            }
        }
        return maxStdDegCount;
    }
    public int GetCorrectNextDegIndex(int nextDegIndex, int? noteIndex = null){
        if(noteIndex == null)
        noteIndex = this.noteIndex;
        return nextDegIndex >= 0 ? nextDegIndex : nextDegIndex + GetWorldInfo(noteIndex).stdDegs.Count;
    }
    Handy()
    {
        minJudgmentRange = 0.05f;
        maxJudgmentRange = 0.5f;
    }
}