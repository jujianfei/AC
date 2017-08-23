using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AC.Base.Tasks
{
    /// <summary>
    /// 任务周期到达运行时间后产生的事件所调用的委托。
    /// </summary>
    /// <param name="taskPeriod">到达运行时间的任务周期。</param>
    /// <param name="taskPlanTime">该任务计划执行时间。由于定时器精确性及通过线程启动任务的时差，使用 DateTime.Now 获得的时间并不是该任务预期的执行时间，taskPlanTime 参数指的是精确的任务预期执行时间。</param>
    public delegate void TaskPeriodTickEventHandler(TaskPeriod taskPeriod, DateTime taskPlanTime);

    /// <summary>
    /// 自动任务周期，用以控制自动任务以何种间隔运行。继承该基类的实体类必须提供一个无参数的构造函数，且必须添加 TaskPeriodTypeAttribute 特性。
    /// </summary>
    public abstract class TaskPeriod
    {
        /// <summary>
        /// 应用程序框架。
        /// </summary>
        public ApplicationClass Application { get; internal set; }

        private TaskPeriodType m_PeriodType;
        /// <summary>
        /// 任务周期类型。
        /// </summary>
        public TaskPeriodType PeriodType
        {
            get
            {
                if (this.m_PeriodType == null)
                {
                    this.m_PeriodType = this.Application.TaskPeriodTypes.GetPeriodType(this.GetType());
                }
                return this.m_PeriodType;
            }
            internal set
            {
                this.m_PeriodType = value;
            }
        }

        /// <summary>
        /// 任务周期到达运行时间后产生的事件。
        /// </summary>
        public event TaskPeriodTickEventHandler Tick;

        /// <summary>
        /// 产生任务周期到达运行时间的事件。
        /// </summary>
        /// <param name="taskPlanTime">该任务计划执行时间。由于定时器精确性及通过线程启动任务的时差，使用 DateTime.Now 获得的时间并不是该任务预期的执行时间，taskPlanTime 参数指的是精确的任务预期执行时间。对于持续运行、间隔运行或指定任务完成后运行等无法预期时间的周期，该参数可使用当前时间。</param>
        protected void OnTick(DateTime taskPlanTime)
        {
            while (DateTime.Now < taskPlanTime)
            {
                //定时器不是很精确，有时会比预定时间提前若干毫秒回调。
                System.Threading.Thread.Sleep(1);
            }

            if (this.Tick != null)
            {
                this.Tick(this, taskPlanTime);
            }
        }

        /// <summary>
        /// 该任务周期开始计时时所相关的任务，任务周期无需对该任务做任何操作，当计时需要访问相关的任务时可使用该属性。
        /// </summary>
        public TaskConfig Config { get; internal set; }

        /// <summary>
        /// 从保存此任务周期数据的 XML 文档节点初始化当前任务。
        /// </summary>
        /// <param name="taskConfig">该对象节点的数据</param>
        public abstract void SetTaskPeriodConfig(System.Xml.XmlNode taskConfig);

        /// <summary>
        /// 获取当前任务周期的配置信息，将序列化的内容填充至 XmlNode 的 InnerText 属性或者 ChildNodes 子节点中。
        /// </summary>
        /// <param name="xmlDoc">创建 XmlNode 时所需使用到的 System.Xml.XmlDocument。</param>
        /// <returns></returns>
        public abstract System.Xml.XmlNode GetTaskPeriodConfig(System.Xml.XmlDocument xmlDoc);

        /// <summary>
        /// 获取该任务周期的描述。
        /// </summary>
        /// <returns></returns>
        public abstract string GetTaskPeriodDescription();

        /// <summary>
        /// 按任务周期的设定开始计时，并在周期时间到达时调用 OnTick() 方法。
        /// </summary>
        public abstract void Start();

        /// <summary>
        /// 下一个周期到达的时间，如果下个周期不可预知则返回 DateTime.MaxValue。
        /// </summary>
        /// <returns></returns>
        public abstract DateTime GetNextRunTime();

        /// <summary>
        /// 停止周期计时。
        /// </summary>
        public abstract void Stop();

    }
}
