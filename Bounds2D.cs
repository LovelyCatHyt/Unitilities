using System;
using Newtonsoft.Json;
using UnityEngine;


namespace Unitilities
{
    /// <summary>
    /// 2D 的平行于坐标轴的包围盒
    /// </summary>
    [Serializable]
    public struct Bounds2D
    {
        /// <summary>
        /// 中心
        /// </summary>
        [JsonIgnore]
        public Vector2 Center
        {
            get => _center;
            set => _center = value;
        }
        /// <summary>
        /// 半长宽
        /// </summary>
        [JsonIgnore]
        public Vector2 Extents
        {
            get => _extents;
            set => _extents = new Vector2(Mathf.Abs(value.x), Mathf.Abs(value.y));
        }
        /// <summary>
        /// 坐标最小点
        /// </summary>
        [JsonIgnore]
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
        /// 坐标最大点
        /// </summary>
        [JsonIgnore]
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
        /// 大小
        /// </summary>
        [JsonIgnore]
        public Vector2 Size
        {
            get => _extents * 2f;
            set => Extents = value * .5f;
        }

        [JsonProperty("center")] [SerializeField] private Vector2 _center;
        [JsonProperty("extents")] [SerializeField] private Vector2 _extents;

        public Bounds2D(Vector2 center, Vector2 size)
        {
            _center = center;
            _extents = size * .5f;
        }

        public bool Equals(Bounds2D other)
        {
            return _center.Equals(other._center) && _extents.Equals(other._extents);
        }

        public override bool Equals(object other)
        {
            if (other == null) return false;
            try
            {
                var b = (Bounds2D)other;
                return _center.Equals(b._center) && _extents.Equals(b._extents);
            }
            catch (InvalidCastException)
            {
                return false;
            }
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (_center.GetHashCode() * 397) ^ _extents.GetHashCode();
            }
        }

        public static bool operator ==(Bounds2D a, Bounds2D b)
        {
            return a._center == b._center && a._extents == b._extents;
        }

        public static bool operator !=(Bounds2D a, Bounds2D b)
        {
            return a._center != b._center || a._extents != b._extents;
        }

        /// <summary>
        /// 点是否在范围内, 包括边界
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
        /// 扩大范围, 使得该范围包围指定点
        /// </summary>
        /// <param name="point"></param>
        public Bounds2D Encapsulate(Vector2 point)
        {
            var min_x = Mathf.Min(Min.x, point.x);
            var min_y = Mathf.Min(Min.y, point.y);
            var max_x = Mathf.Max(Max.x, point.x);
            var max_y = Mathf.Max(Max.y, point.y);
            var b = new Bounds2D();
            b.Min = new Vector2(min_x, min_y);
            b.Max = new Vector2(max_x, max_y);
            return b;
        }

        /// <summary>
        /// 将点坐标限制到范围内
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
        /// 沿中心缩小 amount 大小的尺寸. 任意方向上的尺寸最小为0.
        /// </summary>
        /// <param name="amount"></param>
        public Bounds2D Shrink(Vector2 amount)
        {
            var temp = this;
            temp.Extents -= amount;
            return temp;
        }

        public void DrawGizmos(Color c)
        {
            var lB = new Vector3(Min.x, Min.y, 0);
            var rB = new Vector3(Max.x, Min.y, 0);
            var rT = new Vector3(Max.x, Max.y, 0);
            var lT = new Vector3(Min.x, Max.y, 0);
            Gizmos.color = c;
            Gizmos.DrawLine(lB, rB);
            Gizmos.DrawLine(rB, rT);
            Gizmos.DrawLine(rT, lT);
            Gizmos.DrawLine(lT, lB);
        }

        public void DrawGizmos()
        {
            DrawGizmos(Color.cyan);
        }

        public static implicit operator Bounds2D(BoundsInt bInt)
        {
            return new Bounds2D(bInt.center, (Vector3)bInt.size);
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
