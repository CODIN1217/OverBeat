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
    static InfoViewer instance = null;
    public static InfoViewer Property
    {
        get
        {
            return instance;
        }
    }
    void Awake()
    {
        instance = this;
        infoViewer_TMP = GetComponent<TextMeshProUGUI>();
        infos = new Dictionary<string, Func<object>>();
        names = new List<string>();
        StartCoroutine(UpdateOnEndOfFrame());
    }
    IEnumerator UpdateOnEndOfFrame()
    {
        while (true)
        {
            yield return new WaitForEndOfFrame();
            StringBuilder content = new StringBuilder("<Info>");
            for (int i = 0; i < infos.Count; i++)
            {
                content.AppendLine().Append(names[i]).AppendLine().Append("= ").Append(infos[names[i]]().ToString());
            }
            infoViewer_TMP.text = content.ToString();
        }
    }
    public void SetInfo(string parentsName, string varName, Func<object> infoFunc, int? index = null)
    {
        string fullName = $"{parentsName} / {varName}" + (index != null ? $" ( Index : {index} )" : "");
        if (infos.ContainsKey(fullName) && names.Contains(fullName))
            infos[fullName] = infoFunc;
        else if (infos.ContainsKey(fullName) && !names.Contains(fullName))
        {
            names.Add(fullName);
        }
        else if (!infos.ContainsKey(fullName) && names.Contains(fullName))
        {
            infos.Add(fullName, infoFunc);
        }
        else
        {
            names.Add(fullName);
            infos.Add(fullName, infoFunc);
        }
    }
    /* public string ArrayToText<T>(T[] array)
    {
        StringBuilder content = new StringBuilder();
        for (int i = 0; i < array.Length; i++)
        {
            if (i > 0)
                content.Append("  ");
            content.Append("( Index : ").Append(i.ToString()).Append(", Value : ").Append(array[i]).Append(" )");
        }
        return content.ToString();
    }
    public string ListToText<T>(List<T> array)
    {
        StringBuilder content = new StringBuilder();
        for (int i = 0; i < array.Count; i++)
        {
            if (i > 0)
                content.Append("  ");
            content.Append("( Index : ").Append(i.ToString()).Append(", Value : ").Append(array[i]).Append(" )");
        }
        return content.ToString();
    } */
}
