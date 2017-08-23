using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AMS.Monitoring.General.Tables
{
    /// <summary>
    /// 湿度分秒曲线数据。湿度数值：1位整数5位小数，数据在0-1之间；单位：%。
    /// </summary>
    [Database.Table("湿度分秒曲线数据", Database.TableCreateRuleOptions.Monthly, "")]
    public struct HumidityS
    {
        /// <summary>
        /// 数据表名
        /// </summary>
        public const string TableName = "HumidityS";

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
        /// 数据日期
        /// </summary>
        [Database.Column("数据日期", "YYYYMMDD", Database.DataTypeOptions.Int, true, true)]
        public const string DateNum = "DateNum";

        /// <summary>
        /// 数据时间
        /// </summary>
        [Database.Column("数据时间", "hhmmss", Database.DataTypeOptions.Int, true, true)]
        public const string TimeNum = "TimeNum";

        /// <summary>
        /// 设备编号
        /// </summary>
        [Database.Column("设备编号", "", Database.DataTypeOptions.Int, true, true)]
        public const string DeviceId = "DeviceId";

        /// <summary>
        /// 数值
        /// </summary>
        [Database.Column("数值", "", 6, 5, false, false)]
        public const string DataValue = "DataValue";
    }
}
