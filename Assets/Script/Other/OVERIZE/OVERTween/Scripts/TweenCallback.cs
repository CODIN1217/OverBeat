using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OVERIZE
{
    public class TweenCallback : ITweenCallback
    {
        CallBack onStart = () => { };
        CallBack onPlay = () => { };
        CallBack onPause = () => { };
        CallBack onRewind = () => { };
        CallBack onUpdate = () => { };
        CallBack onComplete = () => { };
        CallBack onCompleteLoop = () => { };
        public virtual void Init()
        {
            onStart = TweenCore.TweenPreference.onStart;
            onPlay = TweenCore.TweenPreference.onPlay;
            onPause = TweenCore.TweenPreference.onPause;
            onRewind = TweenCore.TweenPreference.onRewind;
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
        public TweenCallback OnPause(CallBack callBack)
        {
            onPause += callBack;
            return this;
        }
        public TweenCallback OnRewind(CallBack callBack)
        {
            onRewind += callBack;
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
        public void OnStart() => onStart();
        public void OnPlay() => onPlay();
        public void OnPause() => onPause();
        public void OnRewind() => onRewind();
        public void OnUpdate() => onUpdate();
        public void OnComplete() => onComplete();
        public void OnCompleteLoop() => onCompleteLoop();
    }
}