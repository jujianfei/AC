using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AMS.Drives;

namespace AMS.Monitoring.General
{
    /// <summary>
    /// 日湿度曲线段值。
    /// </summary>
    public class HumidityCurvePartValue : CurvePartValue
    {
        /// <summary>
        /// 日湿度曲线段值。
        /// </summary>
        /// <param name="device">该数据所属的设备。</param>
        /// <param name="date">该数据的日期。</param>
        /// <param name="curvePoint">曲线点数，该参数为 0:00:00 至 23:59:59 完整的曲线数据点数。</param>
        /// <param name="startTimeNum">第一个点的数值所对应的时间（hhmmss）。</param>
        /// <param name="endTimeNum">最后一个点的数值所对应的时间（hhmmss）。</param>
        public HumidityCurvePartValue(Device device, DateTime date, CurvePointOptions curvePoint, int startTimeNum, int endTimeNum)
            : base(curvePoint, startTimeNum, endTimeNum)
        {
            this.m_Device = device;
            this.m_Date = date;
        }

        /// <summary>
        /// 日湿度曲线段值。
        /// </summary>
        /// <param name="device">该数据所属的设备。</param>
        /// <param name="date">该数据的日期。</param>
        /// <param name="curvePoint">曲线点数，该参数为 0:00:00 至 23:59:59 完整的曲线数据点数。</param>
        /// <param name="startTimeNum">第一个点的数值所对应的时间（hhmmss）。</param>
        /// <param name="values">曲线段各点数据。</param>
        public HumidityCurvePartValue(Device device, DateTime date, CurvePointOptions curvePoint, int startTimeNum, decimal?[] values)
            : base(curvePoint, startTimeNum, values)
        {
            this.m_Device = device;
            this.m_Date = date;
        }

        /// <summary>
        /// 日湿度曲线段值。
        /// </summary>
        /// <param name="device">该数据所属的设备。</param>
        /// <param name="date">该数据的日期。</param>
        /// <param name="startTimeNum">第一个点的数值所对应的时间（hhmmss）。</param>
        /// <param name="endTimeNum">最后一个点的数值所对应的时间（hhmmss）。</param>
        /// <param name="values">曲线段各点数据。</param>
        public HumidityCurvePartValue(Device device, DateTime date, int startTimeNum, int endTimeNum, decimal?[] values)
            : base(startTimeNum, endTimeNum, values)
        {
            this.m_Device = device;
            this.m_Date = date;
        }

        private Device m_Device;
        /// <summary>
        /// 该数据所属的设备。
        /// </summary>
        public Device Device
        {
            get { return this.m_Device; }
        }

        private DateTime m_Date;
        /// <summary>
        /// 该数据的日期。
        /// </summary>
        public DateTime Date
        {
            get { return this.m_Date; }
        }

        /// <summary>
        /// 将当前的湿度曲线数据转换为另一种点数的湿度曲线数据。
        /// </summary>
        /// <param name="convertedInto">转换后的曲线点数。</param>
        public void Convert(CurvePointOptions convertedInto)
        {
            base.Convert(convertedInto, CurveMergeOptions.Addition, CurveSplitOptions.Division);
        }
    }
}
