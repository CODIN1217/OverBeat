using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OVERIZE
{
    public interface ITweenBehavior
    {
        void Init();
        ITween InitLoop();
        ITween Play();
        ITween Pause();
        ITween Rewind();
        ITween ManualUpdate();
        ITween Complete();
        ITween CompleteLoop();
        ITween Kill();
    }
}
