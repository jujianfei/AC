using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AC.Base.Drives.Electrics
{
    /// <summary>
    /// 功效类型及相位组合值集合。可使用 GetValue(EfficacyOptions) 获取指定功效类型的值。
    /// </summary>
    public class EfficacyPhaseValueCollection : List<EfficacyPhaseValue>
    {
        /// <summary>
        /// 获取指定功效类型的值，并且当指定的功效类型数据不存在时返回一个无数值的 EfficacyPhaseValue 对象。该方法始终返回 EfficacyPhaseValue 实例，不会返回 null。
        /// </summary>
        /// <param name="efficacy">功效类型。</param>
        /// <returns></returns>
        public EfficacyPhaseValue GetValue(EfficacyOptions efficacy)
        {
            foreach (EfficacyPhaseValue value in this)
            {
                if (value.Efficacy == efficacy)
                {
                    return value;
                }
            }

            return new EfficacyPhaseValue(efficacy);
        }
    }
}
