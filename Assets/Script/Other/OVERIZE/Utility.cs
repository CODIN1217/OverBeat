using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OVERIZE
{
    public static class Utility
    {
        public static void Swap<T>(ref T item1, ref T item2)
        {
            T item_temp = item1;
            item1 = item2;
            item2 = item_temp;
        }
        public static void Swap<T>(T item1, T item2, Setter<T> item1Setter, Setter<T> item2Setter)
        {
            T item_temp = item1;
            item1Setter(item2);
            item2Setter(item_temp);
        }
        public static (T, T) Swap<T>(T item1, T item2)
        {
            T item_temp = item1;
            item1 = item2;
            item2 = item_temp;
            return (item1, item2);
        }
    }
}