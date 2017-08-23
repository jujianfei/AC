using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AC.Base.Logs
{
    /// <summary>
    /// 描述继承 Log 的日志类型。如果 ParentTypes 属性为 null 则表明该日志是一个通道，必须作为根节点日志；如果 ChildrenTypes 属性为 null 则表明该日志是一个末端节点的日志，不允许再连接下级日志。
    /// </summary>
    public class LogType
    {
        private ApplicationClass m_Application;

        internal LogType(ApplicationClass application, Type type)
        {
            this.m_Application = application;
            this.Type = type;
        }

        /// <summary>
        /// 获取当前日志的类型声明。
        /// </summary>
        public Type Type { get; private set; }

        internal string m_Name;
        /// <summary>
        /// 日志类型名。
        /// </summary>
        public string Name
        {
            get
            {
                if (this.m_Name == null)
                {
                    this.m_Name = ((LogTypeAttribute)this.Type.GetCustomAttributes(typeof(LogTypeAttribute), false)[0]).Name;
                }
                return this.m_Name;
            }
        }

        internal string m_Code;
        /// <summary>
        /// 日志类型代码。
        /// </summary>
        public string Code
        {
            get
            {
                if (this.m_Code == null)
                {
                    return this.Type.FullName;
                }
                else
                {
                    return this.m_Code;
                }
            }
        }

        /// <summary>
        /// 该日志类型所属分类。
        /// </summary>
        public LogTypeSort Sort { get; internal set; }

        internal string m_Description;
        /// <summary>
        /// 日志描述。有关该日志详细的说明。
        /// </summary>
        public string Description
        {
            get
            {
                if (this.m_Description == null)
                {
                    this.m_Description = ((LogTypeAttribute)this.Type.GetCustomAttributes(typeof(LogTypeAttribute), false)[0]).Description;
                }
                return this.m_Description;
            }
        }

        internal Log CreateLog(System.Data.IDataReader dr)
        {
            System.Reflection.ConstructorInfo ci = this.Type.GetConstructor(new System.Type[] { typeof(ApplicationClass) });
            object objInstance = ci.Invoke(new object[] { this.m_Application });

            Log log = objInstance as Log;
            log.LogType = this;
            log.SetDataReader(dr);
            return log;
        }

        /// <summary>
        /// 获取当前对象的字符串表示形式。
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return this.Name;
        }
    }


    /// <summary>
    /// 日志类型集合。
    /// </summary>
    public class LogTypeCollection : ReadOnlyCollection<LogType>
    {
        internal LogTypeCollection()
        {
        }

        internal new void Add(LogType item)
        {
            if (LogTypeSort.AllLogTypes.ContainsKey(item.Code) == false)
            {
                LogTypeSort.AllLogTypes.Add(item.Code, item);
                base.Items.Add(item);
            }
        }

        /// <summary>
        /// 获取当前对象的字符串表示形式。
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return "共 " + this.Count + " 个日志类型";
        }
    }
}
