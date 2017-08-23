using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AC.Base.Drives.GDW130_376
{
    /// <summary>
    /// 密度枚举
    /// </summary>
    public enum DenistyOption
    {
        /// <summary>
        /// 无冻结
        /// </summary>
        DenistyNone = 0,
        /// <summary>
        /// 15分钟冻结
        /// </summary>
        Denisty15 = 1,
        /// <summary>
        /// 30分钟冻结
        /// </summary>
        Denisty30 = 2,
        /// <summary>
        /// 60分冻结
        /// </summary>
        Denisty60 = 3,
    }
    /// <summary>
    /// 密度枚举扩展
    /// </summary>
    public static class DenistyExtensions
    {
        /// <summary>
        /// 根据密度获取一天的点数
        /// </summary>
        /// <param name="den"></param>
        /// <returns></returns>
        public static int GetDayPoint(this DenistyOption den)
        {
            switch (den)
            {
                case DenistyOption.Denisty15: return 96;
                case DenistyOption.Denisty30: return 48;
                case DenistyOption.Denisty60: return 24;
                default:
                    {
                        throw new NotImplementedException("曲线密度无法解析。");
                    }
            }
        }
        /// <summary>
        /// 根据密度获取曲线点数枚举
        /// </summary>
        /// <param name="den"></param>
        /// <returns></returns>
        public static CurvePointOptions GetCurvePointOption(this DenistyOption den)
        {
            switch (den)
            {
                case DenistyOption.Denisty15: return CurvePointOptions.Point96;
                case DenistyOption.Denisty30: return CurvePointOptions.Point48;
                case DenistyOption.Denisty60: return CurvePointOptions.Point24;
                default:
                    {
                        throw new NotImplementedException("曲线密度无法解析。");
                    }
            }
        }
        /// <summary>
        /// 根据密度获取间隔分钟
        /// </summary>
        /// <param name="den"></param>
        /// <returns></returns>
        public static int GetIntervalMin(this DenistyOption den)
        {
            switch (den)
            {
                case DenistyOption.Denisty15: return 15;
                case DenistyOption.Denisty30: return 30;
                case DenistyOption.Denisty60: return 60;
                default:
                    {
                        throw new NotImplementedException("曲线密度无法解析。");
                    }
            }
        }
    }
}
