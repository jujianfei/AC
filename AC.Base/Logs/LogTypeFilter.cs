using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AC.Base.Logs
{
    /// <summary>
    /// 按照日志类型或日志分类筛选日志，可通过 LogTypeSorts 或 LogTypes 属性添加需要筛选的类型。
    /// </summary>
    public class LogTypeFilter : ILogFilter
    {
        private ApplicationClass m_Application;

        /// <summary>
        /// 按照日志类型或日志分类筛选日志，可通过 LogTypeSorts 或 LogTypes 属性添加需要筛选的类型。
        /// </summary>
        public LogTypeFilter()
        {
        }

        /// <summary>
        /// 按照日志类型或日志分类筛选日志，可通过 LogTypeSorts 或 LogTypes 属性添加需要筛选的类型。
        /// </summary>
        /// <param name="logTypeSort"></param>
        public LogTypeFilter(LogTypeSort logTypeSort)
        {
            this.LogTypeSorts.Add(logTypeSort);
        }

        /// <summary>
        /// 按照日志类型或日志分类筛选日志，可通过 LogTypeSorts 或 LogTypes 属性添加需要筛选的类型。
        /// </summary>
        /// <param name="logTypeSorts"></param>
        public LogTypeFilter(LogTypeSort[] logTypeSorts)
        {
            this.LogTypeSorts.AddRange(logTypeSorts);
        }

        /// <summary>
        /// 按照日志类型或日志分类筛选日志，可通过 LogTypeSorts 或 LogTypes 属性添加需要筛选的类型。
        /// </summary>
        /// <param name="logType"></param>
        public LogTypeFilter(LogType logType)
        {
            this.LogTypes.Add(logType);
        }

        /// <summary>
        /// 按照日志类型或日志分类筛选日志，可通过 LogTypeSorts 或 LogTypes 属性添加需要筛选的类型。
        /// </summary>
        /// <param name="logTypes"></param>
        public LogTypeFilter(LogType[] logTypes)
        {
            this.LogTypes.AddRange(logTypes);
        }

        private Type m_LogTypeType;
        /// <summary>
        /// 按照日志类型或日志分类筛选日志，可通过 LogTypeSorts 或 LogTypes 属性添加需要筛选的类型。
        /// </summary>
        /// <param name="logTypeType">添加指定日志的类型声明的日志。</param>
        public LogTypeFilter(Type logTypeType)
        {
            this.m_LogTypeType = logTypeType;
        }

        private List<LogTypeSort> m_LogTypeSorts;
        /// <summary>
        /// 需要筛选的日志类型分类，将搜索属于该分类下的所有日志类型。
        /// </summary>
        public List<LogTypeSort> LogTypeSorts
        {
            get
            {
                if (this.m_LogTypeSorts == null)
                {
                    this.m_LogTypeSorts = new List<LogTypeSort>();
                }
                return this.m_LogTypeSorts;
            }
        }

        private List<LogType> m_LogTypes;
        /// <summary>
        /// 需要筛选的日志类型，将搜索该集合中所有的日志类型。
        /// </summary>
        public List<LogType> LogTypes
        {
            get
            {
                if (this.m_LogTypes == null)
                {
                    this.m_LogTypes = new List<LogType>();
                }
                return this.m_LogTypes;
            }
        }

        private List<LogType> GetLogTypes()
        {
            List<LogType> lst = new List<LogType>();

            foreach (LogType logType in this.LogTypes)
            {
                if (lst.Contains(logType) == false)
                {
                    lst.Add(logType);
                }
            }

            return lst;
        }

        private void GetLogTypes(LogTypeSort logTypeSort, List<LogType> lst)
        {
            foreach (LogType logType in logTypeSort.LogTypes)
            {
                if (lst.Contains(logType) == false)
                {
                    lst.Add(logType);
                }
            }

            foreach (LogTypeSort children in logTypeSort.Children)
            {
                this.GetLogTypes(children, lst);
            }
        }

        #region ILogFilter 成员

        /// <summary>
        /// 设置应用程序框架。
        /// </summary>
        /// <param name="application"></param>
        public void SetApplication(ApplicationClass application)
        {
            this.m_Application = application;

            if (this.m_LogTypeType != null)
            {
                LogType logType = this.m_Application.LogTypeSort.GetLogType(this.m_LogTypeType);
                if (logType != null)
                {
                    this.LogTypes.Add(logType);
                }
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
            string strSql = "";
            foreach (LogType logType in this.GetLogTypes())
            {
                strSql += "," + Function.SqlStr(logType.Code);
            }

            if (strSql.Length > 0)
            {
                strSql = strSql.Substring(1);
                strSql = (enableTableName ? Tables.Log.GetTableName(month) + "." : "") + Tables.Log.LogType + " IN (" + strSql + ")";
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
            foreach (LogType logType in this.GetLogTypes())
            {
                strDescription += "、" + Function.SqlStr(logType.Code);
            }

            if (strDescription.Length > 0)
            {
                strDescription = strDescription.Substring(1);
                strDescription = "类型是" + strDescription + "的日志";
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
