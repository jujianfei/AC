using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AC.Base.Drives.Electrics
{
    /// <summary>
    /// 功效类型。包括正向有功、反向有功、正向无功、反向无功、第一象限无功、第二象限无功等。
    /// </summary>
    public enum EfficacyOptions
    {
        /// <summary>
        /// 正向有功
        /// </summary>
        ForwardActivePower = 1 << 0,

        /// <summary>
        /// 正向无功（一、二象限无功）
        /// </summary>
        ForwardReactivePower = 1 << 1,

        /// <summary>
        /// 反向有功
        /// </summary>
        ReverseActivePower = 1 << 2,

        /// <summary>
        /// 反向无功（三、四象限无功）
        /// </summary>
        ReverseReactivePower = 1 << 3,

        /// <summary>
        /// 第一象限无功（感性无功）
        /// </summary>
        ReactivePower1 = 1 << 4,

        /// <summary>
        /// 第二象限无功
        /// </summary>
        ReactivePower2 = 1 << 5,

        /// <summary>
        /// 第三象限无功
        /// </summary>
        ReactivePower3 = 1 << 6,

        /// <summary>
        /// 第四象限无功（容性无功）
        /// </summary>
        ReactivePower4 = 1 << 7,

        /// <summary>
        /// 一、四象限无功（双向无功）
        /// </summary>
        ReactivePower14 = 1 << 8,

        /// <summary>
        /// 二、三象限无功
        /// </summary>
        ReactivePower23 = 1 << 9,

        /// <summary>
        /// 正向视在
        /// </summary>
        ForwardApparent = 1 << 10,

        /// <summary>
        /// 反向视在
        /// </summary>
        ReverseApparent = 1 << 11
    }

    /// <summary>
    /// 功效类型扩展。
    /// </summary>
    public static class EfficacyExtensions
    {
        /// <summary>
        /// 获取功效类型的文字说明。
        /// </summary>
        /// <param name="efficacy">功效类型</param>
        /// <returns>功效类型的文字说明</returns>
        public static string GetDescription(this EfficacyOptions efficacy)
        {
            switch (efficacy)
            {
                case EfficacyOptions.ForwardActivePower:
                    return "正向有功";

                case EfficacyOptions.ReverseActivePower:
                    return "反向有功";

                case EfficacyOptions.ForwardReactivePower:
                    return "正向无功";

                case EfficacyOptions.ReverseReactivePower:
                    return "反向无功";

                case EfficacyOptions.ReactivePower1:
                    return "第一象限无功";

                case EfficacyOptions.ReactivePower2:
                    return "第二象限无功";

                case EfficacyOptions.ReactivePower3:
                    return "第三象限无功";

                case EfficacyOptions.ReactivePower4:
                    return "第四象限无功";

                case EfficacyOptions.ReactivePower14:
                    return "一、四象限无功";

                case EfficacyOptions.ReactivePower23:
                    return "二、三象限无功";

                case EfficacyOptions.ForwardApparent:
                    return "正向视在";

                case EfficacyOptions.ReverseApparent:
                    return "反向视在";

                default:
                    throw new NotImplementedException("尚未实现该枚举。");
            }
        }
    }
}
