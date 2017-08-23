using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AC.Base.Tasks
{
    /// <summary>
    /// 自动任务周期类型特性。每个继承 TaskPeriod 的非抽象类必须添加该特性，以便描述该任务周期的一些信息。
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class TaskPeriodTypeAttribute : System.Attribute
    {
        /// <summary>
        /// 自动任务周期类型特性。
        /// </summary>
        /// <param name="name">自动任务周期类型名。</param>
        public TaskPeriodTypeAttribute(string name)
        {
            this.Name = name;
        }

        /// <summary>
        /// 自动任务周期类型名。
        /// </summary>
        public string Name { get; private set; }
    }
}
