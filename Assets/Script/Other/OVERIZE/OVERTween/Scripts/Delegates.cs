using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OVERIZE
{
    public delegate void CallBack();
    public delegate T Getter<T>();
    public delegate void Setter<T>(T value);
}