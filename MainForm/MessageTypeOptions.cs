using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MainForm
{
    /// <summary>
    /// 通道服务消息类型。
    /// </summary>
    public enum MessageTypeOptions
    {
        SystemInformation = 0,
        SystemException = 1,
        ZHUZANBAOWEN = 2,
        MOXABAOWEN = 3,
        CHAOBIAOBAOWEN = 4
        
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
        public static string GetDescription(this MessageTypeOptions messageType)
        {
            switch (messageType)
            {
                case MessageTypeOptions.ZHUZANBAOWEN:
                    return "主站报文";

                case MessageTypeOptions.MOXABAOWEN:
                    return "MOXA报文";

                case MessageTypeOptions.CHAOBIAOBAOWEN:
                    return "抄表报文";

                case MessageTypeOptions.SystemException:
                    return "系统异常";

                case MessageTypeOptions.SystemInformation:
                    return "系统信息";

                default:
                    throw new NotImplementedException("尚未实现该枚举。");
            }
        }
    }



}