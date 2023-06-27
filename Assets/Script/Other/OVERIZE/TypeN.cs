using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct TypeN<T>
{
    T[] values;
    public T this[int index] { get { return values[index]; } set { values[index] = value; } }
    public TypeN(int count)
    {
        values = new T[count];
    }
    public TypeN(params T[] values)
    {
        this.values = values;
    }
}
