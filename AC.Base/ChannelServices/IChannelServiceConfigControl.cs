using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AC.Base.ChannelServices
{
    /// <summary>
    /// 通道服务配置控件接口。实现该接口的类必须添加 ControlAttribute 属性。
    /// </summary>
    public interface IChannelServiceConfigControl : IControl
    {
        /// <summary>
        /// 设置应用程序框架。
        /// </summary>
        /// <param name="application">应用程序框架。</param>
        void SetApplication(ApplicationClass application);

        /// <summary>
        /// 设置需配置的通道服务对象。
        /// </summary>
        /// <param name="channelService">通道服务。</param>
        /// <param name="isLocal">当前是否在通道服务运行的计算机上对通道进行配置。</param>
        void SetChannelService(IChannelService channelService, bool isLocal);

        /// <summary>
        /// 获取配置的通道服务对象。
        /// </summary>
        /// <returns>通道服务。</returns>
        IChannelService GetChannelService();
    }
}
