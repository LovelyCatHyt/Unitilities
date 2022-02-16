using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Unitilities
{
    /// <summary>
    /// 2D ��ƽ����������İ�Χ��
    /// </summary>
    [Serializable]
    public struct Bounds2D
    {
        /// <summary>
        /// ����
        /// </summary>
        public Vector2 Center
        {
            get => _center;
            set => _center = value;
        }
        /// <summary>
        /// �볤��
        /// </summary>
        public Vector2 Extents
        {
            get => _extents;
            set => _extents = new Vector2(Mathf.Abs(value.x), Mathf.Abs(value.y));
        }
        /// <summary>
        /// ������С��
        /// </summary>
        public Vector2 Min
        {
            get => _center - _extents;
            set
            {
                _center = (Max + value) * .5f;
                _extents = _center - value;
            }
        }
        /// <summary>
        /// ��������
        /// </summary>
        public Vector2 Max
        {
            get => _center + _extents;
            set
            {
                _center = (Min + value) * .5f;
                _extents = value - _center;
            }
        }
        /// <summary>
        /// ��С
        /// </summary>
        public Vector2 Size
        {
            get => _extents * 2f;
            set => Extents = value * .5f;
        }

        [SerializeField] private Vector2 _center;
        [SerializeField] private Vector2 _extents;

        public Bounds2D(Vector2 center, Vector2 size)
        {
            _center = center;
            _extents = size * .5f;
        }

        /// <summary>
        /// ���Ƿ��ڷ�Χ��, �����߽�
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        public bool Contains(Vector2 point)
        {
            var min = Min;
            var max = Max;
            return min.x <= point.x && point.x <= max.x &&
                   min.y <= point.y && point.y <= max.y;
        }

        /// <summary>
        /// ����Χ, ʹ�ø÷�Χ��Χָ����
        /// </summary>
        /// <param name="point"></param>
        public void Encapsulate(Vector2 point)
        {
            var min_x = Mathf.Min(Min.x, point.x);
            var min_y = Mathf.Min(Min.y, point.y);
            var max_x = Mathf.Max(Max.x, point.x);
            var max_y = Mathf.Max(Max.y, point.y);
            Min = new Vector2(min_x, min_y);
            Max = new Vector2(max_x, max_y);
        }

        /// <summary>
        /// �����������Ƶ���Χ��
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        public Vector2 Clamp(Vector2 point)
        {
            var min = Min;
            var max = Max;
            point.x = Mathf.Clamp(point.x, min.x, max.x);
            point.y = Mathf.Clamp(point.y, min.y, max.y);
            return point;
        }

        /// <summary>
        /// ��������С amount ��С�ĳߴ�. ���ⷽ���ϵĳߴ���СΪ0.
        /// </summary>
        /// <param name="amount"></param>
        public Bounds2D Shrink(Vector2 amount)
        {
            var temp = this;
            temp.Extents -= amount;
            return temp;
        }

        public static implicit operator Bounds(Bounds2D b2d)
        {
            return new Bounds(b2d._center, b2d.Size);
        }

        public static implicit operator Bounds2D(Bounds b3d)
        {
            return new Bounds2D(b3d.center, b3d.size);
        }
    }

}