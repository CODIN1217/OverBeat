using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OVERIZE
{
    public enum UpdateMode { Manual, Fixed, Nomal, Late }
    [ExecuteInEditMode]
    partial class TweenUpdater : MonoBehaviour
    {
        internal static float time;
        internal bool isExecuteInEditMode;
        internal UpdateMode updateMode = UpdateMode.Nomal;

        bool isUpdated;
        void FixedUpdate()
        {
            if (!isExecuteInEditMode || updateMode != UpdateMode.Fixed)
                return;
            UpdateTweens();
        }
        void Update()
        {
            if (!isExecuteInEditMode || updateMode != UpdateMode.Nomal)
                return;
            UpdateTweens();
        }
        void LateUpdate()
        {
            if (!isExecuteInEditMode || updateMode != UpdateMode.Late)
                return;
            UpdateTweens();
        }
        public void UpdateTweens()
        {
            if (!isUpdated)
            {
                foreach (var TC in TweenCore.TweenChains)
                {
                    foreach (var TS in TC.TweenSettings)
                    {
                        TS.Time = time - TS.startTime;
                        TS.Update();
                    }
                }
                isUpdated = true;
                StartCoroutine(SetIsUpdated());
            }
        }
        IEnumerator SetIsUpdated()
        {
            yield return new WaitForEndOfFrame();
            isUpdated = false;
        }
    }
}
