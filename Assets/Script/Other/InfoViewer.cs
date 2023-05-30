using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;
using System.Text;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

public class InfoViewer : MonoBehaviour
{
    TextMeshProUGUI infoViewer_TMP;
    Dictionary<string, Func<object>> infos;
    List<string> names;
    Dictionary<string, float> intervalSecses;
    Dictionary<string, StringBuilder> contents;
    Dictionary<string, bool> isDelayed;
    void Awake()
    {
        infoViewer_TMP = GetComponent<TextMeshProUGUI>();
        infos = new Dictionary<string, Func<object>>();
        names = new List<string>();
        intervalSecses = new Dictionary<string, float>();
        contents = new Dictionary<string, StringBuilder>();
        isDelayed = new Dictionary<string, bool>();
    }
    public void UpdateInfo()
    {
        StringBuilder content = new StringBuilder("< Info >");
        for (int i = 0; i < names.Count; i++)
        {
            string predicateName = names[i];
            content.AppendLine().Append(contents[predicateName]);
            if (!isDelayed[predicateName])
            {
                isDelayed[predicateName] = true;
                DevTool.Member.StartCoroutine(Handy.WaitCodeForSecsCo(intervalSecses[predicateName], () => { contents[predicateName] = new StringBuilder().Append(predicateName).AppendLine().Append("= ").Append(infos[predicateName]().ToString()); isDelayed[predicateName] = false; }));
            }
        }
        infoViewer_TMP.text = content.ToString();
    }
    public void SetInfo(string predicateName, Func<object> infoFunc, float intervalSecs = 0f)
    {
        if (infos.ContainsKey(predicateName))
            infos[predicateName] = infoFunc;
        else
            infos.Add(predicateName, infoFunc);

        if (!names.Contains(predicateName))
            names.Add(predicateName);

        if (intervalSecses.ContainsKey(predicateName))
            intervalSecses[predicateName] = intervalSecs;
        else
            intervalSecses.Add(predicateName, intervalSecs);

        if (!contents.ContainsKey(predicateName))
            contents.Add(predicateName, new StringBuilder().Append(predicateName).AppendLine().Append("= ").Append(infoFunc().ToString()));

        if (!isDelayed.ContainsKey(predicateName))
            isDelayed.Add(predicateName, false);
    }
}
