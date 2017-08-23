using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AC.Base.Drives.Electrics
{
    /// <summary>
    /// 相位、相序。包括总、A/AB相、B/BC相、C/CA相、零相。
    /// </summary>
    public enum PhaseOptions
    {
        /// <summary>
        /// 总
        /// </summary>
        Total = 1 << 0,

        /// <summary>
        /// A/AB相
        /// </summary>
        A = 1 << 1,

        /// <summary>
        /// B/BC相
        /// </summary>
        B = 1 << 2,

        /// <summary>
        /// C/CA相
        /// </summary>
        C = 1 << 3,

        /// <summary>
        /// 零相
        /// </summary>
        Zero = 1 << 4
    }

    /// <summary>
    /// 相位、相序扩展。
    /// </summary>
    public static class PhaseExtensions
    {
        /// <summary>
        /// 获取相位、相序的文字说明。
        /// </summary>
        /// <param name="phase">相位、相序</param>
        /// <returns>相位、相序的文字说明</returns>
        public static string GetDescription(this PhaseOptions phase)
        {
            switch (phase)
            {
                case PhaseOptions.Total:
                    return "总";

                case PhaseOptions.A:
                    return "A相";

                case PhaseOptions.B:
                    return "B相";

                case PhaseOptions.C:
                    return "C相";

                case PhaseOptions.Zero:
                    return "零相";

                default:
                    throw new NotImplementedException("尚未实现该枚举。");
            }
        }
    }
}
