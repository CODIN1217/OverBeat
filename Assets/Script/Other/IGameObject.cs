using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IGameObject
{
    void InitGameObjectScript();
    void UpdateTransform();
    void UpdateRenderer();
}
