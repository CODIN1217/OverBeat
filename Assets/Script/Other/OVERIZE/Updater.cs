using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OVERIZE
{
    public abstract class Updater : MonoBehaviour
    {
        CallBack onUpdate = () => { };
        // protected CallBack OnUpdate() => onUpdate;
        // public Updater OnUpdate(CallBack onUpdate)
        // {
        //     this.onUpdate += onUpdate;
        //     return this;
        // }
        public CallBack OnUpdate { get => onUpdate; set => onUpdate = value; }
        // UpdateMode updateMode;
        // protected UpdateMode UpdateMode { get => updateMode; set => updateMode = value; }
        // protected void FixedUpdate(UpdateMode updateMode)
        // {
        //     if ((updateMode & UpdateMode.Fixed) != 0)
        //         onUpdate();
        // }
        // protected virtual void Update() => onUpdate();
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
        // protected void LateUpdate(UpdateMode updateMode)
        // {
        //     if ((updateMode & UpdateMode.Late) != 0)
        //         onUpdate();
        // }
        // internal virtual void ManualUpdate(UpdateMode updateMode, float deltaTime)
        // {
        //     if (UpdateMode == 0)
        //         onUpdate();
        // }
        // internal void ManualUpdate(UpdateMode updateMode)
        // {
        //     if (updateMode == 0)
        //         onUpdate();
        // }
    }
}