using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OVERIZE
{
    public partial interface IDeepCopy<T>
    {
        T Clone();
    }
}
