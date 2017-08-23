using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AC.Base.Tables
{
    /// <summary>
    /// 日志。
    /// </summary>
    [Database.Table("日志", Database.TableCreateRuleOptions.Monthly, "")]
    public struct Log
    {
        /// <summary>
        /// 数据表名
        /// </summary>
        public const string TableName = "Log";

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
        /// 日志编号
        /// </summary>
        [Database.Column("日志编号", "带有毫秒的精确时间值(Ticks)", Database.DataTypeOptions.Long, true, true)]
        public const string LogId = "LogId";

        /// <summary>
        /// 日志类型
        /// </summary>
        [Database.Column("日志类型", "", 250, false, true, new string[] { "Log_LogType" })]
        public const string LogType = "LogType";

        /// <summary>
        /// 对象编号
        /// </summary>
        [Database.Column("对象编号", "设备编号、设备分类编号，与对象无关的日志此字段存 0。", Database.DataTypeOptions.Int, false, true)]
        public const string ObjId = "ObjId";

        /// <summary>
        /// 配置信息
        /// </summary>
        [Database.Column("配置信息", "", Database.DataTypeOptions.Text, false, false)]
        public const string XMLConfig = "XMLConfig";
    }
}
