using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AC.Base.ChannelServices
{
    /// <summary>
    /// 用于通道服务产生后台连接事件的接口。
    /// </summary>
    public interface IBackConnectChannelService
    {
        /// <summary>
        /// 新的后台连接后产生的事件。
        /// </summary>
        event ChannelServiceConnectEventHandler BackConnected;

        /// <summary>
        /// 后台断开连接后产生的事件。
        /// </summary>
        event ChannelServiceConnectEventHandler BackDisconnected;
    }
}
