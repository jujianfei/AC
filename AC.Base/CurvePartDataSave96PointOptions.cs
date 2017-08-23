using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AC.Base
{
    /// <summary>
    /// 保存曲线段数据，当数据点数大于96点时是否同时计算并保存一份96点的数据。
    /// </summary>
    public enum CurvePartDataSave96PointOptions
    {
        /// <summary>
        /// 如果数据段的截止时间正好是每15分钟最后一个点的时间则进行计算，否则不计算。例如288点数据，当时间是10分、25分、40分、55分时进行96点数据计算；1440点数据，当时间是14分、29分、44分、59分时进行96点数据计算。
        /// </summary>
        Auto,

        /// <summary>
        /// 始终计算96点的数据。
        /// </summary>
        Yes,

        /// <summary>
        /// 不计算96点的数据。
        /// </summary>
        No,
    }
}
