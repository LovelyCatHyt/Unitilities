using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Unitilities.Effect
{
    public class ColorChange : MonoBehaviour, IEffect
    {
        public MeshRenderer targetRenderer;
        public float changeTime = 0.5f;
        public Color targetColor;
        /// <summary>
        /// 实现动画的协程, 保证同一时间仅一个协程实例
        /// </summary>
        private IEnumerator loopCorountine;
        private event Action Finish;

        private IEnumerator Loop()
        {
            Color originColor = targetRenderer.material.color;
            for(float timeUsed = 0f; timeUsed < changeTime; timeUsed += Time.deltaTime)
            {
                targetRenderer.material.color = Color.Lerp(originColor, targetColor, timeUsed / changeTime);
                yield return null;
            }
            OnFinish();
        }

        /// <summary>
        /// 重设颜色
        /// </summary>
        public void ResetColor(Color target)
        {
            targetColor = target;
        }

        public void Play()
        {
            if (loopCorountine != null) StopCoroutine(loopCorountine);
            StartCoroutine(loopCorountine = Loop());
        }

        public void Stop()
        {
            if (loopCorountine != null) StopCoroutine(loopCorountine);
            OnFinish();
        }


        public void AddFinishListener(Action action)
        {
            Finish += action;
        }

        protected virtual void OnFinish()
        {
            targetRenderer.material.color = targetColor;
            Finish?.Invoke();
        }
    }

}