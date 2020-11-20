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
    }
}