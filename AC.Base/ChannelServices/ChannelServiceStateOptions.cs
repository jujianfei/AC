using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AC.Base.ChannelServices
{
    /// <summary>
    /// 通道服务当前运行状态选项。
    /// </summary>
    public enum ChannelServiceStateOptions
    {
        /// <summary>
        /// 准备就绪。
        /// </summary>
        Ready,

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
    public static class ChannelServiceStateExtensions
    {
        /// <summary>
        /// 获取通道服务运行状态的文字说明。
        /// </summary>
        /// <param name="channelServiceState">通道服务状态</param>
        /// <returns>通道服务运行状态的文字说明</returns>
        public static string GetDescription(this ChannelServiceStateOptions channelServiceState)
        {
            switch (channelServiceState)
            {
                case ChannelServiceStateOptions.Ready:
                    return "就绪";

                case ChannelServiceStateOptions.Running:
                    return "运行";

                case ChannelServiceStateOptions.Stopped:
                    return "停止";

                case ChannelServiceStateOptions.Exception:
                    return "故障";

                default:
                    throw new NotImplementedException("尚未实现该枚举。");
            }
        }
    }
}
