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
    public float[] judgmentRange;
    public float[] bestJudgmentRange;
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
        judgmentRange = new float[GetMaxPlayerCount()];
        bestJudgmentRange = new float[GetMaxPlayerCount()];
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
        UpdateJudgmentRange();
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
    public void UpdateJudgmentRange()
    {
        for (int i = 0; i < GetMaxPlayerCount(); i++)
        {
            if (GetPlayer(i).activeSelf)
            {
                judgmentRange[i] = GetWorldInfo(i, GM.closestNoteIndex[i]).judgmentInfo.range;
                bestJudgmentRange[i] = judgmentRange[i] * 0.2f;
            }
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
    public WorldInfo GetWorldInfo(/* int? noteIndex = null */int worldInfoIndex)
    {/* 
        if (worldInfoIndex == null)
            worldInfoIndex = GM.curWorldInfoIndex; */
        // if (noteIndex == null)
        //     noteIndex = this.noteIndexes[(int)Mathf.Clamp(playerIndex, 0, playerControllerScript.players.Count)];
        return worldReaderScript.worldInfos[GetCorrectIndex(/* (int) */worldInfoIndex, GetMaxWorldInfoIndex())];
    }
    public WorldInfo GetWorldInfo(int playerIndex, int eachNoteIndex)
    {
        playerIndex = GetCorrectIndex(playerIndex, GetMaxPlayerIndex());
        eachNoteIndex = GetCorrectIndex(eachNoteIndex, GetMaxNoteIndex(playerIndex));
        for (int i = 0; i < GetWorldInfoCount(); i++)
        {
            WorldInfo curWorldInfo = GetWorldInfo(i);
            if (curWorldInfo.noteInfo.tarPlayerIndex == playerIndex && curWorldInfo.noteInfo.eachNoteIndex == eachNoteIndex)
                return curWorldInfo;
        }
        return null;
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
    public float GetStartDeg(int playerIndex, int eachNoteIndex)
    {
        playerIndex = GetCorrectIndex(playerIndex, GetMaxPlayerIndex());
        WorldInfo worldInfo = GetWorldInfo(playerIndex, eachNoteIndex);
        return worldInfo.playerInfo[playerIndex].stdDegs[worldInfo.noteInfo.startDegIndex];
    }
    public float GetEndDeg(int playerIndex, int eachNoteIndex)
    {
        playerIndex = GetCorrectIndex(playerIndex, GetMaxPlayerIndex());
        WorldInfo worldInfo = GetWorldInfo(playerIndex, eachNoteIndex);
        return worldInfo.playerInfo[playerIndex].stdDegs[worldInfo.noteInfo.endDegIndex];
    }
    /* public float GetNextDeg(WorldInfo worldInfo)
    {
        playerIndex = GetCorrectIndex(playerIndex, worldInfo.variousModeInfo.variousModeCount - 1);
        return worldInfo.playerInfo[playerIndex].stdDegs[worldInfo.noteInfo.curDegIndex];
    } */
    /* public float GetBeforeDeg(int worldInfoIndex)
    {
        // if (worldInfoIndex == null)
        //     worldInfoIndex = GM.curWorldInfoIndex;
        playerIndex = GetCorrectIndex(playerIndex, GetWorldInfo(worldInfoIndex - 1).variousModeInfo.variousModeCount - 1);
        return GetWorldInfo(worldInfoIndex - 1).playerInfo[playerIndex].stdDegs[GetWorldInfo(worldInfoIndex - 1).noteInfo.startDegIndex];
    } */
    public float GetNoteWaitSecs(int playerIndex, int eachNoteIndex)
    {
        playerIndex = GetCorrectIndex((int)playerIndex, GetMaxPlayerIndex());
        // if (worldInfoIndex == null)
        //     worldInfoIndex = GM.curWorldInfoIndex;
        return (float)noteGeneratorScript.notesWaitSecs[playerIndex][GetCorrectIndex((int)eachNoteIndex, GetMaxNoteIndex(playerIndex))];
    }
    public float GetNoteLengthSecs(int playerIndex, int eachNoteIndex)
    {
        playerIndex = GetCorrectIndex((int)playerIndex, GetMaxPlayerIndex());
        // if (worldInfoIndex == null)
        //     worldInfoIndex = GM.curWorldInfoIndex;
        return (float)noteGeneratorScript.notesLengthSecs[playerIndex][GetCorrectIndex((int)eachNoteIndex, GetMaxNoteIndex(playerIndex))];
    }
    public GameObject GetNote(int playerIndex, int eachNoteIndex)
    {
        playerIndex = GetCorrectIndex((int)playerIndex, GetMaxPlayerIndex());
        // if (worldInfoIndex == null)
        //     worldInfoIndex = GM.curWorldInfoIndex;
        return noteGeneratorScript.notes[playerIndex][GetCorrectIndex((int)eachNoteIndex, GetMaxNoteIndex(playerIndex))];
    }
    public NotePrefab GetNoteScript(int playerIndex, int eachNoteIndex)
    {
        playerIndex = GetCorrectIndex((int)playerIndex, GetMaxPlayerIndex());
        // if (worldInfoIndex == null)
        //     worldInfoIndex = GM.curWorldInfoIndex;
        return noteGeneratorScript.noteScripts[playerIndex][GetCorrectIndex((int)eachNoteIndex, GetMaxNoteIndex(playerIndex))];
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
    public float GetJudgmentValue(int playerIndex, float? elapsedSecsWhenNeedlessInput01 = null, float? elapsedSecsWhenNeedInput01 = null)
    {
        if (elapsedSecsWhenNeedlessInput01 == null)
            elapsedSecsWhenNeedlessInput01 = GetElapsedSecsWhenNeedlessInput01(playerIndex, GM.closestNoteIndex[playerIndex]);
        if (elapsedSecsWhenNeedInput01 == null)
            elapsedSecsWhenNeedInput01 = GetElapsedSecsWhenNeedInput01(playerIndex, GM.closestNoteIndex[playerIndex]);
        float judgmentValue = 1f;
        if (elapsedSecsWhenNeedInput01 > 0f)
            judgmentValue = (float)elapsedSecsWhenNeedInput01;
        else if (elapsedSecsWhenNeedlessInput01 >= 1f - judgmentRange[playerIndex])
            judgmentValue = 1f - (float)elapsedSecsWhenNeedlessInput01;
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
    public float GetSign0Is0(float value)
    {
        if (value != 0f)
            return Mathf.Sign(value);
        return 0f;
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
            int _playerIndex = GetCorrectIndex(playerIndex, GetWorldInfo(i).playerInfo.Length - 1);
            if (maxStdDegCount < GetWorldInfo(i).playerInfo[_playerIndex].stdDegs.Length)
            {
                maxStdDegCount = GetWorldInfo(i).playerInfo[_playerIndex].stdDegs.Length;
            }
        }
        return maxStdDegCount;
    }
    public int GetCorrectNextDegIndex(int nextDegIndex, int playerIndex, int eachNoteIndex)
    {
        // if (worldInfoIndex == null)
        //     worldInfoIndex = GM.curWorldInfoIndex;
        // return GetCorrectIndex(nextDegIndex, GetWorldInfo(worldInfoIndex).PlayerInfo[playerIndex].StdDegs.Count - 1);
        playerIndex = GetCorrectIndex(playerIndex, GetMaxPlayerIndex());
        return nextDegIndex >= 0 ? nextDegIndex : nextDegIndex + GetWorldInfo(playerIndex, eachNoteIndex).playerInfo[playerIndex].stdDegs.Length;
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
            if (maxPlayerCount < GetWorldInfo(i).playerInfo.Length)
            {
                maxPlayerCount = GetWorldInfo(i).playerInfo.Length;
            }
        }
        return maxPlayerCount;
    }
    /* public int GetMaxEachNoteCount()
    {
        int maxPlayerCount = 0;
        for (int i = 0; i < GetWorldInfoCount(); i++)
        {
            if (maxPlayerCount < GetWorldInfo(i).playerInfo.Length)
            {
                maxPlayerCount = GetWorldInfo(i).playerInfo.Length;
            }
        }
        return maxPlayerCount;
    } */
    public int GetMaxPlayerIndex()
    {
        return GetMaxPlayerCount() - 1;
    }
    public int GetWorldInfoCount()
    {
        return worldReaderScript.worldInfos.Count;
    }
    public int GetMaxWorldInfoIndex()
    {
        return GetWorldInfoCount() - 1;
    }
    public int GetNoteCountWithOutStartNote(int playerIndex)
    {
        int noteCount = 0;
        for (int i = 0; i < GetWorldInfoCount(); i++)
        {
            if (GetWorldInfo(i).noteInfo.tarPlayerIndex == playerIndex && GetWorldInfo(i).noteInfo.eachNoteIndex != 0)
                noteCount++;
        }
        return noteCount;
    }
    public int GetMaxNoteIndexWithOutStartNote(int playerIndex)
    {
        playerIndex = GetCorrectIndex((int)playerIndex, GetMaxPlayerIndex());
        return GetNoteCountWithOutStartNote(playerIndex) - 1;
    }
    public int GetNoteCount(int playerIndex)
    {
        int noteCount = 0;
        for (int i = 0; i < GetWorldInfoCount(); i++)
        {
            if (GetWorldInfo(i).noteInfo.tarPlayerIndex == playerIndex)
                noteCount++;
        }
        return noteCount;
    }
    public int GetMaxNoteIndex(int playerIndex)
    {
        playerIndex = GetCorrectIndex((int)playerIndex, GetMaxPlayerIndex());
        return GetNoteCount(playerIndex) - 1;
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
    public Color GetColor01WithPlayerIndex(Color color01, int playerIndex)
    {
        Color color01_temp = color01;
        for (int i = 0; i < playerIndex; i++)
        {
            color01_temp = Color.white - color01_temp;
        }
        color01_temp /= Mathf.Floor(playerIndex * 0.5f) + 1;
        color01_temp.a = color01.a;
        return color01_temp;
    }
    /* public GameObject GetNextNoteToSamePlayer(int noteIndex)
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
    public NotePrefab GetNextNoteScriptToSamePlayer(int noteIndex)
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
    public int GetNextNoteIndexToSamePlayer(int noteIndex)
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
    public GameObject GetClosestNoteToSamePlayer(int noteIndex)
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
    public NotePrefab GetClosestNoteScriptToSamePlayer(int noteIndex)
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
    public int GetClosestNoteIndexToSamePlayer(int noteIndex)
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
    public float GetElapsedSecsWhenNeedlessInput01(int playerIndex, int eachNoteIndex)
    {
        playerIndex = GetCorrectIndex((int)playerIndex, GetMaxPlayerIndex());
        // if (worldInfoIndex == null)
        //     worldInfoIndex = GM.curWorldInfoIndex;
        if (GetNoteScript(playerIndex, eachNoteIndex) != null)
            return GetNoteScript(playerIndex, eachNoteIndex).elapsedSecsWhenNeedlessInput01;
        return 0f;
    }
    public float GetElapsedSecsWhenNeedInput01(int playerIndex, int eachNoteIndex)
    {
        playerIndex = GetCorrectIndex((int)playerIndex, GetMaxPlayerIndex());
        // if (worldInfoIndex == null)
        //     worldInfoIndex = GM.curWorldInfoIndex;
        if (GetNoteScript(playerIndex, eachNoteIndex) != null)
            return GetNoteScript(playerIndex, eachNoteIndex).elapsedSecsWhenNeedInput01;
        return 0f;
    }
    public float[] GetCorrectStdDegs(float[] stdDegs)
    {
        float[] stdDegs_temp = new float[stdDegs.Length];
        for (int i = 0; i < stdDegs.Length; i++)
        {
            stdDegs_temp[i] = GetCorrectDegMaxIs0(stdDegs[i]);
        }
        return stdDegs_temp;
    }
    public Vector2 GetCorrectXY(Vector2 XY, float minXY, float maxXY)
    {
        return new Vector2(Mathf.Clamp(XY.x, minXY, maxXY), Mathf.Clamp(XY.y, minXY, maxXY));
    }
    public Vector2? GetCorrectXY(Vector2? XY, float minXY, float maxXY)
    {
        if (XY == null)
            return null;
        return new Vector2(Mathf.Clamp(((Vector2)XY).x, minXY, maxXY), Mathf.Clamp(((Vector2)XY).y, minXY, maxXY));
    }
    public Color GetCorrectRGBA(Color RGBA, float minRGBA = 0f, float maxRGBA = 255f)
    {
        return new Color(Mathf.Clamp(RGBA.r, minRGBA, maxRGBA), Mathf.Clamp(RGBA.g, minRGBA, maxRGBA), Mathf.Clamp(RGBA.b, minRGBA, maxRGBA), Mathf.Clamp(RGBA.a, minRGBA, maxRGBA));
    }
    public Color? GetCorrectRGBA(Color? RGBA, float minRGBA = 0f, float maxRGBA = 255f)
    {
        if (RGBA == null)
            return null;
        return new Color(Mathf.Clamp(((Color)RGBA).r, minRGBA, maxRGBA), Mathf.Clamp(((Color)RGBA).g, minRGBA, maxRGBA), Mathf.Clamp(((Color)RGBA).b, minRGBA, maxRGBA), Mathf.Clamp(((Color)RGBA).a, minRGBA, maxRGBA));
    }
    public void PlayEachCodeWithBoolens(List<bool> boolen, List<Action> codes)
    {
        if (boolen.Count < codes.Count)
            codes.RemoveRange(boolen.Count, codes.Count - 1);
        for (int i = 0; i < boolen.Count; i++)
        {
            if (boolen[i])
            {
                codes[i]();
            }
        }
    }
}