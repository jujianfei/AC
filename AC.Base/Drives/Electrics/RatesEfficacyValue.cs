using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AC.Base.Drives.Electrics
{
    /// <summary>
    /// 费率及功效类型组合的值。
    /// </summary>
    public class RatesEfficacyValue : RatesValue
    {
        /// <summary>
        /// 费率及功效类型组合的值。
        /// </summary>
        /// <param name="efficacy">该数据的功效类型。</param>
        public RatesEfficacyValue(EfficacyOptions efficacy)
        {
            this.Efficacy = efficacy;
        }

        /// <summary>
        /// 费率及功效类型组合的值。
        /// </summary>
        /// <param name="efficacy">该数据的功效类型。</param>
        /// <param name="total">总费率的数据。</param>
        /// <param name="pike">尖费率的数据。</param>
        /// <param name="peak">峰费率的数据。</param>
        /// <param name="flat">平费率的数据。</param>
        /// <param name="vale">谷费率的数据。</param>
        public RatesEfficacyValue(EfficacyOptions efficacy, decimal? total, decimal? pike, decimal? peak, decimal? flat, decimal? vale)
            : base(total, pike, peak, flat, vale)
        {
            this.Efficacy = efficacy;
        }

        /// <summary>
        /// 该数据的功效类型。
        /// </summary>
        public EfficacyOptions Efficacy { get; private set; }
    }
}
