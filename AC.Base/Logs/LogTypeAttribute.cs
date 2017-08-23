using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AC.Base.Logs
{
    /// <summary>
    /// 日志类型特性。每个继承 Log 的非抽象类必须添加该特性，以便描述该日志的一些信息。
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class LogTypeAttribute : System.Attribute
    {
        /// <summary>
        /// 日志类型特性。每个继承 Log 的非抽象类必须添加该特性，以便描述该日志的一些信息。
        /// </summary>
        /// <param name="name">日志类型名。</param>
        /// <param name="sort">日志分类。例如“设备事件/停电事件”，分类之间使用“/”符号分隔。</param>
        /// <param name="description">日志描述。有关该日志详细的说明。</param>
        public LogTypeAttribute(string name, string sort, string description)
        {
            this.Name = name;
            this.Sort = sort;
            this.Description = description;
        }

        /// <summary>
        /// 日志类型名。
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// 日志分类。例如“设备事件/停电事件”，分类之间使用“/”符号分隔。
        /// </summary>
        public string Sort { get; private set; }

        /// <summary>
        /// 日志描述。有关该日志详细的说明。
        /// </summary>
        public string Description { get; private set; }

    }
}
