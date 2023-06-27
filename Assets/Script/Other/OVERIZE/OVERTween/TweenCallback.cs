using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OVERIZE
{
    public class TweenCallback
    {
        CallBack onPlay = () => { };
        internal CallBack OnPlay { get => onPlay; set => onPlay = value; }
        CallBack onUpdate = () => { };
        internal CallBack OnUpdate { get => onUpdate; set => onUpdate = value; }
        CallBack onComplete = () => { };
        internal CallBack OnComplete { get => onComplete; set => onComplete = value; }
        CallBack onCompleteLoop = () => { };
        internal CallBack OnCompleteLoop { get => onCompleteLoop; set => onCompleteLoop = value; }
    }
}