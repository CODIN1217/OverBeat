using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoteGenerator : MonoBehaviour
{
    public GameObject notePrefab;
    public List<GameObject> closestNotes;
    public List<NotePrefab> closestNoteScripts;
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
        for (int i = 0; i <= handy.GetTotalMaxPlayerIndex(); i++)
        {
            closestNotes[handy.GetNextNoteIndexToSamePlayer(i, 0)].SetActive(true);
            closestNotes.Add(transform.GetChild(handy.GetNextNoteIndexToSamePlayer(i, 0)).gameObject);
            closestNoteScripts.Add(closestNotes[handy.GetNextNoteIndexToSamePlayer(i, 0)].GetComponent<NotePrefab>());
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
    void SetNotePrefab(int index)
    {
        GameObject newNote = Instantiate(notePrefab, transform);
        NotePrefab newNoteScript = newNote.GetComponent<NotePrefab>();

        WorldInfo worldInfo = handy.worldReaderScript.worldInfos[index];
        newNoteScript.noteWaitTime = Mathf.Abs(worldInfo.NoteInfo.StartRadius - worldInfo.PlayerInfo.TarRadius) / 3.5f / worldInfo.NoteInfo.Speed;
        newNoteScript.noteWaitTime *= Mathf.Clamp01(index);
        noteWaitTimes.Add(newNoteScript.noteWaitTime);
        newNoteScript.noteLengthTime = worldInfo.NoteInfo.Length / worldInfo.NoteInfo.Speed;
        newNoteScript.noteLengthTime *= Mathf.Clamp01(index);
        noteLengthTimes.Add(newNoteScript.noteLengthTime);
        notes.Add(newNote);
        noteScripts.Add(newNoteScript);
        newNoteScript.myNoteIndex = index;
    }
}
