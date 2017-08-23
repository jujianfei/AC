using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AC.Base.Drives.DLT645
{
    /// <summary>
    /// DL/T 645-1997 控制码。
    /// </summary>
    public enum DLT645_1997ControlCodeOptions
    {
        /// <summary>
        /// 读数据
        /// </summary>
        ReadData = 0x01,

        /// <summary>
        /// 读后续数据
        /// </summary>
        ReadNextData = 0x02,

        /// <summary>
        /// 重读数据
        /// </summary>
        ReRead = 0x03,

        /// <summary>
        /// 写数据
        /// </summary>
        WriteData = 0x04,

        /// <summary>
        /// 广播校时
        /// </summary>
        ProofreadTime = 0x08,

        /// <summary>
        /// 写通信地址
        /// </summary>
        WriteAddress = 0x0A,

        /// <summary>
        /// 更改通信速率
        /// </summary>
        UpdateBaudrates = 0x0C,

        /// <summary>
        /// 修改密码
        /// </summary>
        UpdatePassword = 0x0F,

        /// <summary>
        /// 最大需量清零
        /// </summary>
        DemandReset = 0x10,
    }

    /// <summary>
    /// DL/T 645-1997 控制码扩展。
    /// </summary>
    public static class DLT645_1997ControlCodeExtensions
    {
        /// <summary>
        /// 获取 DL/T 645-1997 控制码的文字说明。
        /// </summary>
        /// <param name="controlCode"></param>
        /// <returns></returns>
        public static string GetDescription(this DLT645_1997ControlCodeOptions controlCode)
        {
            switch (controlCode)
            {
                case DLT645_1997ControlCodeOptions.ReadData:
                    return "读数据";

                case DLT645_1997ControlCodeOptions.ReadNextData:
                    return "读后续数据";

                case DLT645_1997ControlCodeOptions.ReRead:
                    return "重读数据";

                case DLT645_1997ControlCodeOptions.WriteData:
                    return "写数据";

                case DLT645_1997ControlCodeOptions.ProofreadTime:
                    return "广播校时";

                case DLT645_1997ControlCodeOptions.WriteAddress:
                    return "写通信地址";

                case DLT645_1997ControlCodeOptions.UpdateBaudrates:
                    return "更改通信速率";

                case DLT645_1997ControlCodeOptions.UpdatePassword:
                    return "修改密码";

                case DLT645_1997ControlCodeOptions.DemandReset:
                    return "最大需量清零";

                default:
                    throw new NotImplementedException("尚未实现该枚举。");
            }
        }
    }
}
