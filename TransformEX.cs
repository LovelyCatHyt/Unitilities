using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Unitilities
{
    /// <summary>
    /// Transform 的一些额外方法
    /// </summary>
    public static class TransformEX
    {
        /// <summary>
        /// 获取2D坐标
        /// </summary>
        /// <param name="tran"></param>
        /// <returns></returns>
        public static Vector2 Position2D(this Transform tran) => tran.position;

        /// <summary>
        /// 设置2D坐标
        /// </summary>
        /// <param name="tran"></param>
        /// <param name="pos"></param>
        public static void SetPosition2D(this Transform tran, Vector2 pos)
        {
            var z = tran.position.z;
            tran.position = new Vector3(pos.x, pos.y, z);
        }
    }

}