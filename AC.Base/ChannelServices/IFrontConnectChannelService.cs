using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AC.Base.ChannelServices
{
    /// <summary>
    /// 用于通道服务产生设备侧连接事件的接口，通常该接口应用于 TCP/IP 方式进行连接的通道。
    /// </summary>
    public interface IFrontConnectChannelService
    {
        /// <summary>
        /// 新的设备连接后产生的事件。
        /// </summary>
        event ChannelServiceConnectEventHandler FrontConnected;

        /// <summary>
        /// 设备断开连接后产生的事件。
        /// </summary>
        event ChannelServiceConnectEventHandler FrontDisconnected;
    }
}
