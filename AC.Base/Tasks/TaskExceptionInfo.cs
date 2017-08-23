using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AC.Base.Tasks
{
    /// <summary>
    /// 任务运行中产生的异常信息。
    /// </summary>
    public class TaskExceptionInfo
    {
        internal TaskExceptionInfo(Exception exception)
        {
            this.ExceptionTime = DateTime.Now;
            this.ExceptionInfo = exception.ToString();
        }

        internal TaskExceptionInfo(long exceptionTime, string exceptionMessage)
        {
            this.ExceptionTime = new DateTime(exceptionTime);
            this.ExceptionInfo = exceptionMessage;
        }

        /// <summary>
        /// 该异常产生的时间。
        /// </summary>
        public DateTime ExceptionTime { get; private set; }

        /// <summary>
        /// 异常详细信息。
        /// </summary>
        public string ExceptionInfo { get; private set; }
    }

    /// <summary>
    /// 任务异常信息集合。
    /// </summary>
    public class TaskExceptionInfoCollection : ReadOnlyCollection<TaskExceptionInfo>
    {
        internal TaskExceptionInfoCollection()
        {
        }

        internal void Add(Exception exception)
        {
            base.Add(new TaskExceptionInfo(exception));
        }

        internal void Add(long exceptionTime, string exceptionMessage)
        {
            base.Add(new TaskExceptionInfo(exceptionTime, exceptionMessage));
        }

        internal new void Add(TaskExceptionInfo exceptionInfo)
        {
            base.Add(exceptionInfo);
        }
    }
}
