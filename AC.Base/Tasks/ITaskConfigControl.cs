using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace AC.Base.Tasks
{
    /// <summary>
    /// 任务配置控件接口。实现该接口的类必须添加 ControlAttribute 属性，并且将 forType 参数指向继承自 TaskConfig 的任务配置类。
    /// </summary>
    public interface ITaskConfigControl : IControl
    {
        /// <summary>
        /// 设置控件颜色
        /// </summary>
        /// <param name="color">主要颜色</param>
        void SetControlColor(Color color);

        /// 设置需配置的任务对象。
        /// </summary>
        /// <param name="taskConfig">任务配置。</param>
        void SetTaskConfig(TaskConfig taskConfig);

        /// <summary>
        /// 获取配置的任务配置对象。
        /// </summary>
        /// <returns>任务配置。</returns>
        TaskConfig GetTaskConfig();
    }
}
