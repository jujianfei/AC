using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AC.Base
{
    /// <summary>
    /// 分组合计的数据分析数值。
    /// </summary>
    public class DataAnalysisGroupValue
    {
        /// <summary>
        /// 各设备最大值中出现的最大值。
        /// </summary>
        public decimal? MaxValue_Max { get; set; }

        /// <summary>
        /// 各设备最大值的平均值。
        /// </summary>
        public decimal? MaxValue_Avg { get; set; }

        /// <summary>
        /// 各设备最小值中出现的最小值。
        /// </summary>
        public decimal? MinValue_Min { get; set; }

        /// <summary>
        /// 各设备最小值的平均值。
        /// </summary>
        public decimal? MinValue_Avg { get; set; }

        /// <summary>
        /// 各设备平均值的平均值。
        /// </summary>
        public decimal? AvgValue_Avg { get; set; }

        /// <summary>
        /// 各设备合计超上上限累计时间(秒)
        /// </summary>
        public long? UpperUpperTimes { get; set; }

        /// <summary>
        /// 各设备合计超上上限次数(次)
        /// </summary>
        public long? UpperUpperCount { get; set; }

        /// <summary>
        /// 各设备合计超上限累计时间(秒)
        /// </summary>
        public long? UpperTimes { get; set; }

        /// <summary>
        /// 各设备合计超上限次数(次)
        /// </summary>
        public long? UpperCount { get; set; }

        /// <summary>
        /// 各设备合计超下限累计时间(秒)
        /// </summary>
        public long? LowerTimes { get; set; }

        /// <summary>
        /// 各设备合计超下限次数(次)
        /// </summary>
        public long? LowerCount { get; set; }

        /// <summary>
        /// 各设备合计超下下限累计时间(秒)
        /// </summary>
        public long? LowerLowerTimes { get; set; }

        /// <summary>
        /// 各设备合计超下下限次数(次)
        /// </summary>
        public long? LowerLowerCount { get; set; }
    }
}
