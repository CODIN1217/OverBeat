using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TweenValue;

public class NoteGenerator : MonoBehaviour
{
    public GameObject notePrefab;
    public List<float>[] notesWaitSecs;
    public List<float>[] notesLengthSecs;
    public List<GameObject>[] notes;
    public List<NotePrefab>[] noteScripts;
    public int[] eachNoteCount;
    bool isAwake;
    Handy handy;
    PlayGameManager playGM;
    void Awake()
    {
        handy = Handy.Property;
        playGM = PlayGameManager.Property;
        notesWaitSecs = new List<float>[playGM.GetMaxPlayerCount()];
        notesLengthSecs = new List<float>[playGM.GetMaxPlayerCount()];
        notes = new List<GameObject>[playGM.GetMaxPlayerCount()];
        noteScripts = new List<NotePrefab>[playGM.GetMaxPlayerCount()];
        for (int i = 0; i < playGM.GetMaxPlayerCount(); i++)
        {
            notesWaitSecs[i] = new List<float>();
            notesLengthSecs[i] = new List<float>();
            notes[i] = new List<GameObject>();
            noteScripts[i] = new List<NotePrefab>();
        }
        eachNoteCount = new int[playGM.GetMaxPlayerCount()];

        for (int j = 0; j < playGM.GetWorldInfoCount(); j++)
        {
            SetNotePrefab(j);
        }

        for (int i = 0; i < playGM.GetMaxPlayerCount(); i++)
        {
            for (int j = 0; j < playGM.GetNoteCount(i); j++)
            {
                playGM.GetNoteScript(i, j).InitNote();
            }
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
    void SetNotePrefab(int worldInfoIndex)
    {
        GameObject newNote = Instantiate(notePrefab, transform);
        NotePrefab newNoteScript = newNote.GetComponent<NotePrefab>();

        WorldInfo worldInfo = playGM.GetWorldInfo(worldInfoIndex);
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

        newNoteScript.myEachNoteIndex = eachNoteIndex;
        newNoteScript.tarPlayerIndex = playerIndex;
        if (playerIndex != -1)
            eachNoteCount[playerIndex]++;
        if (worldInfoIndex == 0)
        {
            for (int i = 0; i < playGM.GetMaxPlayerCount(); i++)
            {
                playGM.closestNotes[i] = newNote;
                playGM.closestNoteScripts[i] = newNoteScript;
            }
        }
        newNote.SetActive(false);
    }
}
