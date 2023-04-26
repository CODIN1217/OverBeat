using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoteGenerator : MonoBehaviour
{
    public GameObject notePrefab;
    public GameObject[] closestNotes;
    public NotePrefab[] closestNoteScripts;
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
        closestNotes = new GameObject[playGM.GetMaxPlayerCount()];
        closestNoteScripts = new NotePrefab[playGM.GetMaxPlayerCount()];
        for (int j = 0; j < playGM.GetWorldInfoCount(); j++)
        {
            SetNotePrefab(j);
        }
        for (int j = 0; j < playGM.GetWorldInfoCount(); j++)
        {
            GameObject curNote = playGM.GetNote(playGM.GetWorldInfo(j).noteInfo.tarPlayerIndex, playGM.GetWorldInfo(j).noteInfo.eachNoteIndex);
            NotePrefab curNoteScript = playGM.GetNoteScript(playGM.GetWorldInfo(j).noteInfo.tarPlayerIndex, playGM.GetWorldInfo(j).noteInfo.eachNoteIndex);
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
        int eachNoteIndex = worldInfo.noteInfo.eachNoteIndex;

        newNoteScript.noteWaitSecs = worldInfo.noteInfo.waitRadiusTween.duration / worldInfo.noteInfo.speed;
        newNoteScript.noteWaitSecs *= Mathf.Clamp01(eachNoteIndex);
        notesWaitSecs[worldInfo.noteInfo.tarPlayerIndex].Add(newNoteScript.noteWaitSecs);

        newNoteScript.noteLengthSecs = worldInfo.noteInfo.holdRadiusTween.duration / worldInfo.noteInfo.speed;
        newNoteScript.noteLengthSecs *= Mathf.Clamp01(eachNoteIndex);
        notesLengthSecs[worldInfo.noteInfo.tarPlayerIndex].Add(newNoteScript.noteLengthSecs);

        notes[worldInfo.noteInfo.tarPlayerIndex].Add(newNote);
        noteScripts[worldInfo.noteInfo.tarPlayerIndex].Add(newNoteScript);

        newNoteScript.myEachNoteIndex = eachNoteIndex;
        newNoteScript.tarPlayerIndex = worldInfo.noteInfo.tarPlayerIndex;
        eachNoteCount[worldInfo.noteInfo.tarPlayerIndex]++;
    }
}
