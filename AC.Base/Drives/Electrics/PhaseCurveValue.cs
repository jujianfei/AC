using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AC.Base.Drives.Electrics
{
    /// <summary>
    /// 各相位的曲线值。
    /// </summary>
    public class PhaseCurveValue : CurveValue
    {
        /// <summary>
        /// 各相位的曲线值。
        /// </summary>
        /// <param name="phase">曲线数据的相位。</param>
        /// <param name="curvePoint">该曲线的点数。</param>
        public PhaseCurveValue(PhaseOptions phase, CurvePointOptions curvePoint)
            : base(curvePoint)
        {
            this.Phase = phase;
        }

        /// <summary>
        /// 各相位的曲线值。
        /// </summary>
        /// <param name="phase">曲线数据的相位。</param>
        /// <param name="values">数据数组。数据数组的长度必须符合 CurvePointOptions 中定义的长度。</param>
        public PhaseCurveValue(PhaseOptions phase, decimal?[] values)
            : base(values)
        {
            this.Phase = phase;
        }

        /// <summary>
        /// 获取曲线数据的相位。
        /// </summary>
        public PhaseOptions Phase { get; private set; }
    }
}
