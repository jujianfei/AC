using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AC.Base.Tasks
{
    /// <summary>
    /// 每日执行一次的自动任务周期。
    /// </summary>
    [TaskPeriodType("每日运行一次")]
    public class DayTaskPeriod : TaskPeriod
    {
        private DateTime m_NextRunTime = DateTime.MinValue;
        private System.Threading.Timer m_Timer;

        /// <summary>
        /// 获取或设置该任务周期每天运行的时间(hhmmss)。
        /// </summary>
        public int TimeNum { get; set; }

        /// <summary>
        /// 从保存此任务周期数据的 XML 文档节点初始化当前任务。
        /// </summary>
        /// <param name="taskConfig">该对象节点的数据</param>
        public override void SetTaskPeriodConfig(System.Xml.XmlNode taskConfig)
        {
            this.TimeNum = Function.ToInt(taskConfig.InnerText);
        }

        /// <summary>
        /// 获取当前任务周期的配置信息，将序列化的内容填充至 XmlNode 的 InnerText 属性或者 ChildNodes 子节点中。
        /// </summary>
        /// <param name="xmlDoc">创建 XmlNode 时所需使用到的 System.Xml.XmlDocument。</param>
        /// <returns></returns>
        public override System.Xml.XmlNode GetTaskPeriodConfig(System.Xml.XmlDocument xmlDoc)
        {
            System.Xml.XmlNode xnConfig = xmlDoc.CreateElement(this.GetType().Name);
            xnConfig.InnerText = this.TimeNum.ToString();
            return xnConfig;
        }

        /// <summary>
        /// 获取该任务周期的描述。
        /// </summary>
        /// <returns></returns>
        public override string GetTaskPeriodDescription()
        {
            return "每天" + (this.TimeNum / 10000) + ":" + Function.StringInterceptFill((this.TimeNum / 100) % 100, 2, "0", false) + ":" + Function.StringInterceptFill(this.TimeNum % 100, 2, "0", false);
        }

        /// <summary>
        /// 按任务周期的设定开始计时，并在周期时间到达时调用 OnTick() 方法。
        /// </summary>
        public override void Start()
        {
            this.m_Timer = new System.Threading.Timer(new System.Threading.TimerCallback(this.TimerTick));

            this.m_NextRunTime = DateTime.Today;
            this.m_NextRunTime = this.m_NextRunTime.AddHours(this.TimeNum / 10000);
            this.m_NextRunTime = this.m_NextRunTime.AddMinutes((this.TimeNum / 100) % 100);
            if (this.m_NextRunTime > DateTime.Now)
            {
                this.m_NextRunTime = this.m_NextRunTime.AddDays(-1);
            }
            this.NextRunTime();
        }

        private void TimerTick(object obj)
        {
            DateTime dtm = this.m_NextRunTime;
            this.NextRunTime();
            base.OnTick(dtm);
        }

        private void NextRunTime()
        {
            this.m_NextRunTime = this.m_NextRunTime.AddDays(1);
            this.m_Timer.Change(this.m_NextRunTime - DateTime.Now, TimeSpan.FromTicks(-1));
        }

        /// <summary>
        /// 下一个周期到达的时间。
        /// </summary>
        /// <returns></returns>
        public override DateTime GetNextRunTime()
        {
            return this.m_NextRunTime;
        }

        /// <summary>
        /// 停止周期计时。
        /// </summary>
        public override void Stop()
        {
            if (this.m_Timer != null)
            {
                this.m_Timer.Dispose();
                this.m_Timer = null;
            }
        }
    }
}
