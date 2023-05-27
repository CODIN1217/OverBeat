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
    public List<Note>[] noteScripts;
    public GameObject startNote;
    public Note startNoteScript;
    public int[] eachNoteCount;
    PlayManager PM;
    void Awake()
    {
        PM = PlayManager.Member;
        notesWaitSecs = new List<float>[PM.GetMaxPlayerCount()];
        notesLengthSecs = new List<float>[PM.GetMaxPlayerCount()];
        notes = new List<GameObject>[PM.GetMaxPlayerCount()];
        noteScripts = new List<Note>[PM.GetMaxPlayerCount()];
        for (int i = 0; i < PM.GetMaxPlayerCount(); i++)
        {
            notesWaitSecs[i] = new List<float>();
            notesLengthSecs[i] = new List<float>();
            notes[i] = new List<GameObject>();
            noteScripts[i] = new List<Note>();
        }
        eachNoteCount = new int[PM.GetMaxPlayerCount()];

        for (int j = 0; j < PM.GetLevelInfoCount(); j++)
        {
            SetNotePrefab(j);
        }

        for (int i = 1; i < PM.GetLevelInfoCount(); i++)
        {
            PM.GetNoteScript(i).InitNote();
        }
    }
    void SetNotePrefab(int levelInfoIndex)
    {
        GameObject newNote = Instantiate(notePrefab, transform);
        Note newNoteScript = newNote.GetComponent<Note>();

        LevelInfo levelInfo = PM.GetLevelInfo(levelInfoIndex);
        int playerIndex = levelInfo.noteInfo.tarPlayerIndex;
        int eachNoteIndex = levelInfo.noteInfo.eachNoteIndex;

        newNoteScript.noteWaitSecs = Mathf.Abs(levelInfo.noteInfo.waitDeltaRadiusTween.startValue - levelInfo.noteInfo.waitDeltaRadiusTween.endValue) / 3.5f / levelInfo.noteInfo.speed;
        newNoteScript.noteWaitSecs *= Mathf.Clamp01(levelInfoIndex);
        if (playerIndex != -1)
            notesWaitSecs[playerIndex].Add(newNoteScript.noteWaitSecs);

        newNoteScript.holdNoteSecs = Mathf.Abs(levelInfo.noteInfo.holdDeltaRadiusTween.startValue - levelInfo.noteInfo.holdDeltaRadiusTween.endValue) / levelInfo.noteInfo.speed;
        newNoteScript.holdNoteSecs *= Mathf.Clamp01(levelInfoIndex);
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

        newNoteScript.myLevelInfoIndex = levelInfoIndex;
        newNoteScript.myEachNoteIndex = eachNoteIndex;
        newNoteScript.tarPlayerIndex = playerIndex;
        if (playerIndex != -1)
            eachNoteCount[playerIndex]++;
        if (levelInfoIndex == 0)
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
