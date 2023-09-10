using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

namespace OVERIZE
{
    public class OrderedElement<T>
    {
        int origIndex;
        T value;
        public int OrigIndex { get => origIndex; set => origIndex = value; }
        public T Value { get => value; set => this.value = value; }
        public OrderedElement(int origIndex, T value)
        {
            OrigIndex = origIndex;
            Value = value;
        }
    }
    /* public class OrderedArray<T>
    {
        OrderedElement<T>[] orderedArray;
        public OrderedArray(T[] array, bool descendingOrder)
        {
            orderedArray = array.Select((item, index) => new OrderedElement<T>(index, item)).OrderBy(e => e.Value).ToArray();
            if (descendingOrder)
                orderedArray.Reverse();
        }
    } */
}