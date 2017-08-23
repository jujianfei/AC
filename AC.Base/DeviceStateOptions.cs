using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AC.Base
{
    /// <summary>
    /// 设备状态选项。
    /// </summary>
    public enum DeviceStateOptions
    {
        /// <summary>
        /// 运行
        /// </summary>
        Running,

        /// <summary>
        /// 待装
        /// </summary>
        Waiting,

        /// <summary>
        /// 停用
        /// </summary>
        Stopped,

        /// <summary>
        /// 故障
        /// </summary>
        Exception,

        /// <summary>
        /// 拆除
        /// </summary>
        Backout,
    }

    /// <summary>
    /// 设备状态扩展。
    /// </summary>
    public static class DeviceStateExtensions
    {
        /// <summary>
        /// 获取设备状态的文字说明。
        /// </summary>
        /// <param name="deviceState">设备状态</param>
        /// <returns>设备状态的文字说明</returns>
        public static string GetDescription(this DeviceStateOptions deviceState)
        {
            switch (deviceState)
            {
                case DeviceStateOptions.Running:
                    return "运行";

                case DeviceStateOptions.Waiting:
                    return "待装";

                case DeviceStateOptions.Stopped:
                    return "停用";

                case DeviceStateOptions.Exception:
                    return "故障";

                case DeviceStateOptions.Backout:
                    return "拆除";

                default:
                    throw new NotImplementedException("尚未实现该枚举。");
            }
        }
    }
}
