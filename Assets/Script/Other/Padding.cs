using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct Padding
{
    int left;
    int right;
    int top;
    int bottom;
    public Padding(int left, int right, int top, int bottom)
    {
        this.left = left;
        this.right = right;
        this.top = top;
        this.bottom = bottom;
    }
}