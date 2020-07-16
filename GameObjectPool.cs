using System.Collections.Generic;
using UnityEngine;

namespace Unitilities
{

    /// <summary>
    /// 针对GameObject的对象池
    /// </summary>
    public class GameObjectPool : MonoBehaviour
    {
        /// <summary>
        /// 命名模式
        /// </summary>
        public enum InstanciateNameMode
        {
            /// <summary>
            /// 啥都不干,实例化结果是咋样就咋样
            /// </summary>
            Raw,
            /// <summary>
            /// 使用预制体原本的名字
            /// </summary>
            Origin,
            /// <summary>
            /// 打上索引标签
            /// </summary>
            Index
        }
        /// <summary>
        /// 允许扩充对象池
        /// </summary>
        public bool canExtend = false;
        public InstanciateNameMode mode = InstanciateNameMode.Index;
        /// <summary>
        /// 预制体
        /// </summary>
        public GameObject prefab;
        /// <summary>
        /// 不活跃的GameObject列表
        /// </summary>
        public List<GameObject> inactiveObjects = new List<GameObject>();
        /// <summary>
        /// 活跃的GameObject列表
        /// </summary>
        public List<GameObject> activeObjects = new List<GameObject>();
        ///<summary>
        /// 对象池当前容量
        /// </summary>
        public int poolCapacity { get; private set; } = 0;

        /// <summary>
        /// 对象池初始数量
        /// </summary>
        public int initObject = 10;

        /// <summary>
        /// 在对象池中创建新的GameObject
        /// </summary>
        private void CreateNew()
        {
            GameObject temp = Instantiate(prefab) as GameObject;
            //禁用并添加到子transform
            temp.SetActive(false);
            temp.transform.SetParent(transform, false);
            inactiveObjects.Add(temp);
            poolCapacity++;
            //重命名
            switch(mode)
            {
                case InstanciateNameMode.Raw:
                    //Nothing
                    break;
                case InstanciateNameMode.Origin:
                    temp.name = prefab.name;
                    break;
                case InstanciateNameMode.Index:
                    temp.name = prefab.name + "(" + (poolCapacity - 1).ToString() + ")";
                    break;
            }
        }

        public void Awake()
        {
            //创建对象池
            for(int i = 0; i < initObject; i++)
            {
                CreateNew();
            }
        }

        /// <summary>
        /// 尝试取出一个GameObject,对象池用尽则返回false
        /// </summary>
        /// <param name="output"></param>
        /// <returns></returns>
        public bool Pop(out GameObject output)
        {
            if(inactiveObjects.Count == 0)
            {
                if(canExtend)
                {
                    CreateNew();
                }
                else
                {
                    //不可扩充
                    output = null;
                    return false;
                }
            }
            output = inactiveObjects[inactiveObjects.Count - 1];
            inactiveObjects.RemoveAt(inactiveObjects.Count - 1);
            activeObjects.Add(output);
            output.SetActive(true);
            return true;
        }

        /// <summary>
        /// 尝试回收一个GameObject,本对象池中不存在则返回false
        /// </summary>
        /// <param name="toPush"></param>
        /// <returns></returns>
        public bool Push(GameObject toPush)
        {
            if(!activeObjects.Contains(toPush))
            {
                return false;
            }
            activeObjects.Remove(toPush);
            inactiveObjects.Add(toPush);
            toPush.SetActive(false);
            return true;
        }

        /// <summary>
        /// 回收全部的物体
        /// </summary>
        public void PushAll()
        {
            List<GameObject> tempList = new List<GameObject>(activeObjects);
            tempList.ForEach(obj => Push(obj));
        }
    }
}