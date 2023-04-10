using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoteGenerator : MonoBehaviour
{
    public GameObject notePrefab;
    public GameObject[] closestNotes;
    public NotePrefab[] closestNoteScripts;
    public List<float>[] noteWaitTimes;
    public List<float>[] noteLengthTimes;
    public List<GameObject>[] notes;
    public List<NotePrefab>[] noteScripts;
    public int[] eachNoteCount;
    // public GameObject worldReader;
    // public WorldReader worldReaderScript;
    bool isAwake;
    Handy handy;
    GameManager GM;
    void Awake()
    {
        handy = Handy.Property;
        GM = GameManager.Property;
        noteWaitTimes = new List<float>[handy.GetMaxPlayerCount()];
        noteLengthTimes = new List<float>[handy.GetMaxPlayerCount()];
        notes = new List<GameObject>[handy.GetMaxPlayerCount()];
        noteScripts = new List<NotePrefab>[handy.GetMaxPlayerCount()];
        for (int i = 0; i < handy.GetMaxPlayerCount(); i++)
        {
            noteWaitTimes[i] = new List<float>();
            noteLengthTimes[i] = new List<float>();
            notes[i] = new List<GameObject>();
            noteScripts[i] = new List<NotePrefab>();
        }
        eachNoteCount = new int[handy.GetMaxPlayerCount()];
        closestNotes = new GameObject[handy.GetMaxPlayerCount()];
        closestNoteScripts = new NotePrefab[handy.GetMaxPlayerCount()];
        for (int j = 0; j < handy.GetWorldInfoCount(); j++)
        {
            SetNotePrefab(j);
        }
        for (int j = 0; j < handy.GetWorldInfoCount(); j++)
        {
            GameObject curNote = handy.GetNote(handy.GetWorldInfo(j).noteInfo.tarPlayerIndex, handy.GetWorldInfo(j).noteInfo.eachNoteIndex);
            NotePrefab curNoteScript = handy.GetNoteScript(handy.GetWorldInfo(j).noteInfo.tarPlayerIndex, handy.GetWorldInfo(j).noteInfo.eachNoteIndex);
            curNoteScript.InitNote();
            if (curNoteScript.myEachNoteIndex == 0)
            {
                closestNotes[curNoteScript.tarPlayerIndex] = curNote;
                closestNoteScripts[curNoteScript.tarPlayerIndex] = curNoteScript;
            }
            curNote.SetActive(false);
        }
        foreach (var CN in closestNotes)
        {
            CN.SetActive(true);
        }
        isAwake = true;
        /* for (int i = 0; i < transform.childCount; i++)
        {
            int curNoteIndex = transform.GetChild(i).GetComponent<NotePrefab>().myNoteIndex;
            for (int j = 0; j < handy.GetMaxPlayerCount(); j++)
            {
                if (curNoteIndex == 0 && handy.GetWorldInfo().SeveralModeInfo.Count == j)
                {
                    closestNotes.Add(transform.GetChild(i).gameObject);
                    closestNoteScripts.Add(closestNotes[j].GetComponent<NotePrefab>());
                }
            }
            handy.GetNoteScripts(0, i).InitNote();
            handy.GetNote(0, i).SetActive(false);
        } */
    }
    void Update()
    {
        if (isAwake)
        {
            isAwake = false;
        }
    }
    void SetNotePrefab(int worldInfoIndex)
    {
        GameObject newNote = Instantiate(notePrefab, transform);
        NotePrefab newNoteScript = newNote.GetComponent<NotePrefab>();

        WorldInfo worldInfo = handy.GetWorldInfo(worldInfoIndex);
        int eachNoteIndex = worldInfo.noteInfo.eachNoteIndex;

        newNoteScript.noteWaitTime = Mathf.Abs(worldInfo.noteInfo.startRadius - worldInfo.playerInfo[worldInfo.noteInfo.tarPlayerIndex].tarRadius) / 3.5f / worldInfo.noteInfo.speed;
        newNoteScript.noteWaitTime *= Mathf.Clamp01(eachNoteIndex);
        noteWaitTimes[worldInfo.noteInfo.tarPlayerIndex].Add(newNoteScript.noteWaitTime);

        newNoteScript.noteLengthTime = worldInfo.noteInfo.length / worldInfo.noteInfo.speed;
        newNoteScript.noteLengthTime *= Mathf.Clamp01(eachNoteIndex);
        noteLengthTimes[worldInfo.noteInfo.tarPlayerIndex].Add(newNoteScript.noteLengthTime);

        notes[worldInfo.noteInfo.tarPlayerIndex].Add(newNote);
        noteScripts[worldInfo.noteInfo.tarPlayerIndex].Add(newNoteScript);

        newNoteScript.myEachNoteIndex = eachNoteIndex;
        newNoteScript.tarPlayerIndex = worldInfo.noteInfo.tarPlayerIndex;
        eachNoteCount[worldInfo.noteInfo.tarPlayerIndex]++;
        // GM.notesActivedSeconds[playerIndex, noteIndex] = newNoteScript.noteWaitTime + (newNoteScript.noteLengthTime == 0f ? handy.judgmentRange[playerIndex] : newNoteScript.noteLengthTime);
        // GM.notesActivedSeconds[playerIndex, noteIndex] *= Mathf.Clamp01(noteIndex);
        // lastNoteActivedTotalSeconds[playerIndex] += GM.notesActivedSeconds[playerIndex, noteIndex];
        // GM.notesActivedTotalSeconds[playerIndex, noteIndex] = lastNoteActivedTotalSeconds[playerIndex];
        /* newNoteScript.InitNote();
        if (noteIndex == 0)
        {
            closestNotes[playerIndex] = newNote;
            closestNoteScripts[playerIndex] = newNoteScript;
        }
        newNote.SetActive(false); */
    }
}
