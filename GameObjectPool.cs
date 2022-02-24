using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Unitilities
{

    [Serializable]
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
        /// 不活跃的GameObject列表的列表
        /// </summary>
        public List<List<GameObject>> inactiveObjects = new List<List<GameObject>>();
        /// <summary>
        /// 活跃的GameObject集合的列表
        /// </summary>
        public List<HashSet<GameObject>> activeObjects = new List<HashSet<GameObject>>();
        ///<summary>
        /// 对象池当前容量
        /// </summary>
        public List<int> poolCapacities = new List<int>();

        /// <summary>
        /// 对象池初始数量
        /// </summary>
        public int initObject = 10;

        private int _currentPrefabIndex;

        /// <summary>
        /// 在对象池中创建新的GameObject
        /// </summary>
        private void CreateNew(int prefabIndex)
        {
            var prefab = prefabs[prefabIndex];
            // 创建并设置 Transform
            GameObject temp = Instantiate(prefab, transform, false);
            // 禁用
            temp.SetActive(false);
            inactiveObjects[prefabIndex].Add(temp);
            poolCapacities[prefabIndex]++;
            // 重命名
            switch (instanciateMode)
            {
                case ObjectNamingMode.Raw:
                    // Nothing
                    break;
                case ObjectNamingMode.Origin:
                    temp.name = prefab.name;
                    break;
                case ObjectNamingMode.Index:
                    temp.name = $"{prefab.name} ({poolCapacities[prefabIndex] - 1})";
                    break;
            }
        }

        public void Awake()
        {
            // 按照不同的 prefab 创建对象池
            for (int index = 0; index < prefabs.Length; index++)
            {
                // 新建相应的结构
                inactiveObjects.Add(new List<GameObject>());
                activeObjects.Add(new HashSet<GameObject>());
                poolCapacities.Add(0);
                for (int i = 0; i < initObject; i++)
                {
                    CreateNew(index);
                }
            }
        }

        /// <summary>
        /// 尝试取出一个 GameObject, 对象池用尽则返回 false
        /// <para>根据 <see cref="prefabSelectMode"/> 决定选择的 prefab 实例</para>
        /// </summary>
        /// <param name="output"></param>
        /// <param name="parent">需要挂载的父物体</param>
        /// <returns></returns>
        public bool Pop(out GameObject output, Transform parent = null)
        {
            return Pop(SelectPrefab(), out output, parent);
        }

        /// <summary>
        /// 尝试取出一个 GameObject, 对象池用尽则返回 false
        /// <para>传入一个具体的索引, 限定需要的 prefab 实例</para>
        /// </summary>
        /// <param name="index"></param>
        /// <param name="output"></param>
        /// <param name="parent">需要挂载的父物体</param>
        /// <returns></returns>
        public bool Pop(int index, out GameObject output, Transform parent = null)
        {
            if (inactiveObjects[index].Count == 0)
            {
                if (canExtend)
                {
                    CreateNew(index);
                }
                else
                {
                    //不可扩充
                    output = null;
                    return false;
                }
            }
            output = inactiveObjects[index][inactiveObjects[index].Count - 1];
            inactiveObjects[index].RemoveAt(inactiveObjects[index].Count - 1);
            activeObjects[index].Add(output);
            output.SetActive(true);
            output.transform.parent = parent ? parent : transform;
            return true;
        }

        /// <summary>
        /// 尝试回收一个GameObject,本对象池中不存在则返回false
        /// </summary>
        /// <param name="toPush"></param>
        /// <returns></returns>
        public bool Push(GameObject toPush)
        {
            var index = activeObjects.FindIndex(set => set.Contains(toPush));
            if (index == -1) return false;
            activeObjects[index].Remove(toPush);
            inactiveObjects[index].Add(toPush);
            toPush.transform.SetParent(transform);
            toPush.SetActive(false);
            return true;
        }

        /// <summary>
        /// 回收全部的物体
        /// </summary>
        public void PushAll()
        {
            foreach (var set in activeObjects)
            {
                // ToList是为了复制一个列表, 防止在循环中修改集合元素导致的未定义行为
                set.ToList().ForEach(go => Push(go));
            }
        }

        /// <summary>
        /// 选择一个 prefab, 返回索引
        /// </summary>
        /// <returns></returns>
        private int SelectPrefab()
        {
            var temp = _currentPrefabIndex;
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
            Debug.Log($"{{prefab index: {temp}->{_currentPrefabIndex}");
            return _currentPrefabIndex;
        }
    }
}
