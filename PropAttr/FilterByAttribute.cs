using System;
using System.Linq;
using UnityEngine;

namespace Util.PropAttr
{
    /// <summary>
    /// 按指定的字段或属性的值来确定是否显示
    /// </summary>
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
    public class FilterByAttribute : PropertyAttribute
    {
        /// <summary>
        /// 过滤源, 必须为字段
        /// </summary>
        public string filterSource;
        /// <summary>
        /// 当过滤源的值在 values 列表中时显示
        /// </summary>
        public object[] values;

        /// <summary>
        /// 无参构造是私有的
        /// </summary>
        private FilterByAttribute()
        {
        }

        public FilterByAttribute(string filterSource, params object[] values)
        {
            this.filterSource = filterSource;
            this.values = values;
        }

        public bool CanDraw(object target)
        {
            var src = target.GetType().GetField(filterSource);
            if (src == null) return true;
            var value = src.GetValue(target);
            return values.Contains(value);
        }
    }

}
