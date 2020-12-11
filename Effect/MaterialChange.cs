using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Unitilities.Effect
{
    /// <summary>
    /// 材质渐变器
    /// </summary>
    public class MaterialChange : MonoBehaviour, IEffect
    {
        [SerializeField] private float duration = 1f;
        /// <summary>
        /// 要控制的渲染器
        /// </summary>
        public MeshRenderer targetRenderer;
        /// <summary>
        /// 目标材质
        /// </summary>
        public Material targetMaterial;
        /// <summary>
        /// 持续时间
        /// </summary>
        public float Duration
        {
            get => duration;
            set
            {
                if (value < 0) value = 0.1f;
                duration = value;
            }
        }
        public event Action onEffectFinished; 

        private IEnumerator Loop()
        {
            Material origin = new Material(targetRenderer.material);
            for(float time = 0; time < Duration; time += Time.deltaTime)
            {
                targetRenderer.material.Lerp(origin, targetMaterial, time / Duration);
                yield return null;
            }
            Finish();
        }

        private void Finish()
        {
            onEffectFinished?.Invoke();
            targetRenderer.material = targetMaterial;
        }

        public void Play()
        {
            StopAllCoroutines();
            StartCoroutine(Loop());
        }

        public void Stop()
        {
            StopAllCoroutines();
            Finish();
        }

        public void AddFinishListener(Action action)
        {
            onEffectFinished += action;
        }
    }

}