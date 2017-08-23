using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AC.Base.Drives
{
    /// <summary>
    /// 将曲线上多个点的数据合并为一个点的计算方法。
    /// </summary>
    public enum CurveMergeOptions
    {
        /// <summary>
        /// 将多个点的数值相加后合并为一个数值。可用于电量数据。
        /// </summary>
        Addition,

        /// <summary>
        /// 将多个点的数值平均后合并为一个数值。可用于电压、电流、功率、功率因数数据。
        /// </summary>
        Average,

        /// <summary>
        /// 使用多个点中的第一个有效数值。可用于示值。
        /// </summary>
        First,

        /// <summary>
        /// 使用多个数值中最大的一个作为合并后的数值。
        /// </summary>
        Maximum,

        /// <summary>
        /// 使用多个数值中最小的一个作为合并后的数值。
        /// </summary>
        Minimum,
    }
}
