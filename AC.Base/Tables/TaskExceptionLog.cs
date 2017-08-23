using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AC.Base.Tables
{
    /// <summary>
    /// 任务运行异常日志。
    /// </summary>
    [Database.Table("任务运行异常日志", Database.TableCreateRuleOptions.Monthly, "")]
    public struct TaskExceptionLog
    {
        /// <summary>
        /// 数据表名
        /// </summary>
        public const string TableName = "TaskExceptionLog";

        /// <summary>
        /// 获取指定月份的数据表名。
        /// </summary>
        /// <param name="date">指定的数据月份。</param>
        /// <returns>含有年月标志的数据表名。</returns>
        public static string GetTableName(DateTime date)
        {
            return Database.DbConnection.GetMonthlyName(TableName, date.Year, date.Month);
        }

        /// <summary>
        /// 任务运行编号
        /// </summary>
        [Database.Column("任务运行编号", "带有毫秒的精确时间值(Ticks)", Database.DataTypeOptions.Long, true, true)]
        public const string TaskLogId = "TaskLogId";

        /// <summary>
        /// 重试次数，第几次重试，该值为 0 表示首次运行。
        /// </summary>
        [Database.Column("重试次数", "第几次重试，该值为 0 表示首次运行。", Database.DataTypeOptions.Int, true, true)]
        public const string RetryTimes = "RetryTimes";

        /// <summary>
        /// 异常序号
        /// </summary>
        [Database.Column("异常序号", "依次产生的异常", Database.DataTypeOptions.Int, true, true)]
        public const string ExceptionId = "ExceptionId";

        /// <summary>
        /// 异常发生时间
        /// </summary>
        [Database.Column("异常发生时间", "带有毫秒的精确时间值(Ticks)", Database.DataTypeOptions.Long, false, true)]
        public const string ExceptionTime = "ExceptionTime";

        /// <summary>
        /// 异常信息
        /// </summary>
        [Database.Column("异常信息", "", Database.DataTypeOptions.Text, false, false)]
        public const string XMLConfig = "XMLConfig";

    }
}
