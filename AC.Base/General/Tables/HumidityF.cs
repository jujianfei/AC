using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AMS.Monitoring.General.Tables
{
    /// <summary>
    /// 湿度15分钟曲线数据。湿度数值：1位整数5位小数，数据在0-1之间；单位：%。
    /// </summary>
    [Database.Table("湿度15分钟曲线数据", Database.TableCreateRuleOptions.Monthly, "")]
    public struct HumidityF
    {
        /// <summary>
        /// 数据表名
        /// </summary>
        public const string TableName = "HumidityF";

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
        /// 设备编号
        /// </summary>
        [Database.Column("设备编号", "", Database.DataTypeOptions.Int, true, true)]
        public const string DeviceId = "DeviceId";

        /// <summary>
        /// 00:00 的数值
        /// </summary>
        [Database.Column("00:00 的数值", "", 6, 5, false, false)]
        public const string Value1 = "Value1";

        /// <summary>
        /// 00:15 的数值
        /// </summary>
        [Database.Column("00:15 的数值", "", 6, 5, false, false)]
        public const string Value2 = "Value2";

        /// <summary>
        /// 00:30 的数值
        /// </summary>
        [Database.Column("00:30 的数值", "", 6, 5, false, false)]
        public const string Value3 = "Value3";

        /// <summary>
        /// 00:45 的数值
        /// </summary>
        [Database.Column("00:45 的数值", "", 6, 5, false, false)]
        public const string Value4 = "Value4";

        /// <summary>
        /// 01:00 的数值
        /// </summary>
        [Database.Column("01:00 的数值", "", 6, 5, false, false)]
        public const string Value5 = "Value5";

        /// <summary>
        /// 01:15 的数值
        /// </summary>
        [Database.Column("01:15 的数值", "", 6, 5, false, false)]
        public const string Value6 = "Value6";

        /// <summary>
        /// 01:30 的数值
        /// </summary>
        [Database.Column("01:30 的数值", "", 6, 5, false, false)]
        public const string Value7 = "Value7";

        /// <summary>
        /// 01:45 的数值
        /// </summary>
        [Database.Column("01:45 的数值", "", 6, 5, false, false)]
        public const string Value8 = "Value8";

        /// <summary>
        /// 02:00 的数值
        /// </summary>
        [Database.Column("02:00 的数值", "", 6, 5, false, false)]
        public const string Value9 = "Value9";

        /// <summary>
        /// 02:15 的数值
        /// </summary>
        [Database.Column("02:15 的数值", "", 6, 5, false, false)]
        public const string Value10 = "Value10";

        /// <summary>
        /// 02:30 的数值
        /// </summary>
        [Database.Column("02:30 的数值", "", 6, 5, false, false)]
        public const string Value11 = "Value11";

        /// <summary>
        /// 02:45 的数值
        /// </summary>
        [Database.Column("02:45 的数值", "", 6, 5, false, false)]
        public const string Value12 = "Value12";

        /// <summary>
        /// 03:00 的数值
        /// </summary>
        [Database.Column("03:00 的数值", "", 6, 5, false, false)]
        public const string Value13 = "Value13";

        /// <summary>
        /// 03:15 的数值
        /// </summary>
        [Database.Column("03:15 的数值", "", 6, 5, false, false)]
        public const string Value14 = "Value14";

        /// <summary>
        /// 03:30 的数值
        /// </summary>
        [Database.Column("03:30 的数值", "", 6, 5, false, false)]
        public const string Value15 = "Value15";

        /// <summary>
        /// 03:45 的数值
        /// </summary>
        [Database.Column("03:45 的数值", "", 6, 5, false, false)]
        public const string Value16 = "Value16";

        /// <summary>
        /// 04:00 的数值
        /// </summary>
        [Database.Column("04:00 的数值", "", 6, 5, false, false)]
        public const string Value17 = "Value17";

        /// <summary>
        /// 04:15 的数值
        /// </summary>
        [Database.Column("04:15 的数值", "", 6, 5, false, false)]
        public const string Value18 = "Value18";

        /// <summary>
        /// 04:30 的数值
        /// </summary>
        [Database.Column("04:30 的数值", "", 6, 5, false, false)]
        public const string Value19 = "Value19";

        /// <summary>
        /// 04:45 的数值
        /// </summary>
        [Database.Column("04:45 的数值", "", 6, 5, false, false)]
        public const string Value20 = "Value20";

        /// <summary>
        /// 05:00 的数值
        /// </summary>
        [Database.Column("05:00 的数值", "", 6, 5, false, false)]
        public const string Value21 = "Value21";

        /// <summary>
        /// 05:15 的数值
        /// </summary>
        [Database.Column("05:15 的数值", "", 6, 5, false, false)]
        public const string Value22 = "Value22";

        /// <summary>
        /// 05:30 的数值
        /// </summary>
        [Database.Column("05:30 的数值", "", 6, 5, false, false)]
        public const string Value23 = "Value23";

        /// <summary>
        /// 05:45 的数值
        /// </summary>
        [Database.Column("05:45 的数值", "", 6, 5, false, false)]
        public const string Value24 = "Value24";

        /// <summary>
        /// 06:00 的数值
        /// </summary>
        [Database.Column("06:00 的数值", "", 6, 5, false, false)]
        public const string Value25 = "Value25";

        /// <summary>
        /// 06:15 的数值
        /// </summary>
        [Database.Column("06:15 的数值", "", 6, 5, false, false)]
        public const string Value26 = "Value26";

        /// <summary>
        /// 06:30 的数值
        /// </summary>
        [Database.Column("06:30 的数值", "", 6, 5, false, false)]
        public const string Value27 = "Value27";

        /// <summary>
        /// 06:45 的数值
        /// </summary>
        [Database.Column("06:45 的数值", "", 6, 5, false, false)]
        public const string Value28 = "Value28";

        /// <summary>
        /// 07:00 的数值
        /// </summary>
        [Database.Column("07:00 的数值", "", 6, 5, false, false)]
        public const string Value29 = "Value29";

        /// <summary>
        /// 07:15 的数值
        /// </summary>
        [Database.Column("07:15 的数值", "", 6, 5, false, false)]
        public const string Value30 = "Value30";

        /// <summary>
        /// 07:30 的数值
        /// </summary>
        [Database.Column("07:30 的数值", "", 6, 5, false, false)]
        public const string Value31 = "Value31";

        /// <summary>
        /// 07:45 的数值
        /// </summary>
        [Database.Column("07:45 的数值", "", 6, 5, false, false)]
        public const string Value32 = "Value32";

        /// <summary>
        /// 08:00 的数值
        /// </summary>
        [Database.Column("08:00 的数值", "", 6, 5, false, false)]
        public const string Value33 = "Value33";

        /// <summary>
        /// 08:15 的数值
        /// </summary>
        [Database.Column("08:15 的数值", "", 6, 5, false, false)]
        public const string Value34 = "Value34";

        /// <summary>
        /// 08:30 的数值
        /// </summary>
        [Database.Column("08:30 的数值", "", 6, 5, false, false)]
        public const string Value35 = "Value35";

        /// <summary>
        /// 08:45 的数值
        /// </summary>
        [Database.Column("08:45 的数值", "", 6, 5, false, false)]
        public const string Value36 = "Value36";

        /// <summary>
        /// 09:00 的数值
        /// </summary>
        [Database.Column("09:00 的数值", "", 6, 5, false, false)]
        public const string Value37 = "Value37";

        /// <summary>
        /// 09:15 的数值
        /// </summary>
        [Database.Column("09:15 的数值", "", 6, 5, false, false)]
        public const string Value38 = "Value38";

        /// <summary>
        /// 09:30 的数值
        /// </summary>
        [Database.Column("09:30 的数值", "", 6, 5, false, false)]
        public const string Value39 = "Value39";

        /// <summary>
        /// 09:45 的数值
        /// </summary>
        [Database.Column("09:45 的数值", "", 6, 5, false, false)]
        public const string Value40 = "Value40";

        /// <summary>
        /// 10:00 的数值
        /// </summary>
        [Database.Column("10:00 的数值", "", 6, 5, false, false)]
        public const string Value41 = "Value41";

        /// <summary>
        /// 10:15 的数值
        /// </summary>
        [Database.Column("10:15 的数值", "", 6, 5, false, false)]
        public const string Value42 = "Value42";

        /// <summary>
        /// 10:30 的数值
        /// </summary>
        [Database.Column("10:30 的数值", "", 6, 5, false, false)]
        public const string Value43 = "Value43";

        /// <summary>
        /// 10:45 的数值
        /// </summary>
        [Database.Column("10:45 的数值", "", 6, 5, false, false)]
        public const string Value44 = "Value44";

        /// <summary>
        /// 11:00 的数值
        /// </summary>
        [Database.Column("11:00 的数值", "", 6, 5, false, false)]
        public const string Value45 = "Value45";

        /// <summary>
        /// 11:15 的数值
        /// </summary>
        [Database.Column("11:15 的数值", "", 6, 5, false, false)]
        public const string Value46 = "Value46";

        /// <summary>
        /// 11:30 的数值
        /// </summary>
        [Database.Column("11:30 的数值", "", 6, 5, false, false)]
        public const string Value47 = "Value47";

        /// <summary>
        /// 11:45 的数值
        /// </summary>
        [Database.Column("11:45 的数值", "", 6, 5, false, false)]
        public const string Value48 = "Value48";

        /// <summary>
        /// 12:00 的数值
        /// </summary>
        [Database.Column("12:00 的数值", "", 6, 5, false, false)]
        public const string Value49 = "Value49";

        /// <summary>
        /// 12:15 的数值
        /// </summary>
        [Database.Column("12:15 的数值", "", 6, 5, false, false)]
        public const string Value50 = "Value50";

        /// <summary>
        /// 12:30 的数值
        /// </summary>
        [Database.Column("12:30 的数值", "", 6, 5, false, false)]
        public const string Value51 = "Value51";

        /// <summary>
        /// 12:45 的数值
        /// </summary>
        [Database.Column("12:45 的数值", "", 6, 5, false, false)]
        public const string Value52 = "Value52";

        /// <summary>
        /// 13:00 的数值
        /// </summary>
        [Database.Column("13:00 的数值", "", 6, 5, false, false)]
        public const string Value53 = "Value53";

        /// <summary>
        /// 13:15 的数值
        /// </summary>
        [Database.Column("13:15 的数值", "", 6, 5, false, false)]
        public const string Value54 = "Value54";

        /// <summary>
        /// 13:30 的数值
        /// </summary>
        [Database.Column("13:30 的数值", "", 6, 5, false, false)]
        public const string Value55 = "Value55";

        /// <summary>
        /// 13:45 的数值
        /// </summary>
        [Database.Column("13:45 的数值", "", 6, 5, false, false)]
        public const string Value56 = "Value56";

        /// <summary>
        /// 14:00 的数值
        /// </summary>
        [Database.Column("14:00 的数值", "", 6, 5, false, false)]
        public const string Value57 = "Value57";

        /// <summary>
        /// 14:15 的数值
        /// </summary>
        [Database.Column("14:15 的数值", "", 6, 5, false, false)]
        public const string Value58 = "Value58";

        /// <summary>
        /// 14:30 的数值
        /// </summary>
        [Database.Column("14:30 的数值", "", 6, 5, false, false)]
        public const string Value59 = "Value59";

        /// <summary>
        /// 14:45 的数值
        /// </summary>
        [Database.Column("14:45 的数值", "", 6, 5, false, false)]
        public const string Value60 = "Value60";

        /// <summary>
        /// 15:00 的数值
        /// </summary>
        [Database.Column("15:00 的数值", "", 6, 5, false, false)]
        public const string Value61 = "Value61";

        /// <summary>
        /// 15:15 的数值
        /// </summary>
        [Database.Column("15:15 的数值", "", 6, 5, false, false)]
        public const string Value62 = "Value62";

        /// <summary>
        /// 15:30 的数值
        /// </summary>
        [Database.Column("15:30 的数值", "", 6, 5, false, false)]
        public const string Value63 = "Value63";

        /// <summary>
        /// 15:45 的数值
        /// </summary>
        [Database.Column("15:45 的数值", "", 6, 5, false, false)]
        public const string Value64 = "Value64";

        /// <summary>
        /// 16:00 的数值
        /// </summary>
        [Database.Column("16:00 的数值", "", 6, 5, false, false)]
        public const string Value65 = "Value65";

        /// <summary>
        /// 16:15 的数值
        /// </summary>
        [Database.Column("16:15 的数值", "", 6, 5, false, false)]
        public const string Value66 = "Value66";

        /// <summary>
        /// 16:30 的数值
        /// </summary>
        [Database.Column("16:30 的数值", "", 6, 5, false, false)]
        public const string Value67 = "Value67";

        /// <summary>
        /// 16:45 的数值
        /// </summary>
        [Database.Column("16:45 的数值", "", 6, 5, false, false)]
        public const string Value68 = "Value68";

        /// <summary>
        /// 17:00 的数值
        /// </summary>
        [Database.Column("17:00 的数值", "", 6, 5, false, false)]
        public const string Value69 = "Value69";

        /// <summary>
        /// 17:15 的数值
        /// </summary>
        [Database.Column("17:15 的数值", "", 6, 5, false, false)]
        public const string Value70 = "Value70";

        /// <summary>
        /// 17:30 的数值
        /// </summary>
        [Database.Column("17:30 的数值", "", 6, 5, false, false)]
        public const string Value71 = "Value71";

        /// <summary>
        /// 17:45 的数值
        /// </summary>
        [Database.Column("17:45 的数值", "", 6, 5, false, false)]
        public const string Value72 = "Value72";

        /// <summary>
        /// 18:00 的数值
        /// </summary>
        [Database.Column("18:00 的数值", "", 6, 5, false, false)]
        public const string Value73 = "Value73";

        /// <summary>
        /// 18:15 的数值
        /// </summary>
        [Database.Column("18:15 的数值", "", 6, 5, false, false)]
        public const string Value74 = "Value74";

        /// <summary>
        /// 18:30 的数值
        /// </summary>
        [Database.Column("18:30 的数值", "", 6, 5, false, false)]
        public const string Value75 = "Value75";

        /// <summary>
        /// 18:45 的数值
        /// </summary>
        [Database.Column("18:45 的数值", "", 6, 5, false, false)]
        public const string Value76 = "Value76";

        /// <summary>
        /// 19:00 的数值
        /// </summary>
        [Database.Column("19:00 的数值", "", 6, 5, false, false)]
        public const string Value77 = "Value77";

        /// <summary>
        /// 19:15 的数值
        /// </summary>
        [Database.Column("19:15 的数值", "", 6, 5, false, false)]
        public const string Value78 = "Value78";

        /// <summary>
        /// 19:30 的数值
        /// </summary>
        [Database.Column("19:30 的数值", "", 6, 5, false, false)]
        public const string Value79 = "Value79";

        /// <summary>
        /// 19:45 的数值
        /// </summary>
        [Database.Column("19:45 的数值", "", 6, 5, false, false)]
        public const string Value80 = "Value80";

        /// <summary>
        /// 20:00 的数值
        /// </summary>
        [Database.Column("20:00 的数值", "", 6, 5, false, false)]
        public const string Value81 = "Value81";

        /// <summary>
        /// 20:15 的数值
        /// </summary>
        [Database.Column("20:15 的数值", "", 6, 5, false, false)]
        public const string Value82 = "Value82";

        /// <summary>
        /// 20:30 的数值
        /// </summary>
        [Database.Column("20:30 的数值", "", 6, 5, false, false)]
        public const string Value83 = "Value83";

        /// <summary>
        /// 20:45 的数值
        /// </summary>
        [Database.Column("20:45 的数值", "", 6, 5, false, false)]
        public const string Value84 = "Value84";

        /// <summary>
        /// 21:00 的数值
        /// </summary>
        [Database.Column("21:00 的数值", "", 6, 5, false, false)]
        public const string Value85 = "Value85";

        /// <summary>
        /// 21:15 的数值
        /// </summary>
        [Database.Column("21:15 的数值", "", 6, 5, false, false)]
        public const string Value86 = "Value86";

        /// <summary>
        /// 21:30 的数值
        /// </summary>
        [Database.Column("21:30 的数值", "", 6, 5, false, false)]
        public const string Value87 = "Value87";

        /// <summary>
        /// 21:45 的数值
        /// </summary>
        [Database.Column("21:45 的数值", "", 6, 5, false, false)]
        public const string Value88 = "Value88";

        /// <summary>
        /// 22:00 的数值
        /// </summary>
        [Database.Column("22:00 的数值", "", 6, 5, false, false)]
        public const string Value89 = "Value89";

        /// <summary>
        /// 22:15 的数值
        /// </summary>
        [Database.Column("22:15 的数值", "", 6, 5, false, false)]
        public const string Value90 = "Value90";

        /// <summary>
        /// 22:30 的数值
        /// </summary>
        [Database.Column("22:30 的数值", "", 6, 5, false, false)]
        public const string Value91 = "Value91";

        /// <summary>
        /// 22:45 的数值
        /// </summary>
        [Database.Column("22:45 的数值", "", 6, 5, false, false)]
        public const string Value92 = "Value92";

        /// <summary>
        /// 23:00 的数值
        /// </summary>
        [Database.Column("23:00 的数值", "", 6, 5, false, false)]
        public const string Value93 = "Value93";

        /// <summary>
        /// 23:15 的数值
        /// </summary>
        [Database.Column("23:15 的数值", "", 6, 5, false, false)]
        public const string Value94 = "Value94";

        /// <summary>
        /// 23:30 的数值
        /// </summary>
        [Database.Column("23:30 的数值", "", 6, 5, false, false)]
        public const string Value95 = "Value95";

        /// <summary>
        /// 23:45 的数值
        /// </summary>
        [Database.Column("23:45 的数值", "", 6, 5, false, false)]
        public const string Value96 = "Value96";
    }
}
