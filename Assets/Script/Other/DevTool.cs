using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

public class DevTool : MonoBehaviour
{
    [SerializeField]
    GameObject testDottedLine;
    Dictionary<string, DottedLine> testDottedLines;
    Dictionary<string, Func<List<Vector2>>> testDottedLinePoses;
    List<string> predicateNames;
    [SerializeField]
    InfoViewer _infoViewer;
    public InfoViewer infoViewer { get { return _infoViewer; } }
    static DevTool instance = null;
    public static DevTool Member { get { return instance; } }
    void Awake()
    {
        instance = this;
        testDottedLines = new Dictionary<string, DottedLine>();
        testDottedLinePoses = new Dictionary<string, Func<List<Vector2>>>();
        predicateNames = new List<string>();
        StartCoroutine(UpdateOnEndOfFrame());
    }
    IEnumerator UpdateOnEndOfFrame()
    {
        while (true)
        {
            yield return new WaitForEndOfFrame();
            infoViewer.UpdateInfo();
            UpdateDottedLine();
        }
    }
    public void SetDottedLine(string predicateName, Func<List<Vector2>> poses)
    {
        if (!testDottedLines.ContainsKey(predicateName))
            testDottedLines.Add(predicateName, Instantiate<GameObject>(testDottedLine, transform).GetComponent<DottedLine>());

        if (testDottedLinePoses.ContainsKey(predicateName))
        {
            testDottedLinePoses[predicateName] = poses;
        }
        else
        {
            if (!predicateNames.Contains(predicateName))
                predicateNames.Add(predicateName);
            testDottedLinePoses.Add(predicateName, poses);
        }
    }
    public void UpdateDottedLine()
    {
        foreach (var PN in predicateNames)
        {
            Handy.LineRendMethod.SetDottedLine(testDottedLines[PN], testDottedLinePoses[PN]());
        }
    }
}
