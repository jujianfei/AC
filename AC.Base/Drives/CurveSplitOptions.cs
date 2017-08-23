using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AC.Base.Drives
{
    /// <summary>
    /// 将曲线上一个点的数据拆分为多个点的计算方法。
    /// </summary>
    public enum CurveSplitOptions
    {
        /// <summary>
        /// 将一个点的数值除以需要拆分的点数作为拆分后的每个点的数值。可用于电量数据。
        /// </summary>
        Division,

        /// <summary>
        /// 不做任何运算，将该点直接作为拆分后每个点的数值。可用于示值、电压、电流、功率、功率因数数据。
        /// </summary>
        Copy,
    }
}
