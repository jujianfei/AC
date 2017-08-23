using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AC.Base.Drives.Electrics
{
    /// <summary>
    /// 功效类型及相位组合的曲线段值。
    /// </summary>
    public class EfficacyPhaseCurvePartValue: PhaseCurvePartValue
    {
        /// <summary>
        /// 功效类型及相位组合的曲线段值。
        /// </summary>
        /// <param name="efficacy">该数据的功效类型。</param>
        /// <param name="phase">曲线数据的相位。</param>
        /// <param name="curvePoint">曲线点数，该参数为 0:00:00 至 23:59:59 完整的曲线数据点数。</param>
        /// <param name="startTimeNum">第一个点的数值所对应的时间（hhmmss）。</param>
        /// <param name="endTimeNum">最后一个点的数值所对应的时间（hhmmss）。</param>
        public EfficacyPhaseCurvePartValue(EfficacyOptions efficacy, PhaseOptions phase, CurvePointOptions curvePoint, int startTimeNum, int endTimeNum)
            : base(phase, curvePoint, startTimeNum, endTimeNum)
        {
            this.Efficacy = efficacy;
        }

        /// <summary>
        /// 功效类型及相位组合的曲线段值。
        /// </summary>
        /// <param name="efficacy">该数据的功效类型。</param>
        /// <param name="phase">曲线数据的相位。</param>
        /// <param name="curvePoint">曲线点数，该参数为 0:00:00 至 23:59:59 完整的曲线数据点数。</param>
        /// <param name="startTimeNum">第一个点的数值所对应的时间（hhmmss）。</param>
        /// <param name="values">曲线段各点数据。</param>
        public EfficacyPhaseCurvePartValue(EfficacyOptions efficacy, PhaseOptions phase, CurvePointOptions curvePoint, int startTimeNum, decimal?[] values)
            : base(phase, curvePoint, startTimeNum, values)
        {
            this.Efficacy = efficacy;
        }

        /// <summary>
        /// 功效类型及相位组合的曲线段值。
        /// </summary>
        /// <param name="efficacy">该数据的功效类型。</param>
        /// <param name="phase">曲线数据的相位。</param>
        /// <param name="startTimeNum">第一个点的数值所对应的时间（hhmmss）。</param>
        /// <param name="endTimeNum">最后一个点的数值所对应的时间（hhmmss）。</param>
        /// <param name="values">曲线段各点数据。</param>
        public EfficacyPhaseCurvePartValue(EfficacyOptions efficacy, PhaseOptions phase, int startTimeNum, int endTimeNum, decimal?[] values)
            : base(phase, startTimeNum, endTimeNum, values)
        {
            this.Efficacy = efficacy;
        }

        /// <summary>
        /// 该数据的功效类型。
        /// </summary>
        public EfficacyOptions Efficacy { get; private set; }
    }
}
