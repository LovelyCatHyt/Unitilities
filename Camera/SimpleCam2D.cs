using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Unitilities.Camera
{
    /// <summary>
    /// 简单的 2D 摄像机, 可以跟踪多个目标, 限制运动在矩形范围内.
    /// <para>必须是面向z轴负向的正交视角摄像机, 其它情况的行为可能不符合预期</para>
    /// </summary>
    public class SimpleCam2D : MonoBehaviour
    {
        /// <summary>
        /// 被跟随的目标及相关参数
        /// </summary>
        [Serializable]
        public struct CamFollowed
        {
            /// <summary>
            /// 跟踪目标
            /// </summary>
            public Transform transform;
            /// <summary>
            /// 偏移量
            /// </summary>
            public Vector3 offset;
            /// <summary>
            /// 跟踪对象的权重
            /// </summary>
            [Min(0)] public float weight;
        }

        public List<CamFollowed> targets = new List<CamFollowed>();
        /// <summary>
        /// 鼠标的权重
        /// </summary>
        [Min(0)] public float mouseWeight;
        /// <summary>
        /// 接近速率
        /// </summary>
        [Min(0)] public float approachRate;
        /// <summary>
        /// 相机范围
        /// </summary>
        public Bounds2D cameraBounds;

        private UnityEngine.Camera _camera;

        private Vector2 cameraSize
        {
            get
            {
                var h = 2 * _camera.orthographicSize;
                return new Vector2(_camera.aspect * h, h);
            }
        }

#if UNITY_EDITOR
        private void OnDrawGizmosSelected()
        {
            cameraBounds.DrawGizmos();
        }
#endif
        private void Awake()
        {
            // 优先找本物体的, 否则找子物体, 最后找 Camera.main
            _camera = GetComponent<UnityEngine.Camera>();
            if (!_camera)
            {
                _camera = GetComponentInChildren<UnityEngine.Camera>();
            }
            if (!_camera)
            {
                _camera = UnityEngine.Camera.main;
            }
            Debug.Assert(_camera, "Camera not found!");
            NormalizeWeights();
            Init();
        }

        private void Update()
        {

            // 用这个公式可以实现在不同的帧率下有相同的摄像机速度, 且速度可控
            var t = Mathf.Exp(-approachRate * Time.deltaTime);
            transform.SetPosition2D(Vector2.Lerp(GetTrackCenter(), transform.Position2D(), t));
        }

        /// <summary>
        /// 初始化到中心位置
        /// </summary>
        public void Init() => transform.SetPosition2D(GetTrackCenter());

        /// <summary>
        /// 获取跟踪目标的加权中心, 无限制
        /// </summary>
        /// <returns></returns>
        private Vector2 GetTrackCenterUnClamp()
        {
            var weightedSum = targets.Aggregate(Vector3.zero, (v, x) => v + x.transform.position * x.weight);
            return weightedSum + _camera.ScreenToWorldPoint(Input.mousePosition) * mouseWeight;
        }

        /// <summary>
        /// 获取跟踪目标的加权中心, 限制到 <see cref="cameraBounds"></see> 的范围内
        /// </summary>
        /// <returns></returns>
        private Vector2 GetTrackCenter()
        {
            // 减去摄像机的范围, 这样无论摄像机什么尺寸, 都只会在该范围内拍摄
            return cameraBounds.Shrink(GetCameraBounds().Extents).Clamp(GetTrackCenterUnClamp());
        }

        /// <summary>
        /// 检查并规范化权重, 使其和为1.
        /// </summary>
        private void NormalizeWeights()
        {
            float sum = targets.Aggregate(0f, (f, x) => f + x.weight) + mouseWeight;
            for (var i = 0; i < targets.Count; i++)
            {
                var temp = targets[i];
                temp.weight /= sum;
                targets[i] = temp;
                if (temp.weight == 0)
                {
                    Debug.LogWarning($"tracking target of {name}: {temp.transform.name} has zero weight. Position of this target will not affect the camera.");
                }
            }

            mouseWeight /= sum;
        }

        /// <summary>
        /// 获取摄像机范围
        /// </summary>
        /// <returns></returns>
        private Bounds2D GetCameraBounds() => new Bounds2D(transform.Position2D(), cameraSize);
    }

}
