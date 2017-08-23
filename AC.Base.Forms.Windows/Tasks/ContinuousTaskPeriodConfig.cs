using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using AC.Base.Tasks;

namespace AC.Base.Forms.Windows.Tasks
{
    /// <summary>
    /// 单次运行的自动任务周期配置界面。
    /// </summary>
    [Control(typeof(AC.Base.Tasks.ContinuousTaskPeriod))]
    public class ContinuousTaskPeriodConfig : Label, ITaskPeriodConfigControl
    {
        private ContinuousTaskPeriod m_TaskPeriod;

        /// <summary>
        /// 单次运行的自动任务周期配置界面。
        /// </summary>
        public ContinuousTaskPeriodConfig()
        {
            this.Text = "启动后立即运行任务，直至任务结束或手动停止任务。";
        }

        #region ITaskPeriodConfigControl 成员

        /// <summary>
        /// 设置需配置的任务周期对象。
        /// </summary>
        /// <param name="taskPeriod">任务周期。</param>
        /// <param name="currentTaskConfig">设置周期时所配置的任务配置对象。</param>
        public void SetTaskPeriod(TaskPeriod taskPeriod, TaskConfig currentTaskConfig)
        {
            this.m_TaskPeriod = taskPeriod as ContinuousTaskPeriod;
        }

        /// <summary>
        /// 获取配置的任务周期对象。
        /// </summary>
        /// <returns>任务周期。</returns>
        public TaskPeriod GetTaskPeriod()
        {
            return this.m_TaskPeriod;
        }

        #endregion
    }
}
