using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AC.Base.Tasks
{
    /// <summary>
    /// 描述继承 Task 的自动任务类型。
    /// </summary>
    public class TaskType
    {
        private ApplicationClass m_Application;

        internal TaskType(ApplicationClass application, Type type)
        {
            this.m_Application = application;
            this.Type = type;
        }

        /// <summary>
        /// 获取当前自动任务的类型声明。
        /// </summary>
        public Type Type { get; private set; }

        /// <summary>
        /// 自动任务类型名。
        /// </summary>
        public string Name
        {
            get
            {
                return ((TaskTypeAttribute)this.Type.GetCustomAttributes(typeof(TaskTypeAttribute), false)[0]).Name;
            }
        }

        /// <summary>
        /// 自动任务类型代码。
        /// </summary>
        public string Code
        {
            get
            {
                return this.Type.FullName;
            }
        }

        /// <summary>
        /// 自动任务描述。有关该自动任务详细的说明。
        /// </summary>
        public string Description
        {
            get
            {
                return ((TaskTypeAttribute)this.Type.GetCustomAttributes(typeof(TaskTypeAttribute), false)[0]).Description;
            }
        }

        /// <summary>
        /// 该自动任务配置类的类型声明，指向的类必须继承自 TaskConfig。
        /// </summary>
        public Type ConfigType
        {
            get
            {
                return ((TaskTypeAttribute)this.Type.GetCustomAttributes(typeof(TaskTypeAttribute), false)[0]).ConfigType;
            }
        }

        private TaskPeriodTypeCollection m_PeriodTypes;
        /// <summary>
        /// 该任务可用的周期类型。
        /// </summary>
        public TaskPeriodTypeCollection PeriodTypes
        {
            get
            {
                if (this.m_PeriodTypes == null)
                {
                    this.m_PeriodTypes = new TaskPeriodTypeCollection();

                    Type[] typs = ((TaskTypeAttribute)this.Type.GetCustomAttributes(typeof(TaskTypeAttribute), false)[0]).PeriodTypes;

                    for (int intIndex = 0; intIndex < typs.Length; intIndex++)
                    {
                        this.m_PeriodTypes.Add(this.m_Application.TaskPeriodTypes.GetPeriodType(typs[intIndex]));
                    }
                }
                return this.m_PeriodTypes;
            }
        }

        /// <summary>
        /// 最大允许的重试次数。如果该值为 0 表示无论任务是异常退出还是未能 100% 完成均不重新运行；如果该值大于 0 ，并且首次任务运行后未能 100% 完成任务则在指定延时后重新运行任务。
        /// </summary>
        public int MaxRetryTimes
        {
            get
            {
                return ((TaskTypeAttribute)this.Type.GetCustomAttributes(typeof(TaskTypeAttribute), false)[0]).MaxRetryTimes;
            }
        }

        /// <summary>
        /// 获取当前自动任务配置类型的新实例。
        /// </summary>
        /// <param name="group"></param>
        /// <returns></returns>
        public TaskConfig CreateTaskConfig(TaskGroup group)
        {
            System.Reflection.ConstructorInfo ci = this.ConfigType.GetConstructor(new System.Type[] { });
            object objInstance = ci.Invoke(new object[] { });

            TaskConfig taskConfig = objInstance as TaskConfig;
            taskConfig.Application = this.m_Application;
            taskConfig.TaskType = this;
            taskConfig.Group = group;
            return taskConfig;
        }

        /// <summary>
        /// 获取当前对象的字符串表示形式。
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return this.Name;
        }

        /// <summary>
        /// 该类设备使用的 16 * 16 图标。如果设备未提供图标则此属性提供默认的设备图标。
        /// </summary>
        public System.Drawing.Image Icon16
        {
            get
            {
                IIcon img = this.GetImages(this.Type);
                if (img != null && img.Icon16 != null)
                {
                    return img.Icon16;
                }
                else
                {
                    return Properties.Resources.Device16;
                }
            }
        }

        /// <summary>
        /// 该类设备使用的 32 * 32 图标。如果设备未提供图标则此属性提供默认的设备图标。
        /// </summary>
        public System.Drawing.Image Icon32
        {
            get
            {
                IIcon img = this.GetImages(this.Type);
                if (img != null && img.Icon32 != null)
                {
                    return img.Icon32;
                }
                else
                {
                    return Properties.Resources.Device32;
                }
            }
        }

        //该任务的图标和照片。
        private IIcon GetImages(Type taskType)
        {
            if (taskType.GetCustomAttributes(typeof(TaskTypeAttribute), false).Length > 0)
            {
                TaskTypeAttribute attr = (TaskTypeAttribute)taskType.GetCustomAttributes(typeof(TaskTypeAttribute), false)[0];
                if (attr.ImageType != null && attr.ImageType.GetInterface(typeof(IIcon).FullName) != null)
                {
                    System.Reflection.ConstructorInfo ci = attr.ImageType.GetConstructor(new System.Type[] { });
                    object objInstance = ci.Invoke(new object[] { });

                    return objInstance as IIcon;
                }
                else
                {
                    if (taskType.BaseType != typeof(Task) && taskType.BaseType.IsAbstract == false)
                    {
                        return GetImages(taskType.BaseType);
                    }
                    else
                    {
                        return null;
                    }
                }
            }
            else
            {
                return null;
            }
        }
    }

    /// <summary>
    /// 自动任务类型集合。
    /// </summary>
    public class TaskTypeCollection : ReadOnlyCollection<TaskType>
    {
        internal TaskTypeCollection()
        {
        }

        internal new void Add(TaskType item)
        {
            base.Items.Add(item);
        }

        /// <summary>
        /// 获取指定类型声明的任务类型。
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public TaskType GetTaskType(Type type)
        {
            foreach (TaskType taskType in this)
            {
                if (taskType.Type.Equals(type))
                {
                    return taskType;
                }
            }
            return null;
        }

        /// <summary>
        /// 获取指定类型声明的任务类型。
        /// </summary>
        /// <param name="typeName"></param>
        /// <returns></returns>
        public TaskType GetTaskType(string typeName)
        {
            foreach (TaskType taskType in this)
            {
                if (taskType.Type.FullName.Equals(typeName))
                {
                    return taskType;
                }
            }
            return null;
        }

        /// <summary>
        /// 获取当前对象的字符串表示形式。
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return "共 " + this.Count + " 个自动任务类型";
        }
    }
}
