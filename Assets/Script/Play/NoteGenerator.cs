using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TweenManager;

public class NoteGenerator : MonoBehaviour
{
    public GameObject notePrefab;
    public List<float>[] notesWaitSecs;
    public List<float>[] notesLengthSecs;
    public List<GameObject>[] notes;
    public List<NotePrefab>[] noteScripts;
    public GameObject startNote;
    public NotePrefab startNoteScript;
    public int[] eachNoteCount;
    Handy handy;
    PlayManager PM;
    void Awake()
    {
        handy = Handy.Property;
        PM = PlayManager.Property;
        notesWaitSecs = new List<float>[PM.GetMaxPlayerCount()];
        notesLengthSecs = new List<float>[PM.GetMaxPlayerCount()];
        notes = new List<GameObject>[PM.GetMaxPlayerCount()];
        noteScripts = new List<NotePrefab>[PM.GetMaxPlayerCount()];
        for (int i = 0; i < PM.GetMaxPlayerCount(); i++)
        {
            notesWaitSecs[i] = new List<float>();
            notesLengthSecs[i] = new List<float>();
            notes[i] = new List<GameObject>();
            noteScripts[i] = new List<NotePrefab>();
        }
        eachNoteCount = new int[PM.GetMaxPlayerCount()];

        for (int j = 0; j < PM.GetWorldInfoCount(); j++)
        {
            SetNotePrefab(j);
        }

        for (int i = 0; i < PM.GetMaxPlayerCount(); i++)
        {
            for (int j = 0; j < PM.GetNoteCount(i); j++)
            {
                PM.GetNoteScript(i, j).InitNote();
            }
        }
    }
    void SetNotePrefab(int worldInfoIndex)
    {
        GameObject newNote = Instantiate(notePrefab, transform);
        NotePrefab newNoteScript = newNote.GetComponent<NotePrefab>();

        WorldInfo worldInfo = PM.GetWorldInfo(worldInfoIndex);
        int playerIndex = worldInfo.noteInfo.tarPlayerIndex;
        int eachNoteIndex = worldInfo.noteInfo.eachNoteIndex;

        newNoteScript.noteWaitSecs = Mathf.Abs(worldInfo.noteInfo.waitRadiusTween.startValue - worldInfo.noteInfo.waitRadiusTween.endValue) / 3.5f / worldInfo.noteInfo.speed;
        newNoteScript.noteWaitSecs *= Mathf.Clamp01(worldInfoIndex);
        if (playerIndex != -1)
            notesWaitSecs[playerIndex].Add(newNoteScript.noteWaitSecs);

        newNoteScript.holdNoteSecs = Mathf.Abs(worldInfo.noteInfo.holdRadiusTween.startValue - worldInfo.noteInfo.holdRadiusTween.endValue) / worldInfo.noteInfo.speed;
        newNoteScript.holdNoteSecs *= Mathf.Clamp01(worldInfoIndex);
        if (playerIndex != -1)
            notesLengthSecs[playerIndex].Add(newNoteScript.holdNoteSecs);

        if (playerIndex != -1)
        {
            notes[playerIndex].Add(newNote);
            noteScripts[playerIndex].Add(newNoteScript);
        }
        else
        {
            startNote = newNote;
            startNoteScript = newNoteScript;
        }

        newNoteScript.myWorldInfoIndex = worldInfoIndex;
        newNoteScript.myNoteIndex = eachNoteIndex;
        newNoteScript.tarPlayerIndex = playerIndex;
        if (playerIndex != -1)
            eachNoteCount[playerIndex]++;
        if (worldInfoIndex == 0)
        {
            for (int i = 0; i < PM.GetMaxPlayerCount(); i++)
            {
                PM.closestNotes[i] = newNote;
                PM.closestNoteScripts[i] = newNoteScript;
            }
        }
        newNote.SetActive(false);
    }
}
