using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Unitilities
{

    [System.Serializable]
    public class NamedPool
    {
        public string name;
        public GameObjectPool pool;
        public NamedPool(string name, GameObjectPool pool)
        {
            this.name = name;
            this.pool = pool;
        }
        public static implicit operator GameObjectPool(NamedPool namedPool)
        {
            return namedPool.pool;
        }
    }

    /// <summary>
    /// 针对GameObject的对象池
    /// </summary>
    public class GameObjectPool : MonoBehaviour
    {
        /// <summary>
        /// 命名模式
        /// </summary>
        public enum ObjectNamingMode
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

        public enum PrefabSelectMode
        {
            /// <summary>
            /// 从0开始循环选取
            /// </summary>
            Loop,
            /// <summary>
            /// 随机选取
            /// </summary>
            Random
        }
        /// <summary>
        /// 允许扩充对象池
        /// </summary>
        public bool canExtend = false;
        public ObjectNamingMode instanciateMode = ObjectNamingMode.Index;
        public PrefabSelectMode prefabSelectMode = PrefabSelectMode.Loop;
        /// <summary>
        /// 预制体
        /// </summary>
        public GameObject[] prefabs;
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

        private int _currentPrefabIndex;

        /// <summary>
        /// 在对象池中创建新的GameObject
        /// </summary>
        private void CreateNew()
        {
            var prefab = SelectPrefab();// TODO;
            // 创建并设置 Transform
            GameObject temp = Instantiate(prefab, transform, false);
            // 禁用
            temp.SetActive(false);
            inactiveObjects.Add(temp);
            poolCapacity++;
            // 重命名
            switch(instanciateMode)
            {
                case ObjectNamingMode.Raw:
                    // Nothing
                    break;
                case ObjectNamingMode.Origin:
                    temp.name = prefab.name;
                    break;
                case ObjectNamingMode.Index:
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
            toPush.transform.SetParent(transform);
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

        private GameObject SelectPrefab()
        {
            switch (prefabSelectMode)
            {
                case PrefabSelectMode.Loop:
                    _currentPrefabIndex++;
                    _currentPrefabIndex %= prefabs.Length;
                    break;
                case PrefabSelectMode.Random:
                    _currentPrefabIndex = Random.Range(0, prefabs.Length);
                    break;
                default:
                    _currentPrefabIndex = 0;
                    break;
            }
            return prefabs[_currentPrefabIndex];
        }
    }
}