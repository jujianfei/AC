using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AMS.Drives;

namespace AMS.Monitoring.General
{
    /// <summary>
    /// 设备日温度曲线数据功能实现。
    /// </summary>
    [DeviceImplement("温度曲线数据")]
    public interface ITemperatureCurveDataDeviceImplement : IDeviceImplement
    {
        /// <summary>
        /// 获取设备日温度曲线数据默认的数据点数。
        /// </summary>
        CurvePointOptions TemperatureCurveDataPoint { get; }

        /// <summary>
        /// 获取设备日温度曲线数据。
        /// </summary>
        TemperatureCurveData TemperatureCurveData { get; }
    }
}
