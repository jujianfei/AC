using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AC.Base.Drives.Electrics
{
    /// <summary>
    /// 总相、A相、B相、C相的值。
    /// </summary>
    public class PhaseTotalValue : PhaseValue
    {
        /// <summary>
        /// 总相、A相、B相、C相的值。
        /// </summary>
        public PhaseTotalValue()
        {
        }

        /// <summary>
        /// 总相、A相、B相、C相的值。
        /// </summary>
        /// <param name="total">总相的数据。</param>
        /// <param name="a">A相的数据。</param>
        /// <param name="b">B相的数据。</param>
        /// <param name="c">C相的数据。</param>
        public PhaseTotalValue(decimal? total, decimal? a, decimal? b, decimal? c)
            : base(a, b, c)
        {
            this.Total = total;
        }

        /// <summary>
        /// 获取或设置总相位的数值。
        /// </summary>
        public decimal? Total { get; set; }

        /// <summary>
        /// 获取或设置指定相位的数值（不包含零相）。
        /// </summary>
        /// <param name="phase">相位。</param>
        /// <returns></returns>
        public override decimal? this[PhaseOptions phase]
        {
            get
            {
                if (phase == PhaseOptions.Total)
                {
                    return this.Total;
                }
                else
                {
                    return base[phase];
                }
            }
            set
            {
                if (phase == PhaseOptions.Total)
                {
                    this.Total = value;
                }
                else
                {
                    base[phase] = value;
                }
            }
        }

        /// <summary>
        /// 获取当前对象的字符串表示形式。
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return "总:" + this.Total + " " + base.ToString();
        }

        /// <summary>
        /// 将总相、A相、B相、C相的值相加。
        /// </summary>
        /// <param name="value1"></param>
        /// <param name="value2"></param>
        /// <returns></returns>
        public static PhaseTotalValue operator +(PhaseTotalValue value1, PhaseTotalValue value2)
        {
            PhaseTotalValue value = new PhaseTotalValue();
            value.Total = Function.DoAdd(value1.Total, value2.Total);
            value.A = Function.DoAdd(value1.A, value2.A);
            value.B = Function.DoAdd(value1.B, value2.B);
            value.C = Function.DoAdd(value1.C, value2.C);
            return value;
        }
    }
}
