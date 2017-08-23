using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AC.Base.Tables
{
    /// <summary>
    /// 任务运行日志。
    /// </summary>
    [Database.Table("任务运行日志", Database.TableCreateRuleOptions.Monthly, "")]
    public struct TaskLog
    {
        /// <summary>
        /// 数据表名
        /// </summary>
        public const string TableName = "TaskLog";

        /// <summary>
        /// 获取指定月份的数据表名。
        /// </summary>
        /// <param name="date">指定的数据月份。</param>
        /// <returns>含有年月标志的数据表名。</returns>
        public static string GetTableName(DateTime date)
        {
            return Database.DbConnection.GetMonthlyName(TableName, date.Year,date.Month);
        }

        /// <summary>
        /// 任务运行编号
        /// </summary>
        [Database.Column("任务运行编号", "带有毫秒的精确时间值(Ticks)", Database.DataTypeOptions.Long, true, true)]
        public const string TaskLogId = "TaskLogId";

        /// <summary>
        /// 任务配置编号
        /// </summary>
        [Database.Column("任务配置编号", "", Database.DataTypeOptions.Int, false, true)]
        public const string TaskConfigId = "TaskConfigId";

        /// <summary>
        /// 首次是否自动运行
        /// </summary>
        [Database.Column("首次是否自动运行", "0:手动；1:自动", Database.DataTypeOptions.Int, false, true)]
        public const string IsAuto = "IsAuto";

        /// <summary>
        /// 计划执行时间
        /// </summary>
        [Database.Column("计划执行时间", "带有毫秒的精确时间值(Ticks)", Database.DataTypeOptions.Long, false, true)]
        public const string PlanTime = "PlanTime";

        /// <summary>
        /// 已重试次数
        /// </summary>
        [Database.Column("已重试次数", "", Database.DataTypeOptions.Int, false, true)]
        public const string RetryTimes = "RetryTimes";

        /// <summary>
        /// 最终完成进度
        /// </summary>
        [Database.Column("最终完成进度", "", 5, 4, false, true)]
        public const string LastProgress = "LastProgress";

        /// <summary>
        /// 已重试次数
        /// </summary>
        [Database.Column("任务名称", "", Database.DataTypeOptions.VarChar, false, true)]
        public const string TaskName = "TaskName";
        
    }
}
