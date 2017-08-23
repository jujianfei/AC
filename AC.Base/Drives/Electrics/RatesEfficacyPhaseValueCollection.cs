using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AC.Base.Drives.Electrics
{
    /// <summary>
    /// 费率、功效类型及相位组合值集合。可使用 GetValue(EfficacyOptions, PhaseOptions) 获取指定相位及功效类型的值。
    /// </summary>
    public class RatesEfficacyPhaseValueCollection : List<RatesEfficacyPhaseValue>
    {
        /// <summary>
        /// 获取指定功效类型的值，并且当指定的功效类型数据不存在时返回一个无数值的 RatesEfficacyPhaseValue 对象。
        /// </summary>
        /// <param name="efficacy">功效类型。</param>
        /// <param name="phase">相位。</param>
        /// <returns>该方法始终返回 RatesEfficacyPhaseValue 实例，不会返回 null。</returns>
        public RatesEfficacyPhaseValue GetValue(EfficacyOptions efficacy, PhaseOptions phase)
        {
            foreach (RatesEfficacyPhaseValue value in this)
            {
                if (value.Efficacy == efficacy && value.Phase == phase)
                {
                    return value;
                }
            }

            return new RatesEfficacyPhaseValue(efficacy, phase);
        }
    }
}
