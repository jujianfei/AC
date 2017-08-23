using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;

namespace AC.Base
{
    /// <summary>
    /// 表示可按照只读索引单独访问的一组对象。
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class ReadOnlyCollection<T> : IReadOnlyCollection<T>
    {
        /// <summary>
        /// 存储对象数据的 List
        /// </summary>
        protected System.Collections.Generic.List<T> Items = new List<T>();

        #region IList<T> 成员

        /// <summary>
        /// 将[元素]插入集合的指定索引处。
        /// </summary>
        /// <param name="index"></param>
        /// <param name="item"></param>
        protected void Insert(int index, T item)
        {
            Items.Insert(index, item);
        }

        /// <summary>
        /// 移除集合的指定索引处的[元素]。
        /// </summary>
        /// <param name="index"></param>
        protected void RemoveAt(int index)
        {
            Items.RemoveAt(index);
        }

        #endregion

        #region ICollection<T> 成员

        /// <summary>
        /// 将[元素]添加到集合的结尾处。
        /// </summary>
        /// <param name="item"></param>
        protected void Add(T item)
        {
            Items.Add(item);
        }

        /// <summary>
        /// 从集合中移除所有[元素]。
        /// </summary>
        protected void Clear()
        {
            Items.Clear();
        }

        /// <summary>
        /// 获取一个值，该值指示[元素]集合是否为只读。
        /// </summary>
        protected bool IsReadOnly
        {
            get { return true; }
        }

        /// <summary>
        /// 从集合中移除特定[元素]的第一个匹配项。
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        protected bool Remove(T item)
        {
            return Items.Remove(item);
        }

        #endregion

        #region IReadOnlyCollection<T> 成员

        /// <summary>
        /// 搜索指定的[元素]，并返回整个集合中第一个匹配项的从零开始的索引。
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public int IndexOf(T item)
        {
            return Items.IndexOf(item);
        }

        /// <summary>
        /// 获取指定索引处的[元素]。
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public T this[int index]
        {
            get
            {
                return Items[index];
            }
            protected set
            {
                Items[index] = value;
            }
        }

        /// <summary>
        /// 确定某[元素]是否在集合中。
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public bool Contains(T item)
        {
            return Items.Contains(item);
        }

        /// <summary>
        /// 将整个[元素]集合复制到兼容的一维数组中，从目标数组的指定索引位置开始放置。
        /// </summary>
        /// <param name="array"></param>
        /// <param name="arrayIndex"></param>
        public void CopyTo(T[] array, int arrayIndex)
        {
            Items.CopyTo(array, arrayIndex);
        }

        /// <summary>
        /// 获取集合中实际包含的[元素]数。
        /// </summary>
        public int Count
        {
            get { return Items.Count; }
        }

        #endregion

        #region IEnumerable<T> 成员

        /// <summary>
        /// 返回循环访问[元素]集合的枚举数。
        /// </summary>
        /// <returns></returns>
        public IEnumerator<T> GetEnumerator()
        {
            return Items.GetEnumerator();
        }

        #endregion

        #region IEnumerable 成员

        /// <summary>
        /// 返回一个循环访问[元素]集合的枚举数。
        /// </summary>
        /// <returns></returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return Items.GetEnumerator();
        }

        #endregion
    }
}
