using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Unitilities.Effect
{

    public class ShowUp : MonoBehaviour, IEffect
    {
        [SerializeField] private Transform target = default;
        private event Action finish;
        /// <summary>
        /// 实现动画的协程实例, 保证同一时间只有一个
        /// </summary>
        private IEnumerator runningLoop;
        public float effectTime = 0.5f;
        public Vector3 targetScale = Vector3.one;

        private IEnumerator Loop()
        {
            float usedTime;
            for(usedTime = 0f; usedTime < effectTime; usedTime += Time.deltaTime)
            {
                target.localScale = Vector3.Lerp(Vector3.zero, targetScale,
                    MathTool.SpeedUpLerp(0, 1, usedTime / effectTime));
                yield return null;
            }
            OnFinish();
        }

        [ContextMenu("Play")]
        public void Play()
        {
            if (runningLoop != null) StopCoroutine(runningLoop);
            StartCoroutine(runningLoop = Loop());
        }

        [ContextMenu("Stop")]
        public void Stop()
        {
            if (runningLoop != null) StopCoroutine(runningLoop);
            runningLoop = null;
            OnFinish();
        }

        public void AddFinishListener(Action action)
        {
            finish += action;
        }

        protected virtual void OnFinish()
        {
            target.localScale = targetScale;
            finish?.Invoke();
        }
    }

}