using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoteGenerator : MonoBehaviour
{
    public GameObject notePrefab;
    public GameObject closestNote;
    public NotePrefab closestNoteScript;
    public List<float> noteWaitTimes;
    public List<float> noteLengthTimes;
    public List<GameObject> notes;
    public List<NotePrefab> noteScripts;
    bool isAwake;
    Handy handy;
    void Awake()
    {
        handy = Handy.Property;
        noteWaitTimes = new List<float>();
        noteLengthTimes = new List<float>();
        notes = new List<GameObject>();
        noteScripts = new List<NotePrefab>();
        for (int i = 0; i < handy.worldReaderScript.worldInfos.Count; i++)
        {
            SetNotePrefab(i);
        }
        for (int i = 0; i < transform.childCount; i++)
        {
            handy.GetNoteScripts(i).InitNote();
            handy.GetNote(i).SetActive(false);
        }
        closestNote = transform.GetChild(0).gameObject;
        closestNoteScript = closestNote.GetComponent<NotePrefab>();
        isAwake = true;
        closestNote.SetActive(true);
    }
    void Update()
    {
        if (isAwake)
        {
            isAwake = false;
        }
    }
    void SetNotePrefab(int index)
    {
        GameObject newNote = Instantiate(notePrefab, transform);
        NotePrefab newNoteScript = newNote.GetComponent<NotePrefab>();

        WorldInfo worldInfo = handy.GetWorldInfo(index);
        newNoteScript.noteWaitTime = Mathf.Abs(worldInfo.noteStartRadius[worldInfo.playerIndex] - worldInfo.playerTarRadius[worldInfo.playerIndex]) / 3.5f / worldInfo.speed[worldInfo.playerIndex];
        newNoteScript.noteWaitTime *= Mathf.Clamp01(index);
        noteWaitTimes.Add(newNoteScript.noteWaitTime);
        newNoteScript.noteLengthTime = worldInfo.noteLength[worldInfo.playerIndex] / worldInfo.speed[worldInfo.playerIndex];
        newNoteScript.noteLengthTime *= Mathf.Clamp01(index);
        noteLengthTimes.Add(newNoteScript.noteLengthTime);
        notes.Add(newNote);
        noteScripts.Add(newNoteScript);
        newNoteScript.myNoteIndex = index;
    }
}
