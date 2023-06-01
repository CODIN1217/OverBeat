using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[ExecuteInEditMode]
public class LayoutGroupController : MonoBehaviour
{
    [SerializeField]
    GameObject child;
    public List<GameObject> childs;
    void Update()
    {
        int childCountDelta = childs.Count - transform.childCount;
        if (childCountDelta > 0)
        {
            for (int i = transform.childCount; i < childs.Count; i++)
            {
                childs[i] = Instantiate<GameObject>(child, transform);
            }
        }
        else if (childCountDelta < 0)
        {
            for (int i = transform.childCount - 1; i > childs.Count - 1; i--)
            {
                transform.GetChild(i).gameObject.SetActive(false);
                transform.GetChild(i).name = "Need to Destroy";
            }
        }
    }
}
