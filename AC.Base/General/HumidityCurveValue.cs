using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AMS.Drives;

namespace AMS.Monitoring.General
{
    /// <summary>
    /// 日湿度曲线值。
    /// </summary>
    public class HumidityCurveValue : CurveValue
    {
        /// <summary>
        /// 日湿度曲线值。
        /// </summary>
        /// <param name="device">该数据所属的设备。</param>
        /// <param name="date">该数据的日期。</param>
        /// <param name="curvePoint">该曲线的点数。</param>
        public HumidityCurveValue(Device device, DateTime date, CurvePointOptions curvePoint)
            : base(curvePoint)
        {
            this.m_Device = device;
            this.m_Date = date;
        }

        /// <summary>
        /// 日湿度曲线值。
        /// </summary>
        /// <param name="device">该数据所属的设备。</param>
        /// <param name="date">该数据的日期。</param>
        /// <param name="values">数据数组。数据数组的长度必须符合 CurvePointOptions 中定义的长度。</param>
        public HumidityCurveValue(Device device, DateTime date, decimal?[] values)
            : base(values)
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
