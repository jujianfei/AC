using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AC.Base.Tables
{
    /// <summary>
    /// 任务首次及重试运行详细运行日志。
    /// </summary>
    [Database.Table("任务详细运行日志", Database.TableCreateRuleOptions.Monthly, "")]
    public struct TaskRunLog
    {
        /// <summary>
        /// 数据表名
        /// </summary>
        public const string TableName = "TaskRunLog";

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
        /// 开始运行时间
        /// </summary>
        [Database.Column("开始运行时间", "带有毫秒的精确时间值(Ticks)", Database.DataTypeOptions.Long, false, true)]
        public const string StartTime = "StartTime";

        /// <summary>
        /// 结束运行时间
        /// </summary>
        [Database.Column("结束运行时间", "带有毫秒的精确时间值(Ticks)", Database.DataTypeOptions.Long, false, true)]
        public const string StopTime = "StopTime";

        /// <summary>
        /// 任务运行时长，结束时间减起始时间的时间间隔，精确到毫秒
        /// </summary>
        [Database.Column("任务运行时长", "结束时间减起始时间的时间间隔，精确到毫秒", Database.DataTypeOptions.Long, false, true)]
        public const string TimeSpan = "TimeSpan";

        /// <summary>
        /// 任务状态
        /// </summary>
        [Database.Column("任务状态", "", Database.DataTypeOptions.Int, false, true)]
        public const string TaskState = "TaskState";

        /// <summary>
        /// 本次任务完成进度
        /// </summary>
        [Database.Column("本次任务完成进度", "", 5, 4, false, true)]
        public const string TaskProgress = "TaskProgress";

        /// <summary>
        /// 本次任务运行信息
        /// </summary>
        [Database.Column("本次任务运行信息", "有关任务运行进度及完成情况的配置记录，下次重试时根据上次的运行记录继续运行", Database.DataTypeOptions.Text, false, false)]
        public const string XMLConfig = "XMLConfig";

        /// <summary>
        /// 测试设备名称
        /// </summary>
        [Database.Column("测试设备名称", "测试设备名称", Database.DataTypeOptions.VarChar, false, false)]
        public const string DeviceName = "DeviceName";

        /// <summary>
        /// 本次任务运行信息
        /// </summary>
        [Database.Column("日志文件索引", "日志文件索引", Database.DataTypeOptions.VarChar, false, false)]
        public const string TimeStr = "TimeStr";
        
    }
}
