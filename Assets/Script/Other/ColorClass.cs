using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class ColorClass : OVERIZE.IDeepCopy<ColorClass>
{
    [SerializeField]
    Color color;
    ColorClass(Color color)
    {
        this.color = color;
    }
    public static explicit operator Color(ColorClass colorClass) => colorClass.color;
    public static explicit operator ColorClass(Color color) => new ColorClass(color);
    public ColorClass Clone() => new ColorClass(color);
}
