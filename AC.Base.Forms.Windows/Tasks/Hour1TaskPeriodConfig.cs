using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using AC.Base.Tasks;

namespace AC.Base.Forms.Windows.Tasks
{
    /// <summary>
    /// 每小时执行一次的自动任务周期配置界面。
    /// </summary>
    [Control(typeof(AC.Base.Tasks.Hour1TaskPeriod))]
    public class Hour1TaskPeriodConfig : Label, ITaskPeriodConfigControl
    {
        private Hour1TaskPeriod m_TaskPeriod;

        /// <summary>
        /// 每小时执行一次的自动任务周期配置界面。
        /// </summary>
        public Hour1TaskPeriodConfig()
        {
            this.Text = "每小时整点启动运行，每天运行24次。";
        }

        #region ITaskPeriodConfigControl 成员

        /// <summary>
        /// 设置需配置的任务周期对象。
        /// </summary>
        /// <param name="taskPeriod">任务周期。</param>
        /// <param name="currentTaskConfig">设置周期时所配置的任务配置对象。</param>
        public void SetTaskPeriod(TaskPeriod taskPeriod, TaskConfig currentTaskConfig)
        {
            this.m_TaskPeriod = taskPeriod as Hour1TaskPeriod;
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
