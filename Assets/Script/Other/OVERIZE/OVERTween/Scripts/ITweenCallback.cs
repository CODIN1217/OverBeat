using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OVERIZE
{
    public interface ITweenCallback
    {
        TweenCallback OnStart(CallBack callBack);
        TweenCallback OnPlay(CallBack callBack);
        TweenCallback OnPause(CallBack callBack);
        TweenCallback OnRewind(CallBack callBack);
        TweenCallback OnUpdate(CallBack callBack);
        TweenCallback OnComplete(CallBack callBack);
        TweenCallback OnCompleteLoop(CallBack callBack);
        void OnStart();
        void OnPlay();
        void OnPause();
        void OnRewind();
        void OnUpdate();
        void OnComplete();
        void OnCompleteLoop();
    }
}
