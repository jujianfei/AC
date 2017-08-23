using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AC.Base.Searchs;

namespace AC.Base.Tasks
{
    /// <summary>
    /// 任务运行日志筛选器集合。
    /// </summary>
    public class TaskLogFilterCollection : System.Collections.Generic.IList<ITaskLogFilter>, ITaskLogFilter
    {
        private ApplicationClass m_Application;
        private List<ITaskLogFilter> m_Filters = new List<ITaskLogFilter>();

        /// <summary>
        /// 筛选器逻辑运算符。
        /// </summary>
        public FilterLogicOperatorOptions TaskLogicOperator { get; set; }

        #region ITaskLogFilter 成员

        /// <summary>
        /// 设置应用程序框架。
        /// </summary>
        /// <param name="application"></param>
        public void SetApplication(ApplicationClass application)
        {
            this.m_Application = application;

            foreach (ITaskLogFilter filter in this)
            {
                filter.SetApplication(this.m_Application);
            }
        }

        /// <summary>
        /// 此筛选器执行的 SQL 语句。
        /// </summary>
        /// <param name="enableTableName">返回的条件语句是否需要附加表名。例如 table.column = 12 或者 column = 12</param>
        /// <param name="month">所需查询数据的月份，该参数中年、月属性有效。</param>
        /// <returns></returns>
        public string GetFilterSql(bool enableTableName, DateTime month)
        {
            if (this.Count > 0)
            {
                if (this.Count == 1)
                {
                    return this[0].GetFilterSql(enableTableName, month);
                }
                else
                {
                    string strSql = "";

                    foreach (ITaskLogFilter filter in this)
                    {
                        string strWhere = filter.GetFilterSql(enableTableName, month);
                        if (strWhere != null && strWhere.Length > 0)
                        {
                            if (filter is TaskLogFilterCollection)
                            {
                                TaskLogFilterCollection f = filter as TaskLogFilterCollection;
                                if (f.Count > 0)
                                {
                                    strSql += " " + this.TaskLogicOperator.GetFilterSql() + " (" + strWhere + ")";
                                }
                                else
                                {
                                    strSql += " " + this.TaskLogicOperator.GetFilterSql() + " " + strWhere;
                                }
                            }
                            else
                            {
                                strSql += " " + this.TaskLogicOperator.GetFilterSql() + " " + strWhere;
                            }
                        }
                    }

                    if (strSql.Length > 0)
                    {
                        strSql = strSql.Substring(this.TaskLogicOperator.GetFilterSql().Length + 2);
                    }
                    return strSql;
                }
            }
            else
            {
                return "";
            }
        }

        /// <summary>
        /// 获取该筛选器所执行的筛选逻辑的文字描述。
        /// </summary>
        /// <returns></returns>
        public string GetFilterDescription()
        {
            if (this.Count > 0)
            {
                if (this.Count == 1)
                {
                    return this[0].GetFilterDescription();
                }
                else
                {
                    string strSql = "";

                    foreach (ITaskLogFilter filter in this)
                    {
                        string strWhere = filter.GetFilterDescription();
                        if (strWhere != null && strWhere.Length > 0)
                        {
                            if (filter is TaskLogFilterCollection)
                            {
                                TaskLogFilterCollection f = filter as TaskLogFilterCollection;
                                if (f.Count > 0)
                                {
                                    strSql += " " + this.TaskLogicOperator.GetFilterDescription() + " (" + strWhere + ")";
                                }
                                else
                                {
                                    strSql += " " + this.TaskLogicOperator.GetFilterDescription() + " " + strWhere;
                                }
                            }
                            else
                            {
                                strSql += " " + this.TaskLogicOperator.GetFilterDescription() + " " + strWhere;
                            }
                        }
                    }

                    if (strSql.Length > 0)
                    {
                        strSql = strSql.Substring(this.TaskLogicOperator.GetFilterDescription().Length + 2);
                    }
                    return strSql;
                }
            }
            else
            {
                return "";
            }
        }

        /// <summary>
        /// 从保存此筛选器数据的 XML 文档节点集合初始化当前筛选器。
        /// </summary>
        /// <param name="xmlNode">该对象节点的数据集合</param>
        public void SetFilterConfig(System.Xml.XmlNode xmlNode)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 获取当前筛选器的状态，将序列化的内容填充至 XmlNode 的 InnerText 属性或者 ChildNodes 子节点中。
        /// </summary>
        /// <param name="xmlDoc">创建 XmlNode 时所需使用到的 System.Xml.XmlDocument。</param>
        public System.Xml.XmlNode GetFilterConfig(System.Xml.XmlDocument xmlDoc)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region IList<ITaskLogFilter> 成员

        /// <summary>
        /// 搜索指定的筛选器，并返回整个集合中第一个匹配项的从零开始的索引。
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public int IndexOf(ITaskLogFilter item)
        {
            return this.m_Filters.IndexOf(item);
        }

        /// <summary>
        /// 将筛选器插入集合的指定索引处。
        /// </summary>
        /// <param name="index"></param>
        /// <param name="item"></param>
        public void Insert(int index, ITaskLogFilter item)
        {
            item.SetApplication(this.m_Application);
            this.m_Filters.Insert(index, item);
        }

        /// <summary>
        /// 移除集合的指定索引处的筛选器。
        /// </summary>
        /// <param name="index"></param>
        public void RemoveAt(int index)
        {
            this.m_Filters.RemoveAt(index);
        }

        /// <summary>
        /// 获取或设置指定索引处的筛选器。
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public ITaskLogFilter this[int index]
        {
            get
            {
                return this.m_Filters[index];
            }
            set
            {
                this.m_Filters[index] = value;
                this.m_Filters[index].SetApplication(this.m_Application);
            }
        }

        #endregion

        #region ICollection<ITaskLogFilter> 成员

        /// <summary>
        /// 将筛选器添加到集合的结尾处。
        /// </summary>
        /// <param name="item"></param>
        public void Add(ITaskLogFilter item)
        {
            item.SetApplication(this.m_Application);
            this.m_Filters.Add(item);
        }

        /// <summary>
        /// 从集合中移除所有筛选器。
        /// </summary>
        public void Clear()
        {
            this.m_Filters.Clear();
        }

        /// <summary>
        /// 确定某筛选器是否在集合中。
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public bool Contains(ITaskLogFilter item)
        {
            return this.m_Filters.Contains(item);
        }

        /// <summary>
        /// 将整个筛选器集合复制到兼容的一维数组中，从目标数组的指定索引位置开始放置。
        /// </summary>
        /// <param name="array"></param>
        /// <param name="arrayIndex"></param>
        public void CopyTo(ITaskLogFilter[] array, int arrayIndex)
        {
            this.m_Filters.CopyTo(array, arrayIndex);
        }

        /// <summary>
        /// 获取集合中实际包含的筛选器数。
        /// </summary>
        public int Count
        {
            get { return this.m_Filters.Count; }
        }

        /// <summary>
        /// 获取一个值，该值指示筛选器集合是否为只读。
        /// </summary>
        public bool IsReadOnly
        {
            get { return false; }
        }

        /// <summary>
        /// 从集合中移除特定筛选器的第一个匹配项。
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public bool Remove(ITaskLogFilter item)
        {
            return this.m_Filters.Remove(item);
        }

        #endregion

        #region IEnumerable<ITaskLogFilter> 成员

        /// <summary>
        /// 返回循环访问筛选器集合的枚举数。
        /// </summary>
        /// <returns></returns>
        public IEnumerator<ITaskLogFilter> GetEnumerator()
        {
            return this.m_Filters.GetEnumerator();
        }

        #endregion

        #region IEnumerable 成员

        /// <summary>
        /// 返回一个循环访问筛选器集合的枚举数。
        /// </summary>
        /// <returns></returns>
        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return this.m_Filters.GetEnumerator();
        }

        #endregion

    }
}
