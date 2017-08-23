using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AC.Base.Tasks
{
    /// <summary>
    /// 任务周期配置控件接口。实现该接口的类必须添加 ControlAttribute 属性。
    /// </summary>
    public interface ITaskPeriodConfigControl : IControl
    {
        /// <summary>
        /// 设置需配置的任务周期对象。
        /// </summary>
        /// <param name="taskPeriod">任务周期。</param>
        /// <param name="currentTaskConfig">设置周期时所配置的任务配置对象。</param>
        void SetTaskPeriod(TaskPeriod taskPeriod, TaskConfig currentTaskConfig);

        /// <summary>
        /// 获取配置的任务周期对象。
        /// </summary>
        /// <returns>任务周期。</returns>
        TaskPeriod GetTaskPeriod();
    }
}
