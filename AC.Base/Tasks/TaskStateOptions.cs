using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AC.Base.Tasks
{
    /// <summary>
    /// 任务状态。
    /// </summary>
    public enum TaskStateOptions
    {
        /// <summary>
        /// 开始。（任务的初始状态）
        /// </summary>
        Start,

        /// <summary>
        /// 运行中
        /// </summary>
        Running,

        /// <summary>
        /// 任务成功运行完成
        /// </summary>
        Done,

        /// <summary>
        /// 强行终止中
        /// </summary>
        Stopping,

        /// <summary>
        /// 任务被强行终止
        /// </summary>
        Stop,

        /// <summary>
        /// 任务异常结束
        /// </summary>
        Exception,
    }

    /// <summary>
    /// 任务状态扩展。
    /// </summary>
    public static class TaskStateExtensions
    {
        /// <summary>
        /// 获取任务状态的文字说明。
        /// </summary>
        /// <param name="taskState"></param>
        /// <returns></returns>
        public static string GetDescription(this TaskStateOptions taskState)
        {
            switch (taskState)
            {
                case TaskStateOptions.Start:
                    return "启动";

                case TaskStateOptions.Running:
                    return "运行中";

                case TaskStateOptions.Done:
                    return "运行完成";

                case TaskStateOptions.Stopping:
                    return "正在终止";

                case TaskStateOptions.Stop:
                    return "强行终止";

                case TaskStateOptions.Exception:
                    return "异常终止";

                default:
                    throw new NotImplementedException("尚未实现该枚举。");
            }
        }
    }
}
