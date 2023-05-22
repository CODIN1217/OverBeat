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
    Dictionary<string, Color> dottedLineColors;
    [SerializeField]
    InfoViewer _infoViewer;
    public InfoViewer infoViewer { get { return _infoViewer; } }
    static DevTool instance = null;
    public static DevTool Member { get { return instance; } }
    public class StdColors
    {
        static Color[] _stdColors = new Color[] { Color.red, Color.green, Color.blue, Color.white, Color.magenta, Color.yellow, Color.cyan, Color.black };
        public Color this[int index] { get { try { return _stdColors[index]; } catch { return Color.gray; } } }
    }
    void Awake()
    {
        instance = this;
        testDottedLines = new Dictionary<string, DottedLine>();
        testDottedLinePoses = new Dictionary<string, List<Vector2>>();
        predicateNames = new List<string>();
        dottedLineColors = new Dictionary<string, Color>();
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
    public void AddDottedLinePos(string predicateName, Vector2 pos, Color color)
    {
        if (!testDottedLines.ContainsKey(predicateName))
            testDottedLines.Add(predicateName, Instantiate<GameObject>(testDottedLine, transform).GetComponent<DottedLine>());

        if (!predicateNames.Contains(predicateName))
            predicateNames.Add(predicateName);

        if (testDottedLinePoses.ContainsKey(predicateName))
            testDottedLinePoses[predicateName].Add(pos);
        else
            testDottedLinePoses.Add(predicateName, new List<Vector2>() { pos });

        if (dottedLineColors.ContainsKey(predicateName))
            dottedLineColors[predicateName] = color;
        else
            dottedLineColors.Add(predicateName, color);
    }
    public void UpdateDottedLine()
    {
        foreach (var PN in predicateNames)
        {
            Handy.LineRendMethod.SetDottedLine(testDottedLines[PN], testDottedLinePoses[PN]);
            testDottedLines[PN].SetColor(dottedLineColors[PN]);
        }
    }
}
