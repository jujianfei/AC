using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AC.Base.Drives.Electrics
{
    /// <summary>
    /// A相、B相、C相、零相的值。
    /// </summary>
    public class PhaseZeroValue : PhaseValue
    {
        /// <summary>
        /// A相、B相、C相、零相的值。
        /// </summary>
        public PhaseZeroValue()
        {
        }

        /// <summary>
        /// A相、B相、C相、零相的值。
        /// </summary>
        /// <param name="a">A相的数据。</param>
        /// <param name="b">B相的数据。</param>
        /// <param name="c">C相的数据。</param>
        /// <param name="zero">零相的数据。</param>
        public PhaseZeroValue(decimal? a, decimal? b, decimal? c, decimal? zero)
            : base(a, b, c)
        {
            this.Zero = zero;
        }

        /// <summary>
        /// 获取或设置零相位的数值。
        /// </summary>
        public decimal? Zero { get; set; }

        /// <summary>
        /// 获取或设置指定相位的数值（不包含总相）。
        /// </summary>
        /// <param name="phase">相位。</param>
        /// <returns></returns>
        public override decimal? this[PhaseOptions phase]
        {
            get
            {
                if (phase == PhaseOptions.Zero)
                {
                    return this.Zero;
                }
                else
                {
                    return base[phase];
                }
            }
            set
            {
                if (phase == PhaseOptions.Zero)
                {
                    this.Zero = value;
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
            return base.ToString() + " 零:" + this.Zero;
        }

        /// <summary>
        /// 将A相、B相、C相、零相的值相加。
        /// </summary>
        /// <param name="value1"></param>
        /// <param name="value2"></param>
        /// <returns></returns>
        public static PhaseZeroValue operator +(PhaseZeroValue value1, PhaseZeroValue value2)
        {
            PhaseZeroValue value = new PhaseZeroValue();
            value.A = Function.DoAdd(value1.A, value2.A);
            value.B = Function.DoAdd(value1.B, value2.B);
            value.C = Function.DoAdd(value1.C, value2.C);
            value.Zero = Function.DoAdd(value1.Zero, value2.Zero);
            return value;
        }
    }
}
