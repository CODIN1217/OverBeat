using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OVERIZE
{
    public class TweenID
    {
        int id;
        public int ID { get => id; }
        string name;
        public string Name { get => name; internal set => name = value;}
        public TweenID(int id, string name = "")
        {
            this.id = id;
            this.name = name;
        }
        public static implicit operator int(TweenID tweenID) => tweenID.ID;
        public static implicit operator string(TweenID tweenID) => tweenID.Name;
        public static implicit operator TweenID(int id) => new TweenID(id);
    }
}
