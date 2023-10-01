using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Unitilities.AssetManage
{
    /// <summary>
    /// 对象引用表
    /// <para>对象特指 <see cref="UnityEngine.Object"/> 这类能在 Asset 中引用的资产. </para>
    /// <para>最核心的功能是提供一个根据字符串查找对象的方法, int 索引的查询与使用属于附加功能.</para>
    /// </summary>
    [CreateAssetMenu(fileName = "New Object Reference Table", menuName = "Unitilities/Object reference table")]
    public class ObjectRefTable : ScriptableObject
    {
        [SerializeField] private List<Object> refList = new List<Object>();
        /// <summary>
        /// 预处理后的内部的引用列表, 保证有且仅有一个 null 作为首元素
        /// </summary>
        private List<Object> _refList;
        private List<string> _keys;
        private List<Object> _objs;
        private Dictionary<string, Object> _objectLut;
        private Dictionary<string, int> _idLut;
        private bool _dirty = true;

        private void OnValidate()
        {
            _dirty = true;
        }

        /// <summary>
        /// 初始化, 建立必要的数据结构
        /// </summary>
        public void Init()
        {
            // 内部的 List 要求移除 null 并在首元素设置为 null
            _refList = refList.Where(o => o != null).Prepend(null).ToList();
            // 初始化其它数据结构
            _keys = _refList.Select(o => o != null ? o.name : "null").ToList();
            _objectLut = _refList.ToDictionary(o => o ? o.name : "null", o => o);
            _idLut = new Dictionary<string, int> { { "null", 0 } };
            for (var i = 1; i < _refList.Count; i++)
            {
                _idLut[_refList[i].name] = i;
            }

            _objs = _keys.ConvertAll(k => _objectLut[k]);
            _dirty = false;
        }

        /// <summary>
        /// 返回由 <see cref="key"/> 指定的 Object 引用
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public Object this[string key]
        {
            get
            {
                if (_dirty) Init();
                return _objectLut[key];
            }
        }

        /// <summary>
        /// 从 <see cref="obj"/> 返回对应的索引, 若列表不存在该项目则返回 -1
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public int GetIdFromObject(Object obj)
        {
            if (_dirty) Init();
            return _idLut[obj ? obj.name : "null"];
        }

        /// <summary>
        /// 获取有效键的列表
        /// </summary>
        /// <returns></returns>
        public List<string> GetKeys()
        {
            if (_dirty) Init();
            // 注意是复制
            return _keys.ToList();
        }

        /// <summary>
        /// 根据索引获取一个Object. 相当于先从 <see cref="GetKeys"/> 获取键列表后再查询.
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public Object GetObject(int index)
        {
            if (index < 0) return null;
            if (index >= _keys.Count) return null;
            return _objs[index];
        }
    }

}
