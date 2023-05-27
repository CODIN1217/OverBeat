using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct Value<T>
{
    public readonly T defalutValue;
    public T value;
    public Value(T defalutValue)
    {
        this.defalutValue = defalutValue;
        value = this.defalutValue;
    }
}
