using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AC.Base
{
    /// <summary>
    /// 表示可按照只读索引单独访问的一组对象。
    /// </summary>
    public interface IReadOnlyCollection<T> : IEnumerable<T>
    {
        /// <summary>
        /// 确定集合中特定项的索引。
        /// </summary>
        /// <param name="item">要在集合中定位的对象。</param>
        /// <returns>如果在列表中找到，则为 item 的索引；否则为 -1。</returns>
        int IndexOf(T item);

        /// <summary>
        /// 获取或设置指定索引处的元素。
        /// </summary>
        /// <param name="index">要获得或设置的元素从零开始的索引。</param>
        /// <returns>指定索引处的元素。</returns>
        T this[int index]
        {
            get;
        }

        /// <summary>
        /// 确定集合是否包含特定值。
        /// </summary>
        /// <param name="item">要在集合中定位的对象。</param>
        /// <returns>如果在集合中找到 item，则为 true；否则为 false。</returns>
        bool Contains(T item);

        /// <summary>
        /// 从特定的 System.Array 索引开始，将集合的元素复制到一个 System.Array 中。
        /// </summary>
        /// <param name="array">作为从集合复制的元素的目标位置的一维 System.Array。System.Array 必须具有从零开始的索引。</param>
        /// <param name="arrayIndex">array 中从零开始的索引，从此处开始复制。</param>
        void CopyTo(T[] array, int arrayIndex);

        /// <summary>
        /// 获取集合中包含的元素数。
        /// </summary>
        int Count
        {
            get;
        }
    }
}
