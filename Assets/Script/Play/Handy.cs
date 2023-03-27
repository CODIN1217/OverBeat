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
    public GameObject[] closestNotes;
    public NotePrefab[] closestNoteScripts;
    // public List<int> noteIndexes;
    public float judgmentRange;
    public float bestJudgmentRange;
    public readonly float minJudgmentRange;
    public readonly float maxJudgmentRange;
    int stdDegCount;
    int stdDegCount_temp;
    List<float> stdDegs;
    List<float> stdDegs_temp;
    private static Handy instance = null;
    GameManager GM;
    void Awake()
    {
        instance = this;
        GM = GameManager.Property;
        stdDegs = new List<float>();
        stdDegs_temp = new List<float>();
        // noteIndexes = new List<int>();
        // judgmentRange = new List<float>();
        // bestJudgmentRange = new List<float>();
        judgmentRange = Mathf.Clamp(GetWorldInfo().JudgmentInfo.Range, minJudgmentRange, maxJudgmentRange);
        bestJudgmentRange = judgmentRange * 0.2f;
        /* for (int i = 0; i <= GetTotalMaxPlayerIndex(); i++)
        {
            noteIndexes.Add(GetClosestNoteIndexToSamePlayer(i, 0));
            judgmentRange.Add(Mathf.Clamp(GetWorldInfoWithPlayerIndex(i, noteIndexes[i]).JudgmentInfo.Range, minJudgmentRange, maxJudgmentRange));
            bestJudgmentRange.Add(judgmentRange[i] * 0.2f);
        } */
    }
    void Update()
    {
        // noteIndexes.Clear();
        // judgmentRange.Clear();
        // bestJudgmentRange.Clear();
        judgmentRange = Mathf.Clamp(GetWorldInfo().JudgmentInfo.Range, minJudgmentRange, maxJudgmentRange);
        bestJudgmentRange = judgmentRange * 0.2f;
        closestNotes = noteGeneratorScript.closestNotes;
        closestNoteScripts = noteGeneratorScript.closestNoteScripts;
        /* for (int i = 0; i <= GetTotalMaxPlayerIndex(); i++)
        {
            noteIndexes.Add(closestNoteScripts[i].myNoteIndex);
            judgmentRange.Add(Mathf.Clamp(GetWorldInfoWithPlayerIndex(i, noteIndexes[i]).JudgmentInfo.Range, minJudgmentRange, maxJudgmentRange));
            bestJudgmentRange.Add(judgmentRange[i] * 0.2f);
        } */
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
    public WorldInfo GetWorldInfo(/* int playerIndex, int? noteIndex = null */int? worldInfoIndex = null)
    {
        if (worldInfoIndex == null)
            worldInfoIndex = GM.worldInfoIndex;
        // if (noteIndex == null)
        //     noteIndex = this.noteIndexes[(int)Mathf.Clamp(playerIndex, 0, playerControllerScript.players.Count)];
        return worldReaderScript.worldInfos[GetCorrectIndex((int)worldInfoIndex, GetWorldInfoCount() - 1)];
    }
    /* public WorldInfo GetWorldInfo(int noteIndex)
    {
        return worldReaderScript.worldInfos[Mathf.Clamp((int)noteIndex, 0, worldReaderScript.notesCount)];
    } */
    public float GetDistanceDeg(float tarDeg, float curDeg, bool maxIs360, int direction)
    {
        float distanceDeg = (tarDeg - curDeg) * direction/* (float)GetWorldInfo().direction[GetWorldInfo().PlayerInfo.Index] */;
        return maxIs360 ? GetCorrectDegMaxIs360(distanceDeg) : GetCorrectDegMaxIs0(distanceDeg);
    }
    public Vector2 GetCircularPos(float deg, float radius, Vector2? centerPos = null)
    {
        if (centerPos == null)
            centerPos = Vector2.zero;
        float rad = deg * Mathf.Deg2Rad;
        return new Vector2(Mathf.Sin(rad), Mathf.Cos(rad)) * radius + (Vector2)centerPos;
    }
    public float GetNextDeg(int playerIndex/* , int? noteIndex = null */, int? worldInfoIndex = null)
    {
        if (worldInfoIndex == null)
            worldInfoIndex = GM.worldInfoIndex;
        playerIndex = GetCorrectIndex(playerIndex, GetWorldInfo(worldInfoIndex).SeveralModeInfo.Count - 1);
        return GetWorldInfo(worldInfoIndex).PlayerInfo[playerIndex].StdDegs[GetWorldInfo(worldInfoIndex).NoteInfo[playerIndex].NextDegIndex];
    }
    public float GetNextDeg(int playerIndex, WorldInfo worldInfo)
    {
        playerIndex = GetCorrectIndex(playerIndex, worldInfo.SeveralModeInfo.Count - 1);
        return worldInfo.PlayerInfo[playerIndex].StdDegs[worldInfo.NoteInfo[playerIndex].NextDegIndex];
    }
    public float GetBeforeDeg(int playerIndex, int? worldInfoIndex = null)
    {
        if (worldInfoIndex == null)
            worldInfoIndex = GM.worldInfoIndex;
        playerIndex = GetCorrectIndex(playerIndex, GetWorldInfo(worldInfoIndex - 1).SeveralModeInfo.Count - 1);
        return GetWorldInfo(worldInfoIndex - 1).PlayerInfo[playerIndex].StdDegs[GetWorldInfo(worldInfoIndex - 1).NoteInfo[playerIndex].NextDegIndex];
    }
    public float GetNoteWaitTime(int playerIndex, int? worldInfoIndex = null)
    {
        if (worldInfoIndex == null)
            worldInfoIndex = GM.worldInfoIndex;
        playerIndex = GetCorrectIndex(playerIndex, GetWorldInfo(worldInfoIndex).SeveralModeInfo.Count - 1);
        return (float)noteGeneratorScript.noteWaitTimes[playerIndex, GetCorrectIndex((int)worldInfoIndex, GetWorldInfoCount() - 1)];
    }
    public float GetNoteLengthTime(int playerIndex, int? worldInfoIndex = null)
    {
        if (worldInfoIndex == null)
            worldInfoIndex = GM.worldInfoIndex;
        playerIndex = GetCorrectIndex(playerIndex, GetWorldInfo(worldInfoIndex).SeveralModeInfo.Count - 1);
        return (float)noteGeneratorScript.noteLengthTimes[playerIndex, GetCorrectIndex((int)worldInfoIndex, GetWorldInfoCount() - 1)];
    }
    public GameObject GetNote(int playerIndex, int? worldInfoIndex = null)
    {
        if (worldInfoIndex == null)
            worldInfoIndex = GM.worldInfoIndex;
        playerIndex = GetCorrectIndex(playerIndex, GetWorldInfo(worldInfoIndex).SeveralModeInfo.Count - 1);
        return noteGeneratorScript.notes[playerIndex, GetCorrectIndex((int)worldInfoIndex, GetWorldInfoCount() - 1)];
    }
    public NotePrefab GetNoteScripts(int playerIndex, int? worldInfoIndex = null)
    {
        if (worldInfoIndex == null)
            worldInfoIndex = GM.worldInfoIndex;
        playerIndex = GetCorrectIndex(playerIndex, GetWorldInfo(worldInfoIndex).SeveralModeInfo.Count - 1);
        return noteGeneratorScript.noteScripts[playerIndex, GetCorrectIndex((int)worldInfoIndex, GetWorldInfoCount() - 1)];
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
    public float GetScaleAbsAverage(GameObject go)
    {
        return (Mathf.Abs(go.transform.localScale.x) + Mathf.Abs(go.transform.localScale.y)) * 0.5f;
    }
    public float GetScaleAbsAverage(Vector2 scale)
    {
        return (Mathf.Abs(scale.x) + Mathf.Abs(scale.y)) * 0.5f;
    }
    public float GetScaleAbsAverage(float x, float y)
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
    public float GetJudgmentValue(int playerIndex, float? elapsedTimeWhenNeedlessInput01 = null, float? elapsedTimeWhenNeedInput01 = null)
    {
        if (elapsedTimeWhenNeedlessInput01 == null)
            elapsedTimeWhenNeedlessInput01 = GetElapsedTimeWhenNeedlessInput01(playerIndex);
        if (elapsedTimeWhenNeedInput01 == null)
            elapsedTimeWhenNeedInput01 = GetElapsedTimeWhenNeedInput01(playerIndex);
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
        if (Vector2.Distance(includedObj.transform.position, includeObj.transform.position) <= GetScaleAbsAverage(includedObj) * (includedObjImagePixelCount.x + includedObjImagePixelCount.y) * 0.0025f + GetScaleAbsAverage(includeObj) * (includeObjImagePixelCount.x + includeObjImagePixelCount.y) * 0.0025f)
        {
            return true;
        }
        return false;
    }
    public bool CheckObjInOtherObj(Vector2 includedObjPos, Vector2 includedObjScale, GameObject includeObj, Vector2 includedObjImagePixelCount, Vector2 includeObjImagePixelCount)
    {
        if (Vector2.Distance(includedObjPos, includeObj.transform.position) <= GetScaleAbsAverage(includedObjScale) * (includedObjImagePixelCount.x + includedObjImagePixelCount.y) * 0.0025f + GetScaleAbsAverage(includeObj) * (includeObjImagePixelCount.x + includeObjImagePixelCount.y) * 0.0025f)
        {
            return true;
        }
        return false;
    }
    public bool CheckObjInOtherObj(GameObject includedObj, Vector2 includeObjPos, Vector2 includeObjScale, Vector2 includedObjImagePixelCount, Vector2 includeObjImagePixelCount)
    {
        if (Vector2.Distance(includedObj.transform.position, includeObjPos) <= GetScaleAbsAverage(includedObj) * (includedObjImagePixelCount.x + includedObjImagePixelCount.y) * 0.0025f + GetScaleAbsAverage(includeObjScale) * (includeObjImagePixelCount.x + includeObjImagePixelCount.y) * 0.0025f)
        {
            return true;
        }
        return false;
    }
    public bool CheckObjInOtherObj(Vector2 includedObjPos, Vector2 includedObjScale, Vector2 includeObjPos, Vector2 includeObjScale, Vector2 includedObjImagePixelCount, Vector2 includeObjImagePixelCount)
    {
        if (Vector2.Distance(includedObjPos, includeObjPos) <= GetScaleAbsAverage(includedObjScale) * (includedObjImagePixelCount.x + includedObjImagePixelCount.y) * 0.0025f + GetScaleAbsAverage(includeObjScale) * (includeObjImagePixelCount.x + includeObjImagePixelCount.y) * 0.0025f)
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
        if(stdDegCount != GetWorldInfo().PlayerInfo.StdDegs.Count){
            isStdDegsChanged = true;
        }
        else{
            for(int i = 0; i < GetWorldInfo().PlayerInfo.StdDegs.Count; i++){
                if(GetWorldInfo().PlayerInfo.StdDegs[i] != stdDegs[i]){
                    isStdDegsChanged = true;
                }
            }
        }
        stdDegCount_temp = GetWorldInfo().PlayerInfo.StdDegs.Count;
        stdDegs_temp = GetWorldInfo().PlayerInfo.StdDegs;
        return isStdDegsChanged;
    } */
    public int GetMaxStdDegCount(int playerIndex)
    {
        int maxStdDegCount = 0;
        for (int i = 0; i < GetWorldInfoCount(); i++)
        {
            int _playerIndex = GetCorrectIndex(playerIndex, GetWorldInfo(i).SeveralModeInfo.Count - 1);
            if (maxStdDegCount < GetWorldInfo(i).PlayerInfo[_playerIndex].StdDegs.Count)
            {
                maxStdDegCount = GetWorldInfo(i).PlayerInfo[_playerIndex].StdDegs.Count;
            }
        }
        return maxStdDegCount;
    }
    public int GetCorrectNextDegIndex(int playerIndex, int nextDegIndex, int? worldInfoIndex = null)
    {
        if (worldInfoIndex == null)
            worldInfoIndex = GM.worldInfoIndex;
        playerIndex = GetCorrectIndex(playerIndex, GetWorldInfo(worldInfoIndex).SeveralModeInfo.Count - 1);
        // return GetCorrectIndex(nextDegIndex, GetWorldInfo(worldInfoIndex).PlayerInfo[playerIndex].StdDegs.Count - 1);
        return nextDegIndex >= 0 ? nextDegIndex : nextDegIndex + GetWorldInfo(worldInfoIndex).PlayerInfo[playerIndex].StdDegs.Count;
    }
    public int GetCorrectIndex(int index, int? maxIndex = null)
    {
        if (maxIndex == null)
            maxIndex = int.MaxValue;
        return (int)Mathf.Clamp(index, 0f, (float)maxIndex);
    }
    public int GetMaxPlayerCount()
    {
        int maxPlayerCount = 0;
        for (int i = 0; i < GetWorldInfoCount(); i++)
        {
            if (maxPlayerCount < GetWorldInfo(i).SeveralModeInfo.Count)
            {
                maxPlayerCount = GetWorldInfo(i).SeveralModeInfo.Count;
            }
        }
        return maxPlayerCount;
    }
    public int GetMaxPlayerIndex()
    {
        return GetMaxPlayerCount() - 1;
    }
    public int GetWorldInfoCount()
    {
        return worldReaderScript.worldInfos.Count;
    }
    public int GetNoteCount()
    {
        return worldReaderScript.worldInfos.Count - 1;
    }
    public int GetActivePlayerCount()
    {
        int activeMaxPlayerCount = 0;
        for (int i = 0; i < GetMaxPlayerCount(); i++)
        {
            if (GetPlayer(i).activeSelf)
                activeMaxPlayerCount++;
        }
        return activeMaxPlayerCount;
    }
    public GameObject GetPlayer(int playerIndex)
    {
        // if (playerIndex == null)
        //     playerIndex = GetWorldInfo().PlayerInfo.Index;
        return playerControllerScript.players[GetCorrectIndex(playerIndex, GetMaxPlayerIndex())];
    }
    public Player GetPlayerScript(int playerIndex)
    {
        // if (playerIndex == null)
        //     playerIndex = GetWorldInfo().PlayerInfo.Index;
        return playerControllerScript.playerScripts[GetCorrectIndex(playerIndex, GetMaxPlayerIndex())];
    }
    public GameObject GetPlayerSide(int playerIndex)
    {
        // if (playerIndex == null)
        //     playerIndex = GetWorldInfo().PlayerInfo.Index;
        return playerControllerScript.playerSides[GetCorrectIndex(playerIndex, GetMaxPlayerIndex())];
    }
    public SpriteRenderer GetPlayerSideRend(int playerIndex)
    {
        // if (playerIndex == null)
        //     playerIndex = GetWorldInfo().PlayerInfo.Index;
        return playerControllerScript.playerSideRends[GetCorrectIndex(playerIndex, GetMaxPlayerIndex())];
    }
    public GameObject GetPlayerCenter(int playerIndex)
    {
        // if (playerIndex == null)
        //     playerIndex = GetWorldInfo().PlayerInfo.Index;
        return playerControllerScript.playerCenters[GetCorrectIndex(playerIndex, GetMaxPlayerIndex())];
    }
    public SpriteRenderer GetPlayerCenterRend(int playerIndex)
    {
        // if (playerIndex == null)
        //     playerIndex = GetWorldInfo().PlayerInfo.Index;
        return playerControllerScript.playerCenterRends[GetCorrectIndex(playerIndex, GetMaxPlayerIndex())];
    }
    public Vector2 GetSpritePixels(Sprite sprite)
    {
        return new Vector2(sprite.texture.width, sprite.texture.height);
    }
    public Color GetColor01(Color color)
    {
        return color / 255f;
    }
    /* public GameObject GetNextNoteToSamePlayer(int playerIndex, int noteIndex)
    {
        for (int i = noteIndex + 1; i <= GetNoteCount(); i++)
        {
            if (playerIndex == worldReaderScript.worldInfos[i].PlayerInfo.Index)
            {
                return GetNote(i);
            }
        }
        return GetNote(noteIndex);
    }
    public NotePrefab GetNextNoteScriptToSamePlayer(int playerIndex, int noteIndex)
    {
        for (int i = noteIndex + 1; i <= GetNoteCount(); i++)
        {
            if (playerIndex == worldReaderScript.worldInfos[i].PlayerInfo.Index)
            {
                return GetNoteScripts(i);
            }
        }
        return GetNoteScripts(noteIndex);
    }
    public int GetNextNoteIndexToSamePlayer(int playerIndex, int noteIndex)
    {
        for (int i = noteIndex + 1; i <= GetNoteCount(); i++)
        {
            if (playerIndex == worldReaderScript.worldInfos[i].PlayerInfo.Index)
            {
                return i;
            }
        }
        return noteIndex;
    }
    public GameObject GetClosestNoteToSamePlayer(int playerIndex, int noteIndex)
    {
        for (int i = noteIndex; i <= GetNoteCount(); i++)
        {
            if (playerIndex == worldReaderScript.worldInfos[i].PlayerInfo.Index)
            {
                return GetNote(i);
            }
        }
        return GetNote(noteIndex);
    }
    public NotePrefab GetClosestNoteScriptToSamePlayer(int playerIndex, int noteIndex)
    {
        for (int i = noteIndex; i <= GetNoteCount(); i++)
        {
            if (playerIndex == worldReaderScript.worldInfos[i].PlayerInfo.Index)
            {
                return GetNoteScripts(i);
            }
        }
        return GetNoteScripts(noteIndex);
    }
    public int GetClosestNoteIndexToSamePlayer(int playerIndex, int noteIndex)
    {
        for (int i = noteIndex; i <= GetNoteCount(); i++)
        {
            if (playerIndex == worldReaderScript.worldInfos[i].PlayerInfo.Index)
            {
                return i;
            }
        }
        return noteIndex;
    } */
    public void RepeatCode(Action<int> code, int count)
    {
        for (int i = 0; i < count; i++)
        {
            code(i);
        }
    }
    public float GetElapsedTimeWhenNeedlessInput01(int playerIndex, int? worldInfoIndex = null)
    {
        if (worldInfoIndex == null)
            worldInfoIndex = GM.worldInfoIndex;
        playerIndex = GetCorrectIndex(playerIndex, GetWorldInfo(worldInfoIndex).SeveralModeInfo.Count - 1);
        if (GetNoteScripts(playerIndex, worldInfoIndex) != null)
            return GetNoteScripts(playerIndex, worldInfoIndex).elapsedTimeWhenNeedlessInput01;
        return 0f;
    }
    public float GetElapsedTimeWhenNeedInput01(int playerIndex, int? worldInfoIndex = null)
    {
        if (worldInfoIndex == null)
            worldInfoIndex = GM.worldInfoIndex;
        playerIndex = GetCorrectIndex(playerIndex, GetWorldInfo(worldInfoIndex).SeveralModeInfo.Count - 1);
        if (GetNoteScripts(playerIndex, worldInfoIndex) != null)
            return GetNoteScripts(playerIndex, worldInfoIndex).elapsedTimeWhenNeedInput01;
        return 0f;
    }
    Handy()
    {
        minJudgmentRange = 0.05f;
        maxJudgmentRange = 0.5f;
    }
}