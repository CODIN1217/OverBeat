using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OVERIZE
{
    public abstract class Updater : MonoBehaviour
    {
        CallBack onUpdate = () => { };
        public CallBack OnUpdate { get => onUpdate; set => onUpdate = value; }
        static Dictionary<Type, GameObject> instances = new Dictionary<Type, GameObject>();
        public static T Member<T>()
        where T : Updater
        {
            if (!instances.ContainsKey(typeof(T)))
                instances.Add(typeof(T), null);
            instances[typeof(T)] ??= new GameObject("Updater", typeof(T), typeof(DontDestroyOnLoad));
            return instances[typeof(T)].GetComponent<T>();
        }
        protected virtual void FixedUpdate()
        {
            Update(UpdateMode.Fixed);
        }
        protected virtual void Update()
        {
            Update(UpdateMode.Nomal);
            onUpdate();
        }
        protected virtual void LateUpdate()
        {
            Update(UpdateMode.Late);
        }
        protected abstract void Update(UpdateMode updateMode);
    }
}