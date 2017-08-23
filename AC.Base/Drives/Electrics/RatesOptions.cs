using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AC.Base.Drives.Electrics
{
    /// <summary>
    /// 费率。包括总、尖、峰、平、谷。
    /// </summary>
    public enum RatesOptions
    {
        /// <summary>
        /// 总费率。
        /// </summary>
        Total = 1 << 0,

        /// <summary>
        /// 尖费率。
        /// </summary>
        Pike = 1 << 1,

        /// <summary>
        /// 峰费率。
        /// </summary>
        Peak = 1 << 2,

        /// <summary>
        /// 平费率。
        /// </summary>
        Flat = 1 << 3,

        /// <summary>
        /// 谷费率。
        /// </summary>
        Vale = 1 << 4
    }

    /// <summary>
    /// 费率扩展。
    /// </summary>
    public static class RatesExtensions
    {
        /// <summary>
        /// 获取费率的文字说明。
        /// </summary>
        /// <param name="rates">费率</param>
        /// <returns>费率的文字说明</returns>
        public static string GetDescription(this RatesOptions rates)
        {
            switch (rates)
            {
                case RatesOptions.Total:
                    return "总";

                case RatesOptions.Pike:
                    return "尖";

                case RatesOptions.Peak:
                    return "峰";

                case RatesOptions.Flat:
                    return "平";

                case RatesOptions.Vale:
                    return "谷";

                default:
                    throw new NotImplementedException("尚未实现该枚举。");
            }
        }
    }
}
