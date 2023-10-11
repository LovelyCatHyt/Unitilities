using UnityEngine;

namespace Unitilities
{
    public static class MathTool
    {
        /// <summary>
        /// 匀加速到达b, 使用y=x²
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <param name="t"></param>
        /// <returns></returns>
        public static float SpeedUpLerp(float a, float b, float t)
        {
            t = t * t;
            return Mathf.Lerp(a, b, t);
        }
        /// <summary>
        /// 匀减速到达b, 使用y=-x²+2x
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <param name="t"></param>
        /// <returns></returns>
        public static float SpeedDownLerp(float a, float b, float t)
        {
            t = -t * (t + 2);
            return Mathf.Lerp(a, b, t);
        }

        /// <summary>
        /// 按分量计算出正确的最小最大值并以 out 参数返回
        /// </summary>
        public static void CorrectMinMax(Vector2Int a, Vector2Int b, out Vector2Int min, out Vector2Int max)
        {
            min = Vector2Int.Min(a, b); max = Vector2Int.Max(a, b);
        }

        /// <summary>
        /// 按分量计算出正确的最小最大值并以 out 参数返回
        /// </summary>
        public static void CorrectMinMax(Vector2 a, Vector2 b, out Vector2 min, out Vector2 max)
        {
            min = Vector2.Min(a, b); max = Vector2.Max(a, b);
        }

        /// <summary>
        /// 按分量计算出正确的最小最大值并以 out 参数返回
        /// </summary>
        public static void CorrectMinMax(Vector3Int a, Vector3Int b, out Vector3Int min, out Vector3Int max)
        {
            min = Vector3Int.Min(a, b); max = Vector3Int.Max(a, b);
        }

        /// <summary>
        /// 按分量计算出正确的最小最大值并以 out 参数返回
        /// </summary>
        public static void CorrectMinMax(Vector3 a, Vector3 b, out Vector3 min, out Vector3 max)
        {
            min = Vector3.Min(a, b); max = Vector3.Max(a, b);
        }
    }
}
