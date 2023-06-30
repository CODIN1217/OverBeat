using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OVERIZE
{
    public class TweenCallback
    {
        protected CallBack onStart = () => { };
        protected CallBack onPlay = () => { };
        protected CallBack onUpdate = () => { };
        protected CallBack onComplete = () => { };
        protected CallBack onCompleteLoop = () => { };
        public virtual void Init()
        {
            onStart = TweenCore.TweenPreference.onStart;
            onPlay = TweenCore.TweenPreference.onPlay;
            onUpdate = TweenCore.TweenPreference.onUpdate;
            onComplete = TweenCore.TweenPreference.onComplete;
            onCompleteLoop = TweenCore.TweenPreference.onCompleteLoop;
        }
        public TweenCallback OnStart(CallBack callBack)
        {
            onStart += callBack;
            return this;
        }
        public TweenCallback OnPlay(CallBack callBack)
        {
            onPlay += callBack;
            return this;
        }
        public TweenCallback OnUpdate(CallBack callBack)
        {
            onUpdate += callBack;
            return this;
        }
        public TweenCallback OnComplete(CallBack callBack)
        {
            onComplete += callBack;
            return this;
        }
        public TweenCallback OnCompleteLoop(CallBack callBack)
        {
            onCompleteLoop += callBack;
            return this;
        }
    }
}