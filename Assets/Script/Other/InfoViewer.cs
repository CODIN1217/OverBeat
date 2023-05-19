using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;
using System.Text;
using System.Linq;

public class InfoViewer : MonoBehaviour
{
    TextMeshProUGUI infoViewer_TMP;
    Dictionary<string, Func<object>> infos;
    List<string> names;
    /* static InfoViewer instance = null;
    public static InfoViewer Member
    {
        get
        {
            return instance;
        }
    } */
    void Awake()
    {
        // instance = this;
        infoViewer_TMP = GetComponent<TextMeshProUGUI>();
        infos = new Dictionary<string, Func<object>>();
        names = new List<string>();
    }
    public void UpdateInfo()
    {
        StringBuilder content = new StringBuilder("< Info >");
        for (int i = 0; i < infos.Count; i++)
        {
            content.AppendLine().Append(names[i]).AppendLine().Append("= ").Append(infos[names[i]]().ToString());
        }
        infoViewer_TMP.text = content.ToString();
    }
    public void SetInfo(string predicateName, Func<object> infoFunc)
    {
        if (infos.ContainsKey(predicateName))
            infos[predicateName] = infoFunc;
        else
        {
            if (!names.Contains(predicateName))
                names.Add(predicateName);
            infos.Add(predicateName, infoFunc);
        }
    }
}
