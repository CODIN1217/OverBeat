using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnusedAble<T>
{
    static T value;
    static bool _isUsing;
    public UnusedAble(bool isUsing)
    {
        _isUsing = isUsing;
    }
    public static explicit operator T(UnusedAble<T> unusedAble)
    {
        if (_isUsing)
        {
            return value;
        }
        else
        {
            return value;
        }
    }
}
