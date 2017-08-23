using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AC.Base.ChannelServices
{
    /// <summary>
    /// 通道服务消息类型。
    /// </summary>
    public enum ChannelServiceMessageTypeOptions
    {
        /// <summary>
        /// 后端一般的通知消息，例如后台连接、断开连接。
        /// </summary>
        BackInformation,

        /// <summary>
        /// 后端重要消息，例如主备通道切换。
        /// </summary>
        BackImportance,

        /// <summary>
        /// 后端警告消息，例如后台频繁连接断开。
        /// </summary>
        BackWarning,

        /// <summary>
        /// 后端错误消息，例如后端监听端口被占用，后端代码产生未预料的异常将要导致程序终止。
        /// </summary>
        BackException,

        /// <summary>
        /// 前端一般的通知消息，例如终端上线、断线。
        /// </summary>
        FrontInformation,

        /// <summary>
        /// 前端重要消息。
        /// </summary>
        FrontImportance,

        /// <summary>
        /// 前端警告消息，例如设备频繁上下线。
        /// </summary>
        FrontWarning,

        /// <summary>
        /// 前端错误消息，例如前端监听端口被占用，前端代码产生未预料的异常将要导致程序终止。
        /// </summary>
        FrontException,

        /// <summary>
        /// 消息
        /// </summary>
        NormallyInformation,

        /// <summary>
        /// 警告
        /// </summary>
        NormallyWarning,

        /// <summary>
        /// 异常
        /// </summary>
        NormallyException,

        /// <summary>
        /// 重要消息
        /// </summary>
        NormallyImportance
    }

    /// <summary>
    /// 通道服务消息类型方法扩展。
    /// </summary>
    public static class ChannelServiceMessageTypeExtensions
    {
        /// <summary>
        /// 获取通道服务消息类型的文字说明。
        /// </summary>
        /// <param name="messageType">通道服务消息类型。</param>
        /// <returns>通道服务消息类型的文字说明</returns>
        public static string GetDescription(this ChannelServiceMessageTypeOptions messageType)
        {
            switch (messageType)
            {
                case ChannelServiceMessageTypeOptions.BackInformation:
                    return "主站端 - 通知消息";

                case ChannelServiceMessageTypeOptions.BackImportance:
                    return "主站端 - 重要消息";

                case ChannelServiceMessageTypeOptions.BackWarning:
                    return "主站端 - 警告消息";

                case ChannelServiceMessageTypeOptions.BackException:
                    return "主站端 - 错误消息";

                case ChannelServiceMessageTypeOptions.FrontInformation:
                    return "设备端 - 通知消息";

                case ChannelServiceMessageTypeOptions.FrontImportance:
                    return "设备端 -重要消息";

                case ChannelServiceMessageTypeOptions.FrontWarning:
                    return "设备端 -警告消息";

                case ChannelServiceMessageTypeOptions.FrontException:
                    return "设备端 -错误消息";

                case ChannelServiceMessageTypeOptions.NormallyImportance:
                    return "重要消息";

                case ChannelServiceMessageTypeOptions.NormallyInformation:
                    return "消息";

                case ChannelServiceMessageTypeOptions.NormallyWarning:
                    return "警告";

                case ChannelServiceMessageTypeOptions.NormallyException:
                    return "异常";

                default:
                    throw new NotImplementedException("尚未实现该枚举。");
            }
        }
    }
}
