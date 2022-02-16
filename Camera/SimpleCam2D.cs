using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Unitilities.Camera
{
    /// <summary>
    /// �򵥵� 2D �����, ���Ը��ٶ��Ŀ��, �����˶��ھ��η�Χ��.
    /// <para>����������z�Ḻ��������ӽ������, �����������Ϊ���ܲ�����Ԥ��</para>
    /// </summary>
    public class SimpleCam2D : MonoBehaviour
    {
        /// <summary>
        /// �������Ŀ�꼰��ز���
        /// </summary>
        [Serializable]
        public struct CamFollowed
        {
            /// <summary>
            /// ����Ŀ��
            /// </summary>
            public Transform transform;
            /// <summary>
            /// ƫ����
            /// </summary>
            public Vector3 offset;
            /// <summary>
            /// ���ٶ����Ȩ��
            /// </summary>
            [Min(0)] public float weight;
        }

        public List<CamFollowed> targets = new List<CamFollowed>();
        /// <summary>
        /// ����Ȩ��
        /// </summary>
        [Min(0)] public float mouseWeight;
        /// <summary>
        /// �ӽ�����
        /// </summary>
        [Min(0)] public float approachRate;
        /// <summary>
        /// �����Χ
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

        private void Awake()
        {
            // �����ұ������, ������������, ����� Camera.main
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

            // �������ʽ����ʵ���ڲ�ͬ��֡��������ͬ��������ٶ�, ���ٶȿɿ�
            var t = Mathf.Exp(-approachRate * Time.deltaTime);
            transform.SetPosition2D(Vector2.Lerp(GetTrackCenter(), transform.Position2D(), t));
        }

        /// <summary>
        /// ��ʼ��������λ��
        /// </summary>
        public void Init() => transform.SetPosition2D(GetTrackCenter());

        /// <summary>
        /// ��ȡ����Ŀ��ļ�Ȩ����, ������
        /// </summary>
        /// <returns></returns>
        private Vector2 GetTrackCenterUnClamp()
        {
            var weightedSum = targets.Aggregate(Vector3.zero, (v, x) => v + x.transform.position * x.weight);
            return weightedSum + _camera.ScreenToWorldPoint(Input.mousePosition) * mouseWeight;
        }

        /// <summary>
        /// ��ȡ����Ŀ��ļ�Ȩ����, ���Ƶ� <see cref="cameraBounds"></see> �ķ�Χ��
        /// </summary>
        /// <returns></returns>
        private Vector2 GetTrackCenter()
        {
            // ��ȥ������ķ�Χ, �������������ʲô�ߴ�, ��ֻ���ڸ÷�Χ������
            return cameraBounds.Shrink(GetCameraBounds().Extents).Clamp(GetTrackCenterUnClamp());
        }

        /// <summary>
        /// ��鲢�淶��Ȩ��, ʹ���Ϊ1.
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
        /// ��ȡ�������Χ
        /// </summary>
        /// <returns></returns>
        private Bounds2D GetCameraBounds() => new Bounds2D(transform.Position2D(), cameraSize);
    }

}