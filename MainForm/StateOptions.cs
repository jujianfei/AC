using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MainForm
{
    /// <summary>
    /// 通道服务当前运行状态选项。
    /// </summary>
    public enum StateOptions
    {
        /// <summary>
        /// 空闲。
        /// </summary>
        Free,
        /// <summary>
        /// 正常运行。
        /// </summary>
        Running,
        /// <summary>
        /// 停止运行。
        /// </summary>
        Stopped,
        /// <summary>
        /// 故障停止。
        /// </summary>
        Exception,
    }

    /// <summary>
    /// 通道服务当前运行状态方法扩展。
    /// </summary>
    public static class StateExtensions
    {
        /// <summary>
        /// 获取通道服务运行状态的文字说明。
        /// </summary>
        /// <param name="channelServiceState">通道服务状态</param>
        /// <returns>通道服务运行状态的文字说明</returns>
        public static string GetDescription(this StateOptions channelServiceState)
        {
            switch (channelServiceState)
            {
                case StateOptions.Free:
                    return "空闲";

                case StateOptions.Running:
                    return "运行";

                case StateOptions.Stopped:
                    return "停止";

                case StateOptions.Exception:
                    return "故障";

                default:
                    throw new NotImplementedException("尚未实现该枚举。");
            }
        }
    }
}

