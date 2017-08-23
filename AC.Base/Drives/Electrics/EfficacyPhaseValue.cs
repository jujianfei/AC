using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AC.Base.Drives.Electrics
{
    /// <summary>
    /// 功效类型及相位组合的值。
    /// </summary>
    public class EfficacyPhaseValue : PhaseTotalValue
    {
        /// <summary>
        /// 功效类型及相位组合的值。
        /// </summary>
        /// <param name="efficacy">该数据的功效类型。</param>
        public EfficacyPhaseValue(EfficacyOptions efficacy)
        {
            this.Efficacy = efficacy;
        }

        /// <summary>
        /// 功效类型及相位组合的值。
        /// </summary>
        /// <param name="efficacy">该数据的功效类型。</param>
        /// <param name="total">总相的数据。</param>
        /// <param name="a">A相的数据。</param>
        /// <param name="b">B相的数据。</param>
        /// <param name="c">C相的数据。</param>
        public EfficacyPhaseValue(EfficacyOptions efficacy, decimal? total, decimal? a, decimal? b, decimal? c)
            : base(total, a, b, c)
        {
            this.Efficacy = efficacy;
        }

        /// <summary>
        /// 该数据的功效类型。
        /// </summary>
        public EfficacyOptions Efficacy { get; private set; }
    }
}
