using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OVERIZE
{
    public class DontDestroyOnLoad : MonoBehaviour
    {
        void Reset()
        {
            DontDestroyOnLoad(gameObject);
        }
    }
}
