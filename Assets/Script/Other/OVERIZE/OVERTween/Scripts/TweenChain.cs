using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace OVERIZE
{
    public class TweenChain : Tween
    {
        List<List<TweenSetting>> tweenSettings;
        List<float> delays;

        CallBack onAddTween;
        public TweenChain OnAddTween(CallBack callBack)
        {
            onAddTween += callBack;
            return this;
        }

        List<TweenSetting> tweenSettingsTemp;
        internal List<TweenSetting> TweenSettings { get => tweenSettingsTemp; }
        public TweenChain()
        {
            OnAddTween(() => tweenSettingsTemp = GetTweenSettings());
        }
        public override void Init()
        {
            base.Init();
            tweenSettings = new List<List<TweenSetting>>();
            delays = new List<float>() { 0f };
            onAddTween = () => { };
            tweenSettingsTemp = new List<TweenSetting>();
        }
        public override void Play()
        {
            base.Play();
            TweenUpdater.Member.StartCoroutine(PlayCo());
        }
        int tweenPlayIndex = 0;
        IEnumerator PlayCo()
        {
            float delay = delays[0];
            yield return IsUnscaledTime ? new WaitForSecondsRealtime(delay) : new WaitForSeconds(delay);
            for (; tweenPlayIndex < tweenSettings.Count; tweenPlayIndex++)
            {
                foreach (var TS in tweenSettings[tweenPlayIndex])
                    TS.Play();
                yield return new WaitUntil(() => tweenSettings[tweenPlayIndex][0].IsComplete);
                delay = delays[tweenPlayIndex + 1];
                yield return IsUnscaledTime ? new WaitForSecondsRealtime(delay) : new WaitForSeconds(delay);
            }
            bool isComplete = true;
            do
            {
                foreach (var TS in TweenSettings)
                    isComplete = TS.IsComplete;
                yield return new WaitForEndOfFrame();
            }
            while (!isComplete);
            Complete();
        }
        public TweenChain Prepend(TweenSetting tweenSetting)
        {
            tweenSettings.Insert(0, new List<TweenSetting> { tweenSetting });
            delays.Insert(0, 0f);
            onAddTween();
            return this;
        }
        public TweenChain Join(TweenSetting tweenSetting)
        {
            if (tweenSettings.Count > 0)
                tweenSettings[tweenSettings.Count - 1].Add(tweenSetting);
            onAddTween();
            return this;
        }
        public TweenChain Append(TweenSetting tweenSetting)
        {
            tweenSettings.Add(new List<TweenSetting> { tweenSetting });
            delays.Add(0f);
            onAddTween();
            return this;
        }
        public TweenChain PrependDelay(float delay)
        {
            delays[0] += delay;
            return this;
        }
        public TweenChain AppendDelay(float delay)
        {
            delays[delays.Count - 1] += delay;
            return this;
        }
        List<TweenSetting> GetTweenSettings()
        {
            List<TweenSetting> tweens = new List<TweenSetting>();
            for (int i = 0; i < tweenSettings.Count; i++)
            {
                for (int j = 0; j < tweenSettings[i].Count; j++)
                {
                    tweens.Add(tweenSettings[i][j]);
                }
            }
            return tweens;
        }
    }
}