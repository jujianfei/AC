using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AC.Base
{
    /// <summary>
    /// 数据分析数值。通常用于一天多个数据点的曲线数据分析。
    /// </summary>
    public class DataAnalysisValue
    {
        /// <summary>
        /// 最大值。
        /// </summary>
        public decimal? MaxValue { get; set; }

        /// <summary>
        /// 最大值发生时间。
        /// </summary>
        public DateTime? MaxDateTime { get; set; }

        /// <summary>
        /// 最小值。
        /// </summary>
        public decimal? MinValue { get; set; }

        /// <summary>
        /// 最小值发生时间。
        /// </summary>
        public DateTime? MinDateTime { get; set; }

        /// <summary>
        /// 平均值。
        /// </summary>
        public decimal? AvgValue { get; set; }

        /// <summary>
        /// 大小差值（又叫峰谷差，最大值 - 最小值）
        /// </summary>
        public decimal? MaxMinDiff { get; set; }

        /// <summary>
        /// 差值率（%。又叫峰谷差率，(最大值 - 最小值) / 最大值）
        /// </summary>
        public decimal? MaxMinDiffRatio { get; set; }

        /// <summary>
        /// 平均率（%。又叫负荷率，平均值 / 最大值）
        /// </summary>
        public decimal? AvgRatio { get; set; }

        /// <summary>
        /// 超上上限累计时间(秒)
        /// </summary>
        public int? UpperUpperTimes { get; set; }

        /// <summary>
        /// 超上上限次数(次)
        /// </summary>
        public int? UpperUpperCount { get; set; }

        /// <summary>
        /// 超上限累计时间(秒)
        /// </summary>
        public int? UpperTimes { get; set; }

        /// <summary>
        /// 超上限次数(次)
        /// </summary>
        public int? UpperCount { get; set; }

        /// <summary>
        /// 超下限累计时间(秒)
        /// </summary>
        public int? LowerTimes { get; set; }

        /// <summary>
        /// 超下限次数(次)
        /// </summary>
        public int? LowerCount { get; set; }

        /// <summary>
        /// 超下下限累计时间(秒)
        /// </summary>
        public int? LowerLowerTimes { get; set; }

        /// <summary>
        /// 超下下限次数(次)
        /// </summary>
        public int? LowerLowerCount { get; set; }
    }
}
