using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDeepCopy<T>
{
    T Clone();
}
