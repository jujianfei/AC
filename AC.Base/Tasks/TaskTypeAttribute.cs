using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AC.Base.Tasks
{
    /// <summary>
    /// 自动任务类型特性。每个继承 Task 的非抽象类必须添加该特性，以便描述该自动任务的一些信息。
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class TaskTypeAttribute : System.Attribute
    {
        /// <summary>
        /// 自动任务类型特性。
        /// </summary>
        /// <param name="name">自动任务类型名。</param>
        /// <param name="description">自动任务描述。有关该自动任务详细的说明。</param>
        /// <param name="configType">该自动任务配置类的类型声明，指向的类必须继承自 TaskConfig，并且提供无参数的构造函数。</param>
        /// <param name="periodTypes">该任务可用的周期类型，每个任务至少需要使用一种周期类型。</param>
        /// <param name="maxRetryTimes">最大允许的重试次数。如果该值为 0 表示无论任务是异常退出还是未能 100% 完成均不重新运行；如果该值大于 0 ，并且首次任务运行后未能 100% 完成任务则在指定延时后重新运行任务。</param>
        /// <param name="imageType">该设备所使用的图标类，指向该类型的类必须实现 IDeviceTypeImage 接口。如不提供图标可传入 null。</param>
        public TaskTypeAttribute(string name, string description, Type configType, Type[] periodTypes, int maxRetryTimes,Type imageType)
        {
            if (configType == null)
            {
                throw new Exception("自动任务“" + name + "”未设置任务配置类。");
            }
            else if (Function.IsInheritableBaseType(configType, typeof(TaskConfig)) == false)
            {
                throw new Exception("自动任务“" + name + "”的任务配置类 " + configType.FullName + " 必须从 " + typeof(TaskConfig).FullName + " 继承。");
            }

            if (periodTypes == null || periodTypes.Length == 0)
            {
                throw new Exception("至少需要为自动任务“" + name + "”设置一种周期类型。");
            }
            else
            {
                foreach (Type typ in periodTypes)
                {
                    if (Function.IsInheritableBaseType(typ, typeof(TaskPeriod)) == false)
                    {
                        throw new Exception("自动任务“" + name + "”的周期类型 " + typ.FullName + " 必须从 " + typeof(TaskPeriod).FullName + " 继承。");
                    }
                }
            }

            this.Name = name;
            this.Description = description;
            this.ConfigType = configType;
            this.PeriodTypes = periodTypes;
            this.MaxRetryTimes = maxRetryTimes;
            this.ImageType = imageType;
        }

        /// <summary>
        /// 自动任务类型名。
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// 自动任务描述。有关该自动任务详细的说明。
        /// </summary>
        public string Description { get; private set; }

        /// <summary>
        /// 该自动任务配置类的类型声明，指向的类必须继承自 TaskConfig。
        /// </summary>
        public Type ConfigType { get; private set; }

        /// <summary>
        /// 该任务可用的周期类型。
        /// </summary>
        public Type[] PeriodTypes { get; private set; }

        /// <summary>
        /// 最大允许的重试次数。
        /// </summary>
        public int MaxRetryTimes { get; private set; }

        /// <summary>
        /// 该任务所使用的图标类。
        /// </summary>
        public Type ImageType { get; private set; }
    }
}
