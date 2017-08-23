using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AC.Base.ClassifySearchs;

namespace AC.Base
{
    /// <summary>
    /// 分类集合。
    /// </summary>
    public class ClassifyCollection : System.Collections.Generic.IList<Classify>, IClassifyCollection
    {
        private System.Collections.Generic.List<Classify> Items = new List<Classify>();

        /// <summary>
        /// 分类集合。
        /// </summary>
        /// <param name="keepSource">是否保持各分类与该集合的引用关系。</param>
        public ClassifyCollection(bool keepSource)
        {
            this.m_KeepSource = keepSource;
        }

        private bool m_KeepSource;
        /// <summary>
        /// 获取或设置是否保持各分类与该集合的引用关系。
        /// </summary>
        public bool KeepSource
        {
            get
            {
                return this.m_KeepSource;
            }
            set
            {
                if (value)
                {
                    //保持引用
                    foreach (Classify classify in this)
                    {
                        classify.Source = this;
                    }
                }
                else
                {
                    //清空引用
                    foreach (Classify classify in this)
                    {
                        classify.Source = null;
                    }
                }
                this.m_KeepSource = value;
            }
        }

        /// <summary>
        /// 获取当前对象的字符串表示形式。
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return "共 " + this.Count + " 个分类";
        }

        #region IList<Classify> 成员

        /// <summary>
        /// 搜索指定的分类，并返回整个集合中第一个匹配项的从零开始的索引。
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public int IndexOf(Classify item)
        {
            return this.Items.IndexOf(item);
        }

        /// <summary>
        /// 将分类插入集合的指定索引处。
        /// </summary>
        /// <param name="index"></param>
        /// <param name="item"></param>
        public void Insert(int index, Classify item)
        {
            this.Items.Insert(index, item);

            if (this.KeepSource)
            {
                item.Source = this;
            }
            else
            {
                item.Source = null;
            }
        }

        /// <summary>
        /// 移除集合的指定索引处的分类。
        /// </summary>
        /// <param name="index"></param>
        public void RemoveAt(int index)
        {
            this.Items.RemoveAt(index);

            this.Items[index].Source = null;
        }

        /// <summary>
        /// 获取或设置指定索引处的分类。
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public Classify this[int index]
        {
            get
            {
                return this.Items[index];
            }
            set
            {
                if (this.Items[index] != null)
                {
                    this.Items[index].Source = null;
                }

                this.Items[index] = value;

                if (this.KeepSource)
                {
                    this.Items[index].Source = this;
                }
                else
                {
                    this.Items[index].Source = null;
                }
            }
        }

        #endregion

        #region ICollection<Classify> 成员

        /// <summary>
        /// 将分类添加到集合的结尾处。
        /// </summary>
        /// <param name="item"></param>
        public void Add(Classify item)
        {
            this.Items.Add(item);

            if (this.KeepSource)
            {
                item.Source = this;
            }
            else
            {
                item.Source = null;
            }
        }

        /// <summary>
        /// 从集合中移除所有分类。
        /// </summary>
        public void Clear()
        {
            foreach (Classify item in this)
            {
                item.Source = null;
            }

            this.Items.Clear();
        }

        /// <summary>
        /// 确定某分类是否在集合中。
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public bool Contains(Classify item)
        {
            foreach (Classify _Classify in this)
            {
                if (_Classify.Equals(item))
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// 将整个分类集合复制到兼容的一维数组中，从目标数组的指定索引位置开始放置。
        /// </summary>
        /// <param name="array"></param>
        /// <param name="arrayIndex"></param>
        public void CopyTo(Classify[] array, int arrayIndex)
        {
            this.Items.CopyTo(array, arrayIndex);
        }

        /// <summary>
        /// 获取集合中实际包含的分类数。
        /// </summary>
        public int Count
        {
            get { return this.Items.Count; }
        }

        /// <summary>
        /// 获取一个值，该值指示分类集合是否为只读。
        /// </summary>
        public bool IsReadOnly
        {
            get { return false; }
        }

        /// <summary>
        /// 从集合中移除特定分类的第一个匹配项。
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public bool Remove(Classify item)
        {
            item.Source = null;
            return this.Items.Remove(item);
        }

        #endregion

        #region IEnumerable<Classify> 成员

        /// <summary>
        /// 返回循环访问分类集合的枚举数。
        /// </summary>
        /// <returns></returns>
        public IEnumerator<Classify> GetEnumerator()
        {
            return this.Items.GetEnumerator();
        }

        #endregion

        #region IEnumerable 成员

        /// <summary>
        /// 返回一个循环访问分类集合的枚举数。
        /// </summary>
        /// <returns></returns>
        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return this.Items.GetEnumerator();
        }

        #endregion

        #region IClassifyCollection 成员

        /// <summary>
        /// 获取该集合中指定 ID 的分类。
        /// </summary>
        /// <param name="classifyId">分类 ID。</param>
        /// <returns>返回集合中指定 ID 的分类，如果未查找到该分类则返回 null。</returns>
        public Classify GetClassify(int classifyId)
        {
            foreach (Classify classify in this)
            {
                if (classify.ClassifyId == classifyId)
                {
                    return classify;
                }
            }
            return null;
        }

        /// <summary>
        /// 获取该集合中符合筛选条件的分类集合。
        /// </summary>
        /// <param name="filters">分类筛选条件。</param>
        /// <returns></returns>
        public ClassifyCollection GetClassifys(ClassifyFilterCollection filters)
        {
            ClassifyCollection classifys = new ClassifyCollection(false);

            foreach (Classify classify in this)
            {
                if (filters.ClassifyFilterCheck(classify))
                {
                    classifys.Add(classify);
                }
            }

            return classifys;
        }

        /// <summary>
        /// 获取当前集合内所有的分类编号。
        /// </summary>
        /// <returns></returns>
        public int[] GetIdForArray()
        {
            int[] ids = new int[this.Count];

            for (int intIndex = 0; intIndex < this.Count; intIndex++)
            {
                ids[intIndex] = this[intIndex].ClassifyId;
            }

            return ids;
        }

        /// <summary>
        /// 获取当前集合内所有的分类编号。
        /// </summary>
        /// <returns></returns>
        public ICollection<int> GetIdForCollection()
        {
            List<int> lsId = new List<int>(this.Count);

            foreach (Classify classify in this)
            {
                lsId.Add(classify.ClassifyId);
            }

            return lsId;
        }

        /// <summary>
        /// 获取当前分类集合中以逗号分隔的分类编号字符串。
        /// </summary>
        /// <returns></returns>
        public string GetIdForString()
        {
            return this.GetIdForString(",");
        }

        /// <summary>
        /// 获取当前分类集合中以指定字符分隔的分类编号字符串。
        /// </summary>
        /// <param name="separator">用于分隔分类编号的字符。</param>
        /// <returns></returns>
        public string GetIdForString(string separator)
        {
            string strIds = "";

            foreach (Classify classify in this)
            {
                strIds += separator + classify.ClassifyId;
            }

            if (strIds.Length > 0)
            {
                strIds = strIds.Substring(separator.Length);
            }
            return strIds;
        }

        #endregion
    }
}
