using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AC.Base.Drives.Electrics
{
    /// <summary>
    /// 费率、功效类型及相位组合的值。
    /// </summary>
    public class RatesEfficacyPhaseValue : RatesEfficacyValue
    {
        /// <summary>
        /// 费率、功效类型及相位组合的值。
        /// </summary>
        /// <param name="efficacy">该数据的功效类型。</param>
        /// <param name="phase">该数据的相位。</param>
        public RatesEfficacyPhaseValue(EfficacyOptions efficacy, PhaseOptions phase)
            : base(efficacy)
        {
            this.Phase = phase;
        }

        /// <summary>
        /// 费率、功效类型及相位组合的值。
        /// </summary>
        /// <param name="efficacy">该数据的功效类型。</param>
        /// <param name="phase">该数据的相位。</param>
        /// <param name="total">总费率的数据。</param>
        /// <param name="pike">尖费率的数据。</param>
        /// <param name="peak">峰费率的数据。</param>
        /// <param name="flat">平费率的数据。</param>
        /// <param name="vale">谷费率的数据。</param>
        public RatesEfficacyPhaseValue(EfficacyOptions efficacy, PhaseOptions phase, decimal? total, decimal? pike, decimal? peak, decimal? flat, decimal? vale)
            : base(efficacy, total, pike, peak, flat, vale)
        {
            this.Phase = phase;
        }

        /// <summary>
        /// 该数据的相位。
        /// </summary>
        public PhaseOptions Phase { get; private set; }
    }
}
