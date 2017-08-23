using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AMS.Drives;

namespace AMS.Monitoring.General
{
    /// <summary>
    /// 设备日湿度曲线数据功能实现。
    /// </summary>
    [DeviceImplement("湿度曲线数据")]
    public interface IHumidityCurveDataDeviceImplement : IDeviceImplement
    {
        /// <summary>
        /// 获取设备日湿度曲线数据默认的数据点数。
        /// </summary>
        CurvePointOptions HumidityCurveDataPoint { get; }

        /// <summary>
        /// 获取设备日湿度曲线数据。
        /// </summary>
        HumidityCurveData HumidityCurveData { get; }
    }
}
