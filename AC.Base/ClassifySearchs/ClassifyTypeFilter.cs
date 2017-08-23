using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AC.Base.Drives;

namespace AC.Base.ClassifySearchs
{
    /// <summary>
    /// 搜索指定分类类型或指定驱动类型的分类。
    /// </summary>
    public class ClassifyTypeFilter : IClassifyFilter
    {
        private AC.Base.ApplicationClass m_Application;

        /// <summary>
        /// 搜索指定分类类型或指定驱动类型的分类。
        /// </summary>
        public ClassifyTypeFilter()
        {
        }

        /// <summary>
        /// 搜索指定分类类型或指定驱动类型的分类。
        /// </summary>
        /// <param name="classifyType">分类类型或驱动类型。</param>
        public ClassifyTypeFilter(Type classifyType)
        {
            this.ClassifyType = classifyType;
        }

        private Type m_ClassifyType;
        /// <summary>
        /// 获取或设置分类类型或驱动类型。
        /// </summary>
        public Type ClassifyType
        {
            get
            {
                return this.m_ClassifyType;
            }
            set
            {
                this.m_ClassifyType = value;
            }
        }


        #region IClassifyFilter 成员

        /// <summary>
        /// 检查传入的分类是否符合该筛选器筛选要求。
        /// </summary>
        /// <param name="classify">被检查的分类。</param>
        /// <returns></returns>
        public bool ClassifyFilterCheck(Classify classify)
        {
            if (this.ClassifyType != null)
            {
                if (this.ClassifyType.IsClass)
                {
                    return Function.IsInheritableBaseType(classify.GetType(), this.ClassifyType);
                }
                else if (this.ClassifyType.IsInterface)
                {
                    return classify.ClassifyType.Type.GetInterface(this.ClassifyType.FullName) != null;
                }
            }
            return false;
        }

        #endregion

        #region IFilter 成员

        /// <summary>
        /// 筛选器应用场合选项。
        /// </summary>
        public Searchs.FilterAppliesOptions FilterAppliesAttribute
        {
            get { return Searchs.FilterAppliesOptions.All; }
        }

        /// <summary>
        /// 有关该筛选器的功能说明、使用说明。
        /// </summary>
        public string FilterRemarkAttribute
        {
            get { return "搜索指定类型的分类"; }
        }

        /// <summary>
        /// 设置应用程序框架。
        /// </summary>
        /// <param name="application"></param>
        public void SetApplication(ApplicationClass application)
        {
            this.m_Application = application;
        }

        /// <summary>
        /// 筛选器名称。一个筛选器可以有一个或多个名称，通常应至少设置一个名称；如果不希望当前筛选器被使用，则 null。
        /// </summary>
        public string[] FilterNamesAttribute
        {
            get { return new string[] { "分类类型" }; }
        }

        /// <summary>
        /// 设置当前使用的筛选器名称。例如用户自定义分类，筛选器只能有一个，而通过 FilterNamesAttribute 返回诸如地区、行业、类型等参数后，可以将此筛选器变为地区筛选器、行业筛选器、类型筛选器，filterName 参数则是指定当前是地区筛选器还是行业筛选器。
        /// </summary>
        /// <param name="filterName">当前使用的筛选器名称。</param>
        public void SetFilterName(string filterName)
        {
        }

        /// <summary>
        /// 此筛选器执行的 SQL 语句。
        /// </summary>
        /// <param name="enableTableName">返回的条件语句是否需要附加表名。例如 table.column = 12 或者 column = 12</param>
        /// <returns></returns>
        public string GetFilterSql(bool enableTableName)
        {
            string strSql = "";

            if (this.ClassifyType != null)
            {
                foreach (ClassifyType classifyType in this.m_Application.ClassifyTypes)
                {
                    if (this.ClassifyType.IsClass)
                    {
                        if (Function.IsInheritableBaseType(classifyType.Type, this.ClassifyType))
                        {
                            strSql += ",'" + classifyType.Type.FullName + "'";
                        }
                    }
                    else if (this.ClassifyType.IsInterface)
                    {
                        if (classifyType.Type.GetInterface(this.ClassifyType.FullName) != null)
                        {
                            strSql += ",'" + classifyType.Type.FullName + "'";
                        }
                    }
                }

                if (strSql.Length > 0)
                {
                    strSql = (enableTableName ? Tables.Classify.TableName + "." : "") + Tables.Classify.ClassifyType + " IN (" + strSql.Substring(1) + ")";
                }
            }

            return strSql;
        }

        /// <summary>
        /// 获取该筛选器所执行的筛选逻辑的文字描述。
        /// </summary>
        /// <returns></returns>
        public string GetFilterDescription()
        {
            string strDescription = "";

            if (this.ClassifyType != null)
            {
                foreach (ClassifyType classifyType in this.m_Application.ClassifyTypes)
                {
                    if (this.ClassifyType.IsClass)
                    {
                        if (Function.IsInheritableBaseType(classifyType.Type, this.ClassifyType))
                        {
                            strDescription += "、" + classifyType.Type.FullName;
                        }
                    }
                    else if (this.ClassifyType.IsInterface)
                    {
                        if (classifyType.Type.GetInterface(this.ClassifyType.FullName) != null)
                        {
                            strDescription += "、" + classifyType.Type.FullName;
                        }
                    }
                }

                if (strDescription.Length > 0)
                {
                    strDescription = "分类类型是" + strDescription.Substring(1) + "的分类";
                }
            }

            return strDescription;
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
    }
}
