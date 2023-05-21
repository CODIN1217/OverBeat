using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

public class DevTool : MonoBehaviour
{
    [SerializeField]
    GameObject testDottedLine;
    List<string> predicateNames;
    Dictionary<string, DottedLine> testDottedLines;
    Dictionary<string, List<Vector2>> testDottedLinePoses;
    Dictionary<string, ValueN<Color>> dottedLineColors;
    [SerializeField]
    InfoViewer _infoViewer;
    public InfoViewer infoViewer { get { return _infoViewer; } }
    static DevTool instance = null;
    public static DevTool Member { get { return instance; } }
    void Awake()
    {
        instance = this;
        testDottedLines = new Dictionary<string, DottedLine>();
        testDottedLinePoses = new Dictionary<string, List<Vector2>>();
        predicateNames = new List<string>();
        dottedLineColors = new Dictionary<string, ValueN<Color>>();
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
    public void AddDottedLinePos(string predicateName, Vector2 pos, Color startColor, Color endColor)
    {
        if (!testDottedLines.ContainsKey(predicateName))
            testDottedLines.Add(predicateName, Instantiate<GameObject>(testDottedLine, transform).GetComponent<DottedLine>());

        if (!predicateNames.Contains(predicateName))
            predicateNames.Add(predicateName);

        if (testDottedLinePoses.ContainsKey(predicateName))
            testDottedLinePoses[predicateName].Add(pos);
        else
            testDottedLinePoses.Add(predicateName, new List<Vector2>(){pos});

        if (dottedLineColors.ContainsKey(predicateName))
            dottedLineColors[predicateName] = new ValueN<Color>(startColor, endColor);
        else
            dottedLineColors.Add(predicateName, new ValueN<Color>(startColor, endColor));
    }
    public void UpdateDottedLine()
    {
        foreach (var PN in predicateNames)
        {
            Handy.LineRendMethod.SetDottedLine(testDottedLines[PN], testDottedLinePoses[PN]);
            testDottedLines[PN].SetColor(dottedLineColors[PN][0], dottedLineColors[PN][1]);
        }
    }
}
