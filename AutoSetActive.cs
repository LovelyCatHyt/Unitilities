using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Unitilities
{
    /// <summary>
    /// 运行时自动设置 Active, 设置完了之后自动销毁该组件, 假装无事发生
    /// </summary>
    public class AutoSetActive : MonoBehaviour
    {
        public bool activeValue;
        [Tooltip("在 Awake 中设置, false 则在 Start 中设置")]
        public bool runAtAwake;

        private void Awake()
        {
            if(runAtAwake) gameObject.SetActive(activeValue);
        }

        void Start()
        {
            if(!runAtAwake) gameObject.SetActive(activeValue);
            Destroy(this);
        }
    }

}
