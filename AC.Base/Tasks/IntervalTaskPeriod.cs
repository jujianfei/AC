using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AC.Base.Tasks
{
    /// <summary>
    /// 间隔指定的时间反复运行。
    /// </summary>
    [TaskPeriodType("按设定间歇运行")]
    public class IntervalTaskPeriod : TaskPeriod
    {
        private DateTime m_NextRunTime = DateTime.MinValue;
        private System.Threading.Timer m_Timer;

        private int m_IntervalTime = 60;
        /// <summary>
        /// 间隔运行的时间(秒)，该值默认为 60 秒，最小不得小于 1 秒。
        /// </summary>
        public int IntervalTime
        {
            get
            {
                return this.m_IntervalTime;
            }
            set
            {
                if (value < 1)
                {
                    throw new Exception("间隔时间最小不得小于 1 秒。");
                }
                this.m_IntervalTime = value;
            }
        }

        /// <summary>
        /// 从保存此任务周期数据的 XML 文档节点初始化当前任务。
        /// </summary>
        /// <param name="taskConfig">该对象节点的数据</param>
        public override void SetTaskPeriodConfig(System.Xml.XmlNode taskConfig)
        {
            this.IntervalTime = Function.ToInt(taskConfig.InnerText);
        }

        /// <summary>
        /// 获取当前任务周期的配置信息，将序列化的内容填充至 XmlNode 的 InnerText 属性或者 ChildNodes 子节点中。
        /// </summary>
        /// <param name="xmlDoc">创建 XmlNode 时所需使用到的 System.Xml.XmlDocument。</param>
        /// <returns></returns>
        public override System.Xml.XmlNode GetTaskPeriodConfig(System.Xml.XmlDocument xmlDoc)
        {
            System.Xml.XmlNode xnConfig = xmlDoc.CreateElement(this.GetType().Name);
            xnConfig.InnerText = ((int)this.IntervalTime).ToString();
            return xnConfig;
        }

        /// <summary>
        /// 获取该任务周期的描述。
        /// </summary>
        /// <returns></returns>
        public override string GetTaskPeriodDescription()
        {
            if (IntervalTime < 60)
            {
                return "间隔" + IntervalTime + "秒";
            }
            else if (IntervalTime < 3600)
            {
                return "间隔" + (IntervalTime / 60) + "分" + (IntervalTime % 60) + "秒";
            }
            else
            {
                return "间隔" + (IntervalTime / 3600) + "小时" + ((IntervalTime / 60) % 60) + "分" + (IntervalTime % 60) + "秒";
            }
        }

        private bool IsRun = false;

        /// <summary>
        /// 按任务周期的设定开始计时，并在周期时间到达时调用 OnTick() 方法。
        /// </summary>
        public override void Start()
        {
            this.m_Timer = new System.Threading.Timer(new System.Threading.TimerCallback(this.TimerTick));
            base.Config.TaskCreated += new TaskCreatedEventHandler(TaskConfig_TaskCreated);
            this.NextRunTime();
        }

        private void TaskConfig_TaskCreated(Task task)
        {
            if (this.IsRun == false)
            {
                this.IsRun = true;
                task.Stopped += new TaskStoppedEventHandler(task_TaskStopped);
            }
        }

        private void task_TaskStopped(Task task)
        {
            if (this.IsRun)
            {
                this.IsRun = false;
                task.Stopped -= new TaskStoppedEventHandler(task_TaskStopped);
                this.NextRunTime();
            }
        }

        private void TimerTick(object obj)
        {
            base.OnTick(this.m_NextRunTime);
        }

        private void NextRunTime()
        {
            if (this.IsRun == false)
            {
                this.m_NextRunTime = DateTime.Now.AddSeconds(this.IntervalTime);
                if (this.m_Timer != null)
                    this.m_Timer.Change(this.m_NextRunTime - DateTime.Now, TimeSpan.FromTicks(-1));
            }
        }

        /// <summary>
        /// 下一个周期到达的时间。
        /// </summary>
        /// <returns></returns>
        public override DateTime GetNextRunTime()
        {
            if (this.IsRun)
            {
                return DateTime.Now.AddSeconds(this.IntervalTime);
            }
            else
            {
                return this.m_NextRunTime;
            }
        }

        /// <summary>
        /// 停止周期计时。
        /// </summary>
        public override void Stop()
        {
            base.Config.TaskCreated -= new TaskCreatedEventHandler(TaskConfig_TaskCreated);

            if (this.m_Timer != null)
            {
                this.m_Timer.Dispose();
                this.m_Timer = null;
            }
        }
    }
}
