using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AC.Base.Searchs
{
    /// <summary>
    /// 筛选器集合。
    /// </summary>
    public class FilterCollection<F> : System.Collections.Generic.IList<F>, IFilter
        where F : IFilter
    {
        private ApplicationClass m_Application;
        private List<F> m_Filters = new List<F>();

        /// <summary>
        /// 筛选器逻辑运算符。
        /// </summary>
        public FilterLogicOperatorOptions LogicOperator { get; set; }

        #region IFilter 成员

        /// <summary>
        /// 筛选器应用场合选项。
        /// </summary>
        public FilterAppliesOptions FilterAppliesAttribute
        {
            get { return FilterAppliesOptions.Client | FilterAppliesOptions.Manage | FilterAppliesOptions.SuperManage; }
        }

        /// <summary>
        /// 设置应用程序框架。
        /// </summary>
        /// <param name="application"></param>
        public void SetApplication(ApplicationClass application)
        {
            this.m_Application = application;

            foreach (F filter in this)
            {
                filter.SetApplication(this.m_Application);
            }
        }

        /// <summary>
        /// 筛选器名称。一个筛选器可以有一个或多个名称，通常应至少设置一个名称；如果不希望当前筛选器被使用，则 null。
        /// </summary>
        public string[] FilterNamesAttribute
        {
            get { return null; }
        }

        /// <summary>
        /// 设置当前使用的筛选器名称。例如用户自定义分类，筛选器只能有一个，而通过 FilterNamesAttribute 返回诸如地区、行业、类型等参数后，可以将此筛选器变为地区筛选器、行业筛选器、类型筛选器，filterName 参数则是指定当前是地区筛选器还是行业筛选器。
        /// </summary>
        /// <param name="filterName">当前使用的筛选器名称。</param>
        public void SetFilterName(string filterName)
        {
        }

        /// <summary>
        /// 有关该筛选器的功能说明、使用说明。
        /// </summary>
        public string FilterRemarkAttribute
        {
            get { return null; }
        }

        /// <summary>
        /// 此筛选器执行的 SQL 语句。
        /// </summary>
        /// <param name="enableTableName">返回的条件语句是否需要附加表名。例如 table.column = 12 或者 column = 12</param>
        /// <returns></returns>
        public string GetFilterSql(bool enableTableName)
        {
            if (this.Count > 0)
            {
                if (this.Count == 1)
                {
                    return this[0].GetFilterSql(enableTableName);
                }
                else
                {
                    string strSql = "";

                    foreach (F filter in this)
                    {
                        string strWhere = filter.GetFilterSql(enableTableName);
                        if (strWhere != null && strWhere.Length > 0)
                        {
                            if (filter is FilterCollection<F>)
                            {
                                FilterCollection<F> f = filter as FilterCollection<F>;
                                if (f.Count > 0)
                                {
                                    strSql += " " + this.LogicOperator.GetFilterSql() + " (" + strWhere + ")";
                                }
                                else
                                {
                                    strSql += " " + this.LogicOperator.GetFilterSql() + " " + strWhere;
                                }
                            }
                            else
                            {
                                strSql += " " + this.LogicOperator.GetFilterSql() + " " + strWhere;
                            }
                        }
                    }

                    if (strSql.Length > 0)
                    {
                        strSql = strSql.Substring(this.LogicOperator.GetFilterSql().Length + 2);
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

                    foreach (F filter in this)
                    {
                        string strWhere = filter.GetFilterDescription();
                        if (strWhere != null && strWhere.Length > 0)
                        {
                            if (filter is FilterCollection<F>)
                            {
                                FilterCollection<F> f = filter as FilterCollection<F>;
                                if (f.Count > 0)
                                {
                                    strSql += " " + this.LogicOperator.GetFilterDescription() + " (" + strWhere + ")";
                                }
                                else
                                {
                                    strSql += " " + this.LogicOperator.GetFilterDescription() + " " + strWhere;
                                }
                            }
                            else
                            {
                                strSql += " " + this.LogicOperator.GetFilterDescription() + " " + strWhere;
                            }
                        }
                    }

                    if (strSql.Length > 0)
                    {
                        strSql = strSql.Substring(this.LogicOperator.GetFilterDescription().Length + 2);
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
            if (xmlNode != null)
            {
                foreach (System.Xml.XmlAttribute xaFilterLogicOperator in xmlNode.Attributes)
                {
                    if (xaFilterLogicOperator.Name.Equals("FilterLogicOperator"))
                    {
                        this.LogicOperator = (FilterLogicOperatorOptions)Function.ToInt(xaFilterLogicOperator.InnerText);
                        break;
                    }
                }

                foreach (System.Xml.XmlNode xnChildren in xmlNode.ChildNodes)
                {
                    string strType = null;                                                  //类型
                    string strAssembly = null;                                              //程序集

                    //读取保存在控件上的“类型”和“程序集”信息
                    foreach (System.Xml.XmlAttribute xaControlAttribute in xnChildren.Attributes)
                    {
                        if (xaControlAttribute.Name.Equals("Type"))
                        {
                            strType = xaControlAttribute.Value;
                        }
                        else if (xaControlAttribute.Name.Equals("Assembly"))
                        {
                            strAssembly = xaControlAttribute.Value;
                        }
                    }

                    if (strType != null && strAssembly != null)
                    {
                        //通过“类型”和“程序集”信息创建指定的类型
                        Type typControl = Type.GetType(strType + "," + strAssembly);

                        //如果该类型实现了“IResumeFilter”接口
                        if (typControl != null && typControl.GetInterface(typeof(IFilter).Name) != null)
                        {
                            //获取对该类型控件构造函数元数据的访问权
                            System.Reflection.ConstructorInfo asConstructorInfo = typControl.GetConstructor(new System.Type[] { });

                            //创建该类型控件的实例
                            object objInstance = asConstructorInfo.Invoke(new object[] { });
                            F _Filter = (F)objInstance;

                            if (_Filter != null)
                            {
                                _Filter.SetApplication(this.m_Application);
                                _Filter.SetFilterConfig(xnChildren);

                                this.Add(_Filter);
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 获取当前筛选器的状态，将序列化的内容填充至 XmlNode 的 InnerText 属性或者 ChildNodes 子节点中。
        /// </summary>
        /// <param name="xmlDoc">创建 XmlNode 时所需使用到的 System.Xml.XmlDocument。</param>
        public System.Xml.XmlNode GetFilterConfig(System.Xml.XmlDocument xmlDoc)
        {
            System.Xml.XmlNode xn = xmlDoc.CreateElement(this.GetType().Name);

            System.Xml.XmlAttribute xaFilterLogicOperator = xmlDoc.CreateAttribute("FilterLogicOperator");
            xaFilterLogicOperator.InnerText = Function.ToInt(this.LogicOperator).ToString();
            xn.Attributes.Append(xaFilterLogicOperator);

            foreach (F _Filter in this)
            {
                System.Xml.XmlNode xnFilter = _Filter.GetFilterConfig(xmlDoc);

                //类型名
                System.Xml.XmlAttribute xaType = xmlDoc.CreateAttribute("Type");
                xaType.Value = _Filter.GetType().FullName;
                xnFilter.Attributes.Append(xaType);

                //程序集名
                System.Xml.XmlAttribute xaAssembly = xmlDoc.CreateAttribute("Assembly");
                xaAssembly.Value = _Filter.GetType().Assembly.FullName.Substring(0, _Filter.GetType().Assembly.FullName.IndexOf(","));
                xnFilter.Attributes.Append(xaAssembly);

                xn.AppendChild(xnFilter);
            }
            return xn;
        }

        #endregion

        #region IList<F> 成员

        /// <summary>
        /// 搜索指定的筛选器，并返回整个集合中第一个匹配项的从零开始的索引。
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public int IndexOf(F item)
        {
            return this.m_Filters.IndexOf(item);
        }

        /// <summary>
        /// 将筛选器插入集合的指定索引处。
        /// </summary>
        /// <param name="index"></param>
        /// <param name="item"></param>
        public void Insert(int index, F item)
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
        public F this[int index]
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

        #region ICollection<F> 成员

        /// <summary>
        /// 将筛选器添加到集合的结尾处。
        /// </summary>
        /// <param name="item"></param>
        public void Add(F item)
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
        public bool Contains(F item)
        {
            return this.m_Filters.Contains(item);
        }

        /// <summary>
        /// 将整个筛选器集合复制到兼容的一维数组中，从目标数组的指定索引位置开始放置。
        /// </summary>
        /// <param name="array"></param>
        /// <param name="arrayIndex"></param>
        public void CopyTo(F[] array, int arrayIndex)
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
        public bool Remove(F item)
        {
            return this.m_Filters.Remove(item);
        }

        #endregion

        #region IEnumerable<F> 成员

        /// <summary>
        /// 返回循环访问筛选器集合的枚举数。
        /// </summary>
        /// <returns></returns>
        public IEnumerator<F> GetEnumerator()
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
