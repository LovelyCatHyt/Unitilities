using System.Collections;
using System.Text;

namespace Unitilities.DebugUtil
{
    /// <summary>
    /// 打表器
    /// </summary>
    public static class ListPrinter
    {
        /// <summary>
        /// 将列表里的内容拼接成字符串, 元素用逗号分隔
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        public static string Print(this IEnumerable list)
        {
            bool isFirst = true;
            var strBuilder = new StringBuilder();
            foreach (var o in list)
            {
                if (!isFirst)
                {
                    strBuilder.Append(", ");
                }
                strBuilder.Append(o);
                isFirst = false;
            }

            return strBuilder.ToString();
        }

        /// <summary>
        /// 将列表里的内容拼接成字符串, 一行一个元素
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        public static string PrintLines(this IEnumerable list)
        {
            bool isFirst = true;
            var strBuilder = new StringBuilder();
            foreach (var o in list)
            {
                if (!isFirst)
                {
                    strBuilder.Append(", ").AppendLine();
                }
                strBuilder.Append(o);
                isFirst = false;
            }

            return strBuilder.ToString();
        }
    }
}
