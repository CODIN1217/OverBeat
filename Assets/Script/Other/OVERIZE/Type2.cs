using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Type2<T, U>
{
    public T V1;
    public U V2;
    public Type2(T t, U u)
    {
        this.V1 = t;
        this.V2 = u;
    }
}
