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
    // public GameObject worldReader;
    // public WorldReader worldReaderScript;
    bool isAwake;
    Handy handy;
    void Awake()
    {
        handy = Handy.Property;
        noteWaitTimes = new float?[handy.GetMaxPlayerCount(), handy.GetWorldInfoCount()];
        noteLengthTimes = new float?[handy.GetMaxPlayerCount(), handy.GetWorldInfoCount()];
        notes = new GameObject[handy.GetMaxPlayerCount(), handy.GetWorldInfoCount()];
        noteScripts = new NotePrefab[handy.GetMaxPlayerCount(), handy.GetWorldInfoCount()];
        closestNotes = new GameObject[handy.GetMaxPlayerCount()];
        closestNoteScripts = new NotePrefab[handy.GetMaxPlayerCount()];
        for (int i = 0; i < handy.GetMaxPlayerCount(); i++)
        {
            for (int j = 0; j < handy.GetWorldInfoCount(); j++)
            {
                SetNotePrefab(i, j);
            }
        }
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
        foreach (var CN in closestNotes)
        {
            CN.SetActive(true);
        }
        isAwake = true;
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

        WorldInfo worldInfo = handy.GetWorldInfo();
        playerIndex = handy.GetCorrectIndex(playerIndex, handy.GetMaxPlayerIndex());
        newNoteScript.noteWaitTime = Mathf.Abs(worldInfo.NoteInfo[playerIndex].StartRadius - worldInfo.PlayerInfo[playerIndex].TarRadius) / 3.5f / worldInfo.NoteInfo[playerIndex].Speed;
        newNoteScript.noteWaitTime *= Mathf.Clamp01(noteIndex);
        noteWaitTimes[playerIndex, noteIndex] = newNoteScript.noteWaitTime;
        newNoteScript.noteLengthTime = worldInfo.NoteInfo[playerIndex].Length / worldInfo.NoteInfo[playerIndex].Speed;
        newNoteScript.noteLengthTime *= Mathf.Clamp01(noteIndex);
        noteLengthTimes[playerIndex, noteIndex] = newNoteScript.noteLengthTime;
        notes[playerIndex, noteIndex] = newNote;
        noteScripts[playerIndex, noteIndex] = newNoteScript;
        newNoteScript.myNoteIndex = noteIndex;
        newNoteScript.playerIndex = playerIndex;
        newNoteScript.InitNote();
        if (noteIndex == 0)
        {
            closestNotes[playerIndex] = newNote;
            closestNoteScripts[playerIndex] = newNoteScript;
        }
        newNote.SetActive(false);
    }
}
