using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AC.Base.Drives.Electrics
{
    /// <summary>
    /// 各相位的曲线段值。
    /// </summary>
    public class PhaseCurvePartValue : CurvePartValue
    {
        /// <summary>
        /// 各相位的曲线段值。
        /// </summary>
        /// <param name="phase">曲线数据的相位。</param>
        /// <param name="curvePoint">曲线点数，该参数为 0:00:00 至 23:59:59 完整的曲线数据点数。</param>
        /// <param name="startTimeNum">第一个点的数值所对应的时间（hhmmss）。</param>
        /// <param name="endTimeNum">最后一个点的数值所对应的时间（hhmmss）。</param>
        public PhaseCurvePartValue(PhaseOptions phase, CurvePointOptions curvePoint, int startTimeNum, int endTimeNum)
            : base(curvePoint, startTimeNum, endTimeNum)
        {
            this.Phase = phase;
        }

        /// <summary>
        /// 各相位的曲线段值。
        /// </summary>
        /// <param name="phase">曲线数据的相位。</param>
        /// <param name="curvePoint">曲线点数，该参数为 0:00:00 至 23:59:59 完整的曲线数据点数。</param>
        /// <param name="startTimeNum">第一个点的数值所对应的时间（hhmmss）。</param>
        /// <param name="values">曲线段各点数据。</param>
        public PhaseCurvePartValue(PhaseOptions phase, CurvePointOptions curvePoint, int startTimeNum, decimal?[] values)
            : base(curvePoint, startTimeNum, values)
        {
            this.Phase = phase;
        }

        /// <summary>
        /// 各相位的曲线段值。
        /// </summary>
        /// <param name="phase">曲线数据的相位。</param>
        /// <param name="startTimeNum">第一个点的数值所对应的时间（hhmmss）。</param>
        /// <param name="endTimeNum">最后一个点的数值所对应的时间（hhmmss）。</param>
        /// <param name="values">曲线段各点数据。</param>
        public PhaseCurvePartValue(PhaseOptions phase, int startTimeNum, int endTimeNum, decimal?[] values)
            : base(startTimeNum, endTimeNum, values)
        {
            this.Phase = phase;
        }

        /// <summary>
        /// 获取曲线数据的相位。
        /// </summary>
        public PhaseOptions Phase { get; private set; }
    }
}
