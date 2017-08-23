using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AC.Base.Drives.Electrics
{
    /// <summary>
    /// A相、B相、C相的值。
    /// </summary>
    public class PhaseValue
    {
        /// <summary>
        /// A相、B相、C相的值。
        /// </summary>
        public PhaseValue()
        {
        }

        /// <summary>
        /// A相、B相、C相的值。
        /// </summary>
        /// <param name="a">A相的数据。</param>
        /// <param name="b">B相的数据。</param>
        /// <param name="c">C相的数据。</param>
        public PhaseValue(decimal? a, decimal? b, decimal? c)
        {
            this.A = a;
            this.B = b;
            this.C = c;
        }

        /// <summary>
        /// 获取或设置A相的数据。
        /// </summary>
        public decimal? A { get; set; }

        /// <summary>
        /// 获取或设置B相的数据。
        /// </summary>
        public decimal? B { get; set; }

        /// <summary>
        /// 获取或设置C相的数据。
        /// </summary>
        public decimal? C { get; set; }

        /// <summary>
        /// 获取或设置指定相位的数值（仅包含A、B、C三相）。
        /// </summary>
        /// <param name="phase">相位。</param>
        /// <returns></returns>
        public virtual decimal? this[PhaseOptions phase]
        {
            get
            {
                switch (phase)
                {
                    case PhaseOptions.A:
                        return this.A;

                    case PhaseOptions.B:
                        return this.B;

                    case PhaseOptions.C:
                        return this.C;

                    default:
                        return null;
                }
            }
            set
            {
                switch (phase)
                {
                    case PhaseOptions.A:
                        this.A = value;
                        break;

                    case PhaseOptions.B:
                        this.B = value;
                        break;

                    case PhaseOptions.C:
                        this.C = value;
                        break;
                }
            }
        }

        /// <summary>
        /// 获取A相、B相、C相的合计数值。
        /// </summary>
        /// <returns>如果A相、B相、C相均为 null 则返回 null。</returns>
        public decimal? GetSum()
        {
            decimal? value = null;

            if (this.A != null)
            {
                if (value == null)
                {
                    value = this.A;
                }
                else
                {
                    value += this.A;
                }
            }

            if (this.B != null)
            {
                if (value == null)
                {
                    value = this.B;
                }
                else
                {
                    value += this.B;
                }
            }

            if (this.C != null)
            {
                if (value == null)
                {
                    value = this.C;
                }
                else
                {
                    value += this.C;
                }
            }

            return value;
        }

        /// <summary>
        /// 获取当前对象的字符串表示形式。
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return "A:" + this.A + " B:" + this.B + " C:" + this.C;
        }

        /// <summary>
        /// 将A相、B相、C相的值相加。
        /// </summary>
        /// <param name="value1"></param>
        /// <param name="value2"></param>
        /// <returns></returns>
        public static PhaseValue operator +(PhaseValue value1, PhaseValue value2)
        {
            PhaseValue value = new PhaseValue();
            value.A = Function.DoAdd(value1.A, value2.A);
            value.B = Function.DoAdd(value1.B, value2.B);
            value.C = Function.DoAdd(value1.C, value2.C);
            return value;
        }
    }
}
