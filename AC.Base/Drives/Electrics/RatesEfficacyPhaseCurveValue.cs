using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AC.Base.Drives.Electrics
{
    /// <summary>
    /// 费率、功效类型及相位组合的曲线值。
    /// </summary>
    public class RatesEfficacyPhaseCurveValue : EfficacyPhaseCurveValue
    {
        /// <summary>
        /// 费率、功效类型及相位组合的曲线值。
        /// </summary>
        /// <param name="rates">该数据的费率。</param>
        /// <param name="efficacy">该数据的功效类型。</param>
        /// <param name="phase">曲线数据的相位。</param>
        /// <param name="curvePoint">该曲线的点数。</param>
        public RatesEfficacyPhaseCurveValue(RatesOptions rates, EfficacyOptions efficacy, PhaseOptions phase, CurvePointOptions curvePoint)
            : base(efficacy, phase, curvePoint)
        {
            this.Rates = rates;
        }

        /// <summary>
        /// 费率、功效类型及相位组合的曲线值。
        /// </summary>
        /// <param name="rates">该数据的费率。</param>
        /// <param name="efficacy">该数据的功效类型。</param>
        /// <param name="phase">曲线数据的相位。</param>
        /// <param name="values">数据数组。数据数组的长度必须符合 CurvePointOptions 中定义的长度。</param>
        public RatesEfficacyPhaseCurveValue(RatesOptions rates, EfficacyOptions efficacy, PhaseOptions phase, decimal?[] values)
            : base(efficacy, phase, values)
        {
            this.Rates = rates;
        }

        /// <summary>
        /// 该数据的费率。
        /// </summary>
        public RatesOptions Rates { get; private set; }
    }
}
