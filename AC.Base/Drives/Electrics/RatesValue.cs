using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AC.Base.Drives.Electrics
{
    /// <summary>
    /// 费率数据。
    /// </summary>
    public class RatesValue
    {
        /// <summary>
        /// 费率数据。
        /// </summary>
        public RatesValue()
        {
        }

        /// <summary>
        /// 费率数据。
        /// </summary>
        /// <param name="total">总费率的数据。</param>
        /// <param name="pike">尖费率的数据。</param>
        /// <param name="peak">峰费率的数据。</param>
        /// <param name="flat">平费率的数据。</param>
        /// <param name="vale">谷费率的数据。</param>
        public RatesValue(decimal? total, decimal? pike, decimal? peak, decimal? flat, decimal? vale)
        {
            this.Total = total;
            this.Pike = pike;
            this.Peak = peak;
            this.Flat = flat;
            this.Vale = vale;
        }

        /// <summary>
        /// 总费率的数据。
        /// </summary>
        public decimal? Total { get; set; }

        /// <summary>
        /// 尖费率的数据。
        /// </summary>
        public decimal? Pike { get; set; }

        /// <summary>
        /// 峰费率的数据。
        /// </summary>
        public decimal? Peak { get; set; }

        /// <summary>
        /// 平费率的数据。
        /// </summary>
        public decimal? Flat { get; set; }

        /// <summary>
        /// 谷费率的数据。
        /// </summary>
        public decimal? Vale { get; set; }

        /// <summary>
        /// 获取或设置指定费率的数据。
        /// </summary>
        /// <param name="rates"></param>
        /// <returns></returns>
        public decimal? this[RatesOptions rates]
        {
            get
            {
                switch (rates)
                {
                    case RatesOptions.Total:
                        return this.Total;

                    case RatesOptions.Pike:
                        return this.Pike;

                    case RatesOptions.Peak:
                        return this.Peak;

                    case RatesOptions.Flat:
                        return this.Flat;

                    case RatesOptions.Vale:
                        return this.Vale;

                    default:
                        return null;
                }
            }
            set
            {
                switch (rates)
                {
                    case RatesOptions.Total:
                        this.Total = value;
                        break;

                    case RatesOptions.Pike:
                        this.Pike = value;
                        break;

                    case RatesOptions.Peak:
                        this.Peak = value;
                        break;

                    case RatesOptions.Flat:
                        this.Flat = value;
                        break;

                    case RatesOptions.Vale:
                        this.Vale = value;
                        break;
                }
            }
        }

        /// <summary>
        /// 获取尖、峰、平、谷四个费率的合计数据。
        /// </summary>
        /// <returns></returns>
        public decimal? GetSum()
        {
            decimal? decSum = null;

            foreach (int intEnumValue in Enum.GetValues(typeof(RatesOptions)))
            {
                RatesOptions enumValue = (RatesOptions)intEnumValue;
                if (enumValue != RatesOptions.Total)
                {
                    if (this[enumValue] != null)
                    {
                        if (decSum == null)
                        {
                            decSum = this[enumValue];
                        }
                        else
                        {
                            decSum += this[enumValue];
                        }
                    }
                }
            }

            return decSum;
        }
    }
}
