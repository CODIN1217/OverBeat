using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace OVERIZE
{
    public class TweenChain
    {
        List<List<TweenSetting>> tweenSettings = new List<List<TweenSetting>>();
        List<float> delays = new List<float>() { 0f };
        CallBack onAddTween = () => { };
        public CallBack OnAddTween(CallBack callBack) => onAddTween += callBack;
        List<TweenSetting> tweenSettingsTemp = new List<TweenSetting>();
        internal List<TweenSetting> TweenSettings { get => tweenSettingsTemp; }
        public TweenChain()
        {
            OnAddTween(() => tweenSettingsTemp = GetTweenSettings());
        }
        public static implicit operator TweenChain(TweenSetting tweenSetting) => new TweenChain().Append(tweenSetting);
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