using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct Property<T>
{
    T value;
    public T Value(T value) => this.value = value;
}
