using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AC.Base.Tasks
{
    /// <summary>
    /// 前置任务
    /// </summary>
    [TaskPeriodType("指定任务完成后运行")]
    public class PredecessorTaskPeriod : TaskPeriod
    {
        private TaskConfig m_PredecessorTask;
        /// <summary>
        /// 前置任务。
        /// </summary>
        public TaskConfig PredecessorTask
        {
            get
            {
                if (this.m_PredecessorTask == null && this.TaskConfigId > 0)
                {
                    foreach (TaskGroup _TaskGroup in base.Application.TaskGroups)
                    {
                        foreach (TaskConfig _TaskConfig in _TaskGroup.TaskConfigs)
                        {
                            if (_TaskConfig.TaskConfigId == this.TaskConfigId)
                            {
                                this.PredecessorTask = _TaskConfig;
                                break;
                            }
                        }
                    }
                }
                return this.m_PredecessorTask;
            }
            set
            {
                this.m_PredecessorTask = value;
            }
        }

        private int TaskConfigId;

        /// <summary>
        /// 从保存此任务周期数据的 XML 文档节点初始化当前任务。
        /// </summary>
        /// <param name="taskConfig">该对象节点的数据</param>
        public override void SetTaskPeriodConfig(System.Xml.XmlNode taskConfig)
        {
            this.TaskConfigId = Function.ToInt(taskConfig.InnerText);
        }

        /// <summary>
        /// 获取当前任务周期的配置信息，将序列化的内容填充至 XmlNode 的 InnerText 属性或者 ChildNodes 子节点中。
        /// </summary>
        /// <param name="xmlDoc">创建 XmlNode 时所需使用到的 System.Xml.XmlDocument。</param>
        /// <returns></returns>
        public override System.Xml.XmlNode GetTaskPeriodConfig(System.Xml.XmlDocument xmlDoc)
        {
            if (this.PredecessorTask != null)
            {
                System.Xml.XmlNode xnConfig = xmlDoc.CreateElement(this.GetType().Name);
                xnConfig.InnerText = this.PredecessorTask.TaskConfigId.ToString();
                return xnConfig;
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// 获取该任务周期的描述。
        /// </summary>
        /// <returns></returns>
        public override string GetTaskPeriodDescription()
        {
            if (this.PredecessorTask != null)
            {
                return this.PredecessorTask.Name + "运行之后";
            }
            else
            {
                return "未配置";
            }
        }

        /// <summary>
        /// 按任务周期的设定开始计时，并在周期时间到达时调用 OnTick() 方法。
        /// </summary>
        public override void Start()
        {
            if (this.PredecessorTask != null)
            {
                this.PredecessorTask.TaskCreated += new TaskCreatedEventHandler(PredecessorTask_TaskCreated);
            }
        }

        private void PredecessorTask_TaskCreated(Task task)
        {
            task.Stopped += new TaskStoppedEventHandler(task_TaskStopped);
        }

        private void task_TaskStopped(Task task)
        {
            task.Stopped -= new TaskStoppedEventHandler(task_TaskStopped);

            base.OnTick(DateTime.Now);
        }

        /// <summary>
        /// 下一个周期到达的时间。
        /// </summary>
        /// <returns></returns>
        public override DateTime GetNextRunTime()
        {
            return DateTime.MaxValue;
        }

        /// <summary>
        /// 停止周期计时。
        /// </summary>
        public override void Stop()
        {
            if (this.PredecessorTask != null)
            {
                this.PredecessorTask.TaskCreated -= new TaskCreatedEventHandler(PredecessorTask_TaskCreated);
            }
        }
    }
}
