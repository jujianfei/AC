using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AC.Base.Tasks
{
    /// <summary>
    /// 自动任务周期类型。
    /// </summary>
    public class TaskPeriodType
    {
        private ApplicationClass m_Application;

        internal TaskPeriodType(ApplicationClass application, Type type)
        {
            this.m_Application = application;
            this.Type = type;
        }

        /// <summary>
        /// 获取当前自动任务周期的类型声明。
        /// </summary>
        public Type Type { get; private set; }

        /// <summary>
        /// 自动任务周期类型名。
        /// </summary>
        public string Name
        {
            get
            {
                return ((TaskPeriodTypeAttribute)this.Type.GetCustomAttributes(typeof(TaskPeriodTypeAttribute), false)[0]).Name;
            }
        }

        /// <summary>
        /// 自动任务周期类型代码。
        /// </summary>
        public string Code
        {
            get
            {
                return this.Type.FullName;
            }
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
        /// 获取当前任务周期的新实例。
        /// </summary>
        /// <returns></returns>
        public TaskPeriod CreateTaskPeriod()
        {
            System.Reflection.ConstructorInfo ci = this.Type.GetConstructor(new System.Type[] { });
            object objInstance = ci.Invoke(new object[] { });

            TaskPeriod _TaskPeriod = objInstance as TaskPeriod;
            _TaskPeriod.Application = this.m_Application;
            _TaskPeriod.PeriodType = this;
            return _TaskPeriod;
        }

        /// <summary>
        /// 获取该任务周期的配置界面。
        /// </summary>
        /// <param name="baseType">该配置界面必须继承的基类型。</param>
        /// <returns></returns>
        public object GetConfigControl(Type baseType)
        {
            List<Type> lstType = this.m_Application.GetControlTypes(this.Type);
            if (lstType != null)
            {
                foreach (Type typ in lstType)
                {
                    if (Function.IsInheritableBaseType(typ, baseType))
                    {
                        System.Reflection.ConstructorInfo ci = lstType[0].GetConstructor(new System.Type[] { });
                        return ci.Invoke(new object[] { });
                    }
                }
            }
            return null;
        }
    }

    /// <summary>
    /// 任务周期类型集合。
    /// </summary>
    public class TaskPeriodTypeCollection : ReadOnlyCollection<TaskPeriodType>
    {
        internal TaskPeriodTypeCollection()
        {
        }

        internal new void Add(TaskPeriodType item)
        {
            base.Items.Add(item);
        }

        /// <summary>
        /// 获取指定类型声明的任务周期类型。
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public TaskPeriodType GetPeriodType(Type type)
        {
            foreach (TaskPeriodType periodType in this)
            {
                if (periodType.Type.Equals(type))
                {
                    return periodType;
                }
            }
            return null;
        }

        /// <summary>
        /// 获取指定类型声明的任务周期类型。
        /// </summary>
        /// <param name="typeName"></param>
        /// <returns></returns>
        public TaskPeriodType GetPeriodType(string typeName)
        {
            foreach (TaskPeriodType periodType in this)
            {
                if (periodType.Type.FullName.Equals(typeName))
                {
                    return periodType;
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
            return "共 " + this.Count + " 个任务周期类型";
        }
    }

}
