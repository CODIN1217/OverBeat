using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct Value<T>
{
    public readonly T defalutValue;
    public T curValue;
    public Value(T defalutValue)
    {
        this.defalutValue = defalutValue;
        curValue = this.defalutValue;
    }
}
