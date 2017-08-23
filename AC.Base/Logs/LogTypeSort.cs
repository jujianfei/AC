using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AC.Base.Logs
{
    /// <summary>
    /// 日志类型分类。
    /// </summary>
    public class LogTypeSort
    {
        /// <summary>
        /// 日志类型键值集合。
        /// </summary>
        static internal SortedList<string, LogType> AllLogTypes = new SortedList<string, LogType>();
        static LogType st_NonexistentLogType;

        private ApplicationClass m_Application;

        internal LogTypeSort(ApplicationClass application, string name, LogTypeSort parent)
        {
            this.m_Application = application;
            this.Name = name;
            this.Parent = parent;
            this.Children = new LogTypeSortCollection();
            this.LogTypes = new LogTypeCollection();
        }

        /// <summary>
        /// 日志类型分类名。
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// 上层日志类型分类。
        /// </summary>
        public LogTypeSort Parent { get; private set; }

        /// <summary>
        /// 下级日志类型分类。
        /// </summary>
        public LogTypeSortCollection Children { get; private set; }

        /// <summary>
        /// 所属该分类的日志类型。
        /// </summary>
        public LogTypeCollection LogTypes { get; private set; }

        /// <summary>
        /// 获取指定分类路径的日志类型分类。
        /// </summary>
        /// <param name="sortPath">日志类型分类路径。例如“公司名/系列名”，分类之间使用“/”符号分隔。</param>
        /// <returns>日志类型分类。如未查找到该路径的分类则返回 null。</returns>
        public LogTypeSort GetSort(string sortPath)
        {
            string[] strSortNames = sortPath.Split(new char[] { '/' }, StringSplitOptions.RemoveEmptyEntries);

            if (strSortNames.Length == 0)
            {
                return this;
            }
            else
            {
                foreach (LogTypeSort children in this.Children)
                {
                    if (children.Name.Equals(strSortNames[0]))
                    {
                        if (strSortNames.Length == 1)
                        {
                            return children;
                        }
                        else
                        {
                            string strSortName = "";
                            for (int intIndex = 1; intIndex < strSortNames.Length; intIndex++)
                            {
                                strSortName += '/' + strSortNames[intIndex];
                            }
                            return children.GetSort(strSortName.Substring(1));
                        }
                    }
                }

                return null;
            }
        }

        /// <summary>
        /// 获取指定类型名的日志类型。
        /// </summary>
        /// <param name="typeName">类型名称，如“命名空间1.命名空间2.日志类名”</param>
        /// <returns>日志类型。如无对应的日志类型则返回名为“不存在的日志类型”。</returns>
        public LogType GetLogType(string typeName)
        {
            if (AllLogTypes.ContainsKey(typeName))
            {
                return AllLogTypes[typeName];
            }
            else
            {
                if (st_NonexistentLogType == null)
                {
                    st_NonexistentLogType = new LogType(this.m_Application, typeof(NonexistentLog));
                    st_NonexistentLogType.m_Name = "不存在的日志类型";
                    st_NonexistentLogType.m_Code = typeName;
                    st_NonexistentLogType.m_Description = "类型为 " + st_NonexistentLogType.Code + " 的日志不存在";
                }
                return st_NonexistentLogType;
            }
        }

        /// <summary>
        /// 获取指定类型声明的日志类型。
        /// </summary>
        /// <param name="type">日志类型声明</param>
        /// <returns>日志类型。如无对应的日志类型则返回 null。</returns>
        public LogType GetLogType(Type type)
        {
            return this.GetLogType(type.FullName);
        }

        /// <summary>
        /// 获取所有继承自指定类型名的日志类型。
        /// </summary>
        /// <param name="typeName">类型名称或接口名称，如“命名空间1.命名空间2.日志类名”</param>
        /// <returns>继承自指定类型名的日志类型集合。</returns>
        public LogTypeCollection GetLogTypes(string typeName)
        {
            Type t = Function.GetType(typeName);
            if (t != null)
            {
                return this.GetLogTypes(t);
            }
            else
            {
                return new LogTypeCollection();
            }
        }

        /// <summary>
        /// 获取所有继承自指定类型声明的日志类型。
        /// </summary>
        /// <param name="type">日志类型声明或接口声明。</param>
        /// <returns>继承自指定类型声明的日志类型集合。</returns>
        public LogTypeCollection GetLogTypes(Type type)
        {
            LogTypeCollection logTypes = new LogTypeCollection();
            this.FindLogTypes(type, logTypes);
            return logTypes;
        }

        private void FindLogTypes(Type type, LogTypeCollection logTypes)
        {
            foreach (LogType t in this.LogTypes)
            {
                if (type.IsInterface)
                {
                    if (t.Type.GetInterface(type.Name) != null)
                    {
                        logTypes.Add(t);
                    }
                }
                else
                {
                    if (Function.IsInheritableBaseType(t.Type, type) || t.Type.Equals(type))
                    {
                        logTypes.Add(t);
                    }
                }
            }

            foreach (LogTypeSort s in this.Children)
            {
                s.FindLogTypes(type, logTypes);
            }
        }

        internal LogTypeSort FindSort(string sortPath)
        {
            string[] strSortNames = sortPath.Split(new char[] { '/' }, StringSplitOptions.RemoveEmptyEntries);

            if (strSortNames.Length == 0)
            {
                return this;
            }
            else
            {
                foreach (LogTypeSort children in this.Children)
                {
                    if (children.Name.Equals(strSortNames[0]))
                    {
                        if (strSortNames.Length == 1)
                        {
                            return children;
                        }
                        else
                        {
                            string strSortName = "";
                            for (int intIndex = 1; intIndex < strSortNames.Length; intIndex++)
                            {
                                strSortName += '/' + strSortNames[intIndex];
                            }
                            return children.FindSort(strSortName.Substring(1));
                        }
                    }
                }

                LogTypeSort sort = new LogTypeSort(this.m_Application, strSortNames[0], this);
                this.Children.Add(sort);

                if (strSortNames.Length == 1)
                {
                    return sort;
                }
                else
                {
                    string strSortName = "";
                    for (int intIndex = 1; intIndex < strSortNames.Length; intIndex++)
                    {
                        strSortName += '/' + strSortNames[intIndex];
                    }
                    return sort.FindSort(strSortName.Substring(1));
                }
            }
        }

        /// <summary>
        /// 获取当前对象的字符串表示形式。
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            if (this.Name.Length == 0)
            {
                return "[日志分类]";
            }
            else
            {
                return this.Name;
            }
        }

        /// <summary>
        /// 日志类型分类集合。
        /// </summary>
        public class LogTypeSortCollection : ReadOnlyCollection<LogTypeSort>
        {
            internal LogTypeSortCollection()
            {
            }

            internal new void Add(LogTypeSort item)
            {
                base.Add(item);
            }

            /// <summary>
            /// 获取当前对象的字符串表示形式。
            /// </summary>
            /// <returns></returns>
            public override string ToString()
            {
                return "共 " + this.Count + " 个日志类型分类";
            }
        }
    }
}
