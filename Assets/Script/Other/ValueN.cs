using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct ValueN<T>
{
    T[] values;
    public T this[int index] { get { return values[index]; } set { values[index] = value; } }
    public ValueN(int count)
    {
        values = new T[count];
    }
    public ValueN(params T[] values)
    {
        this.values = values;
    }
}
