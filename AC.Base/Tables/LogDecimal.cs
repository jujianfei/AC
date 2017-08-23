using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AC.Base.Tables
{
    /// <summary>
    /// 日志带小数数值属性值。
    /// </summary>
    [Database.Table("日志带小数数值属性值", Database.TableCreateRuleOptions.Monthly, "")]
    public struct LogDecimal
    {
        /// <summary>
        /// 数据表名
        /// </summary>
        public const string TableName = "LogDecimal";

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
        /// 属性类型
        /// </summary>
        [Database.Column("属性类型", "通常使用搜索筛选器的类名", 250, true, true)]
        public const string Code = "Code";

        /// <summary>
        /// 排序编号
        /// </summary>
        [Database.Column("排序编号", "", Database.DataTypeOptions.Int, true, true)]
        public const string OrdinalNumber = "OrdinalNumber";

        /// <summary>
        /// 属性数值
        /// </summary>
        [Database.Column("属性数值", "", 16, 4, false, false)]
        public const string DataValue = "DataValue";
    }
}
