using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoteGenerator : MonoBehaviour
{
    public GameObject notePrefab;
    public GameObject[] closestNotes;
    public NotePrefab[] closestNoteScripts;
    public float?[,] noteWaitTimes;
    public float?[,] noteLengthTimes;
    public GameObject[,] notes;
    public NotePrefab[,] noteScripts;
    float[] lastNoteActivedTotalSeconds;
    // public GameObject worldReader;
    // public WorldReader worldReaderScript;
    bool isAwake;
    Handy handy;
    void Awake()
    {
        handy = Handy.Property;
        noteWaitTimes = new float?[handy.GetPlayerCount(), handy.GetWorldInfoCount()];
        noteLengthTimes = new float?[handy.GetPlayerCount(), handy.GetWorldInfoCount()];
        notes = new GameObject[handy.GetPlayerCount(), handy.GetWorldInfoCount()];
        noteScripts = new NotePrefab[handy.GetPlayerCount(), handy.GetWorldInfoCount()];
        closestNotes = new GameObject[handy.GetPlayerCount()];
        closestNoteScripts = new NotePrefab[handy.GetPlayerCount()];
        lastNoteActivedTotalSeconds = new float[handy.GetPlayerCount()];
        for (int i = 0; i < handy.GetPlayerCount(); i++)
        {
            for (int j = 0; j < handy.GetWorldInfoCount(); j++)
            {
                SetNotePrefab(i, j);
            }
        }
        for (int i = 0; i < handy.GetPlayerCount(); i++)
        {
            for (int j = 0; j < handy.GetWorldInfoCount(); j++)
            {
                GameObject curNote = handy.GetNote(i, j);
                NotePrefab curNoteScript = handy.GetNoteScript(i, j);
                curNoteScript.InitNote();
                if (j == 0)
                {
                    closestNotes[i] = curNote;
                    closestNoteScripts[i] = curNoteScript;
                }
                curNote.SetActive(false);
            }
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
    void SetNotePrefab(int playerIndex, int noteIndex)
    {
        GameObject newNote = Instantiate(notePrefab, transform);
        NotePrefab newNoteScript = newNote.GetComponent<NotePrefab>();

        WorldInfo worldInfo = handy.GetWorldInfo(noteIndex);
        playerIndex = handy.GetCorrectIndex(playerIndex, handy.GetMaxPlayerIndex());
        newNoteScript.noteWaitTime = Mathf.Abs(worldInfo.noteInfo[playerIndex].startRadius - worldInfo.playerInfo[playerIndex].tarRadius) / 3.5f / worldInfo.noteInfo[playerIndex].speed;
        newNoteScript.noteWaitTime *= Mathf.Clamp01(noteIndex);
        noteWaitTimes[playerIndex, noteIndex] = newNoteScript.noteWaitTime;
        newNoteScript.noteLengthTime = worldInfo.noteInfo[playerIndex].length / worldInfo.noteInfo[playerIndex].speed;
        newNoteScript.noteLengthTime *= Mathf.Clamp01(noteIndex);
        noteLengthTimes[playerIndex, noteIndex] = newNoteScript.noteLengthTime;
        notes[playerIndex, noteIndex] = newNote;
        noteScripts[playerIndex, noteIndex] = newNoteScript;
        newNoteScript.myNoteIndex = noteIndex;
        newNoteScript.playerIndex = playerIndex;
        GameManager.Property.notesActivedSeconds[playerIndex, noteIndex] = newNoteScript.noteWaitTime + (newNoteScript.noteLengthTime == 0f ? handy.judgmentRange[playerIndex] : newNoteScript.noteLengthTime);
        GameManager.Property.notesActivedSeconds[playerIndex, noteIndex] *= Mathf.Clamp01(noteIndex);
        lastNoteActivedTotalSeconds[playerIndex] += GameManager.Property.notesActivedSeconds[playerIndex, noteIndex];
        GameManager.Property.notesActivedTotalSeconds[playerIndex, noteIndex] = lastNoteActivedTotalSeconds[playerIndex];
        /* newNoteScript.InitNote();
        if (noteIndex == 0)
        {
            closestNotes[playerIndex] = newNote;
            closestNoteScripts[playerIndex] = newNoteScript;
        }
        newNote.SetActive(false); */
    }
}
