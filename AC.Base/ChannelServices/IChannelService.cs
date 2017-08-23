using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AC.Base.ChannelServices
{
    /// <summary>
    /// 通道服务运行状态改变时产生的事件所调用的委托。
    /// </summary>
    /// <param name="state">即将改变的状态。</param>
    public delegate void ChannelServiceStateChangingEventHandler(ChannelServiceStateOptions state);

    /// <summary>
    /// 通道服务运行状态发生改变后的事件所调用的委托。
    /// </summary>
    public delegate void ChannelServiceStateChangedEventHandler();

    /// <summary>
    /// 通道服务产生的消息事件所调用的委托。
    /// </summary>
    /// <param name="messageType">消息类型。</param>
    /// <param name="message">消息内容。</param>
    public delegate void ChannelServiceMessageEventHandler(ChannelServiceMessageTypeOptions messageType, string message);

    /// <summary>
    /// 通道服务有新的连接接入或现有连接断开时产生的事件所用到的委托。
    /// </summary>
    /// <param name="endPoint">连接网络地址标识。</param>
    public delegate void ChannelServiceConnectEventHandler(System.Net.EndPoint endPoint);

    /// <summary>
    /// 通道服务接口。实现该接口的非抽象类必须添加 ChannelServiceTypeAttribute 属性，以便描述当前通道服务的一些信息，并且必须有一个无参数的构造函数。
    /// </summary>
    public interface IChannelService
    {
        /// <summary>
        /// 从保存此通道服务数据的 XML 文档节点初始化当前通道服务。
        /// </summary>
        /// <param name="channelServiceConfig">该对象节点的数据</param>
        //void SetChannelServiceConfig(System.Xml.XmlNode channelServiceConfig);

        /// <summary>
        /// 获取当前通道服务的配置信息，将序列化的内容填充至 XmlNode 的 InnerText 属性或者 ChildNodes 子节点中。
        /// </summary>
        /// <param name="xmlDoc">创建 XmlNode 时所需使用到的 System.Xml.XmlDocument。</param>
        //System.Xml.XmlNode GetChannelServiceConfig(System.Xml.XmlDocument xmlDoc);

        /// <summary>
        /// 启动通道。该方法由通道服务程序或前置机程序作为宿主进行调用，调用该方法后通道应首先开启后台服务以便主控软件可以与该通道连接，然后开启设备端服务允许设备接入。
        /// </summary>
        void Start();

        /// <summary>
        /// 停止通道。该方法由通道服务程序或前置机程序作为宿主进行调用，调用该方法后通道应首先关闭设备端服务断开所有设备的连接，然后通知所有已连接的后台自身即将关闭的消息并关闭后台服务。对于通道安全运行的多种形式，如主备通道或多主通道，则由具体实现完成该功能。
        /// </summary>
        void Stop();

        /// <summary>
        /// 通道服务当前运行状态。
        /// </summary>
        ChannelServiceStateOptions State { get; }

        /// <summary>
        /// 通道服务运行状态发生改变时产生的事件。
        /// </summary>
        event ChannelServiceStateChangingEventHandler StateChanging;

        /// <summary>
        /// 通道服务运行状态发生改变后的事件。
        /// </summary>
        event ChannelServiceStateChangedEventHandler StateChanged;

        /// <summary>
        /// 通道服务产生的消息事件。
        /// </summary>
        event ChannelServiceMessageEventHandler EventMessage;
    }
}
