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
    public GameObject playerController;
    public PlayerController playerControllerScript;
    /* List<GameObject> player;
    List<Player> playerScript;
    List<GameObject> playerSide;
    List<SpriteRenderer> playerSideRend;
    List<GameObject> playerCenter;
    List<SpriteRenderer> playerCenterRend; */
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
    public float GetDistanceDeg(float tarDeg, float curDeg, bool maxIs360, int direction)
    {
        float distanceDeg = (tarDeg - curDeg) * direction/* (float)GetWorldInfo().direction[GetWorldInfo().playerIndex] */;
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
        return GetWorldInfo(index).stdDegs[GetWorldInfo(index).nextDegIndex[GetWorldInfo(index).playerIndex]];
    }
    public float GetNextDeg(WorldInfo worldInfo)
    {
        return worldInfo.stdDegs[worldInfo.nextDegIndex[worldInfo.playerIndex]];
    }
    public float GetBeforeDeg(int? index = null)
    {
        if (index == null)
            index = noteIndex;
        return GetWorldInfo(index - 1).stdDegs[GetWorldInfo(index - 1).nextDegIndex[GetWorldInfo(index - 1).playerIndex]];
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
    public float GetScaleAverage(float x, float y)
    {
        return (Mathf.Abs(x) + Mathf.Abs(y)) * 0.5f;
    }
    public float GetBiggerAbsScale(GameObject go)
    {
        return Mathf.Abs(go.transform.localScale.x) > Mathf.Abs(go.transform.localScale.y) ? go.transform.localScale.x : go.transform.localScale.y;
    }
    public float GetBiggerAbsScale(Vector2 scale)
    {
        return Mathf.Abs(scale.x) > Mathf.Abs(scale.y) ? scale.x : scale.y;
    }
    public float GetBiggerAbsScale(float x, float y)
    {
        return Mathf.Abs(x) > Mathf.Abs(y) ? x : y;
    }
    public float GetJudgmentValue(float? elapsedTimeWhenNeedlessInput01 = null, float? elapsedTimeWhenNeedInput01 = null)
    {
        if (elapsedTimeWhenNeedlessInput01 == null)
            elapsedTimeWhenNeedlessInput01 = GameManager.Property.elapsedTimeWhenNeedlessInput01;
        if (elapsedTimeWhenNeedInput01 == null)
            elapsedTimeWhenNeedInput01 = GameManager.Property.elapsedTimeWhenNeedInput01;
        float judgmentValue = 1f;
        if (elapsedTimeWhenNeedInput01 > 0f)
            judgmentValue = (float)elapsedTimeWhenNeedInput01;
        else if (elapsedTimeWhenNeedlessInput01 >= 1f - judgmentRange)
            judgmentValue = 1f - (float)elapsedTimeWhenNeedlessInput01;
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
        if (Vector2.Distance(includedObj.transform.position, includeObj.transform.position) <= GetScaleAverage(includedObj) * (includedObjImagePixelCount.x + includedObjImagePixelCount.y) * 0.0025f + GetScaleAverage(includeObj) * (includeObjImagePixelCount.x + includeObjImagePixelCount.y) * 0.0025f)
        {
            return true;
        }
        return false;
    }
    public bool CheckObjInOtherObj(Vector2 includedObjPos, Vector2 includedObjScale, GameObject includeObj, Vector2 includedObjImagePixelCount, Vector2 includeObjImagePixelCount)
    {
        if (Vector2.Distance(includedObjPos, includeObj.transform.position) <= GetScaleAverage(includedObjScale) * (includedObjImagePixelCount.x + includedObjImagePixelCount.y) * 0.0025f + GetScaleAverage(includeObj) * (includeObjImagePixelCount.x + includeObjImagePixelCount.y) * 0.0025f)
        {
            return true;
        }
        return false;
    }
    public bool CheckObjInOtherObj(GameObject includedObj, Vector2 includeObjPos, Vector2 includeObjScale, Vector2 includedObjImagePixelCount, Vector2 includeObjImagePixelCount)
    {
        if (Vector2.Distance(includedObj.transform.position, includeObjPos) <= GetScaleAverage(includedObj) * (includedObjImagePixelCount.x + includedObjImagePixelCount.y) * 0.0025f + GetScaleAverage(includeObjScale) * (includeObjImagePixelCount.x + includeObjImagePixelCount.y) * 0.0025f)
        {
            return true;
        }
        return false;
    }
    public bool CheckObjInOtherObj(Vector2 includedObjPos, Vector2 includedObjScale, Vector2 includeObjPos, Vector2 includeObjScale, Vector2 includedObjImagePixelCount, Vector2 includeObjImagePixelCount)
    {
        if (Vector2.Distance(includedObjPos, includeObjPos) <= GetScaleAverage(includedObjScale) * (includedObjImagePixelCount.x + includedObjImagePixelCount.y) * 0.0025f + GetScaleAverage(includeObjScale) * (includeObjImagePixelCount.x + includeObjImagePixelCount.y) * 0.0025f)
        {
            return true;
        }
        return false;
    }
    public float GetSign0IsMin(float value)
    {
        return value <= 0f ? -1f : 1f;
    }
    /* public bool GetIsStdDegsChanged(){
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
    } */
    public int GetMaxStdDegCount()
    {
        int maxStdDegCount = 0;
        for (int i = 0; i <= worldReaderScript.notesCount; i++)
        {
            if (maxStdDegCount < GetWorldInfo(i).stdDegs.Count)
            {
                maxStdDegCount = GetWorldInfo(i).stdDegs.Count;
            }
        }
        return maxStdDegCount;
    }
    public int GetCorrectNextDegIndex(int nextDegIndex, int? index = null)
    {
        if (index == null)
            index = noteIndex;
        return nextDegIndex >= 0 ? nextDegIndex : nextDegIndex + GetWorldInfo(index).stdDegs.Count;
    }
    public int GetTotalMaxPlayerIndex()
    {
        int totalMaxPlayerIndex = 0;
        for (int i = 0; i <= worldReaderScript.notesCount; i++)
        {
            if (totalMaxPlayerIndex < GetWorldInfo(i).playerIndex)
            {
                totalMaxPlayerIndex = GetWorldInfo(i).playerIndex;
            }
        }
        return totalMaxPlayerIndex;
    }
    public int GetActivePlayerCount()
    {
        int curMaxPlayerIndex = 0;
        for (int i = 0; i <= GetTotalMaxPlayerIndex(); i++)
        {
            if (GetPlayer(i).activeSelf)
                curMaxPlayerIndex++;
        }
        return curMaxPlayerIndex;
    }
    public GameObject GetPlayer(int? playerIndex = null)
    {
        if (playerIndex == null)
            playerIndex = GetWorldInfo().playerIndex;
        return playerControllerScript.players[(int)Mathf.Clamp((float)playerIndex, 0f, GetTotalMaxPlayerIndex())];
    }
    public Player GetPlayerScript(int? playerIndex = null)
    {
        if (playerIndex == null)
            playerIndex = GetWorldInfo().playerIndex;
        return playerControllerScript.playerScripts[(int)Mathf.Clamp((float)playerIndex, 0f, GetTotalMaxPlayerIndex())];
    }
    public GameObject GetPlayerSide(int? playerIndex = null)
    {
        if (playerIndex == null)
            playerIndex = GetWorldInfo().playerIndex;
        return playerControllerScript.playerSides[(int)Mathf.Clamp((float)playerIndex, 0f, GetTotalMaxPlayerIndex())];
    }
    public SpriteRenderer GetPlayerSideRend(int? playerIndex = null)
    {
        if (playerIndex == null)
            playerIndex = GetWorldInfo().playerIndex;
        return playerControllerScript.playerSideRends[(int)Mathf.Clamp((float)playerIndex, 0f, GetTotalMaxPlayerIndex())];
    }
    public GameObject GetPlayerCenter(int? playerIndex = null)
    {
        if (playerIndex == null)
            playerIndex = GetWorldInfo().playerIndex;
        return playerControllerScript.playerCenters[(int)Mathf.Clamp((float)playerIndex, 0f, GetTotalMaxPlayerIndex())];
    }
    public SpriteRenderer GetPlayerCenterRend(int? playerIndex = null)
    {
        if (playerIndex == null)
            playerIndex = GetWorldInfo().playerIndex;
        return playerControllerScript.playerCenterRends[(int)Mathf.Clamp((float)playerIndex, 0f, GetTotalMaxPlayerIndex())];
    }
    public Vector2 GetSpritePixels(Sprite sprite){
        return new Vector2(sprite.texture.width, sprite.texture.height);
    }
    public Color GetRGB01(Color color){
        return new Color(color.r / 255f, color.g / 255f, color.b / 255f, color.a);
    }
    public Color GetColor01(Color color){
        return color / 255f;
    }
    public GameObject GetNextNoteToSamePlayer(int playerIndex, int noteIndex){
        for(int i = noteIndex; i <= worldReaderScript.notesCount; i++){
            if(playerIndex == GetWorldInfo(i).playerIndex){
                return GetNote(i);
            }
        }
        return GetNote(noteIndex);
    }
    public NotePrefab GetNextNoteScriptToSamePlayer(int playerIndex, int noteIndex){
        for(int i = noteIndex; i <= worldReaderScript.notesCount; i++){
            if(playerIndex == GetWorldInfo(i).playerIndex){
                return GetNoteScripts(i);
            }
        }
        return GetNoteScripts(noteIndex);
    }
    Handy()
    {
        minJudgmentRange = 0.05f;
        maxJudgmentRange = 0.5f;
    }
}