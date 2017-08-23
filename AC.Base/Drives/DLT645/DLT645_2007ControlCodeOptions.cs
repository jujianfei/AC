using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AC.Base.Drives.DLT645
{
    /// <summary>
    /// DL/T 645-2007 控制码。
    /// </summary>
    public enum DLT645_2007ControlCodeOptions
    {
        /// <summary>
        /// 广播校时
        /// </summary>
        ProofreadTime = 0x08,

        /// <summary>
        /// 读数据
        /// </summary>
        ReadData = 0x11,

        /// <summary>
        /// 读后续数据
        /// </summary>
        ReadNextData = 0x12,

        /// <summary>
        /// 读通信地址
        /// </summary>
        ReadAddress = 0x13,

        /// <summary>
        /// 写数据
        /// </summary>
        WriteData = 0x14,

        /// <summary>
        /// 写通信地址
        /// </summary>
        WriteAddress = 0x15,

        /// <summary>
        /// 冻结命令
        /// </summary>
        Freeze = 0x16,

        /// <summary>
        /// 更改通信速率
        /// </summary>
        UpdateBaudrates = 0x17,

        /// <summary>
        /// 修改密码
        /// </summary>
        UpdatePassword = 0x18,

        /// <summary>
        /// 最大需量清零
        /// </summary>
        DemandReset = 0x19,

        /// <summary>
        /// 电量清零
        /// </summary>
        IndicatedReset = 0x1A,

        /// <summary>
        /// 事件清零
        /// </summary>
        EventReset = 0x1B,
    }

    /// <summary>
    /// DL/T 645-2007 控制码扩展。
    /// </summary>
    public static class DLT645_2007ControlCodeExtensions
    {
        /// <summary>
        /// 获取 DL/T 645-2007 控制码的文字说明。
        /// </summary>
        /// <param name="controlCode"></param>
        /// <returns></returns>
        public static string GetDescription(this DLT645_2007ControlCodeOptions controlCode)
        {
            switch (controlCode)
            {
                case DLT645_2007ControlCodeOptions.ProofreadTime:
                    return "广播校时";

                case DLT645_2007ControlCodeOptions.ReadData:
                    return "读数据";

                case DLT645_2007ControlCodeOptions.ReadNextData:
                    return "读后续数据";

                case DLT645_2007ControlCodeOptions.ReadAddress:
                    return "读通信地址";

                case DLT645_2007ControlCodeOptions.WriteData:
                    return "写数据";

                case DLT645_2007ControlCodeOptions.WriteAddress:
                    return "写通信地址";

                case DLT645_2007ControlCodeOptions.Freeze:
                    return "冻结命令";

                case DLT645_2007ControlCodeOptions.UpdateBaudrates:
                    return "更改通信速率";

                case DLT645_2007ControlCodeOptions.UpdatePassword:
                    return "修改密码";

                case DLT645_2007ControlCodeOptions.DemandReset:
                    return "最大需量清零";

                case DLT645_2007ControlCodeOptions.IndicatedReset:
                    return "电量清零";

                case DLT645_2007ControlCodeOptions.EventReset:
                    return "事件清零";

                default:
                    throw new NotImplementedException("尚未实现该枚举。");
            }
        }
    }
}
